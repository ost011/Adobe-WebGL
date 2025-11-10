using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class RewardModule : MonoBehaviour
{
    [Header("RewardModule 을 상속받은 SpecialRewardModule 에서는 StageType")]
    [Header("LessonType, btnGetARewardInGuestSignInType 할당을 하지않는다")]
    [Space]
    public EnumSets.StageType stageType;
    public EnumSets.LessonType lessonType;

    protected bool recievedAReward = false;
    public bool RecievedAReward => this.recievedAReward;

    protected string whatReward = "";
    public string WhatReward => this.whatReward;

    protected string targetEmailAddress = "";
    public string TargetEmailAddress => this.targetEmailAddress;

    protected string randomKey = ""; // -NAhc1kviGFydf_WnHIO
    public string RandomKey => this.randomKey;

    public GameObject[] rewardItems; // 0 on, 1 off, 2 lock

    [Space]
    public GameObject btnGetARewardInGuestSignInType;

    [SerializeField]
    protected string stageTypeStr = "";
    public string StageTypeStr => this.stageTypeStr;

    [SerializeField]
    protected string lessonTypeStr = "";
    public string LessonTypeStr => this.lessonTypeStr;

    private bool isInit = false;

    protected PrizeItem prizeItem;
    public PrizeItem MyPrizeItem => this.prizeItem;

    //---------------------------------------------
    protected string prizeItemInfoId = "";
    public string PrizeItemInfoId => this.prizeItemInfoId;

    protected PrizeItemInfo myPrizeItemInfo;
    public PrizeItemInfo MyPrizeItemInfo => this.myPrizeItemInfo;

    protected bool isSpecialPrize = false;
    public bool IsSpecialPrize => this.isSpecialPrize;

    protected const string NORMAL_PRIZE_TYPE = "normal";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Init()
    {
        // 꺼진 채로 시작하는 객체라서
        // start 단에서 Init()을 수행할 타이밍이 맞질않음
        // UpdateRewardItemState() 에서 Init() 을 수행함
        if (!isInit)
        {
            this.stageTypeStr = this.stageType.ToString().ToLower();
            this.lessonTypeStr = this.lessonType.ToString().ToLower();

            isInit = true;
        }
    }

    public virtual void UpdateRewardItemState()
    {
        // CustomDebug.Log("----------- UpdateRewardItemState ---------------");

        Init();

        var specificStageCompleteTable = UserManager.Instance.GetSpecificStageCompletedInfo(this.stageTypeStr);

        // CustomDebug.Log($"this.stageTypeStr : {this.stageTypeStr}");

        if (specificStageCompleteTable != null) // 해당 스테이지 완료 정보는 있다
        {
            var prizeTable = UserManager.Instance.GetSpecificGottenPrizeItemInfos(this.stageTypeStr, this.lessonTypeStr);

            if (prizeTable != null) // 해당 스테이지와 관련된 상품 기록이 있다
            {
                var properValues = from data in prizeTable
                                     where data.Value.category.Equals(NORMAL_PRIZE_TYPE)
                                     select data;

                if(properValues.Count() > 0) // 해당 스테이지 -레슨 상품 기록은 있는데
                {
                    this.randomKey = properValues.ElementAt(0).Key;

                    UpdateLocalRewardState(properValues.ElementAt(0).Value);
                }
                else
                {
                    // 해당 스테이지 - 레슨 상품 기록이 없다
                    SetLockState();
                }
            }
            else
            {
                // 해당 스테이지와 관련된 상품 기록 자체가 없다
                CustomDebug.LogWarning($"{this.stageTypeStr}, prizeTable is NULL");

                SetLockState();
            }
        }
        else
        {
            // 이전 스테이지를 어떻게든 해치우고, 다음 스테이지 첫번째 레슨 대기중인 상태(아무것도 안한 상태)
            SetLockState();
        }

        // CustomDebug.Log("----------- END UpdateRewardItemState ---------------");
    }

    public void JustInit()
    {
        Init();
    }

    protected virtual void UpdateLocalRewardState(PrizeItem prizeItem)
    {
        var prizeData = prizeItem;

        this.recievedAReward = prizeData.receive;
        UpdateDrawBtnState(); // 상품 획득여부에 따라 달리 띄우기

        this.whatReward = prizeData.type;
        this.targetEmailAddress = prizeData.targetEmailAddress;

        this.prizeItem = prizeData;

        CustomDebug.Log($"prizeData, recievedAReward : {recievedAReward} / whatReward : {whatReward} / targetEmailAddress : {targetEmailAddress}");
    }

    private void ActivateLockState()
    {
        this.rewardItems[2].SetActive(true);
    }

    private void ActivateOnState()
    {
        this.rewardItems[0].SetActive(true);
    }

    private void ActivateOffState()
    {
        this.rewardItems[1].SetActive(true);
    }

    public void DeActivateAllItems()
    {
        for (int i = 0; i < rewardItems.Length; i++)
        {
            rewardItems[i].SetActive(false);
        }
    }

    protected void UpdateDrawBtnState()
    {
        if (!this.recievedAReward)
        {
            SetOnState();
        }
        else
        {
            SetOffState();
        }
    }

    private void SetLockState()
    {
        DeActivateAllItems();

        ActivateLockState();
    }

    protected void SetOnState()
    {
        DeActivateAllItems();

        ActivateOnState();
    }

    protected void SetOffState()
    {
        DeActivateAllItems();

        ActivateOffState();
    }

    public void SetWhatReward(string rewardType)
    {
       // CustomDebug.Log($"SetWhatReward : {rewardType} <- where : {transform.name}");

        this.whatReward = rewardType;
    }

    public void SetTargetEmailAddress(string target)
    {
        this.targetEmailAddress = target;
    }

    public void SetPrizeItem(PrizeItem prizeItem)
    {
        this.prizeItem = prizeItem;

        CustomDebug.Log($"SetPrizeItem, prizeItem Data : {prizeItem.targetEmailAddress} / {prizeItem.type}");

        // this.whatReward = this.prizeItem.type;
    }

    public void SetPrizeItemIfnoId(string id)
    {
        this.prizeItemInfoId = id;
    }

    public void SetPrizeItemInfo(PrizeItemInfo prizeItemInfo)
    {
        this.myPrizeItemInfo = prizeItemInfo;

        SetWhatReward(this.myPrizeItemInfo.name);
    }

    public void CheckIsSpecialPrize()
    {
        if(this.myPrizeItemInfo.special)
        {
            this.isSpecialPrize = true;
        }
    }

    public void SetStageTypeStr(string stageType)
    {
        this.stageTypeStr = stageType;
    }

    public void SetLessonTypeStr(string lessonType)
    {
        this.lessonTypeStr = lessonType;
    }

    public void SetRandomKey(string randomKey)
    {
        this.randomKey = randomKey;
    }

    //------------------------------------------

    public virtual void OnClickGettingGift()
    {
        RewardManager.Instance.OnClickTryDrawingBtn(this);
    }

    public void ActivateGuestTypeBtn()
    {
        DeActivateAllItems();

        btnGetARewardInGuestSignInType.SetActive(true);
    }

    // CompleteGift의 On-GuestType에도 참조되어있음
    public void OnClickGuestTypeBtn()
    {
        HomeController.Instance.ActivateCommonErrorPopUp(EnumSets.ErrorType.GuestSignInType);
    }

}
