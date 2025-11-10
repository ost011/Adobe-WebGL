using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipMiddleEndGuideManager : MiddleEndGuideManager
{
    public RectTransform targetTransform;  // 해당 트랜스폼의 위치에 말풍선 꼬다리가 위치함

    private int xPos = 0;
    private int yPos = 0;

    private const float X_POS_OFFSET = -76f;  // 가이드 버블 최좌측에서 말풍선 꼬다리까지 ~= 76
    private const float Y_POS_OFFSET = 10f;


    protected override void OnEnable()
    {
        Init();
    }

    protected override void Init()
    {
        CheckMiddeGuidePos();

        Invoke(nameof(DynamicLoadGuidePanel), 0.02f); // 연속해서 나올 때 출력 안되는 문제 방지용

        slmControllModule.SetActionOnClickSkipBtnOnMiddleGuidePanel(() =>
        {
            onSkipMiddleGuideFunc?.Invoke();
        });
    }

    protected override void DynamicLoadGuidePanel()
    {
        if (middleGuideBubblePropertyChanger == null)
        {
            middleGuideBubblePropertyChanger = gameObject.AddComponent<MiddleGuideBubblePropertyChanger>();
        }

        middleGuideBubblePropertyChanger.SetGuideBubbleProperty(middleGuideBubbleType);

        SetSentenceInMiddleGuidePanel(guideLabelKey);

        ActivateTipMiddleGuidePanelWithPos(xPos, yPos);
    }

    private void ActivateTipMiddleGuidePanelWithPos(int middleGuideXPos, int middleGuideYPos)
    {
        // SmartLearningModeController.Instance.ActivateTipMiddleGuidePanelWithPos(middleGuideXPos, middleGuideYPos);
        SmartLearningModeController.Instance.ActivateMiddleGuidePanelWithPos(middleGuideXPos, middleGuideYPos);
    }

    private void CheckMiddeGuidePos()
    {
        xPos = Convert.ToInt32(targetTransform.localPosition.x + X_POS_OFFSET);
        yPos = Convert.ToInt32(targetTransform.localPosition.y + Y_POS_OFFSET);
    }
}
