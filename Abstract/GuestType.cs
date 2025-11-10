using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestType : SigningType
{
    private const string GUEST_STR = "손님으로 입장";
    
    private EnumSets.SignInType signInType = EnumSets.SignInType.Guest;

    public override void InitView(HomeUIManager uiManager)
    {
        CustomDebug.Log(" guest - InitView");

        uiManager.UpdateExitContentsOnGuestType();
    }

    public override string GetUserEmailValue()
    {
        return GUEST_STR;
    }

    public override EnumSets.SignInType GetExtraFuncBtnType()
    {
        return signInType;
    }

    public override void CheckNeedToShowReAskPopUpWhenUserDoExtraFunc()
    {
        HomeController.Instance.ActivatePopUp(EnumSets.PopUpType.ConversionReAsk);
    }

    public override void OnClickExtraFuncBtn()
    {
        CustomDebug.Log("계정 연동하기 try try - ");

#if UNITY_EDITOR
        CustomDebug.LogWithColor("In Editor 계정 연동하기 try try - ", CustomDebug.ColorSet.Cyan);

#elif UNITY_WEBGL
        // 게스트 로그인 시 익명로그인을 통해서 로그인, 필요한 정보만 db에 저장함
        // 연동시 GetIdToken() 을 통해 얻은 idToken을 이용해서 서버사이드 렌더링(파베 admin sdk)을 거쳐
        // 실제 계정으로 승급, 풀 데이터를 익명 로그인-uid에 저장함

        LoadingManager.Instance.ActivateLoading();

        PathJsLibData pathJsLibData = new PathJsLibData();
        pathJsLibData.objectName = "HomeController";
        pathJsLibData.callbackName = "OnSuccessTryingSignUpFromGuest";
        pathJsLibData.fallbackName = "OnFailedTryingSignUpFromGuest";

        // FirebaseFunctionsController.Instance.TryingSignUpFromGuest("tmpData, with localStorage!", pathJsLibData);
        // WebWindowController.TryingSignUpFromGuestWithLocalStorage("tmpData, with localStorage!", pathJsLibData.objectName, pathJsLibData.callbackName, pathJsLibData.fallbackName);
        FirebaseAuthController.Instance.GetIdToken(pathJsLibData);
#endif
    }

    public override void RecordingUserPlayingTime()
    {
        CustomDebug.Log("게스트 타입에서는 Playing Time 기록 안함");
    }

    public override void StopRecordingUserPlayingTime()
    {
        CustomDebug.Log("게스트 타입에서는 Playing Time 기록 안함 2");
    }

}
