using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpecialRewardModule : RewardModule
{
    private bool completeLesson = false; // 레슨 클리어 안함
    private bool notYetGetAGift = true; // 아직 선물 안받음

    protected const string SPECIAL_PRIZE_TYPE = "special";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Init()
    {
        this.stageTypeStr = this.myPrizeItemInfo.stagePos;
        this.lessonTypeStr = this.myPrizeItemInfo.lessonPos;

        SetWhatReward(this.myPrizeItemInfo.name);

        CustomDebug.Log($"special reward module, {this.stageTypeStr} / {this.lessonTypeStr}");
    }

    public void SetSpecialPrizeItemPos(string prizeItemInfoId, PrizeItemInfo prizeItemInfo)
    {
        SetPrizeItemIfnoId(prizeItemInfoId);
        SetPrizeItemInfo(prizeItemInfo);
        CheckIsSpecialPrize();

        Init();

        var obtainPrizeTable = UserManager.Instance.GetSpecificGottenPrizeItemInfos(prizeItemInfo.stagePos, prizeItemInfo.lessonPos);

        if (obtainPrizeTable == null)
        {
            SetOnState(); // 아직 해당 스테이지-레슨 을 클리어 안함
        }
        else
        {
            var properValues = from data in obtainPrizeTable
                               where data.Value.category.Equals(GetPrizeTypeKeyword(prizeItemInfo))
                               select data;

            if(properValues.Count() > 0) // 무조건 1개 존재함
            {
                this.randomKey = properValues.ElementAt(0).Key;

                if (obtainPrizeTable.TryGetValue(this.randomKey, out var prizeItem))
                {
                    if (prizeItem.receive)
                    {
                        SetOffState(); // 선물 받았음

                        completeLesson = true;
                        notYetGetAGift = false;
                    }
                    else
                    {
                        SetOnState(); // 해당 스테이지-레슨을 클리어했으나 아직 선물 안받음

                        completeLesson = true;
                    }
                }
                else
                {
                    SetOnState(); // 아직 해당 스테이지-레슨 을 클리어 안함
                }
            }
        }
    }

    public string GetPrizeTypeKeyword(PrizeItemInfo info)
    {
        if (info.special)
        {
            return SPECIAL_PRIZE_TYPE;
        }

        return NORMAL_PRIZE_TYPE;
    }

    public override void UpdateRewardItemState()
    {
        CustomDebug.Log("----------- special UpdateRewardItemState ---------------");

        var specificStageCompleteTable = UserManager.Instance.GetSpecificStageCompletedInfo(this.stageTypeStr);

        CustomDebug.Log($"this.stageTypeStr : {this.stageTypeStr}");

        if (specificStageCompleteTable != null) // 해당 스테이지 완료 정보는 있다
        {
            var prizeTable = UserManager.Instance.GetSpecificGottenPrizeItemInfos(this.stageTypeStr, this.lessonTypeStr);

            if (prizeTable != null) // 해당 스테이지와 관련된 상품 기록이 있다
            {
                CustomDebug.Log($"this.random Key : {this.randomKey}");

                var values = from data in prizeTable
                             where data.Key.Equals(this.randomKey)
                             select data;

                if (values.Count() > 0) // 해당 스테이지 -레슨 상품 기록은 있는데
                {
                    UpdateLocalRewardState(values.ElementAt(0).Value);
                }
                else
                {
                    // 해당 스테이지 - 레슨 상품 기록이 없다
                    // 아무것도 하지않음
                    CustomDebug.Log("해당 스테이지 - 레슨 상품 기록이 없다 - 아무것도 하지않음");
                }
            }
            else
            {
                // 해당 스테이지와 관련된 상품 기록 자체가 없다
                // 아무것도 하지않음
                CustomDebug.Log("해당 스테이지와 관련된 상품 기록 자체가 없다 - 아무것도 하지않음");
            }
        }
        else
        {
            // 이전 스테이지를 어떻게든 해치우고, 다음 스테이지 첫번째 레슨 대기중인 상태(아무것도 안한 상태)
            // 아무것도 하지않음
            CustomDebug.Log("이전 스테이지를 어떻게든 해치우고, 다음 스테이지 첫번째 레슨 대기중인 상태(아무것도 안한 상태) - 아무것도 하지않음");
        }

        CustomDebug.Log("----------- END special UpdateRewardItemState ---------------");
    }

    public override void OnClickGettingGift()
    {
        if (AppInfo.Instance.IsGuestLogin)
        {
            OnClickGuestTypeBtn();
        }
        else
        {
            LoadSignInTypeRoutine();
        }
        
    }

    private void LoadSignInTypeRoutine()
    {
        if (!completeLesson)
        {
            HomeController.Instance.ActivateCommonErrorPopUp(EnumSets.ErrorType.NotYetReadyToGetAGift);
        }
        else
        {
            if (notYetGetAGift)
            {
                RewardManager.Instance.OnClickTryDrawingBtn(this);
            }
        }
    }
}
