using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperLineControllerInSLMMode : UpperLineController
{
    [Space]
    [Header("전체화면 됐을 때 사라져야할 상단 뷰")]
    public GameObject[] panelsUpperLine;


    // 전체화면 버튼에 참조되어 있음
    public void OnClickFullScreenBtn()
    {
        SmartLearningModeController.Instance.SetFullScreenMode();
    }

    public void OnClickExitFullScreenBtn()
    {
        SmartLearningModeController.Instance.SetOrdinaryScreenMode();
    }

    public override void OnClickBackwardBtn()
    {
        SmartLearningModeController.Instance.ActivateSpecificPopUp(EnumSets.SLMPopUpType.AskingGoBackToHomeScene);
    }

    public override void OnClickExitBtn()
    {
        // SmartLearningModeController.Instance.ActivateSpecificPopUp(EnumSets.SLMPopUpType.ExitWebGL);
        SmartLearningModeController.Instance.ActivateExitWebGLPopUp();
    }

    public override void OnClickUserIdBtn()
    {
        SmartLearningModeController.Instance.OnClickUserIdBtn();
    }

    public override void OnClickExitBtnInLogOutPanel()
    {
        SmartLearningModeController.Instance.OnClickExitBtnInLogOutPanel();
    }

    public override void ActivatePanelUpperLine()
    {
        foreach(var panel in panelsUpperLine)
        {
            panel.SetActive(true);
        }
    }

    public override void DeActivatePanelUpperLine()
    {
        foreach (var panel in panelsUpperLine)
        {
            panel.SetActive(false);
        }
    }
}
