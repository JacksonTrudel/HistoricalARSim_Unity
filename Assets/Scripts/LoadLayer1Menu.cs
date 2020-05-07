using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLayer1Menu : MonoBehaviour
{
    public void Layer1(GameObject layer1Menu)
    {
        gameObject.SetActive(false);
        layer1Menu.SetActive(true);
    }
}
