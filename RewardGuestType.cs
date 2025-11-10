using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class RewardGuestType : MonoBehaviour, IRewardControl
{
    private RewardModule[] rewardModules; // 템플릿들만 가지고있음

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Init(RewardModule[] rewardModules)
    {
        CustomDebug.Log("reward Guest type, Init");

        this.rewardModules = rewardModules;

        UpdateRewardItemState();

        AllDeActivateRewardIcons(rewardModules);
    }

    public void UpdateRewardItemState()
    {
        CustomDebug.Log("reward Guest type, UpdateRewardItemState Do nothing");
        // 게스트 모드에서는 상품 습득 여부 상관없이 상품받기 버튼이 활성화 되어있음
        // 해당 상품받기 버튼을 누르면 계정연동하라는 팝업이 출력됨

        //230114, 김형철 추가
        // 마지막 강의 상품정보뿐만 아니라 일반상품정보도 표시해야하므로, rewardModule 의 Init 호출이 필요함
        for (int i = 0; i < rewardModules.Length; i++)
        {
            rewardModules[i].JustInit();
        }
    }

    private void AllDeActivateRewardIcons(RewardModule[] rewardModules)
    {
        for (int i = 0; i < rewardModules.Length; i++)
        {
            rewardModules[i].ActivateGuestTypeBtn();
        }
    }

    public void SetRewardItemWithPrizeItemInfo()
    {
        CustomDebug.Log("guestType, SetRewardItemWithPrizeItemInfo");

        // rewardModules 기반 데이터를 가지고 선물기록 틀을 만들어야하므로 게스트에서도 수행

        try
        {
            // 마지막 상품버튼에는 게스트 전용버튼만을 활성화시켰음
            // 상품정보 부여 루틴은 수행하지않음

            // 일반 상품표시를 해야해서 필요한 상품정보는 세팅함
            // SpecialRewardModule 에서 게스트 전용 로직이 돌아감
            SetOrdinaryRewardItemWithPrizeItemInfo();

            // special reward 들은 lesson Manager start 단에서 수행
        }
        catch (Exception e)
        {
            CustomDebug.LogError($"error : {e.Message}");
        }
    }

    public void SetOrdinaryRewardItemWithPrizeItemInfo()
    {
        try
        {
            if (AppInfo.Instance.IsAleadySetSetOrdinaryPrizeItemInfoTable())
            {
                return;
            }

            var table = AppInfo.Instance.prizeItemInfoTable;

            // test debug
            //foreach (var item in table)
            //{
            //    CustomDebug.Log($"db prizeItem table -> {item.Key} / {item.Value.stagePos} / {item.Value.lessonPos}");
            //}

            var tmpTable = table.ToDictionary((item => item.Key), (item => item.Value));

            // test debug
            //foreach (var item in tmpTable)
            //{
            //    CustomDebug.Log($"SetOrdinaryRewardItemWithPrizeItemInfo -> {item.Key} / {item.Value}");
            //}

            for (int i = 0; i < this.rewardModules.Length; i++)
            {
                var module = this.rewardModules[i];

                //CustomDebug.Log($"module ? :{module.StageTypeStr} / {module.LessonTypeStr}");

                var values = from data in table
                             where data.Value.stagePos.Equals(module.StageTypeStr) && data.Value.lessonPos.Equals(module.LessonTypeStr)
                             select data;

                if (values.Count() > 0)
                {
                    var value = values.ElementAt(0);

                    // 마지막 강의상품 정보는 제외시키기
                    //CustomDebug.Log($"SetOrdinaryRewardItemWithPrizeItemInfo key? : {value.Key}");

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
}
