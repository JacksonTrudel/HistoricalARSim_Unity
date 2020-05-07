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
    public float[] RegUT = new float[3];// shift 1, 2, 3

    public ArrayList Departments;
    public ArrayList FoodItems;
    public ArrayList Shifts;

    public const int MinEmployeesWorking = 2;

    public Store()
    {
        StoreDay = 1;
        Cash = 112.00m;
        DayHours = 12;
        HourlyPay = 10;
        Shift = 0;

        for (int i = 0; i < 3; i++)
            RegUT[i] = 100.0f;

        // not sure if necessary
        Departments = new ArrayList();

        FoodItems = new ArrayList();

        Shifts = new ArrayList();
        for (int i = 0; i < 3; i++)
            Shifts.Add(new ShiftInfo());
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
        this.FoodItems = other.FoodItems;
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

    public float GetRegUT(int shiftNo)
    {
        return RegUT[shiftNo - 1];
    }

    public void SetRegUT(int shiftNo, float val)
    {
        RegUT[shiftNo] = val;
    }


    public void SetShifts(ArrayList shifts)
    {
        Shifts = shifts;
    }

    public void InitStore()
    {
        FoodItems = new ArrayList();

        // public FoodItem(int department, string itemName, decimal unitPriceCustomer, decimal unitPriceFarmer, int maxFOH, 
        //                  int maxBOH, int stockFOH, int stockBOH)
        FoodItems.Add(new FoodItem(1, "Apples", 1.00m, 0.15m, 100, 100, 100, 100));
        FoodItems.Add(new FoodItem(1, "Bananas", 1.00m, 0.10m, 100, 100, 100, 100));
        FoodItems.Add(new FoodItem(2, "Cheese", 4.00m, 0.50m, 100, 100, 100, 100));
        FoodItems.Add(new FoodItem(2, "Milk", 3.00m, 0.20m, 100, 100, 100, 100));
        FoodItems.Add(new FoodItem(3, "Cereal", 3.00m, 0.20m, 100, 100, 100, 100));
        FoodItems.Add(new FoodItem(3, "Cookies", 3.00m, 0.20m, 100, 100, 100, 100));
        FoodItems.Add(new FoodItem(4, "Pizza", 6.00m, 0.50m, 100, 100, 100, 100));
        FoodItems.Add(new FoodItem(4, "Dessert", 5.00m, 0.25m, 100, 100, 100, 100));
    }

    public FoodItem GetFoodItem(string name)
    {
        foreach (FoodItem food in FoodItems)
        {
            //UnityEngine.Debug.Log("food.Name: " + food.Name + "   name: " + name);
            if (food.Name == name)
            {
                return food;
            }
        }

        return null;
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
    }
}
