using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct GottenRewardInfo
{
    public int successRewardCount;
    public string targetEmailAddress;
    public string stageType;
    public string lessonType;
    public string rewardName;
    public bool isSpecial;
}

public class PopUpGottenRewardInfoModuleInSLM : PopUpGottenRewardInfoModule
{
    // 해당 오브젝트들은 parentsGiftImg[1] 의 자식들임
    public GameObject[] specialGiftImgs;  // 0 : special, 1 : ACP, 2 : combined(S3 L6)

    private GottenRewardInfo gottenRewardInfo;

    private const string NORMAL_TEXT = "※ 아래 이메일로 발송되었습니다! ※";

    public void UpdateState(GottenRewardInfo gottenRewardInfo)
    {
        this.gottenRewardInfo = gottenRewardInfo;

        ActivateTargetGiftImg();

        SetTextWhatPrizeGotten(gottenRewardInfo.rewardName);
        SetTextWhereOnPopUpGottenRewardInfo(gottenRewardInfo.targetEmailAddress);

        SetNoticeText();
    }

    private void ActivateTargetGiftImg()
    {
        AllDeActivateGiftImgs();

        var stageType = this.gottenRewardInfo.stageType;
        var lessonType = this.gottenRewardInfo.lessonType;

        if(this.gottenRewardInfo.successRewardCount > 1)
        {
            ActivateCombinedGiftImg(stageType, lessonType);
        }
        else
        {
            if (gottenRewardInfo.isSpecial)
            {
                ActivateSpecialGiftImg(stageType, lessonType);
            }
            else
            {
                ActivateDefaultGiftImg(stageType);
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

            if (i == 1)
            {
                for (int j = 0; j < specialGiftImgs.Length; j++)
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

        if (stageType.Equals("s1") && lessonType.Equals("l1"))
        {
            CustomDebug.Log("S1L1 Special Icon Activated");

            specialGiftImgs[0].SetActive(true);
        }
        else
        {
            if (stageType.Equals("s3") && lessonType.Equals("l6"))
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

    private void SetNoticeText()
    {
        this.textNotice.text = NORMAL_TEXT;
    }
}
