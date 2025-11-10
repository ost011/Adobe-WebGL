using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendedMiddleEndGuideManagerForReAsk : MiddleEndGuideManager
{
    // 포토샵 기획서 10강(s2l1) 에서 퀴즈 오답시 띄울 미들가이드 재사용을 위함
    // 미들가이드가 꺼질 때 enabled = false 가 되는 것 삭제

    // Start is called before the first frame update
    protected override void OnEnable()
    {
        Init();
    }

    protected override void Init()
    {
        CheckMiddleGuideXPos();
        CheckMiddleGuideYPos();

        Invoke(nameof(DynamicLoadGuidePanel), 0.02f); // 연속해서 나올 때 출력 안되는 문제 방지용

        slmControllModule.SetActionOnClickSkipBtnOnMiddleGuidePanel(() =>
        {
            onSkipMiddleGuideFunc?.Invoke();
        });
    }

    protected override void DeActivateMiddleGuidePanelOnDisable()
    {
        if (!IsBubbleSkippableType())
        {
            DeActivateMiddleGuidePanelWithItsComponents();
        }
    }
}
