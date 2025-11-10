using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UpperLineController : MonoBehaviour
{
    public SmartLearningModeController smartLearningModeController;
    public GameObject panelUpperLine;

    [Space]
    public GameObject btnBack;
    public GameObject btnFullScreen;

    [Space]
    [Header("Left Area -----")]
    public Button[] btnsLoginInfo;
    public RectTransform rtExtraFuncBtn;

    public float initYPosOfExtraFuncBtn; // 0
    public float endYPosOfExtraFuncBtn; // -70

    public Ease tweeningEase;

    [Space]
    [Header("Notice Area -----")]
    public NoticeModule noticeModule;

    [Space]
    public GameObject[] panelsHiddenExtraBtn;

    private bool isExtraFuncBtnMoved = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public virtual void OnClickBackwardBtn()
    {
        HomeController.Instance.OnClickBackwardBtn();
    }

    //public void OnClickBackwardBtnInSmartLearningMode()
    //{
    //    smartLearningModeController.ActivateSpecificPopUp(EnumSets.SLMPopUpType.AskingGoBackToHomeScene);
    //}

    //public void OnClickFullScreenBtn()
    //{
    //    Debug.Log("Try OnClickFulScreenBtn >>> ");

    //    smartLearningModeController.SetFullScreenMode(); // 스마트학습 모드에서만 풀스크린 버튼이 존재하기로 함

    //    // HomeController.Instance.SetFullScreenMode();
    //}

    //public void OnClickExitFullScreenBtn()
    //{
    //    smartLearningModeController.SetOrdinaryScreenMode();

    //    // HomeController.Instance.SetOrdinaryScreenMode();
    //}

    public virtual void OnClickUserIdBtn()
    {
        HomeController.Instance.OnClickUserIdBtn();
    }

    public virtual void OnClickExitBtnInLogOutPanel()
    {
        HomeController.Instance.OnClickExitBtnInLogOutPanel();
    }

    public virtual void OnClickExitBtn()
    {
        HomeController.Instance.ActivatePopUp(EnumSets.PopUpType.Exit);
    }

    //public void OnClickExitBtnInSmartLearningMode()
    //{
    //    smartLearningModeController.ActivateSpecificPopUp(EnumSets.SLMPopUpType.ExitWebGL);
    //}

    public virtual void ActivatePanelUpperLine()
    {
        this.panelUpperLine.SetActive(true);
    }

    public virtual void DeActivatePanelUpperLine()
    {
        this.panelUpperLine.SetActive(false);
    }

    public void ActivateBackwardBtn()
    {
        if (!btnBack.activeSelf)
        {
            btnBack.SetActive(true);
        }
    }

    public void DeActivateBackwardBtn()
    {
        btnBack.SetActive(false);
    }

    public void ActivateFullScreenBtn()
    {
        this.btnFullScreen.SetActive(true);
    }

    public void DeActivateFullScreenBtn()
    {
        this.btnFullScreen.SetActive(false);
    }

    //-------------------------------------------
   
    // 버튼에 참조
    public void OnClickLoginInfoBtn()
    {
        SetLoginBtnDisable();

        if (!isExtraFuncBtnMoved)
        {
            MoveLowerExtraFuncBtn();
        }
        else
        {
            MoveUpperExtraFuncBtn();
        }
    }

    private void SetLoginBtnDisable()
    {
        for (int i = 0; i < btnsLoginInfo.Length; i++)
        {
            btnsLoginInfo[i].enabled = false;
        }
    }

    private void SetLoginBtnEnable()
    {
        for (int i = 0; i < btnsLoginInfo.Length; i++)
        {
            btnsLoginInfo[i].enabled = true;
        }
    }

    private void MoveLowerExtraFuncBtn()
    {
        this.rtExtraFuncBtn.DOAnchorPosY(endYPosOfExtraFuncBtn, 0.5f, true).SetEase(tweeningEase).OnComplete(() => {

            this.isExtraFuncBtnMoved = true;

            SetLoginBtnEnable();

            ActivatePanelsHiddenExtraBtn();
        });
    }

    public void MoveUpperExtraFuncBtn()
    {
        DeActivatePanelsHiddenExtraBtn();

        this.rtExtraFuncBtn.DOAnchorPosY(initYPosOfExtraFuncBtn, 0.5f, true).SetEase(tweeningEase).OnComplete(() => {

            this.isExtraFuncBtnMoved = false;

            SetLoginBtnEnable();

        });
    }

    private void ActivatePanelsHiddenExtraBtn()
    {
        for (int i = 0; i < panelsHiddenExtraBtn.Length; i++)
        {
            panelsHiddenExtraBtn[i].SetActive(true);
        }
    }

    private void DeActivatePanelsHiddenExtraBtn()
    {
        for (int i = 0; i < panelsHiddenExtraBtn.Length; i++)
        {
            panelsHiddenExtraBtn[i].SetActive(false);
        }
    }

}
