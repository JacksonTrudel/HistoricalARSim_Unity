using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowErrorMessage : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI error;

    public void Show(string message)
    {
        StartCoroutine(DisplayError(message, 5));
    }

    IEnumerator DisplayError(string message, float delay) {
        Debug.Log(message);
        error.text = message;
        gameObject.transform.localScale = new Vector3(1, 1, 1); // shows the error message, kinda hacky
        //Vector3 originalPosition = new Vector3(2, 2, 2);
        //gameObject.transform.Translate(originalPosition);
        yield return new WaitForSeconds(delay);
        gameObject.transform.localScale = new Vector3(0, 0, 0); // hides error
    }
}
