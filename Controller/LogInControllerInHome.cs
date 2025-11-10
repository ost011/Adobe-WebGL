using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogInControllerInHome : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    public void SignOut(Action callback, Action fallback)
    {
        CustomDebug.Log("Try SignOut");

        FirebaseAuthController.Instance.SignOut();
    }

    // 로그인 씬에서 물고온 objectName (LogInController) 을 활용하기
    private void SucceededSignOut(string info)
    {
        CustomDebug.Log($"home scene, DisplayUser Signed Out >> : {info}");

        // 전역콜백으로 등록하고, 여기서 호출하지만 전역콜백은 계속 null 이었음
        HomeController.Instance.WhenSignOutSucceeded();
    }

    private void WhenSignOutFailed(string error)
    {
        HomeController.Instance.WhenSignOutFailed();
    }

}
