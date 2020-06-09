using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Store : MonoBehaviour
{
    public int StoreDay { get; set; }
    public decimal Cash { get; set; }
    public int DayHours { get; set; }
    public int HourlyPay { get; set; } // different from (no AR historical)
    public int Shift { get; set; }
    private double[] RegUT;// shift 1, 2, 3

    public decimal StartOfDayCash;

    public ArrayList Departments;
    public ArrayList Stock;
    public ArrayList Shifts;
    public ArrayList Deliveries;

    public const int MinEmployeesWorking = 2;
    private decimal DeliveryCost;

    private decimal hoursPerShift = 4;

    public static string[] DepartmentNames = new string[] { "Produce", "Dairy", "Dry Goods", "Frozen", "Registers" };
    // IMPORTANT: if Simulation.TimeBetweenCheckouts == 10, numCheckoutsPerHour MUST equal 6 
    //               for checkouts to be balanced
    public const int NUM_CHECKOUTS_PER_HOUR = 6;
    // Should be equal to FoodTakenFromShelvesPerCheckout in real-time AR simulation
    public const int MAX_FOODS_PER_CHECKOUT = 3;
    // this should match RegisterRate in Simulation
    public const int REGISTER_CHECKOUT_RATE = 8;
    public const int MAX_EMPLOYEES = 8;

    // data for PostDayManager
    public decimal DailyRevenue;
    public decimal [] ShiftRevenue;
    public int DailyItemsSold;
    public int[] ShiftItemsSold;
    public decimal DailyEmployeePayout;
    public decimal DailyDeliveryCost;
    public int TotalExpired;
    public int DeliveriesOrdered;

    // captures the total number of foodItems that are overflow at the END of the day
    public int totalOverFlow;

    // used to calculate RegUT (Register utilization)
    public int CheckoutsPossible;
    public int CheckoutsPerformed;


    public Store()
    {
        StoreDay = 1;
        Cash = 10000.00m;
        DayHours = 12;
        HourlyPay = 10;
        Shift = 0;
        // not sure if necessary
        Departments = new ArrayList();

        Deliveries = new ArrayList();

        Stock = new ArrayList();
    }

    public Store(Store other)
    {
        this.StoreDay = other.StoreDay;
        this.Cash = other.Cash;
        this.DayHours = other.DayHours;
        this.HourlyPay = other.HourlyPay;
        this.Shift = other.Shift;
        this.Departments = other.Departments;
        this.Shifts = other.Shifts;
        this.Stock = other.Stock;
        this.RegUT = other.RegUT;
    }

    void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public double GetRegUT(int shiftNo)
    {
        return RegUT[shiftNo - 1];
    }

    public void SetRegUT(int shiftNo, float val)
    {
        RegUT[shiftNo] = val;
    }


    public void SetShifts(ArrayList shifts)
    {
        Shifts = new ArrayList();
        for (int i = 0; i < 3; i++)
            Shifts.Add(new ShiftInfo());
        for (int i = 0; i < 3; i++)
        {
            Shifts[i] = shifts[i];
        }
    }

    public void SetDeliveries(ArrayList deliveries, decimal deliveryCost)
    {
        DeliveryCost = deliveryCost;
        for (int i = 0; i < deliveries.Count; i++)
        {
            Deliveries.Add(deliveries[i]);
        }
    }

    /*
    public void PrintShifts()
    {
        int i = 1;
        foreach(Store.ShiftInfo shift in Shifts)
        {
            UnityEngine.Debug.Log("Shift " + i + " has " + shift.Employees["Registers"] + " employees on registers and " + shift.TotalEmployees + " total employees");
            i++;
        }
    }*/

    public void InitStore()
    {
        Stock = new ArrayList();

        // public FoodItem(int department, string itemName, decimal unitPriceCustomer, decimal unitPriceFarmer, int maxFOH, 
        //                  int maxBOH, int stockFOH, int stockBOH)
        Stock.Add(new FoodItem(1, "Apples", 1.00m, 0.15m, 100, 100, 100, 100));
        Stock.Add(new FoodItem(1, "Bananas", 1.00m, 0.10m, 100, 100, 100, 100));
        Stock.Add(new FoodItem(2, "Cheese", 4.00m, 0.50m, 100, 100, 100, 100));
        Stock.Add(new FoodItem(2, "Milk", 3.00m, 0.20m, 100, 100, 100, 100));
        Stock.Add(new FoodItem(3, "Cereal", 3.00m, 0.20m, 100, 100, 100, 100));
        Stock.Add(new FoodItem(3, "Cookies", 3.00m, 0.20m, 100, 100, 100, 100));
        Stock.Add(new FoodItem(4, "Pizza", 6.00m, 0.50m, 100, 100, 100, 100));
        Stock.Add(new FoodItem(4, "Dessert", 5.00m, 0.25m, 100, 100, 100, 100));
    }

    public FoodItem GetFoodItem(string name)
    {
        foreach (FoodItem food in Stock)
        {
            //UnityEngine.Debug.Log("food.Name: " + food.Name + "   name: " + name);
            if (food.Name == name)
            {
                return food;
            }
        }

        return null;
    }

    public void RunDay()
    {
        int i, j;
        Delivery delivery;

        UnityEngine.Debug.Log("Before RunDay: " + SimController.DayNum);

        // print shifts
        UnityEngine.Debug.Log("Shifts:");
        for (i = 0; i < 3; i++)
        {

            UnityEngine.Debug.Log("Shift" + (i+ 1) + " --> " + ((ShiftInfo) Shifts[i]).ToString());
        }
        // print deliveries 
        for (i = 0; i < Deliveries.Count; i++)
        {
            Delivery temp = (Delivery)Deliveries[i];
            UnityEngine.Debug.Log("Delivery Item: " + temp.foodItem.Name + " Quantity: " + temp.Quantity + " Exp: " + temp.Expedited + " ArrivalDate: " + temp.ArrivalDate);
        }
        // print BOH stock 
        for (i = 0; i < Stock.Count; i++)
        {
            FoodItem food = (FoodItem)Stock[i];
            UnityEngine.Debug.Log("BOH " + food.Name + ": " + food.StockBOH);
        }

        // simulate day
        DailyEmployeePayout = 0.0m; 
        DailyDeliveryCost = 0.0m;
        DailyRevenue = 0;
        ShiftRevenue = new decimal[3];
        DailyItemsSold = 0;
        ShiftItemsSold = new int[3];
        RegUT = new double[3];
        StartOfDayCash = Cash;

        for (i = 0; i < 3; i++)
        {
            ShiftRevenue[i] = 0;
            ShiftItemsSold[i] = 0;
        }
        CheckoutsPerformed = 0;
        CheckoutsPossible = 0;
        DeliveriesOrdered = 0;
        // pay for deliveries ordered today
        for (i = 0; i < Deliveries.Count; i++)
        {
            delivery = (Delivery)Deliveries[i];

            // check for expedited is redundant (thus not here)
            if (delivery.OrderDate == SimController.DayNum)
            {
                Cash -= delivery.Cost;
                DailyDeliveryCost += delivery.Cost;
                DeliveriesOrdered++;
            }
        }

        // get standard deliveries 
        for (i = 0; i < Deliveries.Count; i++)
        {
            delivery = (Delivery)Deliveries[i];

            // check for expedited is redundant (thus not here)
            if (delivery.ArrivalDate == SimController.DayNum && delivery.Expedited)
            {
                ProcessDelivery(delivery.foodItem, delivery.Quantity);
                Deliveries.RemoveAt(i);
                i--;
            }
        }

        // For each shift
        for (i = 0; i < 3; i++)
        {
            Store.ShiftInfo shift = (Store.ShiftInfo)Shifts[i];

            // pay employees
            DailyEmployeePayout += (decimal) shift.TotalEmployees * HourlyPay * hoursPerShift;
            
            // 1. process expedited deliveries --> Expedited deliveries come during shift 2
            if (i == 1)
            {
                
                for (j = 0; j < Deliveries.Count; j++)
                { 
                    delivery = (Delivery)Deliveries[j];

                    // check for expedited is redundant (thus not here)
                    if (delivery.ArrivalDate == SimController.DayNum)
                    {
                        ProcessDelivery(delivery.foodItem, delivery.Quantity);
                        Deliveries.RemoveAt(j);
                        j--;
                    }
                }
            }

            // 2. restock before "shift"
            Restock(i);

            // 3. during shift, people buy goods
            // modeled closely to Real-Time AR tablet logic
            // IMPORTANT: if Simulation.TimeBetweenCheckouts == 10, numCheckoutsPerHour MUST equal 6 
            //               for checkouts to be balanced
            for (j = 0; j < NUM_CHECKOUTS_PER_HOUR; j++)
            {
                PullItemsOffShelves(i);
                checkoutFoods(i);
            }

            // items that are not checked out but are brought to registers need to be returned
            ReturnItemsToShelves();

            RegUT[i] = 100.0 * CheckoutsPerformed / CheckoutsPossible;

            UnityEngine.Debug.Log("Register UT for shift " + (i + 1) + " is " + RegUT[i]);

            // 4. restock after "shift"
            Restock(i);
        }

        // expire foods
        ExpireFoods();

        // print BOH stock 
        UnityEngine.Debug.Log("After RunDay" + SimController.DayNum);
        for (i = 0; i < Stock.Count; i++)
        {
            FoodItem food = (FoodItem)Stock[i];
            UnityEngine.Debug.Log("BOH " + food.Name + ": " + food.StockBOH);
        }

        // Deduct today's orders (set by SetDeliveries() )
        Cash -= DeliveryCost;

        Cash -= DailyEmployeePayout;
        UnityEngine.Debug.Log("Cash: " + Cash);

        // load PostDayReport
        SimController.LoadResults();
    }

    private void ProcessDelivery(FoodItem _food, int _quant)
    {
        FoodItem food;
        for (int i = 0; i < Stock.Count; i++)
        {
            food = (FoodItem)Stock[i];
            
            if(food.Name == _food.Name)
            {
                food.StockBOH += _quant;
                return;
            }
        }
    }

    private void Restock(int shiftIdx)
    {
        int dept, amountToRestock;

        foreach (FoodItem food in Stock)
        {
            dept = food.Department;
            ShiftInfo shift = (ShiftInfo) Shifts[shiftIdx];

            // restock if there is an employee in this department
            if (shift.Employees[  DepartmentNames[food.Department]  ] != 0)
            {
                amountToRestock = Mathf.Min(food.MaxFOH - food.StockFOH, food.StockBOH);
                food.StockFOH += amountToRestock;
                food.StockBOH -= amountToRestock;
            }
        }

    }

    private void PullItemsOffShelves(int shiftIdx)
    {
        int rand;

        foreach (FoodItem food in Stock)
        {
            rand = UnityEngine.Random.Range(0, MAX_FOODS_PER_CHECKOUT + 1);

            if (food.StockFOH > rand)
            {
                food.StockFOH -= rand;
                food.AtRegisters += rand;
            }
            else
            {
                food.AtRegisters += food.StockFOH;
                food.StockFOH = 0;
            }
        }
    }

    private void checkoutFoods(int shiftIdx)
    {

        int numRegisters = ((ShiftInfo)Shifts[shiftIdx]).Employees["Registers"];
        int numProcessedByRegister;
        for (int i = 0; i < numRegisters; i++)
        {
            numProcessedByRegister = 0;
            CheckoutsPossible += REGISTER_CHECKOUT_RATE;

            foreach (FoodItem food in Stock)
            {
                if (food.AtRegisters == 0)
                    continue;
                
                food.AtRegisters--; 
                CheckoutsPerformed++;  // calculating RegUT
                numProcessedByRegister++; // enforcing REGISTER_CHECKOUT_RATE

                Cash += food.UnitPriceCustomer;
                DailyRevenue += food.UnitPriceCustomer;
                ShiftRevenue[shiftIdx] += food.UnitPriceCustomer;
                DailyItemsSold++;
                ShiftItemsSold[shiftIdx]++;

                if (numProcessedByRegister == REGISTER_CHECKOUT_RATE)
                    break;
            }

        }
    }

    private void ReturnItemsToShelves()
    {
        foreach (FoodItem food in Stock)
        {
            food.StockFOH += food.AtRegisters;
            food.AtRegisters = 0;
        }
    }

    private void ExpireFoods()
    {
        UnityEngine.Debug.Log("Before expiring: " + ((FoodItem)Stock[0]).StockBOH);
        TotalExpired = 0;
        totalOverFlow = 0;
        foreach (FoodItem food in Stock)
        {
            int numBadFOH, numBadBOH;
            float percentBadFOH = UnityEngine.Random.Range(0.20f, 0.25f);
            float percentBadBOH = UnityEngine.Random.Range(0.05f, 0.10f);

            // natural expiries for front of house
            numBadFOH = (int)(percentBadFOH * food.StockFOH);
            food.StockFOH -= numBadFOH;
            TotalExpired += numBadFOH;

            // natural expiries for back of house
            numBadBOH = (int)(percentBadBOH * food.StockBOH);
            food.StockBOH -= numBadBOH;
            TotalExpired += numBadBOH;

            if (food.StockBOH > food.MaxBOH)
            {
                UnityEngine.Debug.Log("CHANGING OVERFLOW");
                totalOverFlow += food.StockBOH - food.MaxBOH;
                food.StockBOH = food.MaxBOH;
            }
        }
        UnityEngine.Debug.Log("After expiring: " + ((FoodItem)Stock[0]).StockBOH);
        UnityEngine.Debug.Log("Total overflow: " + totalOverFlow);

    }

    public class ShiftInfo
    {
        public Dictionary<string, int> Employees;
        public int TotalEmployees { get; set; }

        public ShiftInfo()
        {
            Employees = new Dictionary<string, int>();
            Employees.Add("Produce", 0);
            Employees.Add("Dairy", 0);
            Employees.Add("Dry Goods", 0);
            Employees.Add("Frozen", 0);
            Employees.Add("Registers", 1);

            TotalEmployees = 1;
        }

        public void addEmployee(string dept)
        {
            int numEmployees = Employees[dept];
            Employees[dept] = numEmployees + 1;
            TotalEmployees++;
        }

        public void removeEmployee(string dept)
        {

            int numEmployees = Employees[dept];
            Employees[dept] = numEmployees - 1;
            TotalEmployees--;
        }

        public string ToString()
        {
            string retText = "";

            return "Produce: " + this.Employees["Produce"] + ",  Dairy: " + this.Employees["Dairy"] + ",  Dry Goods: " + this.Employees["Dry Goods"] + ",  Frozen: "
                        + this.Employees["Frozen"] + ",  Registers: " + this.Employees["Registers"];
        }
    }
}
