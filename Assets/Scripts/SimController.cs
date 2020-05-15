using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SimController : MonoBehaviour
{
    // Static so the values persist between scenes
    public static int DayNum { get; set; }
    public static Store Day { get; set; }
    public static string DaySummary { get; set; }

    // can be set in inspector (only for Historical scene)
    public TextMeshProUGUI DayText;

    TextMeshProUGUI CashText;

    // Start is called before the first frame update
    void Start()
    {

        Scene CurrentScene = SceneManager.GetActiveScene();

        // technically this script is attached to PostDayResults, but the following lines only
        // execute on the historical scene
        if (CurrentScene.name == "Historical")
        {
            if (DayNum == 0)
            {
                Day = new Store();
                Day.InitStore();
                DayNum = 1;

                // Previous Report isn't available on day 1
                GameObject.Find("PrevReport Button").SetActive(false);
            }

            DayText.text = "Day: " + DayNum;

            CashText = GameObject.Find("CashText").GetComponent<TextMeshProUGUI>();
            UnityEngine.Debug.Log("CashText: " + CashText.text);

            // formats Cash in $ format
            string cash_value_neg_or_pos;
            if (SimController.Day.Cash < 0)
            {
                cash_value_neg_or_pos = "-$" + string.Format("{0:n}", Mathf.Abs((float)Day.Cash));
            }
            else
            {
                cash_value_neg_or_pos = "$" + string.Format("{0:n}", Day.Cash);
            }

            CashText.text = cash_value_neg_or_pos;
        }
    }

    // called from menu script upon completion of decisions
    public static void LoadResults()
    { 
        SceneManager.LoadScene("PostDayResultsHistorical");
    }

    // attached to the continue button on the PostDayResults
    public void ContinueFromReport()
    {
        if (DayNum == 7)
        {
            SceneManager.LoadScene("End");
        }
        else
        {
            SceneManager.LoadScene("Historical");
        }
    }

    // Attached to button on End scene, which starts a new simulation
    public void ReturnFromEndScene()
    {
        DayNum = 0;
        SceneManager.LoadScene("Main Menu");
    }
}
