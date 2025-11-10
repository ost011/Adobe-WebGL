using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SigningType 
{
    public virtual void InitView(HomeUIManager uiManager)
    {

    }

    public virtual string GetUserEmailValue()
    {
        return "";
    }

    public virtual EnumSets.SignInType GetExtraFuncBtnType()
    {
        return EnumSets.SignInType.SignedIn;
    }

    public virtual void OnClickExtraFuncBtn()
    {

    }

    public abstract void RecordingUserPlayingTime();
    public abstract void StopRecordingUserPlayingTime();

    // 로그아웃, 계정연동을 수행하려할 때 재확인 팝업을 띄워야하는 지 확인하는 메소드
    public abstract void CheckNeedToShowReAskPopUpWhenUserDoExtraFunc();
}
