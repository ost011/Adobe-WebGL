using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RewardManagerInSLM : RewardManager
{
    public PopUpCheckUserInfoBeforeDrawModuleInSLM popUpCheckUserInfoBeforeDrawModuleInSLM;
    public PopUpGottenRewardInfoModuleInSLM popUpGottenRewardInfoModuleInSLM;
    public PopUpSuccessFailResultInfoModule popUpSuccessFailResultInfoModule;

    private RewardModuleInSLM rewardModuleInSLM = null;

    private int successCount = 0;
    private int failCount = 0;
    private int maxCount = 0;

    private Dictionary<string[], PrizeItemInfo> rewardInfoTable = new Dictionary<string[], PrizeItemInfo>();
    private List<string> gottenRewardNames = new List<string>();
    private List<ResendData> listResendDatas = new List<ResendData>();  // GottenReward 팝업에서 다시 보낼 ResendData 리스트

    private StringBuilder sb = new StringBuilder();

    private const string PRIZE_ITEM_CATEGORY_SPECIAL_STR = "special";

    private struct TmpPrizeData
    {
        public string email;
        public string stageType;
        public string lessonType;
        public string prizeName;
        public string prizeId;
        public bool isSpecialPrize;
        public string randomKey; // -NAhc1kviGFydf_WnHIO
        public string attachment;

        public string program;

        public void SetPrizeData(KeyValuePair<string[], PrizeItemInfo> kvp, int index)
        {
            var arrayLessonInfosWithPrizeInfo = kvp.Key;

            var prizeItemInfo = kvp.Value;

            this.stageType = prizeItemInfo.stagePos;
            this.lessonType = prizeItemInfo.lessonPos;

            this.prizeName = prizeItemInfo.name;

            this.prizeId = arrayLessonInfosWithPrizeInfo[2];

            this.randomKey = arrayLessonInfosWithPrizeInfo[3];

            if(prizeItemInfo.special)
            {
                isSpecialPrize = true;
            }
            else
            {
                isSpecialPrize = false;
            }

            this.attachment = prizeItemInfo.attachment;

            this.program = AppInfo.Instance.GetProgramInfo();
        }

        public void SetEmail(string email)
        {
            this.email = email;
        }
    }

    protected override void ActivateCheckUserInfoBeforeDrawPopUp()
    {
        this.currentRewardData.SetStageTypeStr(rewardInfoTable.ElementAt(0).Value.stagePos);

        popUpCheckUserInfoBeforeDrawModuleInSLM.UpdateState(rewardModuleInSLM);

        popUpCheckUserInfoBeforeDrawModuleInSLM.ActivateBody();

        popUpCheckUserInfoBeforeDrawModuleInSLM.SetTextWhatPrizeGetting(GetIntegratedRewardTitle());

        SmartLearningModeController.Instance.AllDeActivateSpriteMaskUnderCompleteCharacters();
    }

    public override void DeActivateCheckUserInfoBeforeDrawPopUp()
    {
        base.DeActivateCheckUserInfoBeforeDrawPopUp();

        SmartLearningModeController.Instance.ActivateAllSpriteMaskUnderCompleteCharacters();
    }

    public override void ActivateGottenRewardInfoPopUp()
    {
        LoadingManager.Instance.DeActivateLoading();

        // 이미 해당 상품은 받았고, 정보만 확인하기위함

        var gottenRewardInfo = new GottenRewardInfo();

        gottenRewardInfo.successRewardCount = gottenRewardNames.Count;
        gottenRewardInfo.targetEmailAddress = this.currentRewardData.TargetEmailAddress;
        gottenRewardInfo.stageType = currentRewardData.StageTypeStr;
        gottenRewardInfo.lessonType = currentRewardData.LessonTypeStr;
        gottenRewardInfo.rewardName = this.currentRewardData.WhatReward;

        var isSpecial = false;

        // 이번에 받은 경품에 포함되면서 스페셜 경품인 것이 있는가
        var gottenSpecialReward = from data in UserManager.Instance.GetSpecificGottenPrizeItemInfos(currentRewardData.StageTypeStr, currentRewardData.LessonTypeStr)
                                  where gottenRewardNames.Contains(data.Value.type) && data.Value.category.Equals(PRIZE_ITEM_CATEGORY_SPECIAL_STR)
                                  select data;

        if(gottenSpecialReward.Count() > 0)
        {
            // 받은 것중에 하나라도 스페셜이 존재
            isSpecial = true;
        }

        gottenRewardInfo.isSpecial = isSpecial;

        popUpGottenRewardInfoModuleInSLM.UpdateState(gottenRewardInfo);

        popUpGottenRewardInfoModuleInSLM.ActivatePopUp();
    }

    public override void OnClickTryDrawingBtn(RewardModule rewardModule)
    {
        SetCurrentRewardData(rewardModule);

        ActivateCheckUserInfoBeforeDrawPopUp();
    }

    public override void SetCurrentRewardData(RewardModule data)
    {
        this.currentRewardData = data;

        SetCurrentRewardModule();

        InitRewardInfoTable();

        // this.textWhatItem.text = GetIntegratedRewardTitle();
    }

    public override void OnClickConfirmBtnUpdateUserInfosOnCheckUserInfoBeforeDrawPopUp()
    {
        var writtenEmailStr = this.inputFieldUserMailAddressOnPopUpCheckUserInfoBeforeDraw.text;

        if (DevUtil.Instance.CheckIsValidEmailAddress(writtenEmailStr))
        {
            UpdateUserInfosTextOnCheckUserInfoBeforeDrawPopUp();
        }
        else
        {
            SmartLearningModeController.Instance.ActivateCommonErrorPopUp(EnumSets.ErrorType.InValidEmailAddress);
        }
    }

    public override void OnClickConfirmBtnForLuckyDrawOnCheckUserInfoBeforeDrawPopUp()
    {
        CustomDebug.Log("OnClickConfirmBtnForLuckyDrawOnCheckUserInfoBeforeDrawPopUp in SLM>>>>>>>>>");

        // if (DevUtil.Instance.CheckIsValidEmailAddress(this.textUserMailAddressOnPopUpCheckUserInfoBeforeDraw.text))
        if (DevUtil.Instance.CheckIsValidEmailAddress(this.popUpCheckUserInfoBeforeDrawModule.GetUpdatedUserEmailText()))
        {
            DeActivateCheckUserInfoBeforeDrawPopUp();

            SetUserInfosForLuckyDraw();
            
            SendAGiftToUserEmail();
        }
        else
        {
            SmartLearningModeController.Instance.ActivateCommonErrorPopUp(EnumSets.ErrorType.InValidEmailAddress);
        }
    }

    // SuccessFailResult 팝업의 '다시보내기' 버튼에 참조되어 있음
    public void OnClickResendBtnInSuccessFailResultPopUp()
    {
        CustomDebug.Log("OnClickResendBtnInSuccessFailResultPopUp>>>>>>>>>");

        this.popUpSuccessFailResultInfoModule.DeActivateSuccessFailResultInfoPopUp();

        SendAGiftToUserEmail();
    }

    protected override void SendAGiftToUserEmail()
    {
        successCount = 0;
        failCount = 0;

        maxCount = rewardInfoTable.Count;
        
        listResendDatas.Clear();
        gottenRewardNames.Clear();

        LoadingManager.Instance.ActivateLoading();

        // 유저 정보 확인 절차부터 수행해서 들어온 것, 바로 경품을 유저 메일로 보낸다

        // 정해진 아이템 확인
        // 메일 보내기 
        // 메일 전송 성공 후, 파베 db 정보 수정하기

#if UNITY_EDITOR
        CustomDebug.Log("SendAGiftToUserEmail SLM");

#elif UNITY_WEBGL

        for(int i = 0; i < rewardInfoTable.Count; i++)
        {
            TmpPrizeData tmpPrizeData = new TmpPrizeData();

            var kvp = rewardInfoTable.ElementAt(i);

            this.rewardModuleInSLM.InitRewardModuleInfo(kvp.Key, kvp.Value);
            
            tmpPrizeData.SetPrizeData(kvp, i);
            tmpPrizeData.SetEmail(this.targetUserEmailAddressStr);

            var prizeDataJsonStr = DevUtil.Instance.GetJson(tmpPrizeData);

            PathJsLibData pathJsLibData = new PathJsLibData();

            pathJsLibData.objectName = this.transform.name;
            pathJsLibData.callbackName = nameof(GetRewardSucceeded);
            pathJsLibData.fallbackName = nameof(GetRewardFailed);

            FirebaseFunctionsController.Instance.LoadRewardRoutine(prizeDataJsonStr, pathJsLibData);
        }
#endif
    }

    private void GetRewardSucceeded(string result)
    {
        CustomDebug.Log($"Get Reward Succeeded SLM : {result}");

        var table = DevUtil.Instance.GetTargetObjectFromJson<Dictionary<string, string>>(result);

        var gottenRewardName = table["type"];

        gottenRewardNames.Add(gottenRewardName);

        var gottenRewardInfo = from data in rewardInfoTable
                               where data.Value.name.Equals(gottenRewardName)
                               select data;

        if (gottenRewardInfo.Count() > 0)
        {
            UpdatePrizeData(table);

            var kvp = rewardInfoTable.ElementAt(0);

            this.currentRewardData.SetTargetEmailAddress(this.targetUserEmailAddressStr);

            this.currentRewardData.SetWhatReward(GetIntegratedRewardTitle());

            this.currentRewardData.SetPrizeItemInfo(gottenRewardInfo.ElementAt(0).Value);

            this.currentRewardData.CheckIsSpecialPrize();

            var gottenRewardInfosArray = gottenRewardInfo.ElementAt(0).Key;

            // 성공/실패 팝업을 통해 다시 보낼때 rewardInfoTable에 남은 데이터만 이용할 예정이라 성공한 데이터는 지운다
            rewardInfoTable.Remove(gottenRewardInfosArray);
            
            var targetCode = table["targetCode"]; // none or {code}
            
            var prizeId = gottenRewardInfosArray[2];

            SetResendData(targetCode, prizeId);

            successCount++;

            CheckTotalSuccessFailCountReachedToMaxCount(table);
        }
        else
        {
            CustomDebug.LogError("gottenRewardInfo.Count() == 0");
        }
    }

    protected override void GetRewardFailed(string errorResult)
    {
        try
        {
            failCount++;

            var table = DevUtil.Instance.GetTargetObjectFromJson<Dictionary<string, object>>(errorResult);

            var detailsJson = DevUtil.Instance.GetJson(table["details"]); // table["details"] as string 변환이 안됨, json 형태로 변환 후 테이블로 다시 변환 후 사용

            CustomDebug.Log($"GetRewardFailed detailsJson : {detailsJson}");

            var detailsTable = DevUtil.Instance.GetTargetObjectFromJson<Dictionary<string, string>>(detailsJson);

            CheckTotalSuccessFailCountReachedToMaxCount(detailsTable);
        }
        catch(Exception e)
        {
            CustomDebug.LogError($"GetRewardFailed error : {e.Message}");

            if(successCount + failCount == maxCount)
            {
                if(failCount == maxCount)
                {
                    LoadingManager.Instance.DeActivateLoading();

                    HandlingDrawError("0000");  // "details" 키가 없을 때 대비

                    CustomDebug.LogError("failCount == maxCount");
                }
                else
                {
                    HandlingSuccessAndFailExistAtTheSameTime();

                    CustomDebug.LogError("failCount != maxCount");
                }
            }
        }
    }

    private void CheckTotalSuccessFailCountReachedToMaxCount(Dictionary<string ,string> resultTable)
    {
        if(successCount + failCount == maxCount)
        {
            if(successCount == maxCount)
            {
                CustomDebug.Log($"successCount == maxCount, maxCount : {maxCount}");

                #region update currentRewardData Infos
                //if (maxCount == 1)
                //{
                //    var kvp = rewardInfoTable.ElementAt(0);

                //    this.currentRewardData.SetPrizeItemInfo(kvp.Value);

                //    this.currentRewardData.SetStageTypeStr(kvp.Value.stagePos);
                //    this.currentRewardData.SetLessonTypeStr(kvp.Value.lessonPos);

                //    this.currentRewardData.CheckIsSpecialPrize();

                //    this.currentRewardData.SetTargetEmailAddress(this.targetUserEmailAddressStr);

                //    CustomDebug.Log($"data in maxCount == 1, {this.currentRewardData.IsSpecialPrize}, {this.currentRewardData.WhatReward}");
                //}
                //else
                //{
                //    this.currentRewardData.SetStageTypeStr(rewardInfoTable.ElementAt(0).Value.stagePos);
                //    this.currentRewardData.SetLessonTypeStr(rewardInfoTable.ElementAt(0).Value.lessonPos);

                //    //var firstName = rewardInfoTable.ElementAt(0).Value.name;
                //    //var secondName = rewardInfoTable.ElementAt(1).Value.name;

                //    this.currentRewardData.SetWhatReward(GetIntegratedRewardTitle());

                //    this.currentRewardData.SetTargetEmailAddress(this.targetUserEmailAddressStr);

                //    CustomDebug.Log($"data in maxCount > 1, {this.currentRewardData.IsSpecialPrize}, {this.currentRewardData.WhatReward}");
                //}
                #endregion

                this.currentRewardData.SetWhatReward(GetIntegratedRewardTitleAfterSendingEmailSucceeded());

                ActivateGottenRewardInfoPopUp();

                SoundManager.Instance.PlayFX(EnumSets.FxType.OpenPrizeBox);

                // 경품 메일 발송에 성공했기 때문에 선물받기 버튼은 끈다
                SmartLearningModeController.Instance.DeActivateGetRewardBtnOnLessonCompletePanel();

                rewardInfoTable.Clear();

                LoadingManager.Instance.DeActivateLoading();

                CustomDebug.Log("CheckTotalSuccessFailCountReachedToMaxCount All Succeeded~~");
            }
            else
            {
                if(failCount == maxCount)
                {
                    // 모두 실패

                    LoadingManager.Instance.DeActivateLoading();

                    var errorCodeStr = resultTable["code"];

                    HandlingDrawError(errorCodeStr);

                    CustomDebug.Log("SendReward failCount == maxCount");
                }
                else
                {
                    HandlingSuccessAndFailExistAtTheSameTime();
                }
            }
        }
    }

    private void HandlingSuccessAndFailExistAtTheSameTime()
    {
        LoadingManager.Instance.DeActivateLoading();

        var succeededRewardName = gottenRewardNames.ElementAt(0);
        var failedRewardName = rewardInfoTable.ElementAt(0).Value.name;

        CustomDebug.Log($"성공/실패 공존! success : {succeededRewardName}, fail : {failedRewardName}");

        SmartLearningModeController.Instance.DeActivateGetRewardBtnOnLessonCompletePanel(); // SuccessFail 팝업에 '못받은 경품만 다시 보내기' 버튼이 있으므로

        ActivateSuccessFailResultPopUp(succeededRewardName, failedRewardName);
    }

    protected override void HandlingDrawError(string ErrorCode)
    {
        // 로딩 꺼지며 에러팝업 뜨고 해당 팝업 아래는 강의 클리어 팝업뿐

        SmartLearningModeController.Instance.AllDeActivateSpriteMaskUnderCompleteCharacters();

        SmartLearningModeController.Instance.SetCallbackAfterCommonErrorPopUpDeActivated(() =>
        {
            SmartLearningModeController.Instance.ActivateAllSpriteMaskUnderCompleteCharacters();
        });

        switch (ErrorCode)
        {
            case "744":
                {
                    SmartLearningModeController.Instance.ActivateCommonErrorPopUp(EnumSets.ErrorType.SendRewardEmailFailed);
                }
                break;
            case "770":
                {
                    SmartLearningModeController.Instance.ActivateCommonErrorPopUp(EnumSets.ErrorType.DrawError);
                }
                break;
            case "401":
                {
                    SmartLearningModeController.Instance.ActivateCommonErrorPopUp(EnumSets.ErrorType.AuthError);
                }
                break;
            case "440":
                {
                    SmartLearningModeController.Instance.ActivateCommonErrorPopUp(EnumSets.ErrorType.DBUpdateError);
                }
                break;
            case "0000":
                {
                    SmartLearningModeController.Instance.ActivateCommonErrorPopUp(EnumSets.ErrorType.UnavailableError);
                }
                break;
        }
    }

    private void ActivateSuccessFailResultPopUp(string succeededRewardName, string failedRewardName)
    {
        SmartLearningModeController.Instance.AllDeActivateSpriteMaskUnderCompleteCharacters();

        popUpSuccessFailResultInfoModule.SetCallbackAfterCommonErrorPopUpDeActivated(() =>
        {
            SmartLearningModeController.Instance.ActivateAllSpriteMaskUnderCompleteCharacters();
        });

        this.popUpSuccessFailResultInfoModule.ActivateSuccessFailResultInfoPopUp(succeededRewardName, failedRewardName);
    }

    private void SetCurrentRewardModule()
    {
        rewardModuleInSLM = null;

        rewardModuleInSLM = currentRewardData as RewardModuleInSLM;
    }

    private void InitRewardInfoTable()
    {
        rewardInfoTable = rewardModuleInSLM.GetRewardInfoTable();
    }

    // GetIntegratedRewardTitle, GetIntegratedRewardTitleAfterSendEmailSucceeded 헷갈리니 리팩토링 필요
    private string GetIntegratedRewardTitle()
    {
        sb.Clear();

        for (int i = 0; i < rewardInfoTable.Count; i++)
        {
            var rewardName = rewardInfoTable.ElementAt(i).Value.name;

            sb.Append(rewardName);

            if (i != rewardInfoTable.Count - 1)  // 마지막 이름 제외하고 "/" 붙음
            {
                sb.Append(" / ");
            }
        }

        var title = sb.ToString();

        return title;
    }

    // 성공/실패 팝업 -> 전송 -> 실제로 뭘 받았는지 성공할때마다 데이터를 지우는 rewardInfoTable대신
    // gottenRewardNames 이용해 판단하게끔 GetIntegratedRewardTitle에서 분화함
    private string GetIntegratedRewardTitleAfterSendingEmailSucceeded()
    {
        sb.Clear();

        for (int i = 0; i < gottenRewardNames.Count; i++)
        {
            var rewardName = gottenRewardNames.ElementAt(i);

            sb.Append(rewardName);

            if (i != gottenRewardNames.Count - 1)  // 마지막 이름 제외하고 "/" 붙음
            {
                sb.Append(" / ");
            }
        }

        var title = sb.ToString();

        return title;
    }

    protected override void UpdatePrizeData(Dictionary<string, string> rewardingSuccessData)
    {
        var prizeDataList = UserManager.Instance.GetSpecificGottenPrizeItemInfos(this.currentRewardData.StageTypeStr, this.currentRewardData.LessonTypeStr);

        var rewardName = rewardingSuccessData["type"];

        if (prizeDataList != null)
        {
            var datas = from data in prizeDataList
                        // where data.Key.Equals(this.currentRewardData.RandomKey)
                        where data.Value.type.Equals(rewardName)
                        select data;

            if (datas.Count() > 0)
            {
                var prizeData = datas.ElementAt(0);

                var specificPrizeData = prizeData.Value;

                specificPrizeData.receive = true;

                specificPrizeData.receiveDate = Convert.ToInt64(rewardingSuccessData["receiveDate"]);

                specificPrizeData.type = rewardName;

                specificPrizeData.targetEmailAddress = this.targetUserEmailAddressStr;

                this.currentRewardData.SetPrizeItem(specificPrizeData);

                this.currentRewardData.SetRandomKey(prizeData.Key);

                UserManager.Instance.UpdatePrizeData(this.currentRewardData, this.currentRewardData.MyPrizeItem);

                this.currentRewardData.UpdateRewardItemState();
            }
            else
            {
                CustomDebug.LogError($"{this.currentRewardData.RandomKey} 와 일치하는 값이 없음");
            }
        }
    }

    protected override void SetResendData(string targetCode, string prizeId)
    {
        var resendData = new ResendData();

        resendData.SetEmail(this.targetUserEmailAddressStr);
        resendData.SetCode(targetCode);
        resendData.SetPrizeId(prizeId);

        resendData.SetData(this.currentRewardData);

        listResendDatas.Add(resendData);
    }

    // PopUpGottenRewardInfo의 다시 발송하기 버튼에 참조되어 있음
    public override void ReSendRewardEmail()
    {
#if UNITY_EDITOR

        CustomDebug.LogWithColor("Re Send Reward Email in SLM", CustomDebug.ColorSet.Green);

#elif UNITY_WEBGL 
        CustomDebug.Log("Try ReSendRewardEmail");

        LoadingManager.Instance.ActivateLoading();

        foreach(var resendData in listResendDatas)
        {
            if (!string.IsNullOrEmpty(resendData.email))
            {
                var json = DevUtil.Instance.GetJson(resendData);

                PathJsLibData pathJsLibData = new PathJsLibData();

                pathJsLibData.objectName = this.transform.name;
                pathJsLibData.callbackName = nameof(SucceededResend);
                pathJsLibData.fallbackName = nameof(FailedResend);

                FirebaseFunctionsController.Instance.ReSendReward(json, pathJsLibData);
            }
            else
            {
                HomeController.Instance.ActivateCommonErrorPopUp(EnumSets.ErrorType.ReSendRewardEmailFailed);
            }
        }
#endif

    }

    // this.ReSendRewardEmail()에 참조되어 있음
    protected override void SucceededResend()
    {
        CustomDebug.Log("SucceededResend SLM");

        popUpGottenRewardInfoModuleInSLM.DeActivateResendBtn();

        LoadingManager.Instance.DeActivateLoading();
    }

    protected override void FailedResend(string error)
    {
        CustomDebug.Log($"FailedResend SLM : {error}");

        SmartLearningModeController.Instance.ActivateCommonErrorPopUp(EnumSets.ErrorType.ReSendRewardEmailFailed);

        LoadingManager.Instance.DeActivateLoading();
    }

    //protected override void UpdatePrizeData(Dictionary<string, string> rewardingSuccessData)
    //{
    //    base.UpdatePrizeData(rewardingSuccessData, () =>
    //    {
    //        SoundManager.Instance.PlayFX(EnumSets.FxType.OpenPrizeBox);

    //        // 경품 메일 발송에 성공한 상황이기 때문에 선물받기 버튼은 끈다
    //        SmartLearningModeController.Instance.DeActivateGetRewardBtnOnLessonCompletePanel();

    //        CustomDebug.Log("UpdatePrizeData In SLM End~~!");
    //    });
    //}
}
