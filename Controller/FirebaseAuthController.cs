using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.Objects;
using System;

[Serializable]
public class FirebaseUserModified
{
    public string uid = "";
    public string email = "";
    public bool emailVerified = false;
    public bool isAnonymous = false;

    public object providerData = null;
    public object stsTokenManager = null;

    public string createdAt = "";
    public string lastLoginAt = "";

    public string apiKey = "";
    public string appName = "";
}

public class FirebaseAuthController
{
    public static FirebaseAuthController Instance = null;

    private string thisObjectName = "LogInController";

    static FirebaseAuthController()
    {
        Instance = new FirebaseAuthController();
    }

    private FirebaseAuthController()
    {
        CustomDebug.Log("create FirebaseAuthController ");

        Init();
    }

    public void ForceAwake() 
    {
        CustomDebug.Log("ForceAwake FirebaseAuthController ");
    }

    [Obsolete]
    public void SetCallbackObject(string objectName)
    {
        CustomDebug.Log("SetCallbackObject FirebaseAuthController ");

        thisObjectName = objectName;
    }

    private void Init()
    {
        CustomDebug.Log("Init FirebaseAuthController ");

        OnAuthStateChanged(); // 중복 수행 안되도록 수정하기
    }

    public void OnAuthStateChanged()
    {
        CustomDebug.Log($"auth state changed 등록 / thisObjectName : {thisObjectName} ");

        FirebaseAuth.OnAuthStateChanged(thisObjectName, "SucceededSignIn", "SucceededSignOut");
    }

    public void CreateUserWithEmailAndPassword(string email, string pass)
    {
        FirebaseAuth.CreateUserWithEmailAndPassword(email, pass
            , thisObjectName, "SucceededSignIn", "WhenSignInFailed");
    }

    public void SignInWithEmailAndPassword(string email, string pass)
    {
        FirebaseAuth.SignInWithEmailAndPassword(email, pass
            , thisObjectName, "SucceededSignIn", "WhenSignInFailed");
    }

    public void SignOut()
    {
        FirebaseAuth.SignOut(thisObjectName, "SucceededSignOut", "WhenSignOutFailed");       
    }

    public void SignOut(PathJsLibData pathJsLibData)
    {
        FirebaseAuth.SignOut(pathJsLibData.objectName, pathJsLibData.callbackName, pathJsLibData.fallbackName);
    }

    public void ForceSignOut()
    {
        FirebaseAuth.ForceSignOut();
    }

    public void UpdateEmailAddress(string targetEmailAddress)
    {
        FirebaseAuth.UpdateEmail(targetEmailAddress, thisObjectName, "callback", "fallback");
    }

    public void SendPasswordResetEmail(string targetEmailAddress)
    {
        FirebaseAuth.SendPasswordResetEmail(targetEmailAddress);
    }

    public void SignInAnonymously(PathJsLibData pathJsLibData)
    {
        FirebaseAuth.SignInAnonymously(pathJsLibData.objectName, pathJsLibData.callbackName, pathJsLibData.fallbackName);
    }

    public void GetIdToken(PathJsLibData pathJsLibData)
    {
        FirebaseAuth.GetIdToken(pathJsLibData.objectName, pathJsLibData.callbackName, pathJsLibData.fallbackName);
    }
}
