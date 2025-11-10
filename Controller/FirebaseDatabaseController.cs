using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;
using System;
using AOT;
using System.Text;

public class FirebaseDatabaseController
{
    public static FirebaseDatabaseController Instance = null;

    static FirebaseDatabaseController()
    {
        Instance = new FirebaseDatabaseController();
    }

    private FirebaseDatabaseController()
    {

    }

    private static Action<int> responseCallback = null;

    private static Action responseSuccessCallback = null;
    private static Action responseFailCallback = null;

    private static Action forChildChagnedCallback = null;

    private StringBuilder sb = new StringBuilder();


    public void GetTargetData(PathJsLibData pathJsLibData)
    {
        FirebaseDatabase.GetJSON(pathJsLibData.path, pathJsLibData.objectName, pathJsLibData.callbackName, pathJsLibData.fallbackName);
    }

    public void SetTagetData(string path, string valueJsonType, Action<int> response = null)
    {
        responseCallback = response;

       // var targetPath = GetTargetUserPath(uid);

        FirebaseDatabase.SetTargetData(path, valueJsonType, SetResponseCallback);
    }

    public void UpdateJsonWithCallback(string path, string valueJsonType, Action callback, Action fallback)
    {
        responseSuccessCallback = callback;
        responseFailCallback = fallback;

        FirebaseDatabase.UpdateJsonWithCallback(path, valueJsonType, SetSuccessResponseCallback, SetFailResponseCallback);
    }

    public void JustUpdate(string path, string valueJsonType)
    {
        FirebaseDatabase.JustUpdate(path, valueJsonType);
    }

    public string GetPushKey()
    {
        return FirebaseDatabase.GetPushKey();
    }

    public string GetJustPushKey(string path)
    {
        return FirebaseDatabase.GetJustPushKey(path);
    }

    public void ListeningValueChanged(string path, Action callback)
    {
        forChildChagnedCallback = callback;

        FirebaseDatabase.ListeningValueChanged(path, LoadChildChangedCallback);
    }

    public void RemoveListeningValueChanged(string path)
    {
        FirebaseDatabase.RemoveListeningValueChanged(path);
    }

    [MonoPInvokeCallback(typeof(Action<int>))]
    private static void SetResponseCallback(int isSuccess)
    {
        responseCallback?.Invoke(isSuccess);

        responseCallback = null;
    }

    [MonoPInvokeCallback(typeof(Action))]
    private static void SetSuccessResponseCallback()
    {
        responseSuccessCallback?.Invoke();

        responseSuccessCallback = null;
    }

    [MonoPInvokeCallback(typeof(Action))]
    private static void SetFailResponseCallback()
    {
        responseFailCallback?.Invoke();

        responseFailCallback = null;
    }

    [MonoPInvokeCallback(typeof(Action))]
    private static void LoadChildChangedCallback()
    {
        forChildChagnedCallback?.Invoke();

        forChildChagnedCallback = null;
    }
}
