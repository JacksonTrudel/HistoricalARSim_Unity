using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delivery : MonoBehaviour
{
    public FoodItem foodItem { get; set; }
    public int Quantity { get; set; }
    public bool Expedited { get; set; }
    public decimal Cost { get; set; }
    public int OrderDate { get; set; }
    public int ArrivalDate { get; set; }

    public static int StdShpCost = 100;
    public static int ExpShpCost = 250;

    public Delivery(FoodItem food, int quantity)
    {
        foodItem = food;
        Quantity = quantity;
        // default is standard delivery
        Expedited = false;
        Cost = (foodItem.UnitPriceFarmer * Quantity) + (Expedited ? ExpShpCost : StdShpCost);
        OrderDate = SimController.DayNum;
        ArrivalDate = SimController.DayNum + 1;
    }

    public decimal SetQuantity(int quant)
    {
        Quantity = quant;
        decimal oldCost = Cost;
        Cost = (foodItem.UnitPriceFarmer * Quantity) + (Expedited ? ExpShpCost : StdShpCost);
        return Cost - oldCost;
    }

    public decimal SetExpedited(bool a)
    {
        decimal oldCost = Cost;
        Expedited = a;
        Cost = (foodItem.UnitPriceFarmer * Quantity) + (Expedited ? ExpShpCost : StdShpCost);
        ArrivalDate = a ? OrderDate : (OrderDate + 1);
        return Cost - oldCost;
    }

    public bool GetExpedited()
    {
        return Expedited;
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
