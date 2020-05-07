using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    public static DontDestroyOnLoad instance = null;

    void Awake()

    {
        
            if (instance == null)
            {
                //set instance to the gameobject we've placed the script on
                instance = this;
            }

            //If instance already exists and it's not this:
            else if (instance != this)
            {
                Destroy(gameObject);
            }


            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);
        

    }

}
