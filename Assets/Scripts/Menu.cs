using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Net.Mail;
using UnityEngine.SceneManagement;
using System.CodeDom;

public class Menu : MonoBehaviour
{
    private SimController Sim;
    private int CurrentPage;
    ShowErrorMessage Error;

    private ArrayList Shifts;
    private ArrayList Deliveries;
    private GameObject PlusButton;
    private GameObject MinusButton;
    private TextMeshProUGUI CostLabel;
    private decimal deliveryCost;

    private GameObject Page1;
    private GameObject Page2;
    private GameObject Page3;
    private GameObject Page4;
    private GameObject Page5;

    private static int MaxBatches = 5;
    private static Regex regex = new Regex("[1-" + MaxBatches + "]");
    private static int BatchSize = 25;

    
    private bool loopFlag = false;

    public void ReferenceSheet()
    {

        Sim = GameObject.Find("Simulation").GetComponent<SimController>();

        if (GameObject.Find("Reference Sheet Canvas") != null)
            GameObject.Find("Reference Sheet Canvas").SetActive(false);
        else
            GameObject.Find("Reference Sheet").transform.GetChild(0).gameObject.SetActive(true);
    }

    public void MakeDecisions(GameObject nextLayer)
    {
        GameObject.Find("Options").SetActive(false);
        nextLayer.SetActive(true);
        CurrentPage = 1;
        SetButtons();
        // save reference to one (useful for SubmitDecisions())
        Page1 = GameObject.Find("Page 1");
    }

    // This could probably be done a better way, but I got this working quickly
    public void disableOne(GameObject nextPage)
    {
        MinusButton.SetActive(true);
        PlusButton.SetActive(true);
        Page1.SetActive(false);
        nextPage.SetActive(true);

        if (nextPage.name == "Page 2")
        {
            // save reference to one (useful for SubmitDecisions())
            Page2 = nextPage;
            CurrentPage = 2;
            SetButtons();
        }

    }

    public void disableTwo(GameObject nextPage)
    {
        MinusButton.SetActive(true);
        PlusButton.SetActive(true);
        Page2.SetActive(false);
        nextPage.SetActive(true);

        if (nextPage.name == "Page 1")
        {
            CurrentPage = 1;
            SetButtons();
        }
        else
        {
            // save reference to one (useful for SubmitDecisions())
            Page3 = nextPage;
            CurrentPage = 3;
            SetButtons();
        }
    }

    public void disableThree(GameObject nextPage)
    {
        MinusButton.SetActive(true);
        PlusButton.SetActive(true);
        Page3.SetActive(false);
        nextPage.SetActive(true);

        if (nextPage.name == "Page 2")
        {
            CurrentPage = 2;
            SetButtons();
        }
        else
        {
            // save reference to one (useful for SubmitDecisions())
            Page4 = nextPage;
            CurrentPage = 4;
            CostLabel = GameObject.Find("Cost Label").GetComponent<TextMeshProUGUI>();
            CostLabel.text = "Cost: $" + deliveryCost;
        }
    }

    public void disableFour(GameObject nextPage)
    {
        Page4.SetActive(false);
        nextPage.SetActive(true);

        if (nextPage.name == "Page 3")
        {
            CurrentPage = 3;
            SetButtons();
        }
        else
        {
            // save reference to one (useful for SubmitDecisions())
            Page5 = nextPage;
            CurrentPage = 5;
            CostLabel = GameObject.Find("Cost Label").GetComponent<TextMeshProUGUI>();
            CostLabel.text = "Cost: $" + deliveryCost;


        }
    }

    public void disableFive(GameObject nextPage)
    {
        Page5.SetActive(false);
        nextPage.SetActive(true);
        CurrentPage = 4;
        CostLabel = GameObject.Find("Cost Label").GetComponent<TextMeshProUGUI>();
        CostLabel.text = "Cost: $" + deliveryCost;
    }

    
    // Here: verify user choices
    public void SubmitDecisions()
    {
        UnityEngine.Debug.Log("Submitted");

        // Check employee decisions (Has at least 2 employees working)
        for (int i = 0; i < 3; i++)
        {
            Store.ShiftInfo shift = (Store.ShiftInfo)Shifts[i];
            int totalEmployees = shift.TotalEmployees;

            if (totalEmployees < Store.MinEmployeesWorking)
            {
                Page5.SetActive(false);
                switch (i)
                {
                    case 0:
                        Page1.SetActive(true);
                        CurrentPage = 1;
                        SetButtons();
                        break;
                    case 1:
                        Page2.SetActive(true);
                        CurrentPage = 2;
                        SetButtons();
                        break;
                    case 3:
                        Page3.SetActive(true);
                        CurrentPage = 3;
                        SetButtons();
                        break;
                }


                Error.Show("You must have at least " + Store.MinEmployeesWorking + " employees working each shift");
                return;
            }
                       
        }

    }

    // handles the Register Panel (adding Registers)
    public void increment(GameObject quantText)
    {
        TextMeshProUGUI textComponent = quantText.GetComponent<TextMeshProUGUI>();
        int val = int.Parse(textComponent.text);
        if (val < 3)
            val++;

        //  if there was only one employee, activate the decrement button now that they have two
        if (val == 2)
        {
            MinusButton.SetActive(true);
        }
        // disable the plus button, they can not have any more
        else if (val == 3)
        {
            PlusButton.SetActive(false);
        }
        ((Store.ShiftInfo) Shifts[CurrentPage - 1]).addEmployee("Registers");
        textComponent.text = val.ToString();
    }

    // handles the Register Panel (removing Registers)
    public void decrement(GameObject quantText)
    {
        TextMeshProUGUI textComponent = quantText.GetComponent<TextMeshProUGUI>();
        int val = int.Parse(textComponent.text);
        if (val > 1)
            val--;

        //  if there is only one left, de-activate the decrement button 
        if (val == 1)
        {
            MinusButton.SetActive(false);
        }
        // enable the plus button,if they had three and now have two
        else if (val == 2)
        {
            PlusButton.SetActive(true);
        }

        ((Store.ShiftInfo)Shifts[CurrentPage - 1]).removeEmployee("Registers");
        textComponent.text = val.ToString();
    }

    public void ToggleEvent(string dept)
    {
        GameObject toggleSource = GameObject.Find(dept + " " + "Toggle " + CurrentPage);
        if ( toggleSource.GetComponent<Toggle>().isOn)
            ((Store.ShiftInfo)Shifts[CurrentPage - 1]).addEmployee(dept);
        else
            ((Store.ShiftInfo)Shifts[CurrentPage - 1]).removeEmployee(dept);

        UnityEngine.Debug.Log("Num in " + dept + ": " + ((Store.ShiftInfo)Shifts[CurrentPage - 1]).Employees[dept]);
    }

    private void SetButtons()
    {
        //UnityEngine.Debug.Log("CurrentPage: " + CurrentPage);
        MinusButton = GameObject.Find("Minus Button " + CurrentPage);
        PlusButton = GameObject.Find("Plus Button " + CurrentPage);

        UnityEngine.Debug.Log(GameObject.Find("Minus Button " + CurrentPage));

        if (((Store.ShiftInfo)Shifts[CurrentPage - 1]).Employees["Registers"] == 1)
            MinusButton.SetActive(false);
        else if (((Store.ShiftInfo)Shifts[CurrentPage - 1]).Employees["Registers"] == 3)
            PlusButton.SetActive(false);
    }
   
    public void QuantityValueChanged(TMP_InputField source)
    {

        UnityEngine.Debug.Log("The source text: " + source.text);
        string[] sourceName = source.name.Split(' ');
        string foodName = sourceName[0];


        Toggle togStd = GameObject.Find(foodName + " Standard Toggle").GetComponent<Toggle>();
        Toggle togExp = GameObject.Find(foodName + " Expedited Toggle").GetComponent<Toggle>();
        
        if (source.text == "")
        {
            UnityEngine.Debug.Log("Here in 1");
            for (int i = 0; i < Deliveries.Count; i++)
            {
                Delivery d = (Delivery) Deliveries[i];
                
                if (d.foodItem.Name == foodName)
                {
                    decimal canceledCost = d.Cost;
                    deliveryCost -= canceledCost;
                    CostLabel.text = "Cost: $" + deliveryCost;
                    Deliveries.RemoveAt(i);
                    loopFlag = true;
                    togStd.isOn = false;
                    togExp.isOn = false;
                    loopFlag = false;
                    return;
                }
            }
        }
        else if (!regex.IsMatch(source.text) || int.Parse(source.text) > MaxBatches)
        {
            Error.Show("Enter a number of batches (MAX: " + MaxBatches + ")");
            
            bool found = false;
            // reset text
            for (int i = 0; i < Deliveries.Count; i++)
            {
                Delivery d = (Delivery)Deliveries[i];

                if (d.foodItem.Name == foodName)
                {
                    int origQuant = d.Quantity / BatchSize;
                    source.text = origQuant.ToString();
                    
                    return;
                }
            }

            if (!found)
            {
                source.text = "";
                togStd.isOn = false;
                togExp.isOn = false;
            }

        }
        else
        {
            UnityEngine.Debug.Log("Here in 3");
            bool found = false;

            for (int i = 0; i < Deliveries.Count; i++)
            {
                Delivery d = (Delivery) Deliveries[i];

                if (d.foodItem.Name == foodName)
                {

                    UnityEngine.Debug.Log("Here in 4");
                    found = true;
                    int origQuant = d.Quantity / BatchSize;
                    int desiredQuant = int.Parse(source.text);
                    deliveryCost += d.SetQuantity(BatchSize * desiredQuant);
                    // verify they can afford this order
                    if (deliveryCost > Sim.Day.Cash)
                    {
                        decimal missing = deliveryCost - Sim.Day.Cash;
                        Error.Show("You are " + missing.ToString("C2") + " short of editing this order");
                        UnityEngine.Debug.Log("Before");
                        source.text = origQuant.ToString();
                        
                        deliveryCost += d.SetQuantity(BatchSize * origQuant);
                    }
                    
                    CostLabel.text = "Cost: $" + deliveryCost;
                    return;
                }
            }

            if (!found)
            {
                Delivery newDel = new Delivery(Sim.Day.GetFoodItem(foodName), BatchSize * int.Parse(source.text), false);

                // validate input
                if (newDel.Cost + deliveryCost <= Sim.Day.Cash)
                {
                    Deliveries.Add(newDel);
                    deliveryCost += newDel.Cost;
                }
                else
                {
                    decimal missing = (newDel.Cost + deliveryCost) - Sim.Day.Cash;
                    Error.Show("You are " + missing.ToString("C2") + " short of placing this delivery");
                    source.text = "";
                    loopFlag = true;
                    togStd.isOn = false;
                    togExp.isOn = false;
                    loopFlag = false;
                }


                CostLabel.text = "Cost: $" + deliveryCost;
                togStd.isOn = true;
            }

        }

    }


    public void ToggleExp(string foodName)
    {
        if (loopFlag)
            return;
        loopFlag = true;
        TMP_InputField input = GameObject.Find(foodName + " Quantity").GetComponent<TMP_InputField>();
        Toggle togStd = GameObject.Find(foodName + " Standard Toggle").GetComponent<Toggle>();
        Toggle togExp = GameObject.Find(foodName + " Expedited Toggle").GetComponent<Toggle>();
        UnityEngine.Debug.Log("HERE: " + input.text);
        
        // don't allow user to unclick selected toggle
        if (togExp.isOn == false)
        {
            togExp.isOn = true;
            loopFlag = false;
            return;
        }
        else if (input.text == "") 
        {
            togExp.isOn = false;
            loopFlag = false;
            return;
        }

        /*
        if (togStd.isOn)
            togStd.isOn = false;
        */
        for (int i = 0; i < Deliveries.Count; i++)
        {
            Delivery d = (Delivery) Deliveries[i];
            if (d.foodItem.Name == foodName)
            { 
                decimal addedCost = d.SetExpedited(true);

                // verify they can afford to set this delivery as expedited
                if (deliveryCost + addedCost > Sim.Day.Cash)
                {
                    decimal missing = (deliveryCost + addedCost) - Sim.Day.Cash;
                    Error.Show("You are " + missing.ToString("C2") + " short of making this delivery expedited");
                    togStd.isOn = true;
                    togExp.isOn = false;
                    d.SetExpedited(false);
                }
                else
                {
                    togStd.isOn = false;
                    togExp.isOn = true;
                    deliveryCost += addedCost;
                }

                CostLabel.text = "Cost: $" + deliveryCost;
                loopFlag = false;
                return;
            }
        }
    }

    public void ToggleStd(string foodName)
    {
        if (loopFlag)
            return;
        loopFlag = true;
        TMP_InputField input = GameObject.Find(foodName + " Quantity").GetComponent<TMP_InputField>();
        Toggle togStd = GameObject.Find(foodName + " Standard Toggle").GetComponent<Toggle>();
        Toggle togExp = GameObject.Find(foodName + " Expedited Toggle").GetComponent<Toggle>();

        // don't allow user to unclick selected toggle
        if (togStd.isOn == false)
        {
            togStd.isOn = true;
            loopFlag = false;
            return;
        }
        else if (input.text == "") 
        {
            togStd.isOn = false;
            loopFlag = false;
            return;
        }

        if (togExp.isOn)
            togExp.isOn = false;

        for (int i = 0; i < Deliveries.Count; i++)
        {
            Delivery d = (Delivery) Deliveries[i];

            if (d.foodItem.Name == foodName)
            {
                decimal savings = d.SetExpedited(false);
                deliveryCost += savings;
                CostLabel.text = "Cost: $" + deliveryCost;
                loopFlag = false;
                return;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Shifts = new ArrayList();
        Deliveries = new ArrayList();
        Sim = GameObject.Find("Simulation").GetComponent<SimController>();
        Error = GameObject.Find("ErrorMessage").GetComponent<ShowErrorMessage>();
        deliveryCost = 0.00m;

        for (int i = 0; i < 3; i++)
        {
            Shifts.Add(new Store.ShiftInfo());
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
