using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class RewardSignedType : MonoBehaviour, IRewardControl
{
    private RewardModule[] rewardModules; // 템플릿들만 가지고있음

    private void Start()
    {

    }

    public void Init(RewardModule[] rewardModules)
    {
        this.rewardModules = rewardModules;

        UpdateRewardItemState();
    }

    public void SetRewardItemWithPrizeItemInfo()
    {
        // CustomDebug.Log("signedType, SetRewardItemWithPrizeItemInfo");

        try
        {
            var table = AppInfo.Instance.prizeItemInfoTable;

            for (int i = 0; i < rewardModules.Length; i++)
            {
                var module = rewardModules[i];

                var values = from data in table
                             where data.Value.stagePos.Equals(module.StageTypeStr) && data.Value.lessonPos.Equals(module.LessonTypeStr)
                             select data;

                if (values.Count() > 0)
                {
                    var kvp = values.ElementAt(0);

                    module.SetPrizeItemIfnoId(kvp.Key);
                    module.SetPrizeItemInfo(kvp.Value);
                    module.CheckIsSpecialPrize();
                }
            }

            SetOrdinaryRewardItemWithPrizeItemInfo();

            // special reward 들은 lesson Manager start 단에서 수행
        }
        catch (Exception e)
        {
            CustomDebug.LogError($"error : {e.Message}");
        }
    }

    // 스테이지별 마지막 강의완료 시 지급하는 상품을 제외한 일반 강의의 상품 초기화관련
    public void SetOrdinaryRewardItemWithPrizeItemInfo()
    {
        try
        {
            if (AppInfo.Instance.IsAleadySetSetOrdinaryPrizeItemInfoTable())
            {
                return;
            }

            var table = AppInfo.Instance.prizeItemInfoTable;

            var tmpTable = table.ToDictionary((item => item.Key), (item => item.Value));

            for (int i = 0; i < rewardModules.Length; i++)
            {
                var module = rewardModules[i];

                var values = from data in table
                             where data.Value.stagePos.Equals(module.StageTypeStr) && data.Value.lessonPos.Equals(module.LessonTypeStr)
                             select data;

                if (values.Count() > 0)
                {
                    var value = values.ElementAt(0);

                    // 마지막 강의상품 정보는 제외시키기

                    if (tmpTable.ContainsKey(value.Key))
                    {
                        tmpTable.Remove(value.Key);
                    }
                }
            }

            // 스페셜 상품이 아닌 것을 고르기
            var ordinaryValues = from data in tmpTable
                                 where data.Value.special == false
                                 select data;

            var ordinaryPrizeItemTable = ordinaryValues.ToDictionary((item => item.Key), (item => item.Value));

            AppInfo.Instance.SetOrdinaryPrizeItemInfoTable(ordinaryPrizeItemTable);
        }
        catch (Exception e)
        {
            CustomDebug.LogError($"SetOrdinaryRewardItemWithPrizeItemInfo error : {e.Message}");
        }
    }

    public void UpdateRewardItemState()
    {
        for (int i = 0; i < rewardModules.Length; i++)
        {
            rewardModules[i].UpdateRewardItemState();
        }
    }
}
