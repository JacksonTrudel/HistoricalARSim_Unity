using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChartAndGraph;
using UnityEngine.UI;


public class GraphController : MonoBehaviour
{
    /*
    public PieChart Apple_pie;
    public PieChart Banana_pie;
    public PieChart Cereal_pie;
    public PieChart Cookie_pie;
    public PieChart Pizza_pie;
    public PieChart Dessert_pie;
    public PieChart Milk_pie;
    public PieChart Cheese_pie;
    public PieChart Stock_pie;

    [Header("Graphs on Main Screen")]
    public PieChart EmployeeStatusPie;
    public BarChart DepartmentStatusBar;

    // public Text ProduceButtonText;
    // public Text DryButtonText;
    // public Text FrozenButtonText;
    // public Text DairyButtonText;

    public Text ProduceHeaderText;
    public Text DryHeaderText;
    public Text FrozenHeaderText;
    public Text DairyHeaderText;

    Simulation Sim;



    // Start is called before the first frame update
    public void Begin()
    {
        Sim = GameObject.Find("Simulation").GetComponent<Simulation>();

        
        //maybe useful?
        //ArrayList Test = new ArrayList() {Apple_pie, Banana_pie, Cereal_pie, Cookie_pie, Pizza_pie, Dessert_pie, Milk_pie, Cheese_pie };

        //Critical Graphs
        EmployeeStatusPie.DataSource.SetValue("Idle", Sim.numEmployees["Idle"]);
        EmployeeStatusPie.DataSource.SetValue("Offsite", Sim.numEmployees["Offsite"]);
        EmployeeStatusPie.DataSource.SetValue("Active", Sim.TotalActiveEmployees);

        for (int i = 0; i < Sim.DEPARTMENTS.Length - 1; i++)
        {
            DepartmentStatusBar.DataSource.SetValue(Sim.DEPARTMENTS[i], "All", Sim.numEmployees[Sim.DEPARTMENTS[i]] );
        }
        

        //Produce
            //Apples          
                Apple_pie.DataSource.SetValue("Front", Simulation.totalOnShelves["Apples"]);
                Apple_pie.DataSource.SetValue("Back", Simulation.totalInBack["Apples"]);
            //Bananas
                Banana_pie.DataSource.SetValue("Front", Simulation.totalOnShelves["Bananas"]);
                Banana_pie.DataSource.SetValue("Back", Simulation.totalInBack["Bananas"]);
            
            //Employees in department
                ProduceHeaderText.text = "Produce Department\n Employees working: " + Sim.numEmployees["Produce"];
        //Dry
            //Cookies          
                Cookie_pie.DataSource.SetValue("Front", Simulation.totalOnShelves["Cookies"]);
                Cookie_pie.DataSource.SetValue("Back", Simulation.totalInBack["Cookies"]);
                Cookie_pie.DataSource.SetValue("In Transit", TimeController.quantityInQueue("Cookie"));
            //Cereal
                Cereal_pie.DataSource.SetValue("Front", Simulation.totalOnShelves["Cereal"]);
                Cereal_pie.DataSource.SetValue("Back", Simulation.totalInBack["Cereal"]);
                Cereal_pie.DataSource.SetValue("In Transit", TimeController.quantityInQueue("Cereal"));

        //Employees in department
        DryHeaderText.text = "Dry Department\n Employees working: " + Sim.numEmployees["Dry Goods"];
        //Frozen
            //Pizza          
                Pizza_pie.DataSource.SetValue("Front", Simulation.totalOnShelves["Pizza"]);
                Pizza_pie.DataSource.SetValue("Back", Simulation.totalInBack["Pizza"]);
            //Dessert
                Dessert_pie.DataSource.SetValue("Front", Simulation.totalOnShelves["Dessert"]);
                Dessert_pie.DataSource.SetValue("Back", Simulation.totalInBack["Dessert"]);               

            //Employees in department
                FrozenHeaderText.text = "Frozen Department\n Employees working: " + Sim.numEmployees["Frozen"];
        //Dairy
            //Milk          
                Milk_pie.DataSource.SetValue("Front", Simulation.totalOnShelves["Milk"]);
                Milk_pie.DataSource.SetValue("Back", Simulation.totalInBack["Milk"]);
            //Cheese
                Cheese_pie.DataSource.SetValue("Front", Simulation.totalOnShelves["Cheese"]);
                Cheese_pie.DataSource.SetValue("Back", Simulation.totalInBack["Cheese"]);

            //Employees in department
                DairyHeaderText.text = "Dairy Department\n Employees working: " + Sim.numEmployees["Dairy"];
        //Full Stock
        Debug.Log("SET MAIN GRAPH");
            Stock_pie.DataSource.SetValue("Front", Sim.totalInFront_value);
            Stock_pie.DataSource.SetValue("Back", Sim.totalInBack_value);
            //It should be: Stock_pie.DataSource.SetValue("Empty", Sim.MaximumProductInStore - (Sim.totalInBack_value + Sim.totalInFront_value));
            //but that isn't working without discernable reason, so
            Stock_pie.DataSource.SetValue("Empty", 0);
    }

    public void UpdateGraphs() 
    {
        EmployeeStatusPie.DataSource.SlideValue("Idle", Sim.numEmployees["Idle"], 200);
        EmployeeStatusPie.DataSource.SlideValue("Offsite", Sim.numEmployees["Offsite"], 200);

        EmployeeStatusPie.DataSource.SlideValue("Active", Sim.TotalActiveEmployees, 200);

        for (int i = 0; i < Sim.DEPARTMENTS.Length - 1; i++)
        {
            DepartmentStatusBar.DataSource.SetValue(Sim.DEPARTMENTS[i], "All", Sim.numEmployees[Sim.DEPARTMENTS[i]]);
        }

        //Full Stock  
        Stock_pie.DataSource.SlideValue("Front", Sim.totalInFront_value, 200);  
        Stock_pie.DataSource.SlideValue("Back", Sim.totalInBack_value, 200);
        Stock_pie.DataSource.SlideValue("Empty", Sim.MaximumProductInStore - (Sim.totalInBack_value + Sim.totalInFront_value), 200);
        Debug.Log("Value in EMPTY: " + (Sim.MaximumProductInStore - (Sim.totalInBack_value + Sim.totalInFront_value)) );

        //Produce
        Apple_pie.DataSource.SlideValue("Front", Simulation.totalOnShelves["Apples"], 200);
        Apple_pie.DataSource.SlideValue("Back", Simulation.totalInBack["Apples"], 200);
        Apple_pie.DataSource.SlideValue("In Transit", TimeController.quantityInQueue("Apples"), 200);


        Banana_pie.DataSource.SlideValue("Front", Simulation.totalOnShelves["Bananas"], 200);
        Banana_pie.DataSource.SlideValue("Back", Simulation.totalInBack["Bananas"], 200);
        Banana_pie.DataSource.SlideValue("In Transit", TimeController.quantityInQueue("Bananas"), 200);

        ProduceHeaderText.text = "Produce Department\n Employees working: " + Sim.numEmployees["Produce"];

        //Dry
        Cereal_pie.DataSource.SlideValue("Front", Simulation.totalOnShelves["Cereal"], 200);
        Cereal_pie.DataSource.SlideValue("Back", Simulation.totalInBack["Cereal"], 200);
        Cereal_pie.DataSource.SlideValue("In Transit", TimeController.quantityInQueue("Cereal"), 200);

        Cookie_pie.DataSource.SlideValue("Front", Simulation.totalOnShelves["Cookies"], 200);
        Cookie_pie.DataSource.SlideValue("Back", Simulation.totalInBack["Cookies"], 200);
        Cookie_pie.DataSource.SlideValue("In Transit", TimeController.quantityInQueue("Cookies"), 200);

        DryHeaderText.text = "Dry Department\n Employees working: " + Sim.numEmployees["Dry Goods"];
        
        //Frozen
        Pizza_pie.DataSource.SlideValue("Front", Simulation.totalOnShelves["Pizza"], 200);
        Pizza_pie.DataSource.SlideValue("Back", Simulation.totalInBack["Pizza"], 200);
        Pizza_pie.DataSource.SlideValue("In Transit", TimeController.quantityInQueue("Pizza"), 200);

        Dessert_pie.DataSource.SlideValue("Front", Simulation.totalOnShelves["Dessert"], 200);
        Dessert_pie.DataSource.SlideValue("Back", Simulation.totalInBack["Dessert"], 200);
        Dessert_pie.DataSource.SlideValue("In Transit", TimeController.quantityInQueue("Dessert"), 200);

        FrozenHeaderText.text = "Frozen Department\n Employees working: " + Sim.numEmployees["Frozen"];

        //Dairy
        Milk_pie.DataSource.SlideValue("Front", Simulation.totalOnShelves["Milk"], 200);
        Milk_pie.DataSource.SlideValue("Back", Simulation.totalInBack["Milk"], 200);
        Milk_pie.DataSource.SlideValue("In Transit", TimeController.quantityInQueue("Milk"), 200);

        Cheese_pie.DataSource.SlideValue("Front", Simulation.totalOnShelves["Cheese"], 20);
        Cheese_pie.DataSource.SlideValue("Back", Simulation.totalInBack["Cheese"], 200);
        Cheese_pie.DataSource.SlideValue("In Transit", TimeController.quantityInQueue("Cheese"), 200);

        DairyHeaderText.text = "Dairy Department\n Employees working: " + Sim.numEmployees["Dairy"];

    }
    */
}
