using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLayer2Menu : MonoBehaviour
{
    public void Layer2(GameObject layer2Menu)
    {
        gameObject.SetActive(false);
        layer2Menu.SetActive(true);
    }
}
