using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using UnityEngine.UI;
using System.Diagnostics;

public class LogManager : MonoBehaviour
{
    static string file_title;
    static string[] FilePaths = new string[2];
    static StreamWriter writer1;
    static StreamWriter writer2;
    // Start is called before the first frame update

    public static void LogDay()
    {

        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int cur_time = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
        file_title = cur_time.ToString();

        FilePaths[0] = Application.persistentDataPath + file_title + ".txt";
        FilePaths[1] = "Assets/Resources/" + file_title + ".txt";



        float net_change = (float) (SimController.Day.Cash - SimController.Day.StartOfDayCash);
        string net_change_string;

        if (net_change < 0)
        {
            net_change_string = "-$" + string.Format("{0:n}", Mathf.Abs(net_change));
        }

        else
        {
            net_change_string = "$" + string.Format("{0:n}", net_change);
        }

        if (SimController.DayNum == 1)
        {
            UnityEngine.Debug.Log("Opening writer 1");
            writer1 = new StreamWriter(FilePaths[0], true);
            //if(SimController.DayNum == 1)
            writer1.WriteLine("Day Number#Starting Cash#End Cash#Net Change#Revenue#Deliveries Ordered#Delivery Expense#Employee Expense#FOH#BOH#Items Sold#Items Expired#Upcoming Deliveries");
        }

        string entry = SimController.DayNum + "#" + SimController.Day.StartOfDayCash + "#" + SimController.Day.Cash + "#";
        entry += net_change_string + "#" + SimController.Day.DailyRevenue.ToString("C2") + "#" + SimController.Day.DeliveriesOrdered + "#" + SimController.Day.DailyDeliveryCost.ToString("C2") + "#";
        entry += SimController.Day.DailyEmployeePayout.ToString("C2") + "#" + PostDayController.TotalFOH + "#" + PostDayController.TotalBOH + "#" + SimController.Day.DailyItemsSold + "#";
        entry += SimController.Day.TotalExpired + "#" + SimController.Day.Deliveries.Count;
        writer1.WriteLine(entry);

        if (SimController.DayNum == 1)
        {
            UnityEngine.Debug.Log("Opening writer 1");
            writer2 = new StreamWriter(FilePaths[1], true);
            //if(SimController.DayNum == 1)
            writer2.WriteLine("Day Number#Starting Cash#End Cash#Net Change#Revenue#Deliveries Ordered#Delivery Expense#Employee Expense#FOH#BOH#Items Sold#Items Expired#Upcoming Deliveries");
        }

        writer2.WriteLine(entry);

        if (SimController.DayNum == 7)
        {
            writer1.Close();
            writer2.Close();
        }

        Resources.Load(FilePaths[1]);
    }

    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
