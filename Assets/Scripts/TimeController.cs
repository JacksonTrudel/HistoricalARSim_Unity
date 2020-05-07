using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class TimeController : MonoBehaviour
{
    // Start is called before the first frame update
    public Text timerText;

    Simulation Sim;
    RegisterController registers;
    ShowErrorMessage Error;
    LogController logContrl;

    // all deliveries are stored as a list that contains a mapping from the delivery day to food,quantity
    // dont get overwhelmed by this
    public static List<KeyValuePair<int, Tuple<Food, int>>> deliveries = new List<KeyValuePair<int, Tuple<Food, int>>>();
    public static List<Tuple<int, int>> TravellingEmployees = new List<Tuple<int, int>>();

    //our day starts at 8 am
    public int Hour;
    public int Minute = 0;
    
    //static keyword allows for the vars to be passed through scenes
    //but also... they can never be reset again, only manually
    public static int Day = 1;
    [HideInInspector]
    public int Hour_End;

    [Range(1,50)]
    public int TestSlider = 1;
    

    string delivery_text;
    string delivery_text2 = "Delivered:\n";

    public static bool day_bool = true;

    //holds the deliveries going to made next day as a string so I have a nice thing to print to a text object
    public static List<string> upcoming_deliveries = new List<string>();


    void Start()
    {
        Sim = GameObject.Find("Simulation").GetComponent<Simulation>();
        registers = GameObject.Find("Simulation").GetComponent<RegisterController>();
        logContrl = GameObject.Find("Simulation").GetComponent<LogController>();
        Error = GameObject.Find("ErrorMessage").GetComponent<ShowErrorMessage>();
        Scene CurrentScene = SceneManager.GetActiveScene();
        

        Hour = Sim.StartMilitaryTime;
        Hour_End = Sim.EndMilitaryTime;
        day_bool = true;

        IEnumerator coroutine = NextHour(false);

        if (CurrentScene.name != "Historical AR")
            StartCoroutine(coroutine);

        Time.timeScale = 100f;
        Debug.Log("Day bool status: " + day_bool);
        Day = 1;
    }

    // Update is called once per frame
    void Update()
    {
        checkForEmployees();

        if (Minute == 5)
        {
            checkForDelivery();
        }

        //simulation stops after 7 days
        if (Day == Sim.NumberOfDays + 1)
        {
            //should stop the sim

            day_bool = false;
        }

        else if (Minute < 10)
        {
            timerText.text = (Day + "   " + Hour + ":0" + Minute);
        }

        else
        {
            timerText.text = (Day + "   " + Hour + ":" + Minute);
        }
    }

    public void stopSim()
    {

    }

    public IEnumerator NextHour(Boolean historical)
    {
        while (day_bool)
        {
            // END OF THE DAY
            if (Hour == Hour_End)
            {
                
                TravellingEmployees.Clear();

                // Must be done before we change hour 
                Sim.payEmployees(Hour);

                
                Sim.PullItemsOffShelf();
                
                //calculate expired foods
                //must be calculated BEFORE food is returned to shelves
                Sim.expireFoods();

                
                //checkForDelivery();
                

                registers.checkoutFoods();
                // if theres any at the end of the day, return the foods to shelves. This food will not be turning a profit
                registers.returnFoodsToShelves();
                logContrl.Print(Day, Hour, Minute, $"End of day {Day}");
                // must be called every day to reset variables used in RegUtilization calculations
                registers.resetDailyRegisterVariables();
                Sim.updateCheckoutItems();

                Hour = Sim.StartMilitaryTime;
                Day++;
                //load the status page for end of each day
                SceneManager.LoadScene("PostDayResultsHistorical");
                

                

                Debug.Log("Ended Coroutine");
                day_bool = false;
                
                Sim.numEmployees["Idle"] = 2;
                Sim.numEmployees["Offsite"] = 6;
                Sim.numEmployees["Registers"] = 2;
                Sim.updateNumEmployees();
            }

            // END OF EACH HOUR
            else if (Minute == 60)
            {
                Minute = 0;
                Hour++;
                Sim.payEmployees(Hour);
                registers.resetHourlyRegisterVariables();
            }

            if (Minute % Sim.TimeBetweenCheckouts == 0 && !(Hour == 8 && Minute == 0))
            {
                Sim.PullItemsOffShelf();
                registers.checkoutFoods();
                Sim.updateCheckoutItems();
                Sim.updateCash();
            }

            // EACH MINUTE


            // Not sure if this will work
            if (historical)
                yield return null;
            else
                yield return new WaitForSeconds(16);

            Minute++;
        }
    }

    // input: food/quantity to be delivered and what day it will get delivered on, all deliveries are in the morning
    public static void addDelivery(Food food, int quantity, int day)
    {
        // create a new pair
        Tuple<Food, int> foodAndQuantity = new Tuple<Food, int>(food, quantity);
        // insert that pair into the list of deliveries
        deliveries.Add(new KeyValuePair<int, Tuple<Food, int>>(day, foodAndQuantity));
        PostDayManager.num_of_deliveries_in_queue += quantity;
    }

    public void addTravellingEmployee() 
    {
        //employee arrives in 30 mins.   
        Tuple<int, int> Time;
        //if it's like 2:40, arrival would be 2:70
        if (Minute + 30 > 60)
        {
            //so convert it to 3:10
            Time = new Tuple<int, int>(Hour + 1, Minute - 30);
            TravellingEmployees.Add(Time);
            Debug.Log("Added Employee at Hour = " + (Hour + 1) + "Minute = " + (Minute - 30));
        }

        else 
        {
            Time = new Tuple<int, int>(Hour, Minute + 30);
            Debug.Log("Added Employee at Hour = " + Hour + "Minute = " + (Minute + 30));
            TravellingEmployees.Add(Time);
        }

        Error.Show("Employee will arrive in 30 minutes.");

    }

    public void checkForEmployees()
    {
        for (int i = 0; i < TravellingEmployees.Count; i++) 
        {
            if (TravellingEmployees[i].Item1 == Hour && TravellingEmployees[i].Item2 == Minute) 
            {
                Debug.Log("EMPLOYEE ARRIVED AT " + Hour + ":" + Minute);
                Error.Show("Employee arrived.");

                Sim.EmployeeArrives();
                TravellingEmployees.RemoveAt(i);

            }

        }            

    }

    // every morning check the list for any deliveries for that day
    void checkForDelivery()
    {
        // go through each delivery on the list and check its delivery day
        for (int i = 0; i < deliveries.Count; i++)
        {
            // if we are at the delivery day, delivery it!
            //either 5 hrs ahead or next day.


            if (deliveries[i].Key == Hour)
            {
                Food food = deliveries[i].Value.Item1;
                int quantity = deliveries[i].Value.Item2;
                delivery_text = "New delivery for " + quantity + " " + food.getItemDescription() + "!";
                delivery_text2 = "Delivered:\n" + " " + quantity + " " + food.getItemDescription() + "\n";
                //? -COLIN
                //delivery_text2 = delivery_text2 + " " + quantity + " " + food.getItemDescription() + "\n";
                PostDayManager.num_of_deliveries_in_queue -= quantity;
                print(delivery_text);
                deliveries.RemoveAt(i);
                Sim.timeForDelivery(food, quantity); // this will actually update the stocks

                if (i == deliveries.Count)
                {
                    Error.Show(delivery_text2);
                }
            }

        }
    }

    public static void countUpcomingDelivery()
    {
        //this is printed to the status page, thus is reset each time it is run
        //it's a list of strings that says for ex. "20 Apples".
        upcoming_deliveries.Clear();

        for (int i = 0; i < deliveries.Count; i++)
        {
            // if we are at the delivery day, delivery it!
            // if (deliveries[i].Key == Day)
            //{
            Food food = deliveries[i].Value.Item1;
            int quantity = deliveries[i].Value.Item2;

            //populate my list of upcoming deliveries
            upcoming_deliveries.Add(quantity + " " + food.getItemDescription());
            //}
        }

    }
    public static int quantityInQueue()
    {
        int i, quantityInQueue = 0;

        for (i = 0; i < deliveries.Count; i++)
        {
            quantityInQueue += deliveries[i].Value.Item2;
        }

        return quantityInQueue;
    }


    // Argument: The item description of a food
    // Output: returns the quantity of good in transit for the given food.
    //this function exists above as well. This is an example of method overloading, where functions have the same name but diff parameters.
    public static int quantityInQueue(string itemName)
    {
        int total = 0;

        // dictionaries stores list of key value pairs:
        // key: day
        foreach (KeyValuePair<int, Tuple<Food, int>> valuePair in deliveries)
        {
            if (valuePair.Value.Item1.getItemDescription().CompareTo(itemName) == 0)
                total += valuePair.Value.Item2;

        }

        return total;
    }
    

    public void startHistorical()
    {
        Hour = Sim.StartMilitaryTime;
        Minute = 0;
        IEnumerator coroutine = NextHour(true);
        
        StartCoroutine(coroutine);
    }
}
