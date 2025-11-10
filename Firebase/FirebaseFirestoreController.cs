using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Threading.Tasks;

public class FirebaseFirestoreController
{
    public static FirebaseFirestoreController Instance = null;

    [DllImport("__Internal")]
    public static extern void FBInit();

    [DllImport("__Internal")]
    public static extern void GetDocument(string collectionPath, string documentId, string objectName, string callback, string fallback);

    private const string USER_STR = "users";



    static FirebaseFirestoreController()
    {
        Instance = new FirebaseFirestoreController();
    }

    public FirebaseFirestoreController()
    {
        
    }

    public void Init()
    {
        FBInit();
    }

    
    public async void SetUserData<T>(string uid, T data, Action<bool> isReady)
    {
        
    }

    public void GetUserData(string uid, string objectName, string callback, string fallback)
    {
        GetDocument(USER_STR, uid, objectName, callback, fallback);
    }

    
}
