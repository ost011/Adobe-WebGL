using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class PopUpSuccessFailResultInfoModule : MonoBehaviour
{
    public GameObject body;

    public TextMeshProUGUI textSuccessInSuccessFailResultPopUp;
    public TextMeshProUGUI textFailInSuccessFailResultPopUp;
    public TextMeshProUGUI textRewardNameInResendBtn;

    private StringBuilder sb = new StringBuilder();
    private Action afterResultInfoPopUpDeActivated = null;

    public void SetCallbackAfterCommonErrorPopUpDeActivated(Action callback)
    {
        this.afterResultInfoPopUpDeActivated = callback;
    }

    public void ActivateSuccessFailResultInfoPopUp(string succeededRewardName, string failedRewardName)
    {
        body.SetActive(true);

        this.textSuccessInSuccessFailResultPopUp.text = succeededRewardName;
        this.textFailInSuccessFailResultPopUp.text = failedRewardName;
        this.textRewardNameInResendBtn.text = GetRewardNameInResendBtnStr(failedRewardName);

        CustomDebug.Log($"ActivateSuccessFailResultInfoPopUp, succeeded : {succeededRewardName}, failed : {failedRewardName}");
    }

    public void DeActivateSuccessFailResultInfoPopUp()
    {
        body.SetActive(false);

        afterResultInfoPopUpDeActivated?.Invoke();
        afterResultInfoPopUpDeActivated = null;
    }

    private string GetRewardNameInResendBtnStr(string failedRewardName)
    {
        sb.Clear();

        sb.Append("[");
        sb.Append(failedRewardName);
        sb.Append("]");

        var result = sb.ToString();

        return result;
    }
}
