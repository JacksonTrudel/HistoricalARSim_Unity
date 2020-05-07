using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu2 : MonoBehaviour
{
    TimeController Time;

    Text[] Food_Header;
    Canvas Stock;
    Canvas EmployeeStatusPie;
    Text[] produce;
    Text[] dry;
    Text[] frozen;
    Text[] dairy;
    Text[] Idle;
    Text[] Offsite;
    Text registerDebug;

    public void Menu()
    {
        //if one of our menu button children is active aka displayed, turn the rest off
        //make sure our menu children are always in the right order if this breaks
        if (transform.GetChild(1).gameObject.activeInHierarchy)
        {
            for (int i = 1; i <= transform.childCount-1; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            
            GetComponentInChildren<Text>().text = "Open Menu";

        }

        //otherwise turn our menu button children on with our fancy animation
        else
        {
            GetComponentInChildren<Text>().text = "Close Menu";
            StartCoroutine("Grow");
        }
    }

    IEnumerator Grow()
    {
        for (int i = 1; i <= transform.childCount-1; i++)
        {
            transform.GetChild(i).localScale = new Vector3(0, 0, 0);
            transform.GetChild(i).gameObject.SetActive(true);

            float x = 0, y = 0, z = 0;

            for (int j = 0; j < 5; j++)
            {
                x += .2f;
                y += .2f;
                z += .2f;

                transform.GetChild(i).localScale = new Vector3(x, y, z);
                yield return new WaitForSeconds(.5f);
            }
        }
    }

    public void Exit_sim()
    {
        //reset the static day
        TimeController.Day = 1;
        //reset the static Cash
        Simulation.Cash = 10000;

        //clear all upcoming deliveries
        TimeController.deliveries.Clear();
        Object.Destroy(GameObject.Find("Simulation"));
        SceneManager.LoadScene("Main Menu");
    }

    public void Hide_Debug()
    {
        Food_Header = GameObject.Find("Foods").GetComponentsInChildren<Text>();
        Stock = GameObject.Find("stock_Pie").GetComponent<Canvas>();
        EmployeeStatusPie = GameObject.Find("EmployeeStatPieCanvas").GetComponent<Canvas>();

        produce = GameObject.FindGameObjectWithTag("produceText").GetComponentsInChildren<Text>();
        dry = GameObject.FindGameObjectWithTag("DryGoodsText").GetComponentsInChildren<Text>();
        frozen = GameObject.FindGameObjectWithTag("FrozenText").GetComponentsInChildren<Text>();
        dairy = GameObject.FindGameObjectWithTag("DairyText").GetComponentsInChildren<Text>();
        Idle = GameObject.FindGameObjectWithTag("IdleText").GetComponentsInChildren<Text>();
        Offsite = GameObject.FindGameObjectWithTag("OffsiteText").GetComponentsInChildren<Text>();
        registerDebug = GameObject.FindGameObjectWithTag("RegisterDebug").GetComponentInChildren<Text>();

        for (int i = 0; i < Food_Header.Length; i++)
        {
            Food_Header[i].enabled = !Food_Header[i].enabled;
        }

        for(int i = 0; i < 2; i++) 
        {
            produce[i].enabled = !produce[i].enabled;
            dry[i].enabled = !dry[i].enabled;
            frozen[i].enabled = !frozen[i].enabled;
            dairy[i].enabled = !dairy[i].enabled;
            Offsite[i].enabled = !Offsite[i].enabled;
            Idle[i].enabled = !Idle[i].enabled;
        }
        Stock.enabled = !Stock.enabled;
        EmployeeStatusPie.enabled = !EmployeeStatusPie.enabled;

        registerDebug.enabled = !registerDebug.enabled;

    }

    public void skip_day() 
    {
        Time = GameObject.Find("UI Canvas").GetComponent<TimeController>();

        Time.Hour = 19;
        Time.Minute = 59;

    }

    public void ReferenceSheet()
    {
        
        if (GameObject.Find("Reference Sheet Canvas") != null)
            GameObject.Find("Reference Sheet Canvas").SetActive(false);
        else
            GameObject.Find("Reference Sheet").transform.GetChild(0).gameObject.SetActive(true);
    }
    
}
