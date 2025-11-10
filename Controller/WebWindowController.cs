using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public static class WebWindowController
{
    public const string DLL_TYPE = "__Internal";

    [DllImport("__Internal")]
    public static extern void CloseWindow();

    [DllImport("__Internal")]
    public static extern void SetConfirmPopUpDescriptionMessage(string msg);

    [DllImport("__Internal")]
    public static extern void AddEventListenerHandleOnPressSpecificInputKey();
                              

    [DllImport("__Internal")]
    public static extern void RemoveEventListenerHandleOnPressSpecificInputKey();

    [DllImport(DLL_TYPE)]
    public static extern void OpenTargetWindow(string path);

    [DllImport(DLL_TYPE)]
    public static extern void TryingSignUpFromGuestWithLocalStorage(string stageClearDataJsonType, string objectName, string callback, string fallback);

    //[DllImport(DLL_TYPE)]
    //public static extern void ExtendedAddEventListenerHandleOnRefresh(string objectName, string callback);
    [DllImport(DLL_TYPE)]
    public static extern void ExtendedAddEventListenerHandleOnRefresh(Action responseEvent);

    [DllImport(DLL_TYPE)]
    public static extern void ExtendedRemoveEventListenerHandleOnRefresh();

    [DllImport(DLL_TYPE)]
    public static extern void ForceRefresh();

    [DllImport(DLL_TYPE)]
    public static extern void AddEventListenerOnUnload(Action responseEvent);

    [DllImport(DLL_TYPE)]
    public static extern void RemoveEventListenerOnUnload();

    // 20강은 단축키 이벤트들을 막았습니다. 새창 단축키 이벤트는 제외(Ctrl + T, Shift + Ctrl + N)
    [DllImport(DLL_TYPE)]
    public static extern void AddEventListenerHandleOnPreventEventFromBrowser();
    [DllImport(DLL_TYPE)]
    public static extern void RemoveEventListenerHandleOnPreventEventFromBrowser();

[DllImport(DLL_TYPE)]
    public static extern void SetFullScreenState();

    [DllImport(DLL_TYPE)]
    public static extern void SetNormalScreenState();
}
