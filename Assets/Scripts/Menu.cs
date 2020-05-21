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
    private int CurrentPage;
    ShowErrorMessage Error;
    
    // Values to be passed to Store script once decisions are completed
    private ArrayList Shifts;
    private ArrayList Deliveries;

    // Array of plus and minus buttons (size = 5), used on pages 1 and 2 of menu
    private GameObject [] PlusButton;
    private GameObject [] MinusButton;
    private TextMeshProUGUI CostLabel;
    private decimal DeliveryCost;

    private GameObject Page1;
    private GameObject Page2;
    private GameObject Page3;
    private GameObject Page4;
    private GameObject Page5;

    private static int MaxBatches = 5;
    private static int BatchSize = 25;

    
    private bool loopFlag = false;

    public void ReferenceSheet()
    {

        if (GameObject.Find("Reference Sheet Canvas") != null)
            GameObject.Find("Reference Sheet Canvas").SetActive(false);
        else
            GameObject.Find("Reference Sheet").transform.GetChild(0).gameObject.SetActive(true);
    }

    public void MakeDecisions(GameObject nextLayer)
    {
        GameObject.Find("Options").SetActive(false); 
        nextLayer.SetActive(true);

        if (SimController.DayNum == 1)
        {
            GameObject.Find("PrevReport Button").SetActive(false);
        }
        CurrentPage = 1;
        SetButtons();
        // save reference to one (useful for SubmitDecisions())
        Page1 = GameObject.Find("Page 1");

 
    }

    // This could probably be done a better way, but I got this working quickly
    public void disableOne(GameObject nextPage)
    {
        // first enable all buttons
        for (int i = 0; i <= 4; i++)
        {
            MinusButton[i].SetActive(true);
            PlusButton[i].SetActive(true);
        }
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
        // first enable all buttons
        for (int i = 0; i <= 4; i++)
        {
            MinusButton[i].SetActive(true);
            PlusButton[i].SetActive(true);
        }
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
        // first enable all buttons
        for (int i = 0; i <= 4; i++)
        {
            MinusButton[i].SetActive(true);
            PlusButton[i].SetActive(true);
        }
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
            CostLabel.text = "Cost: $" + DeliveryCost;
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
            CostLabel.text = "Cost: $" + DeliveryCost;


        }
    }

    public void disableFive(GameObject nextPage)
    {
        Page5.SetActive(false);
        nextPage.SetActive(true);
        CurrentPage = 4;
        CostLabel = GameObject.Find("Cost Label").GetComponent<TextMeshProUGUI>();
        CostLabel.text = "Cost: $" + DeliveryCost;
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
                    case 2:
                        Page3.SetActive(true);
                        CurrentPage = 3;
                        SetButtons();
                        break;
                }


                Error.Show("You must have at least " + Store.MinEmployeesWorking + " employees working each shift");
                return;
            }
                       
        }

        SimController.Day.SetShifts(Shifts);
        // reset shifts arraylist. Precaution, shouldn't be necessary in final treatment
        Shifts = new ArrayList();
        for (int i = 0; i < 3; i++)
        {
            Shifts.Add(new Store.ShiftInfo());
        }

        SimController.Day.SetDeliveries(Deliveries, DeliveryCost);
        Deliveries = new ArrayList();

        SimController.Day.RunDay();
    }

    // handles the Register Panel (adding Registers)
    public void increment(GameObject quantText)
    {
        string[] buttonName = quantText.name.Split(' ');
        bool isRegText = (buttonName[2] == "Reg");
        int buttonIdx;
        switch (buttonName[2])
        {
            case "Produce":
                buttonIdx = 0;
                break;
            case "Dairy":
                buttonIdx = 1;
                break;
            case "Dry":
                buttonIdx = 2;
                break;
            case "Frozen":
                buttonIdx = 3;
                break;
            default:
                buttonIdx = 4;
                break;
        }

        // check whether they have exceeded max # employees working each shift
        if (((Store.ShiftInfo)Shifts[CurrentPage - 1]).TotalEmployees == Store.MAX_EMPLOYEES)
        {
            Error.Show("You can have a max of " + Store.MAX_EMPLOYEES + " employees working each shift.");
            return;
        }

        TextMeshProUGUI textComponent = quantText.GetComponent<TextMeshProUGUI>();
        int val = int.Parse(textComponent.text);
        val++;
        if (isRegText)
        {
            //  if there was only one employee, activate the decrement button now that they have two
            if (val == 2)
            {
                MinusButton[4].SetActive(true);
            }
            // disable the plus button, they can not have any more
            else if (val == 3)
            {
                PlusButton[4].SetActive(false);
            }
        }
        else
        {
            if (val == 1)
            {
                MinusButton[buttonIdx].SetActive(true);
            }
            else if (val == 5)
            {
                PlusButton[buttonIdx].SetActive(false);
            }
        }

        ((Store.ShiftInfo)Shifts[CurrentPage - 1]).addEmployee(Store.DepartmentNames[buttonIdx]);
        textComponent.text = val.ToString();
    }

    // handles the Register Panel (removing Registers)
    public void decrement(GameObject quantText)
    {
        string[] buttonName = quantText.name.Split(' ');
        bool isRegText = (buttonName[2] == "Reg");
        int buttonIdx;
        switch (buttonName[2])
        {
            case "Produce":
                buttonIdx = 0;
                break;
            case "Dairy":
                buttonIdx = 1;
                break;
            case "Dry":
                buttonIdx = 2;
                break;
            case "Frozen":
                buttonIdx = 3;
                break;
            default:
                buttonIdx = 4;
                break;
        }

        TextMeshProUGUI textComponent = quantText.GetComponent<TextMeshProUGUI>();
        int val = int.Parse(textComponent.text);
        val--;

        if (isRegText)
        {
           
            //  if there was only one employee, activate the decrement button now that they have two
            if (val == 1)
            {
                MinusButton[4].SetActive(false);
            }
            // disable the plus button, they can not have any more
            else if (val == 2)
            {
                PlusButton[4].SetActive(true);
            }
        }
        else
        {
            
            if (val == 0)
            {
                MinusButton[buttonIdx].SetActive(false);
            }
            else if (val == 4)
            {
                PlusButton[buttonIdx].SetActive(true);
            }

            
        }

        ((Store.ShiftInfo)Shifts[CurrentPage - 1]).removeEmployee(Store.DepartmentNames[buttonIdx]);
        textComponent.text = val.ToString();
    }

    private void SetButtons()
    {
        //UnityEngine.Debug.Log("CurrentPage: " + CurrentPage);
        MinusButton[0] = GameObject.Find("Produce Minus Button " + CurrentPage);
        PlusButton[0] = GameObject.Find("Produce Plus Button " + CurrentPage);
        MinusButton[1] = GameObject.Find("Dairy Minus Button " + CurrentPage);
        PlusButton[1] = GameObject.Find("Dairy Plus Button " + CurrentPage);
        MinusButton[2] = GameObject.Find("Dry Minus Button " + CurrentPage);
        PlusButton[2] = GameObject.Find("Dry Plus Button " + CurrentPage);
        MinusButton[3] = GameObject.Find("Frozen Minus Button " + CurrentPage);
        PlusButton[3] = GameObject.Find("Frozen Plus Button " + CurrentPage);
        MinusButton[4] = GameObject.Find("Reg Minus Button " + CurrentPage);
        PlusButton[4] = GameObject.Find("Reg Plus Button " + CurrentPage);

        UnityEngine.Debug.Log(GameObject.Find("Minus Button " + CurrentPage));
        
        // de-activate minus and plus buttons where necessary
        Dictionary<string, int>.ValueCollection empPerDept = ((Store.ShiftInfo)Shifts[CurrentPage - 1]).Employees.Values;
        int idx = 0;
        foreach (int val in empPerDept)
        {
            UnityEngine.Debug.Log(MinusButton[idx].name);
            if (val == 0)
            {
                MinusButton[idx].SetActive(false);
            }
            else if (val == 5)
            {
                PlusButton[idx].SetActive(false);
            }
            idx++;

            // register case handled below
            if (idx == 4)
                break;
        }
        UnityEngine.Debug.Log("Current Page: " + CurrentPage);
        if (((Store.ShiftInfo)Shifts[CurrentPage - 1]).Employees["Registers"] == 1)
        {
            MinusButton[4].SetActive(false);
            UnityEngine.Debug.Log("Minus button deactivated");
        }
        else if (((Store.ShiftInfo)Shifts[CurrentPage - 1]).Employees["Registers"] == 3)
        {
            PlusButton[4].SetActive(false);
            UnityEngine.Debug.Log("Plus button deactivated");
        }
    }
   

    public void QuantityValueChanged(TMP_InputField source)
    {
        int garbVar;
        //UnityEngine.Debug.Log("The source text: " + source.text + " matches? " + int.TryParse(source.text, out a));
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
                    DeliveryCost -= canceledCost;
                    CostLabel.text = "Cost: $" + DeliveryCost;
                    Deliveries.RemoveAt(i);
                    loopFlag = true;
                    togStd.isOn = false;
                    togExp.isOn = false;
                    loopFlag = false;
                    return;
                }
            }
        }
        else if (!int.TryParse(source.text, out garbVar) || garbVar == 0)
        {
            Error.Show("Enter a number of batches (1 - " + MaxBatches + ")");
            
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
                    DeliveryCost += d.SetQuantity(BatchSize * desiredQuant);
                    // verify they can afford this order
                    if (DeliveryCost > SimController.Day.Cash)
                    {
                        decimal missing = DeliveryCost - SimController.Day.Cash;
                        Error.Show("You are " + missing.ToString("C2") + " short of editing this order");
                        UnityEngine.Debug.Log("Before");
                        source.text = origQuant.ToString();
                        
                        DeliveryCost += d.SetQuantity(BatchSize * origQuant);
                    }
                    
                    CostLabel.text = "Cost: $" + DeliveryCost;
                    return;
                }
            }

            if (!found)
            {
                Delivery newDel = new Delivery(SimController.Day.GetFoodItem(foodName), BatchSize * int.Parse(source.text));

                // validate input
                if (newDel.Cost + DeliveryCost <= SimController.Day.Cash)
                {
                    Deliveries.Add(newDel);
                    DeliveryCost += newDel.Cost;
                }
                else
                {
                    decimal missing = (newDel.Cost + DeliveryCost) - SimController.Day.Cash;
                    Error.Show("You are " + missing.ToString("C2") + " short of placing this delivery");
                    source.text = "";
                    loopFlag = true;
                    togStd.isOn = false;
                    togExp.isOn = false;
                    loopFlag = false;
                }


                CostLabel.text = "Cost: $" + DeliveryCost;
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
                if (DeliveryCost + addedCost > SimController.Day.Cash)
                {
                    decimal missing = (DeliveryCost + addedCost) - SimController.Day.Cash;
                    Error.Show("You are " + missing.ToString("C2") + " short of making this delivery expedited");
                    togStd.isOn = true;
                    togExp.isOn = false;
                    d.SetExpedited(false);
                }
                else
                {
                    togStd.isOn = false;
                    togExp.isOn = true;
                    DeliveryCost += addedCost;
                }

                CostLabel.text = "Cost: $" + DeliveryCost;
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
                DeliveryCost += savings;
                CostLabel.text = "Cost: $" + DeliveryCost;
                loopFlag = false;
                return;
            }
        }
    }

    public void ShowPrevEOD(GameObject PrevEODReportPanel)
    {
        PrevEODReportPanel.SetActive(true);
        Text status_text = GameObject.Find("Status Text").GetComponent<Text>();
        status_text.text = PostDayController.LastReport;
    }

    public void ReturnFromPrevEOD(GameObject PrevEODReportPanel)
    {
        PrevEODReportPanel.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        Shifts = new ArrayList();
        Deliveries = new ArrayList();
        //Sim = GameObject.Find("SimController").GetComponent<SimController>();
        Error = GameObject.Find("ErrorMessage").GetComponent<ShowErrorMessage>();
        DeliveryCost = 0.00m;
        MinusButton = new GameObject[5];
        PlusButton = new GameObject[5];

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
