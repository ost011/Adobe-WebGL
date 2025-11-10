using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RewardModuleInSLM : RewardModule
{
    private Dictionary<string[], PrizeItemInfo> rewardInfoTable = new Dictionary<string[], PrizeItemInfo>();

    public void SetRewardModuleInfoTable(string[] arrayLessonInfosWithPrizeInfo, PrizeItemInfo prizeItemInfo)
    {
        rewardInfoTable.Add(arrayLessonInfosWithPrizeInfo, prizeItemInfo);
    }

    // SendAGiftToUserEmail 에 참조되어 있음
    public void InitRewardModuleInfo(string[] arrayLessonInfosWithPrizeInfo, PrizeItemInfo prizeItemInfo)
    {
        this.stageTypeStr = arrayLessonInfosWithPrizeInfo[0];
        this.lessonTypeStr = arrayLessonInfosWithPrizeInfo[1];

        var prizeId = arrayLessonInfosWithPrizeInfo[2];

        this.randomKey = arrayLessonInfosWithPrizeInfo[3];

        var tmpPrizeItemInfo = prizeItemInfo;

        SetPrizeItemIfnoId(prizeId);

        SetPrizeItemInfo(tmpPrizeItemInfo);

        CheckIsSpecialPrize();
    }

    public override void OnClickGettingGift()
    {
        RewardManager.Instance.OnClickTryDrawingBtn(this);
    }

    // 220921 this.randomKey 추가분
    public override void UpdateRewardItemState()
    {
        CustomDebug.Log("----------- UpdateRewardItemState in SLM ---------------");

        var specificStageCompleteTable = UserManager.Instance.GetSpecificStageCompletedInfo(this.stageTypeStr);

        CustomDebug.Log($"UpdateRewardItemState.stageTypeStr : {this.stageTypeStr}");

        if (specificStageCompleteTable != null) // 해당 스테이지 완료 정보는 있다
        {
            var prizeTable = UserManager.Instance.GetSpecificGottenPrizeItemInfos(this.stageTypeStr, this.lessonTypeStr);

            if (prizeTable != null) // 해당 스테이지와 관련된 상품 기록이 있다
            {
                CustomDebug.Log($"UpdateRewardItemState.lessonTypeStr : {this.lessonTypeStr}");

                //var values = from data in prizeTable
                //             where data.Key.Equals(this.lessonTypeStr)
                //             select data;

                var properValues = from data in prizeTable
                                   where data.Value.category.Equals(NORMAL_PRIZE_TYPE)
                                   select data;

                if (properValues.Count() > 0)
                {
                    this.randomKey = properValues.ElementAt(0).Key;

                    UpdateLocalRewardState(properValues.ElementAt(0).Value);

                    CustomDebug.Log("해당 레슨의 prizeItems 기록은 있음");
                }
                else
                {
                    CustomDebug.Log("해당 스테이지의 상품 기록이 있으나 레슨의 prizeItems 기록이 없다, 아무것도 하지 않음");
                }
            }
            else
            {
                CustomDebug.Log(" 해당 스테이지와 관련된 상품 기록 자체가 없다, 아무것도 하지 않음");
            }
        }
        else
        {
            CustomDebug.Log("해당 스테이지 플레이 자체가 처음, 아무것도 하지 않음");
        }

        CustomDebug.Log("----------- END UpdateRewardItemState in SLM ---------------");
    }

    protected override void UpdateLocalRewardState(PrizeItem prizeItem)
    {
        var prizeData = prizeItem;

        this.recievedAReward = prizeData.receive;

        this.whatReward = prizeData.type;
        this.targetEmailAddress = prizeData.targetEmailAddress;

        this.prizeItem = prizeData;

        CustomDebug.Log($"prizeData in SLM, recievedAReward : {recievedAReward} / whatReward : {whatReward} / targetEmailAddress : {targetEmailAddress}");
    }

    public Dictionary<string[], PrizeItemInfo> GetRewardInfoTable()
    {
        return this.rewardInfoTable;
    }
}
