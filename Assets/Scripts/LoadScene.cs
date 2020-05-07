using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    TimeController Time;

    public void LoadNewScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        /*
        Scene CurrentScene = SceneManager.GetActiveScene();
        //Debug.Log("Before scene change: currentScene =  " + CurrentScene.name + " sceneName: " + sceneName);
        if (CurrentScene.name == "Simulation")
        {
            Time = GameObject.Find("UI Canvas").GetComponent<TimeController>();

            if (TimeController.day_bool == false)
            {
                TimeController.day_bool = true;
                IEnumerator coroutine = Time.NextHour(false);

                Time.StartCoroutine(coroutine);

            }

            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }


        else if (CurrentScene.name == "Main Menu")
        {
            Simulation.Cash = 10000;
            SceneManager.LoadScene(sceneName);

            //start useless code:
            //I'm pretty sure none of this code below runs because scene loading interrupts the scripts
            Time = GameObject.Find("UI Canvas").GetComponent<TimeController>();

            if (sceneName != "Historical AR")
                TimeController.day_bool = true;
            else
                TimeController.day_bool = false;
            // TimeController.Day = 1;

            IEnumerator coroutine = Time.NextHour(false);

            if (sceneName != "Historical AR")
                Time.StartCoroutine(coroutine);
           //end useless code
            
        }
        
        else if (CurrentScene.name == "Historical AR")
        {
            SceneManager.LoadScene(sceneName);

            GameObject obj = GameObject.Find("Continue Button Simulation");
            obj.SetActive(false);
            Debug.Log("we are here TimeController line 130");
        }

        else if (CurrentScene.name == "End")
        {
            SceneManager.LoadScene(sceneName);
        }

        else
        {
            if (TimeController.Day != 8)
            {
                
                Time = GameObject.Find("UI Canvas").GetComponent<TimeController>();

                if (TimeController.day_bool == false)
                {
                    TimeController.day_bool = true;
                    IEnumerator coroutine = Time.NextHour(false);

                    if (sceneName != "Historical AR")
                        Time.StartCoroutine(coroutine);

                }

                SceneManager.LoadScene(sceneName);
            }

            else 
            {
                SceneManager.LoadScene("End");

            }

            
        }

        //CurrentScene = SceneManager.GetActiveScene();
        //Debug.Log("After scene change: currentScene =  " + CurrentScene.name + " sceneName: " + sceneName);
        */
    }
}
