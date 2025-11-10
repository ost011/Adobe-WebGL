using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeControl
{
    private HomeUIManager uiManager = null;

    private SigningType signingType = null;

    public HomeControl(SigningType signingType)
    {
        this.signingType = signingType;
    }

    public void InitView(HomeUIManager uiManager)
    {
        this.uiManager = uiManager;

        this.signingType.InitView(uiManager);
    }

    public string GetUserEmailValue()
    {
        return this.signingType.GetUserEmailValue();
    }

    public EnumSets.SignInType GetExtraFuncBtnType()
    {
        return this.signingType.GetExtraFuncBtnType();
    }

    public void OnClickConversionBtn()
    {
        this.signingType.OnClickExtraFuncBtn();
    }

    public void CheckNeedToShowReAskPopUpWhenUserDoExtraFunc()
    {
        this.signingType.CheckNeedToShowReAskPopUpWhenUserDoExtraFunc();
    }

    public void RecordingUserPlayingTime()
    {
        this.signingType.RecordingUserPlayingTime();
    }

    public void StopRecordingUserPlayingTime()
    {
        this.signingType.StopRecordingUserPlayingTime();
    }
}
