using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignedInType : SigningType
{
    private EnumSets.SignInType signInType = EnumSets.SignInType.SignedIn;

    public override void InitView(HomeUIManager uiManager)
    {
        CustomDebug.Log(" SignedInType - InitView");

        uiManager.UpdateExitContentsOnSignInType();

#if UNITY_EDITOR

#elif UNITY_WEBGL

       UserManager.Instance.UpdateLoginRandomKeyAddress();
#endif

    }

    public override string GetUserEmailValue()
    {
        return UserManager.Instance.GetUserEmail();
    }

    public override EnumSets.SignInType GetExtraFuncBtnType()
    {
        return signInType;
    }

    public override void CheckNeedToShowReAskPopUpWhenUserDoExtraFunc()
    {
        OnClickExtraFuncBtn();
    }

    public override void OnClickExtraFuncBtn()
    {
        UserManager.Instance.RemoveListeningAddressChanged();

        HomeController.Instance.StopRecordingUserPlayingTime();

        HomeController.Instance.TryingSignOut();
    }

    public override void RecordingUserPlayingTime()
    {
        CustomDebug.Log("Try RecordingUserPlayingTime---------------");

        // 231116, khc 수정, 유저 플레잉타임 기록하는 것은 더이상 수행하지않음
        // UserManager.Instance.RecordingUserPlayingTime();
    }

    public override void StopRecordingUserPlayingTime()
    {
        UserManager.Instance.ForceCancelRecordingUserPlayingTimeTask();
    }

    
}
