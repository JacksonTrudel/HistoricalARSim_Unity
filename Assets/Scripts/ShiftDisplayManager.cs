using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShiftDisplayManager : MonoBehaviour
{
    // 1 if on shift 1, else 0
    int activeShift;
    Simulation sim;
    Text RegisterText, RestockText;
    ShowErrorMessage error;


    // Start is called before the first frame update
    void Start()
    {
        activeShift = 1;
        sim = GameObject.Find("Simulation").GetComponent<Simulation>();

        error = GameObject.Find("ErrorMessage").GetComponent<ShowErrorMessage>();
        int[] shiftInfo = sim.GetShiftInfo();
        RegisterText = GameObject.Find("Register Text").GetComponent<Text>();
        RestockText = GameObject.Find("Restock Text").GetComponent<Text>();

        RegisterText.text = shiftInfo[0].ToString();
        RestockText.text = shiftInfo[1].ToString();
    }
 
    public void SwitchShift(GameObject backPanel)
    {

        string activePanelName = (activeShift == 1) ? "Shift 1 Panel" : "Shift 2 Panel";
        string otherPanelName = (activeShift == 1) ? "Shift 2 Panel" : "Shift 1 Panel";
        activeShift = (activeShift == 1) ? 0 : 1;

        Image image = GameObject.Find(activePanelName).GetComponent<Image>();
            //GetComponent(activePanelName).;

        // Make current shift background transparent
        Color c = image.color;
        c.a = 0;
        image.color = c;

        // Make Shift 2 Background Opaque
        image = GameObject.Find(otherPanelName).GetComponent<Image>();
        c = image.color;
        c.a = 180;
        image.color = c;

        FixPanel();
    }

    // Called from SwitchShift()
    private void FixPanel()
    {
        int[] shiftInfo = sim.GetShiftInfo();
        RegisterText = GameObject.Find("Register Text").GetComponent<Text>();
        RestockText = GameObject.Find("Restock Text").GetComponent<Text>();
        if (activeShift == 1)
        {
            RegisterText.text = shiftInfo[0].ToString();
            RestockText.text = shiftInfo[1].ToString();
        }
        else
        {
            RegisterText.text = shiftInfo[2].ToString();
            RestockText.text = shiftInfo[3].ToString();
        }
    }

    public void IncrementRegister(GameObject textObject)
    {
        Text textComponent = textObject.GetComponent<Text>();
        int val = int.Parse(textComponent.text);
        val++;

        // Ensure there aren't too many employees on registers 
        int[] shiftInfo = sim.GetShiftInfo();
        if (activeShift == 1)
        {
            if (val > sim.MaximumRegisters)
                error.Show($"Error: You can have at most {sim.MaximumRegisters} registers open.");
            else if (val + shiftInfo[1] > sim.MaximumEmployees)
                error.Show($"Error: You can have at most {sim.MaximumEmployees} employees working.");
            else
            {
                textComponent.text = val.ToString();
                sim.SetShiftInfo(0, val);
            }

        }
        else
        {
            if (val > sim.MaximumRegisters)
                error.Show($"Error: You can have at most {sim.MaximumRegisters} registers open.");
            else if (val + shiftInfo[3] > sim.MaximumEmployees)
                error.Show($"Error: You can have at most {sim.MaximumEmployees} employees working.");
            else
            {
                textComponent.text = val.ToString();
                sim.SetShiftInfo(2, val);
            }
        }
    }

    public void DecrementRegister(GameObject textObject)
    {
        Text textComponent = textObject.GetComponent<Text>();
        int val = int.Parse(textComponent.text);
        val--;

        // Ensure there would be enough people on registers 
        int[] shiftInfo = sim.GetShiftInfo();
        if (activeShift == 1)
        {
            if (val < sim.MinimumRegisters)
                error.Show($"Error: You must have at least {sim.MinimumRegisters} registers open.");
            else if (val + shiftInfo[1] < sim.MinimumEmployees)
                error.Show($"Error: You must have at least {sim.MinimumEmployees} employees working.");
            else
            {
                textComponent.text = val.ToString();
                sim.SetShiftInfo(0, val);
            }

        }
        else
        {
            if (val < sim.MinimumRegisters)
                error.Show($"Error: You must have at least {sim.MinimumRegisters} registers open.");
            else if (val + shiftInfo[3] < sim.MinimumEmployees)
                error.Show($"Error: You must have at least {sim.MinimumEmployees} employees working.");
            else
            {
                textComponent.text = val.ToString();
                sim.SetShiftInfo(2, val);
            }
        }
    }

    public void IncrementRestock(GameObject textObject)
    {
        Text textComponent = textObject.GetComponent<Text>();
        int val = int.Parse(textComponent.text);
        val++;
        // Ensure there aren't too many employees working 
        int[] shiftInfo = sim.GetShiftInfo();
        if (activeShift == 1)
        {
            if (val + shiftInfo[0] > sim.MaximumEmployees)
                error.Show($"Error: You can have at most {sim.MaximumEmployees} employees working.");
            else
            {
                textComponent.text = val.ToString();
                sim.SetShiftInfo(1, val);
            }

        }
        else
        {
            if (val + shiftInfo[2] > sim.MaximumEmployees)
                error.Show($"Error: You can have at most {sim.MaximumEmployees} employees working.");
            else
            {
                textComponent.text = val.ToString();
                sim.SetShiftInfo(3, val);
            }
        }
    }

    public void DecrementRestock(GameObject textObject)
    {
        Text textComponent = textObject.GetComponent<Text>();
        int val = int.Parse(textComponent.text);
        val--;

        // Ensure there would be enough people on registers 
        int[] shiftInfo = sim.GetShiftInfo();
        if (activeShift == 1)
        {
            if (val + shiftInfo[0] < sim.MinimumEmployees)
                error.Show($"Error: You must have at least {sim.MinimumEmployees} employees working.");
            else
            {
                textComponent.text = val.ToString();
                sim.SetShiftInfo(1, val);
            }

        }
        else
        {
            if (val + shiftInfo[2] < sim.MinimumEmployees)
                error.Show($"Error: You must have at least {sim.MinimumEmployees} employees working.");
            else
            {
                textComponent.text = val.ToString();
                sim.SetShiftInfo(3, val);
            }
        }

    }

}
