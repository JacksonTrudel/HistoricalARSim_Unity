using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;

public class SimController : MonoBehaviour
{
    public int DayNum { get; set; }
    public Store Day { get; set; }

    TextMeshProUGUI CashText;
    // Start is called before the first frame update
    void Start()
    {
        if (DayNum == 0)
        {
            Day = new Store();
            Day.InitStore();
        }

        CashText = GameObject.Find("CashText").GetComponent<TextMeshProUGUI>();
        UnityEngine.Debug.Log("CashText: " + CashText.text);

        CashText.text = Day.Cash.ToString("C2");
    }

    // Update is called once per frame
    void Update()
    {
    }
}
