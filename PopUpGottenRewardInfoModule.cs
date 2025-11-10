using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class PopUpGottenRewardInfoModule : MonoBehaviour, IPopUp
{
    public GameObject body;

    [Space]
    public TextMeshProUGUI textWhereOnPopUpGottenRewardInfo;
    public TextMeshProUGUI textWhatRewardGottenOnPopUpGottenRewardInfo;

    [Space]
    public TextMeshProUGUI textNotice;

    [Space]
    public GameObject[] parentsGiftImg; // 0 템플릿, 1 스페셜

    public GameObject[] defaultGiftImgs; // 0 s1 템플릿, 1 s2 템플릿, 2 s3 템플릿

    [Space]
    public GameObject btnsBottomOnPopUpGottenRewardInfo; // resend, confirm

    [Space]
    public Button btnResend;
    public GameObject objBtnSendComplete;

    private const string NORMAL_TEXT = "※ 아래 이메일로 발송되었습니다! ※";
    private const string SPECIAL_TEXT = "※ 아래 이메일로 일련번호가 발송되었습니다! ※";


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateState(RewardModule rewardModule)
    {
        ActivateTargetGiftImg(rewardModule);

        SetTextWhatPrizeGotten(rewardModule.WhatReward);
        SetTextWhereOnPopUpGottenRewardInfo(rewardModule.TargetEmailAddress);

        SetNoticeText(rewardModule);
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
            if(i == 0)
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

    protected void SetTextWhereOnPopUpGottenRewardInfo(string target)
    {
        textWhereOnPopUpGottenRewardInfo.text = target;
    }

    protected void SetTextWhatPrizeGotten(string prizeName)
    {
        textWhatRewardGottenOnPopUpGottenRewardInfo.text = prizeName;
    }

    private void SetNoticeText(RewardModule rewardModule)
    {
        if (rewardModule.IsSpecialPrize)
        {
            this.textNotice.text = SPECIAL_TEXT;
        }
        else
        {
            this.textNotice.text = NORMAL_TEXT;
        }
    }

    public void ActivatePopUp()
    {
        body.SetActive(true);

        ActivateBtnsOnPopUpGottenRewardInfo();
    }

    public void DeActivatePopUp()
    {
        body.SetActive(false);

        DeActivateBtnsOnPopUpGottenRewardInfo();

        ActivateResendBtn();
    }

    private void ActivateBtnsOnPopUpGottenRewardInfo()
    {
        btnsBottomOnPopUpGottenRewardInfo.SetActive(true);
    }

    private void DeActivateBtnsOnPopUpGottenRewardInfo()
    {
        btnsBottomOnPopUpGottenRewardInfo.SetActive(false);
    }

    public void DeActivateResendBtn()
    {
        // btnResend.interactable = false;
        btnResend.gameObject.SetActive(false);

        objBtnSendComplete.SetActive(true);
    }

    private void ActivateResendBtn()
    {
        // btnResend.interactable = true;

        btnResend.gameObject.SetActive(true);

        objBtnSendComplete.SetActive(false);
    }
}
