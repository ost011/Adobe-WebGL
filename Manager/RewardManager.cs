using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

public class RewardManager : MonoBehaviour
{
    private static RewardManager instance = null;
    public static RewardManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<RewardManager>();
            }

            return instance;
        }
    }

    public RewardModule[] rewardModules;

    protected RewardModule currentRewardData = null;

    private string targetUserMobileNumberStr = "";
    protected string targetUserEmailAddressStr = "";

    [Space]
    [Header("Reward PopUps -----")]
    [Header("popUp CheckUserInfoBeforeDraw parts -----------")]
    public GameObject popUpCheckUserInfoBeforeDraw;
    protected PopUpCheckUserInfoBeforeDrawModule popUpCheckUserInfoBeforeDrawModule = null;

    public GameObject popUpUpdateUserInfoBeforeDraw;

    [Space]
    public TMP_InputField inputFieldMobileNumberOnPopUpCheckUserInfoBeforeDraw;
    public TMP_InputField inputFieldUserMailAddressOnPopUpCheckUserInfoBeforeDraw;

    [Space]
    [Header("popUp Gotten Reward Info parts -----------")]
    public GameObject popUpGottenRewardInfo;
    private PopUpGottenRewardInfoModule popUpGottenRewardInfoModule = null;

    private IRewardControl rewardControl = null;

    private struct TempPrizeData
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

        public void SetEmail(string email)
        {
            this.email = email;
        }

        public void SetPrizeData(RewardModule rewardModule)
        {
            stageType = rewardModule.StageTypeStr;
            lessonType = rewardModule.LessonTypeStr;
            prizeName = rewardModule.WhatReward;
            prizeId = rewardModule.PrizeItemInfoId;

            randomKey = rewardModule.RandomKey;

            isSpecialPrize = rewardModule.IsSpecialPrize;

            attachment = rewardModule.MyPrizeItemInfo.attachment;

            program = AppInfo.Instance.GetProgramInfo();
        }
    }

    protected struct ResendData
    {
        public string email;
        public string code;
        public string prizeId;

        public bool isSpecialPrize;
        public string attachment;

        public string program; // "photoShop" / "ai"

        public void SetEmail(string email)
        {
            this.email = email;

            CustomDebug.Log($"ResendData - email : {this.email}");
        }

        public void SetCode(string code)
        {
            this.code = code;

            CustomDebug.Log($"ResendData - code : {this.code}");
        }

        public void SetPrizeId(string prizeId)
        {
            this.prizeId = prizeId;

            CustomDebug.Log($"ResendData - prizeId : {this.prizeId}");
        }

        public void SetData(RewardModule rewardModule)
        {
            this.isSpecialPrize = rewardModule.IsSpecialPrize;
            this.attachment = rewardModule.MyPrizeItemInfo.attachment;

            this.program = AppInfo.Instance.GetProgramInfo();

            CustomDebug.Log($"ResendData - isSpecial : {isSpecialPrize}");
        }
    }

    private ResendData resendData;


    private void Awake()
    {
        Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitComponents();
    }

    private void Init()
    {
        if (!AppInfo.Instance.IsGuestLogin) // 게스트 로그인이 아니라면
        {
            this.rewardControl = this.gameObject.AddComponent<RewardSignedType>();
        }
        else
        {
            this.rewardControl = this.gameObject.AddComponent<RewardGuestType>();
        }

        this.rewardControl.Init(rewardModules);
    }

    private void InitComponents()
    {
        popUpGottenRewardInfoModule = popUpGottenRewardInfo.GetComponent<PopUpGottenRewardInfoModule>();
        popUpCheckUserInfoBeforeDrawModule = popUpCheckUserInfoBeforeDraw.GetComponent<PopUpCheckUserInfoBeforeDrawModule>();
    }

    public void SetPrizeItemInfos()
    {
        this.rewardControl.SetRewardItemWithPrizeItemInfo();
    }

    public virtual void SetCurrentRewardData(RewardModule data)
    {
        this.currentRewardData = data;
    }

    public void ResetData()
    {
        this.currentRewardData = null;

        this.targetUserMobileNumberStr = "";
        this.targetUserEmailAddressStr = "";
    }

    //reward 관련 ---------------------------------------------------

    // 럭키드로우 - 유저정보 확인 팝업 ------------------------------
    public virtual void OnClickTryDrawingBtn(RewardModule rewardModule)
    {
        SetCurrentRewardData(rewardModule);

        ActivateCheckUserInfoBeforeDrawPopUp();
    }

    protected virtual void ActivateCheckUserInfoBeforeDrawPopUp()
    {
        popUpCheckUserInfoBeforeDrawModule.UpdateState(this.currentRewardData);

        this.popUpCheckUserInfoBeforeDrawModule.ActivateBody();
    }

    public void ActivateChangingUserInfosPopUp()
    {
        SetInputFieldDatasOnPopUpCheckUserInfoBeforeDraw();

        popUpUpdateUserInfoBeforeDraw.SetActive(true);
    }

    private void SetInputFieldDatasOnPopUpCheckUserInfoBeforeDraw()
    {
        this.inputFieldMobileNumberOnPopUpCheckUserInfoBeforeDraw.text = UserManager.Instance.GetMoblieNumber(); 
        this.inputFieldUserMailAddressOnPopUpCheckUserInfoBeforeDraw.text = UserManager.Instance.GetUserEmail();
    }

    public virtual void OnClickConfirmBtnUpdateUserInfosOnCheckUserInfoBeforeDrawPopUp()
    {
        var writtenEmailStr = this.inputFieldUserMailAddressOnPopUpCheckUserInfoBeforeDraw.text;

        if(DevUtil.Instance.CheckIsValidEmailAddress(writtenEmailStr))
        {
            UpdateUserInfosTextOnCheckUserInfoBeforeDrawPopUp();
        }
        else
        {
            HomeController.Instance.ActivateCommonErrorPopUp(EnumSets.ErrorType.InValidEmailAddress);
        }
    }

    public void UpdateUserInfosTextOnCheckUserInfoBeforeDrawPopUp()
    {
        DeActivateUpdateUserInfoBeforeDrawPopUp();

        //var updatedMoblieNumberText = this.inputFieldMobileNumberOnPopUpCheckUserInfoBeforeDraw.text;
        //this.textMobileNumberOnPopUpCheckUserInfoBeforeDraw.text = updatedMoblieNumberText;

        var updatedUserEmailText = this.inputFieldUserMailAddressOnPopUpCheckUserInfoBeforeDraw.text;

        this.popUpCheckUserInfoBeforeDrawModule.UpdateUserMailText(updatedUserEmailText);
        // this.textUserMailAddressOnPopUpCheckUserInfoBeforeDraw.text = updatedUserEmailText;
    }

    public void DeActivateUpdateUserInfoBeforeDrawPopUp()
    {
        popUpUpdateUserInfoBeforeDraw.SetActive(false);
    }

    public virtual void OnClickConfirmBtnForLuckyDrawOnCheckUserInfoBeforeDrawPopUp()
    {
        
        // if (DevUtil.Instance.CheckIsValidEmailAddress(this.textUserMailAddressOnPopUpCheckUserInfoBeforeDraw.text))
        if (DevUtil.Instance.CheckIsValidEmailAddress(this.popUpCheckUserInfoBeforeDrawModule.GetUpdatedUserEmailText()))
        {
            DeActivateCheckUserInfoBeforeDrawPopUp();

            SetUserInfosForLuckyDraw();

            // ActivateLuckyDrawPopUp();
            SendAGiftToUserEmail();
        }
        else
        {
            HomeController.Instance.ActivateCommonErrorPopUp(EnumSets.ErrorType.InValidEmailAddress);
        }
    }

    public virtual void DeActivateCheckUserInfoBeforeDrawPopUp()
    {
        // this.popUpCheckUserInfoBeforeDraw.SetActive(false);
        this.popUpCheckUserInfoBeforeDrawModule.DeActivateBody();
    }

    protected void SetUserInfosForLuckyDraw()
    {
        
        // var moblieNumberStr = this.textMobileNumberOnPopUpCheckUserInfoBeforeDraw.text;
        // var emailAddressStr = this.textUserMailAddressOnPopUpCheckUserInfoBeforeDraw.text;
        var emailAddressStr = this.popUpCheckUserInfoBeforeDrawModule.GetUpdatedUserEmailText();

        // this.targetUserMobileNumberStr = moblieNumberStr;
        this.targetUserEmailAddressStr = emailAddressStr;

        this.currentRewardData.SetTargetEmailAddress(emailAddressStr);
    }

    // 럭키 드로우 팝업 ---------------------

    public void OnClickLuckyDrawShot()
    {
        CustomDebug.LogWithColor($"OnClick LuckyDraw Shot, {this.currentRewardData.stageType} / {this.currentRewardData.lessonType} ", CustomDebug.ColorSet.Cyan);

        DoLuckyDrawRoutine();
    }

    [Obsolete]
    private void DoLuckyDrawRoutine()
    {
        // 유저 정보 확인 절차부터 수행해서 들어온 것

        // 럭키 드로우
        // 메일 보내기 
        // 메일 전송 성공 후, 파베 db 정보 수정하기
        // 위의 3가지 내용은 서버단에서 수행함

        // 럭키 드로우 팝업 내 드로우 진행 중 애니메이션 수행

        // luckyDrawSubModule.StartDrawAnim();

#if UNITY_EDITOR
        WhenEmailSendingProcessFinished(true);

#elif UNITY_WEBGL && !TESTAPP

        TempPrizeData tempPrizeData = new TempPrizeData();
        tempPrizeData.email = this.targetUserEmailAddressStr;
        tempPrizeData.stageType = this.currentRewardData.StageTypeStr;
        tempPrizeData.lessonType = this.currentRewardData.LessonTypeStr;

        var prizeDataJsonStr = DevUtil.Instance.GetJson(tempPrizeData);

        PathJsLibData pathJsLibData = new PathJsLibData();

        pathJsLibData.objectName = this.transform.name;
        pathJsLibData.callbackName = nameof(GetRewardSucceeded);
        pathJsLibData.fallbackName = nameof(GetRewardFailed);

        FirebaseFunctionsController.Instance.LoadRewardRoutine(prizeDataJsonStr, pathJsLibData);

#elif UNITY_WEBGL && TESTAPP
            
            WhenEmailSendingProcessFinished(true);
#endif
    }

    protected virtual void SendAGiftToUserEmail()
    {
        LoadingManager.Instance.ActivateLoading();

        // 유저 정보 확인 절차부터 수행해서 들어온 것, 바로 경품을 유저 메일로 보낸다

        // 정해진 아이템 확인
        // 메일 보내기 
        // 메일 전송 성공 후, 파베 db 정보 수정하기

#if UNITY_EDITOR
        WhenEmailSendingProcessFinished(true);
        
#elif UNITY_WEBGL && !TESTAPP

        TempPrizeData tempPrizeData = new TempPrizeData();

        tempPrizeData.SetEmail(this.targetUserEmailAddressStr);
        tempPrizeData.SetPrizeData(this.currentRewardData);

        var prizeDataJsonStr = DevUtil.Instance.GetJson(tempPrizeData);

        PathJsLibData pathJsLibData = new PathJsLibData();

        pathJsLibData.objectName = this.transform.name;
        pathJsLibData.callbackName = nameof(GetRewardSucceeded);
        pathJsLibData.fallbackName = nameof(GetRewardFailed);

        FirebaseFunctionsController.Instance.LoadRewardRoutine(prizeDataJsonStr, pathJsLibData);

#elif UNITY_WEBGL && TESTAPP
            
            WhenEmailSendingProcessFinished(true);
#endif
    }

    // SendAGiftToUserEmail 에서 참조 중
    private void GetRewardSucceeded(string result)
    {
        CustomDebug.Log($"Get Reward Succeeded : {result}");

        var table = DevUtil.Instance.GetTargetObjectFromJson<Dictionary<string, string>>(result);

        UpdatePrizeData(table);

        var targetCode = table["targetCode"]; // none or {code}

        SetResendData(targetCode, this.currentRewardData.PrizeItemInfoId);
    }

    // SendAGiftToUserEmail 에서 참조 중
    protected virtual void GetRewardFailed(string errorResult)
    {
        LoadingManager.Instance.DeActivateLoading();

        try
        {
            CustomDebug.Log($"Get Reward Failed : {errorResult}");
            /*
              errorResult=>
                {
                    "code":"unavailable",
                    "details":{
                    "code":"744",
                    "message":"send Email Error"
                    }
                } 
             */

            // or

            /* 
             "code":"internal"
             */

            var table = DevUtil.Instance.GetTargetObjectFromJson<Dictionary<string, object>>(errorResult);

            // test debug code
            for (int i = 0; i < table.Count; i++)
            {
                var kvp = table.ElementAt(i);

                CustomDebug.Log($"kvp key : {kvp.Key} / value : {kvp.Value}");
            }
            // test debug code

            // CustomDebug.Log($"table[details] type?  : {table["details"].GetType()}"); // Newtonsoft.Json.Linq.JObject

            var detailsJson = DevUtil.Instance.GetJson(table["details"]); // table["details"] as string 변환이 안됨, json 형태로 변환 후 테이블로 다시 변환 후 사용

            CustomDebug.Log($"detailsJson : {detailsJson}");

            var detailsTable = DevUtil.Instance.GetTargetObjectFromJson<Dictionary<string, string>>(detailsJson);

            HandlingDrawError(detailsTable["code"]);
        }
        catch(Exception e)
        {
            CustomDebug.LogError($"GetRewardFailed error : {e.Message}");

            HandlingDrawError("0000");
        }
    }

    protected virtual void HandlingDrawError(string ErrorCode)
    {
        CustomDebug.Log($"HandlingDrawError, {ErrorCode}");

        /*
        744 - 이메일 보내기 오류
        770 - 뽑기 오류
        401 - 인증 오류
        440 - update 오류
        */

        // luckyDrawSubModule.StopDrawAnim();

        HomeController.Instance.SetAfterCommonErrorPopUpDeActivated(()=> {

            // luckyDrawSubModule.ResetData();
        });

        switch (ErrorCode)
        {
            case "744":
                {
                    HomeController.Instance.ActivateCommonErrorPopUp(EnumSets.ErrorType.SendRewardEmailFailed);
                }
                break;
            case "770":
                {
                    HomeController.Instance.ActivateCommonErrorPopUp(EnumSets.ErrorType.DrawError);
                }
                break;
            case "401":
                {
                    HomeController.Instance.ActivateCommonErrorPopUp(EnumSets.ErrorType.AuthError);
                }
                break;
            case "440":
                {
                    HomeController.Instance.ActivateCommonErrorPopUp(EnumSets.ErrorType.DBUpdateError);
                }
                break;
            case "0000":
                {
                    HomeController.Instance.ActivateCommonErrorPopUp(EnumSets.ErrorType.UnavailableError);
                }
                break;
        }
    }

    protected virtual void SetResendData(string targetCode, string prizeId)
    {
        resendData = new ResendData();

        resendData.SetEmail(this.targetUserEmailAddressStr);
        resendData.SetCode(targetCode);
        resendData.SetPrizeId(prizeId);

        resendData.SetData(this.currentRewardData);
    }

    //-------------------------------------------------
    public void OnClickBtnCheckGottenReward(RewardModule rewardModule)
    {
        SetCurrentRewardData(rewardModule);

        ActivateGottenRewardInfoPopUp();
    }

    public virtual void ActivateGottenRewardInfoPopUp()
    {
        LoadingManager.Instance.DeActivateLoading();

        // 이미 해당 상품은 받았고, 정보만 확인하기위함

        // 팝업 내 메일전송 로딩 이미지 대신 '유저 정보(이메일 주소) 띄우기'

        // DeActivateLocalLoading();

        popUpGottenRewardInfoModule.UpdateState(this.currentRewardData);

        popUpGottenRewardInfoModule.ActivatePopUp();
    }

    public void DeActivateGottenRewardInfoPopUp()
    {
        popUpGottenRewardInfoModule.DeActivatePopUp();

        ResetData();
    }

    private void WhenEmailSendingProcessFinished(bool isCompleted)
    {
        // 이메일 전송 성공 시
        if (isCompleted)
        {
#if UNITY_EDITOR
            UpdatePrizeDataOnEditor();
#endif
        }
        else
        {
            // ActivateBtnsOnPopUpGottenRewardInfo();

            // 실패 시, 실패 내용 전달
            HomeController.Instance.ActivateCommonErrorPopUp(EnumSets.ErrorType.SendRewardEmailFailed); // editor 전용
        }
    }

    protected virtual void UpdatePrizeData(Dictionary<string, string> rewardingSuccessData)
    {
        CustomDebug.Log($"UpdatePrizeData >>>>> {this.currentRewardData.RandomKey}");

        var prizeDataList = UserManager.Instance.GetSpecificGottenPrizeItemInfos(this.currentRewardData.StageTypeStr, this.currentRewardData.LessonTypeStr);

        if (prizeDataList != null)
        {
            var datas = from data in prizeDataList
                        where data.Key.Equals(this.currentRewardData.RandomKey)
                        select data;

            if (datas.Count() > 0)
            {
                var prizeData = datas.ElementAt(0);

                var specificPrizeData = prizeData.Value;

                specificPrizeData.receive = true;

                specificPrizeData.receiveDate = Convert.ToInt64(rewardingSuccessData["receiveDate"]);
                CustomDebug.LogWithColor($"prizeData.receiveDate : {specificPrizeData.receiveDate}", CustomDebug.ColorSet.Yellow);

                specificPrizeData.type = rewardingSuccessData["type"];
                CustomDebug.Log($"specificPrizeData.type : {specificPrizeData.type}");

                specificPrizeData.targetEmailAddress = this.targetUserEmailAddressStr;
                CustomDebug.Log($"specificPrizeData.targetEmailAddress : {specificPrizeData.targetEmailAddress}");

                this.currentRewardData.SetPrizeItem(specificPrizeData);

                UserManager.Instance.UpdatePrizeData(this.currentRewardData, this.currentRewardData.MyPrizeItem);

                this.currentRewardData.UpdateRewardItemState();

                ActivateGottenRewardInfoPopUp();
            }
            else
            {
                CustomDebug.LogError($"{this.currentRewardData.RandomKey} 와 일치하는 값이 없음");
            }
        }
    }

    protected void UpdatePrizeData(Dictionary<string, string> rewardingSuccessData, Action onUpdateComplete)
    {
        try
        {
            CustomDebug.Log($"UpdatePrizeData with onUpdateComplete : {this.currentRewardData.RandomKey}");

            var prizeDataList = UserManager.Instance.GetSpecificGottenPrizeItemInfos(this.currentRewardData.StageTypeStr, this.currentRewardData.LessonTypeStr);

            if (prizeDataList != null)
            {
                var datas = from data in prizeDataList
                            where data.Key.Equals(this.currentRewardData.RandomKey)
                            select data;

                if (datas.Count() > 0)
                {
                    var prizeData = datas.ElementAt(0);

                    var specificPrizeData = prizeData.Value;

                    specificPrizeData.receive = true;

                    specificPrizeData.receiveDate = Convert.ToInt64(rewardingSuccessData["receiveDate"]);
                    CustomDebug.LogWithColor($"prizeData.receiveDate : {specificPrizeData.receiveDate}", CustomDebug.ColorSet.Yellow);

                    specificPrizeData.type = rewardingSuccessData["type"];
                    CustomDebug.Log($"specificPrizeData.type : {specificPrizeData.type}");

                    specificPrizeData.targetEmailAddress = this.targetUserEmailAddressStr;
                    CustomDebug.Log($"specificPrizeData.targetEmailAddress : {specificPrizeData.targetEmailAddress}");

                    this.currentRewardData.SetPrizeItem(specificPrizeData);

                    UserManager.Instance.UpdatePrizeData(this.currentRewardData, this.currentRewardData.MyPrizeItem);

                    this.currentRewardData.UpdateRewardItemState();

                    ActivateGottenRewardInfoPopUp();

                    onUpdateComplete?.Invoke();
                }
                else
                {
                    CustomDebug.LogError($"{this.currentRewardData.RandomKey} 와 일치하는 값이 없음");
                }
            }
        }
        catch(Exception e)
        {
            CustomDebug.LogError($"UpdatePrizeData error : {e.Message}");
        }
        #region
        //var prizeDataList = UserManager.Instance.GetSpecificGottenPrizeItemInfos(this.currentRewardData.StageTypeStr, this.currentRewardData.LessonTypeStr);

        //if (prizeDataList != null)
        //{
        //    var datas = from data in prizeDataList
        //                where data.Key.Equals(this.currentRewardData.LessonTypeStr)
        //                select data;

        //    if (datas.Count() > 0)
        //    {
        //        var prizeData = datas.ElementAt(0);

        //        var specificPrizeData = prizeData.Value;

        //        specificPrizeData.receive = true;

        //        specificPrizeData.receiveDate = Convert.ToInt64(rewardingSuccessData["receiveDate"]);
        //        CustomDebug.LogWithColor($"prizeData.receiveDate : {specificPrizeData.receiveDate}", CustomDebug.ColorSet.Cyan);

        //        specificPrizeData.type = rewardingSuccessData["type"];
        //        specificPrizeData.targetEmailAddress = this.targetUserEmailAddressStr;

        //        this.currentRewardData.SetPrizeItem(specificPrizeData);

        //        UserManager.Instance.UpdatePrizeData(this.currentRewardData, this.currentRewardData.MyPrizeItem);

        //        this.currentRewardData.UpdateRewardItemState();

        //        ActivateGottenRewardInfoPopUp();

        //        onUpdateComplete?.Invoke();
        //    }
        //}
        #endregion
    }

    private void LoadLuckyDrawCompleteRoutine()
    {
        
        // 뽑기 진행 애니메이션 끝 - 종료 애니메이션 수행
        // 애니메이션 수행 후 아래 구문 수행할 수 있도록 콜백 등록하기
        //luckyDrawSubModule.SetFinishCallback(() => {

        //    ActivateGottenRewardInfoPopUp();
        //});

        //luckyDrawSubModule.StopDrawAnim();
        //luckyDrawSubModule.ResetData();
    }

    [Obsolete]
    private void UpdatePrizeDataOnEditor()
    {
        CustomDebug.Log($"UpdatePrizeDataOnEditor : {this.currentRewardData.StageTypeStr}");

        var prizeDataList = UserManager.Instance.GetSpecificGottenPrizeItemInfos(this.currentRewardData.StageTypeStr, this.currentRewardData.LessonTypeStr);

        if (prizeDataList != null)
        {
            var datas = from data in prizeDataList
                        where data.Key.Equals(this.currentRewardData.RandomKey)
                        select data;

            if (datas.Count() > 0)
            {
                var prizeData = datas.ElementAt(0);

                var specificPrizeData = prizeData.Value;

                specificPrizeData.receive = true;
                specificPrizeData.receiveDate = DevUtil.Instance.GetUTCNowTimeStamp();

                specificPrizeData.type = "포토샵 1개월 구독권";
                specificPrizeData.targetEmailAddress = this.targetUserEmailAddressStr;

                CustomDebug.LogWithColor($"prizeData.receiveDate : {specificPrizeData.receiveDate} / type : {specificPrizeData.type} / targetEmailAddress : {specificPrizeData.targetEmailAddress}", CustomDebug.ColorSet.Yellow);

                this.currentRewardData.SetPrizeItem(specificPrizeData);

                LoadLuckyDrawCompleteRoutine();

                AfterPrizeDataUpdated(true);
            }
        }
        else
        {
            CustomDebug.LogError("prizeDataList == null");
        }
    }

    // 에디터에서 사용 중
    private void AfterPrizeDataUpdated(bool isSucceeded)
    {
        if(isSucceeded)
        {
            UserManager.Instance.UpdatePrizeData(this.currentRewardData, this.currentRewardData.MyPrizeItem);

           // ActivateBtnsOnPopUpGottenRewardInfo();

            this.rewardControl.UpdateRewardItemState();
        }
        else
        {
            HomeController.Instance.ActivateCommonErrorPopUp(EnumSets.ErrorType.UpdatePrizeDataAfterSendRewardEmailFailed);
        }
    }

    // 다시 보내기 버튼에 참조되어있음
    public virtual void ReSendRewardEmail()
    {
#if UNITY_EDITOR

        CustomDebug.LogWithColor("Re Send Reward Email", CustomDebug.ColorSet.Green);

#elif UNITY_WEBGL 
        CustomDebug.Log("Try ReSendRewardEmail");

        LoadingManager.Instance.ActivateLoading();

        if(!string.IsNullOrEmpty(this.resendData.email))
        {
            var json = DevUtil.Instance.GetJson(this.resendData);

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
#endif

    }

    protected virtual void SucceededResend()
    {
        CustomDebug.Log("이메일 재전송 성공");

        popUpGottenRewardInfoModule.DeActivateResendBtn();

        LoadingManager.Instance.DeActivateLoading();
    }

    protected virtual void FailedResend(string error)
    {
        CustomDebug.LogError($"FailedResend : {error}");

        HomeController.Instance.ActivateCommonErrorPopUp(EnumSets.ErrorType.ReSendRewardEmailFailed);

        LoadingManager.Instance.DeActivateLoading();
    }
}
