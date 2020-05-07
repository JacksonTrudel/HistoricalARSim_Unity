using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master_Commands : MonoBehaviour
{
    TimeController Time;
    // Start is called before the first frame update
    void Start()
    {
        Time = GameObject.Find("UI Canvas").GetComponent<TimeController>();
    }

    // Update is called once per frame
    void Update()
    {
        //will change time to 19:00
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            Time.Hour = 19;
            Time.Minute = 59;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            TimeController.Day = 7;
        }

    }
}
