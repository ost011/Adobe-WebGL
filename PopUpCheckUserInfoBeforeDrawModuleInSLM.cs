using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopUpCheckUserInfoBeforeDrawModuleInSLM : PopUpCheckUserInfoBeforeDrawModule
{
    // 해당 오브젝트들은 parentsGiftImg[1] 의 자식들임
    public GameObject[] specialGiftImgs;  // 0 : special, 1 : ACP, 2 : combined(S3 L6)

    public void UpdateState(RewardModuleInSLM rewardModuleInSLM)
    {
        ActivateTargetGiftImg(rewardModuleInSLM);

        SetDatasOnPopUpCheckUserInfoBeforeDraw();
    }

    private void ActivateTargetGiftImg(RewardModuleInSLM rewardModuleInSLM)
    {
        AllDeActivateGiftImgs();

        var rewardInfoTable = rewardModuleInSLM.GetRewardInfoTable();

        var prizeItemInfo = rewardInfoTable.ElementAt(0).Value;

        var stageType = prizeItemInfo.stagePos;
        var lessonType = prizeItemInfo.lessonPos;

        if (rewardInfoTable.Count > 1)
        {
            // 두개 받는 경우

            ActivateCombinedGiftImg(stageType, lessonType);
        }
        else
        {
            if(prizeItemInfo.special)
            {
                ActivateSpecialGiftImg(stageType, lessonType);
            }
            else
            {
                ActivateDefaultGiftImg(prizeItemInfo.stagePos);
            }
        }
    }

    protected override void AllDeActivateGiftImgs()
    {
        for (int i = 0; i < parentsGiftImg.Length; i++)
        {
            if (i == 0)
            {
                for (int j = 0; j < defaultGiftImgs.Length; j++)
                {
                    defaultGiftImgs[j].SetActive(false);
                }
            }

            if(i == 1)
            {
                for(int j = 0; j < specialGiftImgs.Length; j++)
                {
                    specialGiftImgs[j].SetActive(false);
                }
            }

            parentsGiftImg[i].SetActive(false);
        }
    }

    private void ActivateSpecialGiftImg(string stageType, string lessonType)
    {
        parentsGiftImg[1].SetActive(true);

        if(stageType.Equals("s1") && lessonType.Equals("l1"))
        {
            CustomDebug.Log("S1L1 Special Icon Activated");

            specialGiftImgs[0].SetActive(true);
        }
        else
        {
            if(stageType.Equals("s3") && lessonType.Equals("l6"))
            {
                CustomDebug.Log("S3L6 ACP Icon Activated");

                specialGiftImgs[1].SetActive(true);
            }
        }
    }

    private void ActivateCombinedGiftImg(string stageType, string lessonType)
    {
        parentsGiftImg[1].SetActive(true);

        if (stageType.Equals("s3") && lessonType.Equals("l6"))
        {
            CustomDebug.Log("S3 L6 Combined Icon Activated");

            specialGiftImgs[2].SetActive(true); // ACP + 템플릿
        }
        else
        {
            CustomDebug.LogError("S3 L6가 아닌데 줘야할 경품이 두개 이상 있다!");
        }
    }
}
