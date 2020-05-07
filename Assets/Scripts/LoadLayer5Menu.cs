using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLayer5Menu : MonoBehaviour
{
    public void Layer5(GameObject layer5Menu)
    {
        gameObject.SetActive(false);
        layer5Menu.SetActive(true);
    }
}
