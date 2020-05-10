using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SimController : MonoBehaviour
{
    public static int DayNum { get; set; }
    public static Store Day { get; set; }
    public static string DaySummary { get; set; }
    

    TextMeshProUGUI CashText;
    // Start is called before the first frame update
    void Start()
    {
        Scene CurrentScene = SceneManager.GetActiveScene();
        if (CurrentScene.name == "Historical")
        {
            if (DayNum == 0)
            {
                Day = new Store();
                Day.InitStore();
                DayNum = 1;
                GameObject.Find("PrevReport Button").SetActive(false);
            }
           

            CashText = GameObject.Find("CashText").GetComponent<TextMeshProUGUI>();
            UnityEngine.Debug.Log("CashText: " + CashText.text);

            // formats Cash in $ format
            CashText.text = Day.Cash.ToString("C2");
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public static void LoadResults()
    { 
        SceneManager.LoadScene("PostDayResultsHistorical");
    }

    public void ContinueFromReport()
    {
        if (PostDayController.SimOver)
        {
            SceneManager.LoadScene("End");
        }
        else
        {
            SceneManager.LoadScene("Historical");
        }
    }

    public void ReturnFromEndScene()
    {
        DayNum = 0;
        SceneManager.LoadScene("Main Menu");
    }
}
