using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food
{
    string department;
    string departmentCode;
    string itemDescription;
    string itemCode;
    string SKU;
    int totalInitialQuantity;
    decimal unitPriceCustomer;
    decimal unitPriceFarmer;
    decimal totalPrice;
    int maxFrontQuantity;
    int maxBackQuantity;

    // constructor method
    public Food(string _department, string _departmentCode, string _itemDescription, string _itemCode, string _SKU, int _totalInitialQuantity, decimal _unitPriceCustomer, 
        decimal _unitPriceFarmer, decimal _totalPrice, int _maxFrontQuantity, int _maxBackQuantity)
    {
        department = _department;
        departmentCode = _departmentCode;
        itemDescription = _itemDescription;
        itemCode = _itemCode;
        SKU = _SKU;
        totalInitialQuantity = _totalInitialQuantity;
        //below is the value calculated by registers at checkout
        unitPriceCustomer = _unitPriceCustomer;
        unitPriceFarmer = _unitPriceFarmer;
        totalPrice = _totalPrice;
        maxFrontQuantity = _maxFrontQuantity;
        maxBackQuantity = _maxBackQuantity;
    }

    // setters
    public decimal SetUnitPriceCustomer(decimal newValue) 
    {
        return unitPriceCustomer = newValue;
    }

    public decimal SetUnitPriceFarmer(decimal newValue) 
    {
        return unitPriceFarmer = newValue;
    }

    // getters
    public string getDepartment()
    {
        return department;
    }

    public string getDepartmentCode()
    {
        return departmentCode;
    }

    public string getItemDescription()
    {
        return itemDescription;
    }

    public string getItemCode()
    {
        return itemDescription;
    }

    public string getSKU()
    {
        return SKU;
    }

    public int getTotalInitialQuantity()
    {
        return totalInitialQuantity;
    }

    public decimal getUnitPriceCustomer()
    {
        return unitPriceCustomer;
    }

    public decimal getUnitPriceFarmer()
    {
        return unitPriceFarmer;
    }

    public decimal getTotalPrice()
    {
        return getTotalPrice();
    }

    public int getMaxFrontQuantity()
    {
        return maxFrontQuantity;
    }

    public int getMaxBackQuantity()
    {
        return maxBackQuantity;
    }
}
