using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PostDayManager : MonoBehaviour
{
    Simulation Sim;
    RegisterController registers;

    private int temp_day;
    private static float beginningDayCash;

    public static int day_boolean;

    private float Net_change;
    private string Net_change_string;
    private string cash_value_neg_or_pos;
    private string upcomingDeliveries;
    
    
    public static int total_food_sold = 0;
    public static float total_revenue = 0;
    [HideInInspector] public static decimal total_spent_on_deliveries = 0;
    [HideInInspector] public static int money_spent_paying_employees = 0;
    [HideInInspector] public static int front_of_house_stock = 0;
    [HideInInspector] public static int back_of_house_stock = 0;
    [HideInInspector] public static int num_of_deliveries_in_queue = 0;
    [HideInInspector] public static int count_of_expired_food = 0;
    [HideInInspector] public static float cost_of_expired_food = 0;


    public Text continue_button_text;
    public Text status_text;

    // Initialized in Start(), is true if we are currently running the Historical version
    public bool Historical;

    



    // Start is called before the first frame update
    void Start()
    {
        //Sim = GameObject.Find("Simulation").GetComponent<Simulation>();

        registers = GameObject.Find("Simulation").GetComponent<RegisterController>();

        //Section 1. Display Cash and Net Change across Day

        calculate_netchange();

            //for displaying negative cash or positive cash
            if (Simulation.Cash < 0)
            {
                cash_value_neg_or_pos = "-$" + string.Format("{0:n}", Mathf.Abs(Simulation.Cash));
            }
            else
            {
                cash_value_neg_or_pos = "$" + string.Format("{0:n}", Simulation.Cash);
            }

            //change the net_change cash values to be negative or positive
            if (Net_change < 0)
            {
                //if net change is a negative value, it is a gain i.e went from $50 to $100: 50 - 100 = -50: gained 50 dollars 
                Net_change_string = "$" + string.Format("{0:n}", Mathf.Abs(Net_change));
            }

            else
            {
                //if net_change is a positive value, then it's actually a loss
                Net_change_string = "-$" + string.Format("{0:n}", Net_change);
            }

        //Section 2. Print how much money was spent on deliveries
            //done in Simulation in the makeNewDelivery method
        //Section 3. Print How much money was spent on employees

        //Section 4. FOH Stock
            int TotalFOH = 0;
            
            for(int i = 0; i < Simulation.FOODS.Length; i++)
            { 
                TotalFOH += Simulation.totalOnShelves[ Simulation.FOODS[i] ];
            }
        //Section 5. BOH Stock
            int TotalBOH = 0;

            for (int i = 0; i < Simulation.FOODS.Length; i++)
            {
                TotalBOH += Simulation.totalInBack[Simulation.FOODS[i]];
            }
        //Section 6. Display the upcoming deliveries
            //run the string enqueuer
            //this will turn all the orders into a long string with @ signs in between
            TimeController.countUpcomingDelivery();
                for(int i = 0; i < TimeController.upcoming_deliveries.Count; i++) 
                {
                    //plus an @ symbol that will be turned into newline characters later
                    upcomingDeliveries = upcomingDeliveries + "@" + (TimeController.upcoming_deliveries[i]); 
                }


        //Section 7. Display how many items were sold
        //Calculated in Register Controller with reference to int total_food_sold
        //Section 8. Display how many items expired
        //Done using int Simulation.total_expired

        // Section 9. Display average amount of customers in each line
        int[] peopleAtRegisters = new int[3];
        //peopleAtRegisters[0] = 


        int regUtilShift1 = (int)(registers.totalRegUtilization[0] * 100);
        int regUtilShift2 = (int)(registers.totalRegUtilization[1] * 100);
        int regUtilShift3 = (int)(registers.totalRegUtilization[2] * 100);
        //print to the text on the screen
        status_text.text = ("You completed Day " + (TimeController.Day - 1));
        status_text.text += ("@@You have " + cash_value_neg_or_pos);
        status_text.text += ("@              (Net change: " + Net_change_string + ")@Total Front of House Stock: ");
        status_text.text += (TotalFOH + "@Total Back of House Stock: ");
        status_text.text += ("@              (Net change: " + Net_change_string + ")@Total Front of House Stock: ");
        status_text.text += (TotalBOH + "@You sold ");
        status_text.text += ("@              (Net change: " + Net_change_string + ")@Total Front of House Stock: ");
        status_text.text += (total_food_sold + " items@");
        status_text.text += ("@              (Net change: " + Net_change_string + ")@Total Front of House Stock: ");
        status_text.text += (Simulation.total_expired + " foods expired :(@$");
        status_text.text += ("@              (Net change: " + Net_change_string + ")@Total Front of House Stock: ");
        status_text.text += (money_spent_paying_employees + " spent on employees@$");
        status_text.text += ("@              (Net change: " + Net_change_string + ")@Total Front of House Stock: ");
        status_text.text += (total_spent_on_deliveries + " spent on deliveries@");
        status_text.text += ("@              (Net change: " + Net_change_string + ")@Total Front of House Stock: ");
        status_text.text += (TimeController.upcoming_deliveries.Count + " deliveries will arrive tomorrow.@Register Utilization - SHIFT 1: ");
        status_text.text += ("@              (Net change: " + Net_change_string + ")@Total Front of House Stock: ");
        status_text.text += (regUtilShift1 + "% SHIFT 2: ");
        status_text.text += ("@              (Net change: " + Net_change_string + ")@Total Front of House Stock: ");
        status_text.text += (regUtilShift2 + "% SHIFT 3: ");
        status_text.text += ("@              (Net change: " + Net_change_string + ")@Total Front of House Stock: ");
        status_text.text += (regUtilShift3 + "%");



        status_text.text = status_text.text.Replace("@", System.Environment.NewLine);
        
        //print to log file
        LogController.PrintEndOfDay(Simulation.Cash, Net_change_string, total_spent_on_deliveries, money_spent_paying_employees, TotalFOH, TotalBOH, total_food_sold, Simulation.total_expired, TimeController.upcoming_deliveries.Count);

        if (TimeController.Day != 8) 
        {
            continue_button_text.text = ("Continue with Day " + (TimeController.Day));
        }

        else 
        {
            continue_button_text.text = ("End Game");
        }
        

        //need to reset these every time the script is finished aka each day
        total_revenue = 0;
        total_spent_on_deliveries = 0;
        total_food_sold = 0;
        money_spent_paying_employees = 0;
        cost_of_expired_food = 0;
        count_of_expired_food = 0;
        
        upcomingDeliveries = "";


    }

    //get set property lets us check for when the temp_day variable changes, aka when the day has moved on
    int dayChecker
    {
        get
        {
            return temp_day;
        }

        set
        {
            if (temp_day != value)
            {
                temp_day = value;
                Debug.Log("there was a change to day: " + temp_day);
                
                //changes for a second to let us know the day has been updated
                //so ONLY WHILE day_boolean = 1, the day has changed 
                day_boolean = 1;

                //get total expired food
                //Sim.total_expired;

            }

        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
        dayChecker = TimeController.Day;
        while (day_boolean == 1) 
        {
            //print to the text
            day_boolean = 0;
        }
    }

    void calculate_netchange() 
    {
        //hard coded value of beginning cash
        if (TimeController.Day == 2)
        {
            Net_change = 10000 - Simulation.Cash;
            beginningDayCash = Simulation.Cash;
        }

        else
        {
            Net_change = beginningDayCash - Simulation.Cash;
            Debug.Log("HELLO: " + beginningDayCash);
            beginningDayCash = Simulation.Cash;
            

        }

    }

    public static float getBeginningDayCash()
    {
        return beginningDayCash;
    }
    
}
