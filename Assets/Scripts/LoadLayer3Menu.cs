using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLayer3Menu : MonoBehaviour
{
    public void Layer3(GameObject layer3Menu)
    {
        gameObject.SetActive(false);
        layer3Menu.SetActive(true);
    }
}
