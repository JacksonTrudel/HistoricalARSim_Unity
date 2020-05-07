using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{
    public Text continue_button_text;
    public Text status_text;

    // Start is called before the first frame update
    void Start()
    {
        status_text.text = ("You completed Day " + (TimeController.Day - 1) + ".@You have $ " + Simulation.Cash + ".@");

        status_text.text = status_text.text.Replace("@", "@" + System.Environment.NewLine);


        continue_button_text.text = ("Continue with Day " + (TimeController.Day) );

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
