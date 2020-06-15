using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using UnityEngine.UI;
// using System.Diagnostics;


using Firebase.Storage;
using System.Threading.Tasks;


public class LogManager : MonoBehaviour
{
    public static string file_title { get; private set; }
    static string[] FilePaths = new string[2];

    public static string[] filePaths
    {
        get { return FilePaths; }
    }

    static StreamWriter writer1;

    static StreamWriter writer2;

    static FirebaseStorage storage;

    // Create a storage reference from our storage service
    static StorageReference storage_ref;

    // Create a storage reference to our file(s)
    static StorageReference fileFromUnity;

    static StorageReference fileFromAndroid;


    void Start()
    {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int cur_time = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
        file_title = cur_time.ToString();

        FilePaths[0] = Application.persistentDataPath + file_title + ".txt";
        FilePaths[1] = "Assets/Resources/Logs/" + file_title + ".txt";

        // singleton pattern
        if (FindObjectsOfType<LogManager>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

        if (storage == null)
            storage = FirebaseStorage.DefaultInstance;
        if (storage_ref == null)   
            storage_ref = storage.GetReferenceFromUrl("gs://historical-ar-simulation.appspot.com");
        if (fileFromUnity == null) 
            fileFromUnity = storage_ref.Child("Assets/Resources/Logs/" + file_title + ".txt");
        if (fileFromAndroid == null) 
            fileFromAndroid = storage_ref.Child(Application.persistentDataPath + LogManager.file_title + ".txt");

        StreamWriter writer;
        for (int i = 0; i < 2; i++) 
        {
            // Write some text
            writer = new StreamWriter(FilePaths[i], true);

            writer.WriteLine("Day#Hour#Message#Current Cash#Net Change#Revenue#Delivery Expense#Employee Expense#FOH#BOH#Items Sold#Items Expired#Upcoming Deliveries");
            writer.WriteLine("1#8:00#Day 1: Begin Simulation");
            writer.Close();
        }
    }

    public static void uploadFirebaseUnity()
    {
        if (fileFromUnity == null)
        {
            Debug.Log("fileFromUnity is null");
            return;
        }
        fileFromUnity.PutFileAsync(LogManager.filePaths[1]).ContinueWith((Task<StorageMetadata> task) =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
                // Uh-oh, an error occurred!
            }
            else
            {
                // Metadata contains file metadata such as size, content-type, and download URL.
                StorageMetadata metadata = task.Result;
                string download_url = fileFromUnity.GetDownloadUrlAsync().ToString();
                Debug.Log("Finished uploading to Firebase...");
            }
        });
    }

    public static void uploadFirebaseAndroid()
    {
        if (fileFromAndroid == null)
        {
            Debug.Log("fileFromAndroid is null");
            return;
        }
        fileFromAndroid.PutFileAsync(filePaths[0]).ContinueWith((Task<StorageMetadata> task) =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
                // Uh-oh, an error occurred!
            }
            else
            {
                // Metadata contains file metadata such as size, content-type, and download URL.
                StorageMetadata metadata = task.Result;
                string download_url = fileFromAndroid.GetDownloadUrlAsync().ToString();
                Debug.Log("Finished uploading to Firebase...");
            }
        });
    }

    
    // Start is called before the first frame update
    public static void LogDay()
    {
        Debug.Log("logday called");
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int cur_time = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
        file_title = cur_time.ToString();

        FilePaths[0] = Application.persistentDataPath + file_title + ".txt";
        FilePaths[1] = "Assets/Resources/Logs/" + file_title + ".txt";

        float net_change = (float) (SimController.Day.Cash - SimController.Day.StartOfDayCash);
        string net_change_string;

        if (net_change < 0)
        {
            net_change_string = "-$" + string.Format("{0:n}", Mathf.Abs(net_change));
        }

        else
        {
            net_change_string = "$" + string.Format("{0:n}", net_change);
        }

        if (SimController.DayNum == 1)
        {
            UnityEngine.Debug.Log("Opening writer 1");
            writer1 = new StreamWriter(FilePaths[0], true);
            //if(SimController.DayNum == 1)
            writer1.WriteLine("Day Number#Starting Cash#End Cash#Net Change#Revenue#Deliveries" +
                " Ordered#Delivery Expense#Employee Expense#FOH#BOH#Items Sold#Items Expired#Overflow Items#Upcoming Deliveries");
        }

        string entry = SimController.DayNum + "#" + SimController.Day.StartOfDayCash + "#" + SimController.Day.Cash + "#";
        entry += net_change_string + "#" + SimController.Day.DailyRevenue.ToString("C2") + "#" + SimController.Day.DeliveriesOrdered + "#" + SimController.Day.DailyDeliveryCost.ToString("C2") + "#";
        entry += SimController.Day.DailyEmployeePayout.ToString("C2") + "#" + PostDayController.TotalFOH + "#" + PostDayController.TotalBOH + "#" + SimController.Day.DailyItemsSold + "#";
        entry += SimController.Day.TotalExpired + "#" + SimController.Day.totalOverFlow + "#" + SimController.Day.Deliveries.Count;
        writer1.WriteLine(entry);

        if (SimController.DayNum == 1)
        {
            UnityEngine.Debug.Log("Opening writer 1");
            writer2 = new StreamWriter(FilePaths[1], true);
            //if(SimController.DayNum == 1)
            writer2.WriteLine("Day Number#Starting Cash#End Cash#Net Change#Revenue#Deliveries Ordered#Delivery" +
                " Expense#Employee Expense#FOH#BOH#Items Sold#Items Expired#Overflow Items#Upcoming Deliveries");
        }

        writer2.WriteLine(entry);

        if (SimController.DayNum == 7)
        {
            writer1.Close();
            writer2.Close();
        }

        Resources.Load(FilePaths[1]);
        uploadFirebaseUnity();
        uploadFirebaseAndroid();
    }
}
