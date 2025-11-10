using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirebaseWebGL.Scripts.FirebaseBridge;

public class FirebaseFunctionsController 
{
    public static FirebaseFunctionsController Instance = null;

    static FirebaseFunctionsController()
    {
        Instance = new FirebaseFunctionsController();
    }

    private FirebaseFunctionsController()
    {

    }

    public void LoadRewardRoutine(string prizeDataJsonStr, PathJsLibData pathJsLibData)
    {
        FirebaseFunctions.CallRewardRoutine(prizeDataJsonStr, pathJsLibData.objectName, pathJsLibData.callbackName, pathJsLibData.fallbackName);
    }

    public void ReSendReward(string resendDataJsonStr, PathJsLibData pathJsLibData)
    {
        FirebaseFunctions.ReSendReward(resendDataJsonStr, pathJsLibData.objectName, pathJsLibData.callbackName, pathJsLibData.fallbackName);
    }

    public void TryingSignUpFromGuest(string stageClearDataJsonType, PathJsLibData pathJsLibData)
    {
        FirebaseFunctions.TryingSignUpFromGuest(stageClearDataJsonType, pathJsLibData.objectName, pathJsLibData.callbackName, pathJsLibData.fallbackName);
    }

    public void CountUpKPIData(string subRoot)
    {
        FirebaseFunctions.CountUpKPIData(subRoot);
    }

    public void RecordStageDashboardData(string dataJsonType)
    {
        FirebaseFunctions.RecordStageDashboardData(dataJsonType);
    }

    public void RecordStageStudyKPIData(string studyDataJsonStr)
    {
        FirebaseFunctions.RecordStageStudyKPIData(studyDataJsonStr);
    }
}
