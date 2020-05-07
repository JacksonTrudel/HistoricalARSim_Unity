using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using UnityEngine.UI;
public class LogController : MonoBehaviour
{
    DateTime Now;
    static string file_title;
    static string[] FilePaths = new string[2];

    Simulation Sim;





 
    void Start()
    {
        Sim = GameObject.Find("Simulation").GetComponent<Simulation>();
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int cur_time = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
        file_title = cur_time.ToString();
        //file_title = "/" + file_title;

        
        FilePaths[0] = Application.persistentDataPath + file_title + ".txt";
        FilePaths[1] = "Assets/Resources/" + file_title + ".txt";
        StreamWriter writer;


        Debug.Log("Um, LOGG PLEASE");

       
        for (int i = 0; i < 2; i++) 
        {
            Debug.Log("LOGGING THX");
            //Write some text
            writer = new StreamWriter(FilePaths[i], true);

            writer.WriteLine("Day#Hour#Message#Current Cash#Net Change#Revenue#Delivery Expense#Employee Expense#FOH#BOH#Items Sold#Items Expired#Upcoming Deliveries");

            writer.Close();

        }
        
        //this has to happen for unity to update the resources folder with the new file you've just created
        //so I loaded FilePaths[1] which is the path to the resources folder when the writer accesses it.
        Resources.Load(FilePaths[1]);

        //string path = Application.persistentDataPath + file_title + ".txt";
        //string path = "Assets/Resources/" + file_title + ".txt";


        //Write some text
        // StreamWriter writer = new StreamWriter(FilePaths[1].ToString(), true);

        //writer.WriteLine("Day,Hour,Message,Current Cash,Net Change,Delivery Expense,Employee Expense,FOH,BOH,Items Sold,Items Expired,Upcoming Deliveries");

        //writer.Close();
    }


    public void Print(int day, int hour, int minute, string text)
    {
        float cash = Simulation.Cash, netChange = Simulation.Cash - PostDayManager.getBeginningDayCash();
        decimal deliveryExpense = PostDayManager.total_spent_on_deliveries;
        int employeeExpense = PostDayManager.money_spent_paying_employees;

        // determine FOH stock
        int TotalFOH = 0;

        for (int i = 0; i < Simulation.FOODS.Length; i++)
        {
            TotalFOH += Simulation.totalOnShelves[Simulation.FOODS[i]];
        }

        // determine BOH stock
        int TotalBOH = 0;

        for (int i = 0; i < Simulation.FOODS.Length; i++)
        {
            TotalBOH += Simulation.totalInBack[Simulation.FOODS[i]];
        }

        for (int i = 0; i < 2; i++)
        {

            //string path2 = "Assets/Resources/" + file_title + ".txt";

            //string path = Application.persistentDataPath + file_title + ".txt";


            //Write some text
            StreamWriter writer = new StreamWriter(FilePaths[i], true);

            //writer.WriteLine("Day,Hour,Message,Current Cash,Net Change,Delivery Expense,Employee Expense,FOH,BOH,Items Sold,Items Expired,Upcoming Deliveries");


            if (text.Contains("End of day"))
            {
                writer.WriteLine(day + "#" + hour + (minute < 10 ? ":0" : ":") + minute + "#" + text + "#" + string.Format("{0:n}", cash) + "#" + string.Format("{0:n}", netChange) + "#" + string.Format("{0:n}", PostDayManager.total_revenue) + "#" + string.Format("{0:n}", deliveryExpense) + "#" +
                                    employeeExpense + "#" + TotalFOH + "#" + TotalBOH + "#" + PostDayManager.total_food_sold + "#" + PostDayManager.count_of_expired_food + " @ $" + string.Format("{0:n}", PostDayManager.cost_of_expired_food) + "#" + PostDayManager.num_of_deliveries_in_queue);
                // print a blank line in between days
                writer.WriteLine();
            }
            else
            {
                writer.WriteLine(day + "#" + hour + (minute < 10 ? ":0" : ":") + minute + "#" + text + "#" + string.Format("{0:n}", cash) + "#" + string.Format("{0:n}", netChange) + "#" + string.Format("{0:n}", PostDayManager.total_revenue) + "#" + string.Format("{0:n}", deliveryExpense) + "#" +
                                    employeeExpense + "#" + TotalFOH + "#" + TotalBOH + "#" + PostDayManager.total_food_sold + "##" + PostDayManager.num_of_deliveries_in_queue);
            }

            writer.Close();
        }
        //Re-import the file to update the reference in the editor
        Resources.Load(FilePaths[1]);
    }

    public static void PrintEndOfDay(float cash, string change, decimal deliveryExpense, decimal employeeExpense, int FOH, int BOH, int itemsSold, int itemsExpired, int numDeliveries)
    {

        string path = Application.persistentDataPath + file_title + ".txt";
        StreamWriter writer = new StreamWriter(path, true);
     
        writer.WriteLine("###" + cash + "#" + change + "#" + string.Format("{0:n}", PostDayManager.total_revenue) + "#" + deliveryExpense + "#" + employeeExpense + "#" + FOH + "#" + BOH + "#" + itemsSold + "#" + itemsExpired + "#" + numDeliveries);

        writer.Close();
        Resources.Load(path);
    }

    //last edited by Colin Jackson
    //4/9/20
    


}