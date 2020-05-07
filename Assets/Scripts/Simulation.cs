using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class Simulation : MonoBehaviour
{
    // globals
    public string[] DEPARTMENTS = { "Registers", "Produce", "Dry Goods", "Frozen", "Dairy", "Idle", "Offsite" };
    public static string[] FOODS = { "Apples", "Bananas", "Cereal", "Cookies", "Pizza", "Dessert", "Milk", "Cheese" };

    // references to  food objects
    Food apples = new Food("Produce", "PR", "Apples", "A2", "PR-A2", 150, 1.00m, 0.15m, 148.50m, 50, 100);
    Food bananas = new Food("Produce", "PR", "Bananas", "A2", "PR-A2", 150, 1.00m, 0.10m, 148.50m, 50, 100);
    Food cereal = new Food("Dry Goods", "PR", "Cereal", "A2", "PR-A2", 150, 3.00m, 0.20m, 148.50m, 50, 200);
    Food cookies = new Food("Dry Goods", "PR", "Cookies", "A2", "PR-A2", 150, 3.00m, 0.20m, 148.50m, 50, 200);
    Food pizza = new Food("Frozen", "PR", "Pizza", "A2", "PR-A2", 150, 6.00m, 0.50m, 148.50m, 50, 200);
    Food dessert = new Food("Frozen", "PR", "Dessert", "A2", "PR-A2", 150, 5.00m, 0.25m, 148.50m, 50, 200);
    Food milk = new Food("Dairy", "PR", "Milk", "A2", "PR-A2", 150, 3.00m, 0.20m, 148.50m, 50, 100);
    Food cheese = new Food("Dairy", "PR", "Cheese", "A2", "PR-A2", 150, 4.00m, 0.50m, 148.50m, 50, 100);

    // entered by us
    //these things are here almost for just notation purposes and outside script references
    //they are truly set, and overridden by, the values a user puts into the inspector
    [Header("Simulation Settings")]
    public int LengthOfWorkDayInHours = 12; // how long the store is open (Default: 12 hours)
    public int StartMilitaryTime = 8; // opening time (Default: 8:00)
    public int EndMilitaryTime = 20; // closing time (Default: 20:00)
    public int NumberOfDays = 7; // how many days are in the simulation (Default: 7 days)

    public static float Cash = 10000; // starting cash amount for the store (Default: $10,000)
    public int MaximumEmployees = 10; // max employees you can have working at the same time (Default: 10)
    public int MinimumEmployees = 4; // min employees that must always be in the store (full-time employees) (Default: 4)
    public int MaximumRegisters = 3; // max registers you can have open at the same time (Default: 3)
    public int MinimumRegisters = 1; // min registers to have open, can't have no registers or you'll make no money (Default: 1)
    public int FullTimeEmployees = 4; // same as minimum employees  
    int RestockBatchSize = 5;
    
    public int PayRate = 10; // pay rate for employees (default: 10)
    public int RestockingTimeInMinutes = 60; // how long it takes to restock 10 items (Default: 60 minutes) (e.g. 25 items would take 60min * 2.5 = 2.5 hours)

    public int MaximumProductOnShelves = 400;

    public int MaximumProductInBack = 1200;

    public int MaximumProductInStore = 1600; // maximum total product you are allowed to store in the front and back (Default: 1000)
    [Range(5, 60)]
    public int TimeBetweenCheckouts;
    [Range(1, 50)]
    public int FoodTakenFromShelvesPerCheckOut = 3;

    [Range(1, 20)]
    public float RestockingPeriod = 1.0f;

    [Header("Front Of House Quantities")]
    public int TotalApplesOnShelves = 50;
    public int TotalBananasOnShelves = 50;
    public int TotalCerealOnShelves = 50;
    public int TotalCookiesOnShelves = 50;
    public int TotalPizzaOnShelves = 50;
    public int TotalDessertOnShelves = 50;
    public int TotalMilkOnShelves = 50;
    public int TotalCheeseOnShelves = 50;

    [Header("Back Of House Quantities")]
    public int TotalApplesInBack = 100;
    public int TotalBananasInBack = 50;
    public int TotalCerealInBack = 133;
    public int TotalCookiesInBack = 100;
    public int TotalPizzaInBack = 67;
    public int TotalDessertInBack = 100;
    public int TotalMilkInBack = 67;
    public int TotalCheeseInBack = 33;

    [Header("Employees Per Department")]
    public int NumberEmployeesRegisters = 2;
    public int NumberEmployeesProduce = 0;
    public int NumberEmployeesDryGoods = 0;
    public int NumberEmployeesFrozen = 0;
    public int NumberEmployeesDairy = 0;
    public int NumberEmployeesIdle = 5;
    public int NumberEmployeesOffsite = 5;

    // gets calculated at start
    int totalCurrentEmployees;
    int totalCurrentProduct;
    [HideInInspector] public int totalInBack_value;
    [HideInInspector] public int totalInFront_value;

    //total expired food
    [HideInInspector]
    public static int total_expired;

    // declaring text gameobjects
    Text RegistersText, ProduceText, DryGoodsText, FrozenText, DairyText, IdleText, OffsiteText;
    Text ApplesTotal, ApplesBack, ApplesFront, BananasTotal, BananasBack, BananasFront;
    Text CerealTotal, CerealBack, CerealFront, CookiesTotal, CookiesBack, CookiesFront;
    Text PizzaTotal, PizzaBack, PizzaFront, DessertTotal, DessertBack, DessertFront;
    Text MilkTotal, MilkBack, MilkFront, CheeseTotal, CheeseBack, CheeseFront;

    Text RegisterDebug;

    public Dictionary<string, int> numEmployees = new Dictionary<string, int>();
    public Dictionary<string, int> busyEmployees = new Dictionary<string, int>();
    public static Dictionary<string, int> totalOnShelves = new Dictionary<string, int>();
    public static Dictionary<string, int> totalInBack = new Dictionary<string, int>();
    public Dictionary<string, decimal> shippingCost = new Dictionary<string, decimal>();
    public Dictionary<string, Food> foods = new Dictionary<string, Food>();
    // ONLY FOR HISTORICAL
    // stores: # emp on registers (shift 1), #  emp restocking (shift 1), # emp on registers (shift 2), #  emp restocking (shift 2)
    public int[] ShiftInfo = new int[4];

    public int TotalActiveEmployees;

    // references to be found in Awake()
    Text CashText, CheckoutItemsText;
    RegisterController registers;
    GraphController graphs;
    PostDayManager PostDayManager;
    TimeController TimeC;

    // IEnumerator for restocking
    private IEnumerator restockItem;

    void Awake() 
    {
        CashText = GameObject.Find("CashText").GetComponent<Text>();
        CheckoutItemsText = GameObject.Find("ItemsAtCheckout").GetComponent<Text>();
        registers = GameObject.Find("Simulation").GetComponent<RegisterController>();
        graphs = GameObject.Find("Simulation").GetComponent<GraphController>();
        PostDayManager = GameObject.Find("Simulation").GetComponent<PostDayManager>();
        TimeC = TimeC = GameObject.Find("UI Canvas").GetComponent<TimeController>();
    }
    void Start()
    {
        //DontDestroyOnLoad(this);
        

        // grab values from inspector inputs and assign them to mapping
        numEmployees["Registers"] = NumberEmployeesRegisters;
        numEmployees["Produce"] = NumberEmployeesProduce;
        numEmployees["Dry Goods"] = NumberEmployeesDryGoods;
        numEmployees["Frozen"] = NumberEmployeesFrozen;
        numEmployees["Dairy"] = NumberEmployeesDairy;
        numEmployees["Idle"] = NumberEmployeesIdle;
        numEmployees["Offsite"] = NumberEmployeesOffsite;
        //Debug.Log("Idle: " + numEmployees["Idle"] + "/" + NumberEmployeesIdle + " Offsite: " + numEmployees["Offsite"] + "/" + NumberEmployeesOffsite);


        // dont need to keep track of busy employees on registers, this is just for restocking purposes
        busyEmployees["Produce"] = 0;
        busyEmployees["Dry Goods"] = 0;
        busyEmployees["Frozen"] = 0;
        busyEmployees["Dairy"] = 0;

        totalOnShelves["Apples"] = TotalApplesOnShelves;
        totalOnShelves["Bananas"] = TotalBananasOnShelves;
        totalOnShelves["Cereal"] = TotalCerealOnShelves;
        totalOnShelves["Cookies"] = TotalCookiesOnShelves;
        totalOnShelves["Pizza"] = TotalPizzaOnShelves;
        totalOnShelves["Dessert"] = TotalDessertOnShelves;
        totalOnShelves["Milk"] = TotalMilkOnShelves;
        totalOnShelves["Cheese"] = TotalCheeseOnShelves;

        totalInBack["Apples"] = TotalApplesInBack;
        totalInBack["Bananas"] = TotalBananasInBack;
        totalInBack["Cereal"] = TotalCerealInBack;
        totalInBack["Cookies"] = TotalCookiesInBack;
        totalInBack["Pizza"] = TotalPizzaInBack;
        totalInBack["Dessert"] = TotalDessertInBack;
        totalInBack["Milk"] = TotalMilkInBack;
        totalInBack["Cheese"] = TotalCheeseInBack;

        // can change shipping costs here if you want, or move them to inspector inputs
        shippingCost["standard"] = 100.00m;
        shippingCost["expedited"] = 250.00m;

        // assigning references to text objects
        // may want to change this in the future, and have them passed in through inspector
        // that way you'll grab a direct reference rather than a copy (saves a bit of memory)
        RegistersText = GameObject.Find("RegistersAmount").GetComponent<Text>();
        ProduceText = GameObject.Find("ProduceAmount").GetComponent<Text>();
        DryGoodsText = GameObject.Find("DryGoodsAmount").GetComponent<Text>();
        FrozenText = GameObject.Find("FrozenAmount").GetComponent<Text>();
        DairyText = GameObject.Find("DairyAmount").GetComponent<Text>();
        IdleText = GameObject.Find("IdleAmount").GetComponent<Text>();
        OffsiteText = GameObject.Find("OffsiteAmount").GetComponent<Text>();

        RegisterDebug = GameObject.Find("RegisterDebug").GetComponent<Text>();

        ApplesTotal = GameObject.Find("ApplesAmountTotal").GetComponent<Text>();
        ApplesBack = GameObject.Find("ApplesAmountBack").GetComponent<Text>();
        ApplesFront = GameObject.Find("ApplesAmountFront").GetComponent<Text>();
        BananasTotal = GameObject.Find("BananasAmountTotal").GetComponent<Text>();
        BananasBack = GameObject.Find("BananasAmountBack").GetComponent<Text>();
        BananasFront = GameObject.Find("BananasAmountFront").GetComponent<Text>();
        CerealTotal = GameObject.Find("CerealAmountTotal").GetComponent<Text>();
        CerealBack = GameObject.Find("CerealAmountBack").GetComponent<Text>();
        CerealFront = GameObject.Find("CerealAmountFront").GetComponent<Text>();
        CookiesTotal = GameObject.Find("CookiesAmountTotal").GetComponent<Text>();
        CookiesBack = GameObject.Find("CookiesAmountBack").GetComponent<Text>();
        CookiesFront = GameObject.Find("CookiesAmountFront").GetComponent<Text>();
        PizzaTotal = GameObject.Find("PizzaAmountTotal").GetComponent<Text>();
        PizzaBack = GameObject.Find("PizzaAmountBack").GetComponent<Text>();
        PizzaFront = GameObject.Find("PizzaAmountFront").GetComponent<Text>();
        DessertTotal = GameObject.Find("DessertAmountTotal").GetComponent<Text>();
        DessertBack = GameObject.Find("DessertAmountBack").GetComponent<Text>();
        DessertFront = GameObject.Find("DessertAmountFront").GetComponent<Text>();
        MilkTotal = GameObject.Find("MilkAmountTotal").GetComponent<Text>();
        MilkBack = GameObject.Find("MilkAmountBack").GetComponent<Text>();
        MilkFront = GameObject.Find("MilkAmountFront").GetComponent<Text>();
        CheeseTotal = GameObject.Find("CheeseAmountTotal").GetComponent<Text>();
        CheeseBack = GameObject.Find("CheeseAmountBack").GetComponent<Text>();
        CheeseFront = GameObject.Find("CheeseAmountFront").GetComponent<Text>();
        
        foods["Apples"] = apples;
        foods["Bananas"] = bananas;
        foods["Cereal"] = cereal;
        foods["Cookies"] = cookies;
        foods["Pizza"] = pizza;
        foods["Dessert"] = dessert;
        foods["Milk"] = milk;
        foods["Cheese"] = cheese;

        graphs.Begin();
        for (int i = 0; i < 4; i++)
            ShiftInfo[i] = 2;
        
       

        // grabs total inital amounts
        calculateTotalEmployees();
        calculateTotalProduct();
        calculateTotalInBackandFront();
        updateTextValues();
    }


  
    // description: is called from time controller, will constantly pull available items off shelf and bring them to registers
    public void PullItemsOffShelf()
    {
        int totalTakenOff = 0;
        // pull each of the foods
        for (int i = 0; i < FOODS.Length; i++)
        {
            // Default: random number between 1 and 3
            // New Range
            int rand = UnityEngine.Random.Range(0, FoodTakenFromShelvesPerCheckOut + 1);
            //print("Buying " + rand + " " + FOODS[i]);

            // if there are enough items on shelves to pull off the random amount
            if (!(totalOnShelves[FOODS[i]] - rand < 0))
            {
                totalOnShelves[FOODS[i]] -= rand;
                totalTakenOff += rand;

                // bring the random amount of foods to the registers (add it to the queue)
                for (int j = 0; j < rand; j++)
                {
                    string f = FOODS[i];
                    registers.bringFoodToRegisters(foods[FOODS[i]]);
                }
            }
            else
            {
                // take whatever is left
                int leftOnShelves = totalOnShelves[FOODS[i]];
                totalTakenOff += totalOnShelves[FOODS[i]];
                totalOnShelves[FOODS[i]] -= totalOnShelves[FOODS[i]];

                // bring whatever is left to the registers (add it to the queue)
                for (int j = 0; j < leftOnShelves; j++)
                {
                    registers.bringFoodToRegisters(foods[FOODS[i]]);
                }
            }
        }
        registers.distributeBaseQueue(NumberEmployeesRegisters);
        // print("Total taken off = " + totalTakenOff);
        updateFoodStocks();
        updateCash();
        // registers.newItems(totalTakenOff);
    }

    // called from time controller every in game hour
    public void payEmployees(int hour)
    {
        Scene CurrentScene = SceneManager.GetActiveScene();
        
        int full_employee_payout;

        if (CurrentScene.name == "Simulation")
        {
            full_employee_payout = PayRate * totalCurrentEmployees;
            Cash -= full_employee_payout;

            PostDayManager.money_spent_paying_employees += full_employee_payout;
        }
        else
        {
            // TODO: Fix this section to work with 3 shifts
            // If this hour is in shift one, pay the employees working for shift 1
            if(hour <= (StartMilitaryTime + EndMilitaryTime) / 2)
            {
                totalCurrentEmployees = ShiftInfo[0] + ShiftInfo[1];

                full_employee_payout = PayRate * totalCurrentEmployees;
                Cash -= full_employee_payout;

                PostDayManager.money_spent_paying_employees += full_employee_payout;
            }
            // Currently in shift 2, Pay Shift 2 Workers
            else
            {
                totalCurrentEmployees = ShiftInfo[2] + ShiftInfo[3];


                full_employee_payout = PayRate * totalCurrentEmployees;
                Cash -= full_employee_payout;

                PostDayManager.money_spent_paying_employees += full_employee_payout;
            }
        }
        //Debug.Log("Payout for hour" + TimeC.Hour + " : " + full_employee_payout);
        //Debug.Log("Contents: " + ShiftInfo[0] + " " + ShiftInfo[1] + " " + ShiftInfo[2] + " " + ShiftInfo[3]);

        updateCash();
    }

    // all values displayed on screen are text based
    // so need to convert int to strings everytime one of them gets updated
    void updateTextValues()
    {
        updateNumEmployees();
        updateFoodStocks();
        updateCash();
    }

    // input: food object to get its name and department, quantity to take from back and move to front
    // output: success or error message
    // descrption: takes available stock from back and moves it to the front, need enough stock and employees for it to succeed
    public string restock(Food food)
    {
        int numAvailableEmployees = numEmployees["Idle"];

        // no employeed to fullfill the request
        if(numEmployees["Idle"] == 0)
        {
            return $"Error: There are no free employees to restock {food.getItemDescription()}.";
        }

        
        // cant restock if the shelf is full
        if(totalOnShelves[food.getItemDescription()] == MaximumProductOnShelves)
        {
            return $"Error: Your shelf area for {food.getItemDescription()} is full.";
        }

        // can't restock if there is nothing in the back
        if (totalInBack[food.getItemDescription()] == 0)
        {
            return $"Error: Not enough food in the back. Try making a delivery first.";
        }


        // restock the full quantity
        Debug.Log($"Employee started restocking {food.getItemDescription()}");

        StartCoroutine(Restocking(food)); // mark that employee as busy

        return "Success";
    }

    // TODO: Make RestockingTime equal to game time scale (E.g. 30 minutes should be asjusted to a few seconds)
    IEnumerator Restocking(Food food)
    {
        string itemName = food.getItemDescription(), dept = food.getDepartment();
        //float quantityMultiplier = 19/20; // if quantity is 25 then cooldown time becomes 2.5x longer, 50 would take 5x longer

        float scaledTime = (float)(RestockingTimeInMinutes) * (float)((RestockingPeriod)/5); // the larger the multiplier, the longer it takes to restock. Default: RestockingPeriod = 1 

        numEmployees["Idle"]--;
        numEmployees[dept]++;
        updateNumEmployees();

        int amountInBack = totalInBack[food.getItemDescription()], amountInFront = totalOnShelves[food.getItemDescription()], maxFront = food.getMaxFrontQuantity();
        int restockAmount;

        // wait for the employee to finish task and make them available again
        // delete this soon: while (totalOnShelves[food.getItemDescription()] != MaximumProductOnShelves && totalInBack[food.getItemDescription()] != 0)
        while (amountInFront != maxFront && amountInBack != 0)
        {
            // amount we want to restock 
            restockAmount = Mathf.Min(RestockBatchSize, maxFront - amountInFront, amountInBack);

            totalOnShelves[itemName] += restockAmount;
            totalInBack[itemName] -= restockAmount;
            updateFoodStocks();
            amountInBack = totalInBack[food.getItemDescription()];
            amountInFront = totalOnShelves[food.getItemDescription()];
            yield return new WaitForSeconds(scaledTime);
        }
        
        Debug.Log($"Employee is done restocking {itemName}.");

        numEmployees[dept]--;
        numEmployees["Idle"]++;
        updateNumEmployees();
    }

    // input: food object, delivery quantity, standard or expedited delivery type
    // output: success or error message
    // description: adds a new delivery to the delivery list
    public string makeNewDelivery(Food food, int quantity, string deliveryType)
    {
        int deliveryTime = deliveryType == "standard" ? 8 : TimeC.Hour + 5;

        //if TimeC.Hour + 5, expedited becomes the same as standard
        if (deliveryTime >= 20)
        {
            deliveryTime = 8;
            deliveryType = "standard";
        }

        decimal totalCost = (quantity * food.getUnitPriceFarmer()) + shippingCost[deliveryType];


        // gets quantity of deliveries in queue
        int quantityInQueue = TimeController.quantityInQueue();

        if ((float)totalCost > Cash)
        {
            return $"Error: Not enough money left. Delivery cost: ${totalCost}";
        }

        //I have this inventory check in two different statements, this and the one below. It could be combined into one long && statement but I'd rather not. #Coronavirus_3/25/2020
        if (totalCurrentProduct + quantity + quantityInQueue > MaximumProductInStore)
        {
            print("current product = " + totalCurrentProduct);
            string numLeft = (MaximumProductInStore - totalCurrentProduct - quantityInQueue).ToString();
            return $"Error: Max of {MaximumProductInStore} product in store. With {quantityInQueue} items currently being delivered, you can order up to {numLeft} more.";
        }

        if (quantity + totalInBack[food.getItemDescription()] + TimeController.quantityInQueue(food.getItemDescription()) > food.getMaxBackQuantity()) 
        {
            print("current product in back = " + totalInBack[food.getItemDescription()]);
            string numLeft = ( food.getMaxBackQuantity() - totalInBack[food.getItemDescription()] - TimeController.quantityInQueue( food.getItemDescription() ) ).ToString();
            return $"Error: Max of {MaximumProductInStore} of product allowed in back. With {TimeController.quantityInQueue(food.getItemDescription())} items currently being delivered, you can order up to {numLeft} more.";

        }
        
        TimeController.addDelivery(food, quantity, deliveryTime); // add to the list, this is checked every morning
        Cash -= (float)totalCost; // you pay for the delivery when its ordered

        PostDayManager.total_spent_on_deliveries += totalCost;
        updateFoodStocks();
        updateCash();
        return "Success";
    }

    // input: food and amount to be delivered
    // description: adds the quantity to the back of house
    public void timeForDelivery(Food food, int quantity)
    {
        totalInBack[food.getItemDescription()] += quantity;
        updateFoodStocks();
    }

    // input: department to pull an employee from, department to assign new employee to
    // output: success or error message
    // description: swaps 1 employee from/to a deprtment of choice
    public string reassignEmployee(string fromDepartment, string toDepartment)
    {
        // cant reassign from a department with 0 employees
        if (numEmployees[fromDepartment] == 0)
        {
            return $"Error: Can't reassign employee. {fromDepartment} has 0 employees.";
        }
        // max 3 registers
        if (toDepartment == "Registers" && numEmployees["Registers"] == MaximumRegisters)
        {
            return $"Error: Max of {MaximumRegisters} registers. Try reassigning someone from the register first.";
        }

        // minimum 1 register
        if (fromDepartment == "Registers" && numEmployees["Registers"] == MinimumRegisters)
        {
            return $"Error: There needs to be at least {MinimumRegisters} person at the register.";
        }
        
        if (fromDepartment.CompareTo("Offsite") == 0 && toDepartment.CompareTo("Idle") == 0)
        {
            totalCurrentEmployees++;
        }

        numEmployees[fromDepartment]--;
        numEmployees[toDepartment]++;

        updateNumEmployees();
        return "Success";
    }

    // input: department to call employees into and amount
    // output: success or error message
    // description: calls a new employee to a department, happens instantaneously but probably shouldn't
    public string callInEmployees(int quantity)
    {
        // cant have too many employees
        if (totalCurrentEmployees + quantity + TimeController.TravellingEmployees.Count > MaximumEmployees)
        {
            string numLeft = (MaximumEmployees - totalCurrentEmployees).ToString();
            return $"Error: Max of {MaximumEmployees} total employees.";
        }

        /*
        // max 3 registers
        if (department == "Registers" && numEmployees["Registers"] == MaximumRegisters)
        {
            return $"Error: Max of {MaximumRegisters} registers. Try reassigning someone from the register first.";
        }
        // max 3 registers
        if (department == "Registers" && numEmployees["Registers"] + quantity > MaximumRegisters)
        {
            string numLeft = (MaximumRegisters - numEmployees["Registers"]).ToString();
            return $"Error: Max of {MaximumRegisters} registers. You can call in up to {numLeft} more.";
        }
        */

        if (quantity > numEmployees["Offsite"])
        {
            return $"Error: You only have {numEmployees["Offsite"]} off-site to call in.";
        }

        TimeC.addTravellingEmployee();
        return "Success";
    }

    public void EmployeeArrives() 
    {

        numEmployees["Idle"] += 1;
        numEmployees["Offsite"] -= 1;
        //totalCurrentEmployees += 1;

        updateNumEmployees();

    }

    // input: department to release employees from and amount
    // output: success or error message
    // description: releases employees from a department of choice
    public string releaseEmployees(int quantity)
    {
        // need to have at least 4 employees
        if (totalCurrentEmployees - quantity < MinimumEmployees)
        {
            //string numLeft = (totalCurrentEmployees - MinimumEmployees).ToString();
            string numLeft = (totalCurrentEmployees - quantity).ToString();

            //return $"Error: Min of {MinimumEmployees} employees needed in store. You can release up to {numLeft} more.";
            return $"Error: Min of {MinimumEmployees} allowed.";
            //return $"Error: Min of {MinimumEmployees}. Total of {totalCurrentEmployees}. If you removed, u'd have {numLeft}";
            //You'd have 3???
        }

        // cant remove more employees than exist in that department
        if (numEmployees["Idle"] - quantity < 0)
        {
            return $"Error: Can't release {quantity} employee. Only {numEmployees["Idle"]} are idle and can be released.";
        }

        numEmployees["Idle"] -= quantity;
        numEmployees["Offsite"] += quantity;

        //totalCurrentEmployees -= 1;

        updateNumEmployees();
        return "Success";
    }



    // gets called from time controller at the end of each day
    public void expireFoods()
    {
        total_expired = 0;
        float cost_of_expired = 0;
        for (int i = 0; i < FOODS.Length; i++)
        {
            // these values may need to be updated to draw from a distribution
            float percentGoneBad = UnityEngine.Random.Range(.05f, .10f); // less items from back go bad
            int front_expired = (int)(totalInBack[FOODS[i]] * percentGoneBad);
            totalInBack[FOODS[i]] -= front_expired;
            
            total_expired += front_expired;

            percentGoneBad = UnityEngine.Random.Range(.20f, .25f); // more items in front go bad
            int back_expired = (int)(totalOnShelves[FOODS[i]] * percentGoneBad);
            totalOnShelves[FOODS[i]] -= back_expired;
            
            total_expired += back_expired;
            PostDayManager.cost_of_expired_food += (float)foods[FOODS[i]].getUnitPriceFarmer() * (front_expired + back_expired) ;
        }
        PostDayManager.count_of_expired_food += total_expired;

        updateFoodStocks();
        updateCash();
    }

    // needed to update text on screen
    public void updateCash()
    {
        // make text red
        if (Cash < 0)
        {
            CashText.color = Color.red;
            CashText.text = "-$" + string.Format("{0:n}", Mathf.Abs(Cash));
        }
        else
        {
            CashText.text = "$" + string.Format("{0:n}", Cash);
        }
    }

    // needed to update text on screen
    public void updateNumEmployees()
    {
        // updates the text on screen
        RegistersText.text = "Employees on Register: " + numEmployees["Registers"].ToString();
        ProduceText.text = numEmployees["Produce"].ToString();
        DryGoodsText.text = numEmployees["Dry Goods"].ToString();
        FrozenText.text = numEmployees["Frozen"].ToString();
        DairyText.text = numEmployees["Dairy"].ToString();
        IdleText.text = numEmployees["Idle"].ToString();
        OffsiteText.text = numEmployees["Offsite"].ToString();
        // Debug.Log("Jackson's here in Sim 521" + numEmployees["Offsite"]);
        RegisterDebug.text = "Registers: " + numEmployees["Registers"].ToString();

        // used for other controllers and updating values in inspector
        NumberEmployeesRegisters = numEmployees["Registers"];
        NumberEmployeesProduce = numEmployees["Produce"];
        NumberEmployeesDryGoods = numEmployees["Dry Goods"];
        NumberEmployeesFrozen = numEmployees["Frozen"];
        NumberEmployeesDairy = numEmployees["Dairy"];
        NumberEmployeesIdle = numEmployees["Idle"];
        NumberEmployeesOffsite = numEmployees["Offsite"];

        //updates corresponding graphs
        graphs.UpdateGraphs();

        calculateTotalEmployees();
        CalculateActiveEmployees();
    }

    // needed to update text on screen
    void updateFoodStocks()
    {
        ApplesTotal.text = (totalOnShelves["Apples"] + totalInBack["Apples"]).ToString();
        ApplesBack.text = totalInBack["Apples"].ToString();
        ApplesFront.text = totalOnShelves["Apples"].ToString();
        BananasTotal.text = (totalOnShelves["Bananas"] + totalInBack["Bananas"]).ToString();
        BananasBack.text = totalInBack["Bananas"].ToString();
        BananasFront.text = totalOnShelves["Bananas"].ToString();
        CerealTotal.text = (totalOnShelves["Cereal"] + totalInBack["Cereal"]).ToString();
        CerealBack.text = totalInBack["Cereal"].ToString();
        CerealFront.text = totalOnShelves["Cereal"].ToString();
        CookiesTotal.text = (totalOnShelves["Cookies"] + totalInBack["Cookies"]).ToString();
        CookiesBack.text = totalInBack["Cookies"].ToString();
        CookiesFront.text = totalOnShelves["Cookies"].ToString();
        PizzaTotal.text = (totalOnShelves["Pizza"] + totalInBack["Pizza"]).ToString();
        PizzaBack.text = totalInBack["Pizza"].ToString();
        PizzaFront.text = totalOnShelves["Pizza"].ToString();
        DessertTotal.text = (totalOnShelves["Dessert"] + totalInBack["Dessert"]).ToString();
        DessertBack.text = totalInBack["Dessert"].ToString();
        DessertFront.text = totalOnShelves["Dessert"].ToString();
        MilkTotal.text = (totalOnShelves["Milk"] + totalInBack["Milk"]).ToString();
        MilkBack.text = totalInBack["Milk"].ToString();
        MilkFront.text = totalOnShelves["Milk"].ToString();
        CheeseTotal.text = (totalOnShelves["Cheese"] + totalInBack["Cheese"]).ToString();
        CheeseBack.text = totalInBack["Cheese"].ToString();
        CheeseFront.text = totalOnShelves["Cheese"].ToString();

        calculateTotalInBackandFront();

        //updates corresponding graphs
        graphs.UpdateGraphs();

        calculateTotalProduct();
        
    }

    public void updateCheckoutItems()
    {
        CheckoutItemsText.text = registers.itemsAtCheckout.ToString();
    }

    // needs to be recalculated after calling in or releasing employees
    void calculateTotalEmployees()
    {
        totalCurrentEmployees = 0;
        // Only count onsite employees
        for (int i = 0; i < DEPARTMENTS.Length - 1; i++)
        {
            totalCurrentEmployees += numEmployees[DEPARTMENTS[i]];
        }
    }

    // needs to be calculated to make sure we dont overstock the warehouse through deliveries
    void calculateTotalProduct()
    {
        totalCurrentProduct = 0;
        for (int i = 0; i < FOODS.Length; i++)
        {
            totalCurrentProduct += (totalInBack[FOODS[i]] + totalOnShelves[FOODS[i]]);
        }
    }

    void calculateTotalInBackandFront()
    {
        totalInFront_value = 0;
        totalInBack_value = 0;

        foreach (KeyValuePair<string, int> item in totalOnShelves)
            totalInFront_value += item.Value;

        foreach (KeyValuePair<string, int> item in totalInBack)
            totalInBack_value += item.Value;

        Debug.Log("Total in front: " + totalInFront_value);
        Debug.Log("Total in back: " + totalInBack_value);
    }

    // TODO: Make RestockingTime equal to game time scale (E.g. 30 minutes should be asjusted to a few seconds)
    IEnumerator BusyEmployee(string department, int quantity)
    {
        float quantityMultiplier = quantity / 10; // if quantity is 25 then cooldown time becomes 2.5x longer, 50 would take 5x longer

        float scaledTime = (float)(RestockingTimeInMinutes * 16.7) * quantityMultiplier; // play around with this but its about an hour in the sim
        // wait for the employee to finish task and make them available again
        yield return new WaitForSeconds(scaledTime);
        Debug.Log($"{department} employee is done restocking");
        busyEmployees[department]--;
    }

    public int[] GetShiftInfo()
    {
        return ShiftInfo;
    }

    public void SetShiftInfo(int idx, int value)
    {
        ShiftInfo[idx] = value;
    }

    public void CalculateActiveEmployees()
    {
        TotalActiveEmployees = numEmployees["Registers"] + numEmployees["Produce"] + numEmployees["Dry Goods"] + numEmployees["Frozen"] + numEmployees["Dairy"];

        return;
    }

}
