using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LogInfo : MonoBehaviour
{
    // [SerializeField] InputField firstName;
    // [SerializeField] InputField lastName;
    // [SerializeField] InputField jobRole;
    // [SerializeField] InputField storeName;
    // [SerializeField] InputField storeNumber;
    // string fs, ls, jr, sName, sNum;

    [SerializeField] InputField[] userInfo;

    [SerializeField] bool toggleAppend = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void LogUserInfo()
    {
        StreamWriter sw = new StreamWriter("Assets/Resources/User_Info/user_info.txt", toggleAppend);
        foreach (InputField inputInfo in userInfo)
        {
            sw.WriteLine(inputInfo.text);
        }

        sw.Close();
    }
}
