using System;
using System.Collections;
using System.Collections.Generic;
using AOT;
using FirebaseWebGL.Scripts.FirebaseBridge;

public class FirebaseStorageController 
{
    public static FirebaseStorageController Instance;

    static FirebaseStorageController()
    {
        Instance = new FirebaseStorageController();
    }

    private FirebaseStorageController()
    {

    }

    private static Action<string> successCallback = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void DownloadTargetFile(PathJsLibData pathJsLibData)
    {
        CustomDebug.Log("Try DownloadTargetFile -------------------->>>");

        FirebaseStorage.DownloadFile(pathJsLibData.path, pathJsLibData.objectName, pathJsLibData.callbackName, pathJsLibData.fallbackName);
    }

    public void DownloadTargetFileWithCallback(PathJsLibData pathJsLibData, Action<string> callback)
    {
        CustomDebug.Log("Try DownloadTargetFile WithCallback -------------------->>>");

        successCallback = callback;

        FirebaseStorage.DownloadFileWithCallback(pathJsLibData.path, 
            pathJsLibData.objectName, pathJsLibData.fallbackName, LoadSuccessCallback);
    }

    [MonoPInvokeCallback(typeof(Action<int>))]
    public static void LoadSuccessCallback(string result)
    {
        successCallback?.Invoke(result);

        successCallback = null;
    }
}
