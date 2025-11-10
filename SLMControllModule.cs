using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SLMControllModule : MonoBehaviour
{
    private Action<int> whenSpecificLearningObjectiveSuccess = null;  // 학습 목표 수행 시
    private Action<string> callbackSettingSentenceInMiddleGuidePanel = null;
    private Action<string, float> callbackSettingSentenceInMiddleGuidePanelWithAlpha = null;
    private Action onClickSkipBtnOnMiddleGuidePanel = null;

    public void SetSpecificLearningObjectiveSuccessCallback(Action<int> specificLearningObjectiveSuccess)
    {
        this.whenSpecificLearningObjectiveSuccess = specificLearningObjectiveSuccess;
    }

    // SingleUnityEvent 등 참조될 것
    public void ExecuteSpecificLearningObjectiveSuccessCallback(int progressCount)
    {
        whenSpecificLearningObjectiveSuccess?.Invoke(progressCount);
    }

    #region old completeSLM
    //public void SetActionOnCompleteSmartLearningMode(Action actionOnCompleteSmartLearningMode)
    //{
    //    this.onCompleteSmartLearningMode = actionOnCompleteSmartLearningMode;
    //}

    //// 학습 완료 시 다시하기, 다음 강의로 이동, 나가기 버튼 포함된 패널 출력
    //public void ActivateSmartLearningCompletePanel()
    //{
    //    onCompleteSmartLearningMode?.Invoke();
    //}
    #endregion

    // 스마트학습 마지막 애니메이션 보여주고 끝나면 종료 패널 출력, DB에 value 넣기
    public void CompleteSmartLearning()
    {
        SmartLearningModeController.Instance.OnCompleteSmartLearning();
    }

    public void SetCallbackSettingSentenceInMiddleGuidePanel(Action<string> callbackSetSentenceInMiddleGuidePanel)
    {
        this.callbackSettingSentenceInMiddleGuidePanel = callbackSetSentenceInMiddleGuidePanel;
    }

    public void SetSentenceInMiddleGuidePanel(string guideLabelKey)
    {
        this.callbackSettingSentenceInMiddleGuidePanel?.Invoke(guideLabelKey);
    }

    public void SetActionOnClickSkipBtnOnMiddleGuidePanel(Action actionOnClickSkipBtnOnMiddleGuidePanel)
    {
        onClickSkipBtnOnMiddleGuidePanel = actionOnClickSkipBtnOnMiddleGuidePanel;
    }

    public void OnClickSkipBtnOnMiddleGuidePanel()
    {
        onClickSkipBtnOnMiddleGuidePanel?.Invoke();
    }
}
