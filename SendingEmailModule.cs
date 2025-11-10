using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UE.Email;
using AOT;
using UnityEngine.Networking;

public class SendingEmailModule : MonoBehaviour
{
    private static Action<bool> isSendingEmailCompleted = null; // jslib 내에서 콜백을 사용하려면 static 선언을 해줘야함

    private string targetEmailAddress = "";

    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void SetSendingRewardEmailCallback(Action<bool> whenEmailSendingProcessFinished)
    {
        isSendingEmailCompleted = whenEmailSendingProcessFinished;
    }

    public void SetTargetEmailAddress(string targetUserEmailAddressStr)
    {
        this.targetEmailAddress = targetUserEmailAddressStr;
    }

    public void ReadyToSendRewardEmail()
    {
        GetHtmlBody();
    }

    private void GetHtmlBody()
    {
        StartCoroutine(CorGetHtmlBody());
    }

    IEnumerator CorGetHtmlBody()
    {
        CustomDebug.Log("Try CorGetHtmlBody >>>");

        var bodyUrl = "https://firebasestorage.googleapis.com/v0/b/hostingtest-64d3c.appspot.com/o/Sample_source_email_KO.html?alt=media&token=6b8b5ab0-9530-4b3f-9ab0-b52aede2e959";

        using (UnityWebRequest request = UnityWebRequest.Get(bodyUrl))
        {
            var sendRequest = request.SendWebRequest();

            yield return sendRequest;

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                SendRewardEmail(string.Empty);

                yield break;
            }

            SendRewardEmail(request.downloadHandler.text);
        }
    }

    private void SendRewardEmail(string bodyData)
    {
        CustomDebug.Log($"Try Send Reward Email >>>");
        // CustomDebug.LogWithColor($"Try Send Reward Email >>> {bodyData}", CustomDebug.ColorSet.Yellow);

        if (string.IsNullOrEmpty(bodyData))
        {
            HomeController.Instance.ActivateCommonErrorPopUp(EnumSets.ErrorType.GetRewardBodyTextError);

            WhenSendEmailFinished(0);
        }
        else
        {
#if UNITY_EDITOR
            CustomDebug.LogWithColor($"On Editor, Send Email Succeded >>>", CustomDebug.ColorSet.Magenta);
            
            WhenSendEmailFinished(1);

#elif UNITY_WEBGL
            var fromAddress = "imcremedia@gmail.com";
            var subjectStr = "Reward-Test :D";
            var smtpAddress = "smtp.elasticemail.com";
            var userName = "tricisirius@gmail.com";
            var pass = "A80471EE3D25244491272702DA2CFE8FC6A7";

            SendEmailModule.SendEmail(fromAddress, this.targetEmailAddress, subjectStr,
                bodyData, smtpAddress, userName, pass, WhenSendEmailFinished);
#endif
        }
    }

    [MonoPInvokeCallback(typeof(Action<int>))]
    private static void WhenSendEmailFinished(int completeCode)
    {
        CustomDebug.LogWithColor($"WhenSendEmailFinished >>> {completeCode}", CustomDebug.ColorSet.Magenta);

        var isSucceeded = false;

        if(completeCode == 0)
        {
            isSucceeded = false;

        }
        else
        {
            isSucceeded = true;
        }

        isSendingEmailCompleted?.Invoke(isSucceeded);

        isSendingEmailCompleted = null;
    }
}
