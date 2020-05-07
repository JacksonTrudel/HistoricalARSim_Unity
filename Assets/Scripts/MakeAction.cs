using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakeAction : MonoBehaviour
{
    // variables in inspector
    public GameObject MenuStart; // links back to the start menu after selecting an action
    public Component ErrorMessage; // can be used to directly link error message component rather than search for it on start

    //to be continued. How can I convert these to decimals. Don't want to keep them as floats
    [Header("The Following Menu is non-functional as of 4/9/20")]

    [Header("Selling Price Settings")]
    public float CustomerPrice_Apple;
    public float CustomerPrice_Banana;
    public float CustomerPrice_Cereal;
    public float CustomerPrice_Cookies;
    public float CustomerPrice_Pizza;
    public float CustomerPrice_Dessert;
    public float CustomerPrice_Milk;
    public float CustomerPrice_Cheese;

    [Header("Buying Price Settings")]
    public float FarmerPrice_Apple;
    public float FarmerPrice_Banana;
    public float FarmerPrice_Cereal;
    public float FarmerPrice_Cookies;
    public float FarmerPrice_Pizza;
    public float FarmerPrice_Dessert;
    public float FarmerPrice_Milk;
    public float FarmerPrice_Cheese;

    //sadly Unity doesn't allow decimals in the inspector

    decimal CustomerPrice_Banana_Decimal { get { return (decimal)CustomerPrice_Banana; } }
    decimal CustomerPrice_Cereal_Decimal { get { return (decimal)CustomerPrice_Cereal; } }
    decimal CustomerPrice_Cookies_Decimal { get { return (decimal)CustomerPrice_Cookies; } }
    decimal CustomerPrice_Pizza_Decimal { get { return (decimal)CustomerPrice_Pizza; } }
    decimal CustomerPrice_Dessert_Decimal { get { return (decimal)CustomerPrice_Dessert; } }
    decimal CustomerPrice_Milk_Decimal { get { return (decimal)CustomerPrice_Milk; } }
    decimal CustomerPrice_Cheese_Decimal { get { return (decimal)CustomerPrice_Cheese; } }

    decimal FarmerPrice_Apple_Decimal { get { return (decimal)FarmerPrice_Apple; } }
    decimal FarmerPrice_Banana_Decimal { get { return (decimal)FarmerPrice_Banana; } }
    decimal FarmerPrice_Cereal_Decimal { get { return (decimal)FarmerPrice_Cereal; } }
    decimal FarmerPrice_Cookies_Decimal { get { return (decimal)FarmerPrice_Cookies; } }
    decimal FarmerPrice_Pizza_Decimal { get { return (decimal)FarmerPrice_Pizza; } }
    decimal FarmerPrice_Dessert_Decimal { get { return (decimal)FarmerPrice_Dessert; } }
    decimal FarmerPrice_Milk_Decimal { get { return (decimal)FarmerPrice_Milk; } }
    decimal FarmerPrice_Cheese_Decimal { get { return (decimal)FarmerPrice_Cheese; } }

    // instaniating game objects
    Simulation sim;
    TimeController time;
    ShowErrorMessage error;
    RegisterController registers;
    LogController logContrl;

    // object references to foods to pass into action methods
    // you may make changes to constructor in Food class as needed

    //for reference
    //public Food(string _department, string _departmentCode, string _itemDescription, string _itemCode, string _SKU, int _totalInitialQuantity, decimal _unitPriceCustomer, decimal _unitPriceFarmer, decimal _totalPrice)
    Food apples = new Food("Produce", "PR", "Apples", "A2", "PR-A2", 150, 1.00m, 0.15m, 148.50m, 50, 100);
    Food bananas = new Food("Produce", "PR", "Bananas", "A2", "PR-A2", 150, 1.00m, 0.10m, 148.50m, 50, 100);
    Food cereal = new Food("Dry Goods", "PR", "Cereal", "A2", "PR-A2", 150, 3.00m, 0.20m, 148.50m, 50, 200);
    Food cookies = new Food("Dry Goods", "PR", "Cookies", "A2", "PR-A2", 150, 3.00m, 0.20m, 148.50m, 50, 200);
    Food pizza = new Food("Frozen", "PR", "Pizza", "A2", "PR-A2", 150, 6.00m, 0.50m, 148.50m, 50, 200);
    Food dessert = new Food("Frozen", "PR", "Dessert", "A2", "PR-A2", 150, 5.00m, 0.25m, 148.50m, 50, 200);
    Food milk = new Food("Dairy", "PR", "Milk", "A2", "PR-A2", 150, 3.00m, 0.20m, 148.50m, 50, 100);
    Food cheese = new Food("Dairy", "PR", "Cheese", "A2", "PR-A2", 150, 4.00m, 0.50m, 148.50m, 50, 100);

    // hashmap to reduce need for individual variables
    Dictionary<string, Food> foodMappings = new Dictionary<string, Food>();

    void Start()
    {
        //activates our setters in Food.cs
        apples.SetUnitPriceFarmer( (decimal) FarmerPrice_Apple);
        bananas.SetUnitPriceFarmer( (decimal) FarmerPrice_Banana );
        cereal.SetUnitPriceFarmer( (decimal) FarmerPrice_Cereal);
        cookies.SetUnitPriceFarmer( (decimal) FarmerPrice_Cookies);
        pizza.SetUnitPriceFarmer( (decimal) FarmerPrice_Pizza);
        dessert.SetUnitPriceFarmer( (decimal) FarmerPrice_Dessert);
        milk.SetUnitPriceFarmer( (decimal) FarmerPrice_Milk);
        cheese.SetUnitPriceFarmer( (decimal) FarmerPrice_Cheese);

        apples.SetUnitPriceCustomer( (decimal) CustomerPrice_Apple);
        bananas.SetUnitPriceCustomer( (decimal) CustomerPrice_Banana);
        cereal.SetUnitPriceCustomer( (decimal) CustomerPrice_Cereal);
        cookies.SetUnitPriceCustomer( (decimal) CustomerPrice_Cookies);
        pizza.SetUnitPriceCustomer( (decimal) CustomerPrice_Pizza);
        dessert.SetUnitPriceCustomer( (decimal) CustomerPrice_Dessert);
        milk.SetUnitPriceCustomer( (decimal) CustomerPrice_Milk);
        cheese.SetUnitPriceCustomer( (decimal) CustomerPrice_Cheese);

        // grabbing references to game objects, used to call their respective scripts
        sim = GameObject.Find("Simulation").GetComponent<Simulation>();
        time = GameObject.Find("UI Canvas").GetComponent<TimeController>();
        logContrl = GameObject.Find("Simulation").GetComponent<LogController>();
        error = GameObject.Find("ErrorMessage").GetComponent<ShowErrorMessage>();
        registers = GameObject.Find("Simulation").GetComponent<RegisterController>();

        // create mappings so we can easily get a reference to each object by passing in a string
        foodMappings["Apples"] = apples;
        foodMappings["Bananas"] = bananas;
        foodMappings["Cereal"] = cereal;
        foodMappings["Cookies"] = cookies;
        foodMappings["Pizza"] = pizza;
        foodMappings["Dessert"] = dessert;
        foodMappings["Milk"] = milk;
        foodMappings["Cheese"] = cheese;
    }

    // input: button game object to get it's name. we can then parse the name and execute it's action (E.g. "Reassign Produce Dairy" where "Reassign" = action name)
    // description: finds what action to execute based on the button pressed by user. then calls respective method in Simulation script
    public void Action(Button button)
    {
        string callback = null; // return value of Simulation method
        string logMessage = null;
        string fromDepartment, toDepartment;
        int day = TimeController.Day;
        int hour = time.Hour;
        int minute = time.Minute;
        string[] actions = button.name.Split(' ');
        
        switch (actions[0])
        {
            case "Restock":
                if(actions[2].CompareTo("No") == 0)
                {
                    callback = "Success";
                    break;
                }
                Food food = foodMappings[actions[1]]; // grab food object
                
                callback = sim.restock(food);


                logMessage = $"Restocking {food.getItemDescription().ToLower()}: {callback}";
                logContrl.Print(day, hour, minute, logMessage);
                break;
            case "Reassign":
                fromDepartment = actions[1];
                toDepartment = actions[2];
                // cant have space separated for split function
                if (fromDepartment == "DryGoods") fromDepartment = "Dry Goods";
                if (toDepartment == "DryGoods") toDepartment = "Dry Goods";
                callback = sim.reassignEmployee(fromDepartment, toDepartment);

                logMessage = $"Reassigning employee from {fromDepartment.ToLower()} to {toDepartment.ToLower()}: {callback}";
                logContrl.Print(day, hour, minute, logMessage);
                break;
            case "Call":
                // If they disapproved the confimation message
                if(actions[2].CompareTo("No") == 0)
                {
                    callback = "Success";
                    break;
                }
                
                // currently calls in only 1 employee
                callback = sim.callInEmployees(1);
                logMessage = $"Calling in an employee to Idle: {callback}";
                // logMessage = $"Calling in {callInQuantity} employee(s) to {callInDepartment.ToLower()}: {callback}";
                logContrl.Print(day, hour, minute, logMessage);
                break;
            case "Release":
                /*
                string releaseDepartment = actions[1];
                int releaseQuantity = int.Parse(actions[3]);
                if (releaseDepartment == "DryGoods") releaseDepartment = "Dry Goods";
                */
                int quantity = 1;
                if (actions[1].CompareTo("No") == 0)
                {
                    callback = "Success";
                    break;
                }

                callback = sim.releaseEmployees(quantity);

                logMessage = $"Releasing {quantity} employee(s) from Idle: {callback}";
                logContrl.Print(day, hour, minute, logMessage);
                break;
            case "Back":
                break;
            case "Open":

                if(actions[2].CompareTo("No") == 0)
                {
                    callback = "Success";
                    break;
                }
                fromDepartment = "Idle";
                toDepartment = "Registers";

                callback = sim.reassignEmployee(fromDepartment, toDepartment);

                logMessage = $"Reassigning employee from {fromDepartment.ToLower()} to {toDepartment.ToLower()}: {callback}";

                logContrl.Print(day, hour, minute, logMessage);
                
                break;
            case "Close":
                if(actions[2].CompareTo("Yes") == 0)
                {
                    fromDepartment = "Registers";
                    toDepartment = "Idle";

                    callback = sim.reassignEmployee(fromDepartment, toDepartment);
                    registers.closeRegister();
                    logMessage = $"Reassigning employee from {fromDepartment.ToLower()} to {toDepartment.ToLower()}: {callback}";
                    logContrl.Print(day, hour, minute, logMessage);
                }
                break;
            default: // delivery
                Food deliveryFood = foodMappings[actions[0]];
                int deliveryQuantity = int.Parse(actions[1]);
                string deliveryType = actions[2].ToLower();
                callback = sim.makeNewDelivery(deliveryFood, deliveryQuantity, deliveryType);

                logMessage = $"Ordered {deliveryType} delivery for {deliveryQuantity} {deliveryFood.getItemDescription().ToLower()}: {callback}";
                logContrl.Print(day, hour, minute, logMessage);
                break;
        }

        // callback is either "success" or an error message
        if (callback != "Success")
        {
            error.Show(callback);
        }

        // show main menu
        MenuStart.SetActive(true);
         
        // hide current menu
        gameObject.SetActive(false);
        
    }


}
