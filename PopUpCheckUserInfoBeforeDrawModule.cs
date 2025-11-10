using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUpCheckUserInfoBeforeDrawModule : MonoBehaviour
{
    public GameObject body;

    [Space]
    public TextMeshProUGUI textWhatRewardGetting;
    
    [Space]
    public TextMeshProUGUI textMobileNumberOnPopUpCheckUserInfoBeforeDraw;
    public TextMeshProUGUI textUserMailAddressOnPopUpCheckUserInfoBeforeDraw;

    [Space]
    public GameObject[] parentsGiftImg; // 0 ≈€«√∏¥, 1 Ω∫∆‰º»

    public GameObject[] defaultGiftImgs; // 0 s1 ≈€«√∏¥, 1 s2 ≈€«√∏¥, 2 s3 ≈€«√∏¥

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateState(RewardModule rewardModule)
    {
        ActivateTargetGiftImg(rewardModule);

        SetTextWhatPrizeGetting(rewardModule.WhatReward);

        SetDatasOnPopUpCheckUserInfoBeforeDraw();
    }

    private void ActivateTargetGiftImg(RewardModule rewardModule)
    {
        AllDeActivateGiftImgs();

        if (rewardModule.IsSpecialPrize)
        {
            ActivateSpecialGiftImg();
        }
        else
        {
            ActivateDefaultGiftImg(rewardModule.StageTypeStr);
        }
    }

    protected virtual void AllDeActivateGiftImgs()
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

            parentsGiftImg[i].SetActive(false);
        }
    }

    private void ActivateSpecialGiftImg()
    {
        parentsGiftImg[1].SetActive(true);
    }

    protected void ActivateDefaultGiftImg(string stageType)
    {
        parentsGiftImg[0].SetActive(true);

        switch (stageType)
        {
            case "s1":
                {
                    defaultGiftImgs[0].SetActive(true);
                }
                break;
            case "s2":
                {
                    defaultGiftImgs[1].SetActive(true);
                }
                break;
            case "s3":
                {
                    defaultGiftImgs[2].SetActive(true);
                }
                break;
        }
    }

    public void SetTextWhatPrizeGetting(string prizeName)
    {
        textWhatRewardGetting.text = prizeName;

    }

    protected void SetDatasOnPopUpCheckUserInfoBeforeDraw()
    {
        this.textMobileNumberOnPopUpCheckUserInfoBeforeDraw.text = UserManager.Instance.GetMoblieNumber();
        this.textUserMailAddressOnPopUpCheckUserInfoBeforeDraw.text = UserManager.Instance.GetUserEmail();
    }

    public void UpdateUserMailText(string mailText)
    {
        this.textUserMailAddressOnPopUpCheckUserInfoBeforeDraw.text = mailText;
    }

    public string GetUpdatedUserEmailText()
    {
        return this.textUserMailAddressOnPopUpCheckUserInfoBeforeDraw.text;
    }

    public void ActivateBody()
    {
        this.body.SetActive(true);
    }

    public void DeActivateBody()
    {
        this.body.SetActive(false);
    }
}
