using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodItem : MonoBehaviour
{
    public int Department { get; set; }
    public string Name { get; set; }
    public decimal UnitPriceCustomer { get; set; }
    public decimal UnitPriceFarmer { get; set; }
    public int MaxFOH { get; set; }
    public int MaxBOH { get; set; }
    public int StockFOH { get; set; }
    public int StockBOH { get; set; }

    public FoodItem(int department, string name, decimal unitPriceCustomer, decimal unitPriceFarmer, int maxFOH, 
                        int maxBOH, int stockFOH, int stockBOH)
    {
        Department = department;
        Name = name;
        UnitPriceCustomer = unitPriceCustomer;
        UnitPriceFarmer = unitPriceFarmer;
        MaxFOH = maxFOH;
        MaxBOH = maxBOH;
        StockFOH = stockFOH;
        StockBOH = stockBOH;
    }

    public FoodItem(FoodItem food)
    {
        this.Department = food.Department;
        this.Name = food.Name;
        this.UnitPriceCustomer = food.UnitPriceCustomer;
        this.UnitPriceFarmer = food.UnitPriceFarmer;
        this.MaxBOH = food.MaxBOH;
        this.MaxFOH = food.MaxFOH;
        this.StockBOH = food.StockBOH;
        this.StockFOH = food.StockFOH;
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
