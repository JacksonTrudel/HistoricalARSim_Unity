using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLayer4Menu : MonoBehaviour
{
    public void Layer4(GameObject layer4Menu)
    {
        gameObject.SetActive(false);
        layer4Menu.SetActive(true);
    }
}
