using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Storage;
using System.Threading.Tasks;

public class LogToFireBase
{
    // static string file_title;
    // static string[] FilePaths = new string[2];

    // static FirebaseStorage storage;

    // // Create a storage reference from our storage service
    // static StorageReference storage_ref;

    // // Create a storage reference to our file(s)
    // static StorageReference fileFromUnity;

    // static StorageReference fileFromAndroid;

    // [RuntimeInitializeOnLoadMethod]
    // static void OnRuntimeMethodLoad()
    // {
    //     // FilePaths[0] = Application.persistentDataPath + file_title + ".txt";
    //     // FilePaths[1] = "Assets/Resources/Logs/" + file_title + ".txt";

    //     storage = FirebaseStorage.DefaultInstance;
    //     storage_ref = storage.GetReferenceFromUrl("gs://historical-ar-simulation.appspot.com");

    //     fileFromUnity = storage_ref.Child("Assets/Resources/" + LogManager.file_title + ".txt");
    //     fileFromAndroid = storage_ref.Child(Application.persistentDataPath + LogManager.file_title + ".txt");
    // }

    // public static void uploadFirebaseUnity()
    // {
    //     if (fileFromUnity == null)
    //     {
    //         Debug.Log("fileFromUnity is null");
    //         return;
    //     }
    //     fileFromUnity.PutFileAsync(LogManager.filePaths[1]).ContinueWith((Task<StorageMetadata> task) =>
    //     {
    //         if (task.IsFaulted || task.IsCanceled)
    //         {
    //             Debug.Log(task.Exception.ToString());
    //             // Uh-oh, an error occurred!
    //         }
    //         else
    //         {
    //             // Metadata contains file metadata such as size, content-type, and download URL.
    //             StorageMetadata metadata = task.Result;
    //             string download_url = fileFromUnity.GetDownloadUrlAsync().ToString();
    //             Debug.Log("Finished uploading to Firebase...");
    //         }
    //     });
    // }

    // public static void uploadFirebaseAndroid()
    // {
    //     if (fileFromAndroid == null)
    //     {
    //         Debug.Log("fileFromAndroid is null");
    //         return;
    //     }
    //     fileFromAndroid.PutFileAsync(LogManager.filePaths[0]).ContinueWith((Task<StorageMetadata> task) =>
    //     {
    //         if (task.IsFaulted || task.IsCanceled)
    //         {
    //             Debug.Log(task.Exception.ToString());
    //             // Uh-oh, an error occurred!
    //         }
    //         else
    //         {
    //             // Metadata contains file metadata such as size, content-type, and download URL.
    //             StorageMetadata metadata = task.Result;
    //             string download_url = fileFromAndroid.GetDownloadUrlAsync().ToString();
    //             Debug.Log("Finished uploading to Firebase...");
    //         }
    //     });
    // }
}
