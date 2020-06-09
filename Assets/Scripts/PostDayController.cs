using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class PostDayController : MonoBehaviour
{

    public Text status_text;
    public Text continue_button_text;

    // stores status text string to be retrieved the following day
    public static string LastReport;
    public static int TotalFOH;
    public static int TotalBOH;

    // Start is called before the first frame update
    void Start()
    {
        // Prepare strings and values that will be concatenated to the PostDayResult text
        string cash_value_neg_or_pos, net_change_string;

        if (SimController.Day.Cash < 0)
        {
            cash_value_neg_or_pos = "-$" + string.Format("{0:n}", Mathf.Abs((float)SimController.Day.Cash));
        }
        else
        {
            cash_value_neg_or_pos = "$" + string.Format("{0:n}", SimController.Day.Cash);
        }

        // calculate change in cash value
        float net_change = (float)(SimController.Day.Cash - SimController.Day.StartOfDayCash);
        
        if (net_change < 0)
        {
            net_change_string = "-$" + string.Format("{0:n}", Mathf.Abs(net_change));
        }

        else
        {
            net_change_string = "$" + string.Format("{0:n}", net_change);
        }

        TotalFOH = 0;

        for (int i = 0; i < SimController.Day.Stock.Count; i++)
        {
            TotalFOH += ((FoodItem)SimController.Day.Stock[i]).StockFOH;
        }

        TotalBOH = 0;

        for (int i = 0; i < SimController.Day.Stock.Count; i++)
        {
            TotalBOH += ((FoodItem)SimController.Day.Stock[i]).StockBOH;
        }

        // Constructs the body of the report
        status_text.text = ("You completed Day " + (SimController.DayNum));
        status_text.text += ("@@You have " + cash_value_neg_or_pos);
        status_text.text += ("@(Net change: " + net_change_string + ")@Total Front of House Stock: ");
        status_text.text += (TotalFOH + "@Total Back of House Stock: ");
        status_text.text += (TotalBOH + "@You sold ");
        status_text.text += (SimController.Day.DailyItemsSold + " items worth $" + SimController.Day.DailyRevenue + "@");
        status_text.text += ("Shift 1: " + SimController.Day.ShiftItemsSold[0] + "  Shift 2: " + SimController.Day.ShiftItemsSold[1]);
        status_text.text += (" Shift 3: " + SimController.Day.ShiftItemsSold[2] + "@");
        status_text.text += (SimController.Day.TotalExpired + " foods expired :(@");
        status_text.text += ("Total Overflow: " + SimController.Day.totalOverFlow + "@");
        status_text.text += (SimController.Day.DailyEmployeePayout.ToString("C2") + " spent on employees@");
        status_text.text += (SimController.Day.DailyDeliveryCost.ToString("C2") + " spent on deliveries@");
        status_text.text += (SimController.Day.Deliveries.Count + " deliveries will arrive tomorrow.@");
        status_text.text += ("Register Utilization - SHIFT 1: " + string.Format("{0:0.#}", SimController.Day.GetRegUT(1)) + "% SHIFT 2: " + string.Format("{0:0.#}", SimController.Day.GetRegUT(2)));
        status_text.text += ("%@SHIFT 3: " + string.Format("{0:0.#}", SimController.Day.GetRegUT(3)) + "%");

        status_text.text = status_text.text.Replace("@", System.Environment.NewLine);

        // stores the report to be referenced on the following day
        LastReport = status_text.text;

        // calls static method which logs the day's information
        LogManager.LogDay();

        if (SimController.DayNum != 8)
        {
            ++SimController.DayNum;
            continue_button_text.text = SimController.DayNum >= 8 ? ("End Game") 
                                        : ("Continue with Day " + (SimController.DayNum));
        }

        // else
        // {
        //     continue_button_text.text = ("End Game");
        // }

    }
}
