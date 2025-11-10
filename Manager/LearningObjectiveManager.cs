using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LearningObjectiveManager : MonoBehaviour
{
    public SmartLearningModeUIManager smartLearningModeUIManager;

    private string[] arrayLearningObjective = null;
    private int currentProgressIndex = 0;
    private WaitForSeconds delayDeActivateTextbox = new WaitForSeconds(5f);
    private IEnumerator enumerator = null;

    private const float PROGRESS_BOX_MOVE_AMOUNT_FIRST = 211f;
    private const float PROGRESS_BOX_MOVE_AMOUNT_NORMAL = 169f;
    private const float PROGRESS_BOX_MOVE_AMOUNT_LAST = 200f;
    private const float PROGRESS_GAUGE_MOVE_AMOUNT_NORMAL = 169f;
    // private const float PROGRESS_GAUGE_MOVE_AMOUNT_LAST = 131f;


    public void InitializeLearningObjectiveInfo(LearningModeInfo learningModeInfo)
    {
        arrayLearningObjective = learningModeInfo.arryLearningObjective;
    }

    public void LoadNextLearningObjective(int progressIndex)
    {
        // 기획서 상, 학습목표 최종 달성이후로 진행해야할 것들이 있는 경우를 대비
        // 해당 경우의 레슨은 완전 달성시 progressIndex를 -1 로 전달한다
        if (progressIndex == -1)
        {
            return;
        }

        this.currentProgressIndex = progressIndex;

        MoveProgressBox();

        UpdateProgressGauge();

        UpdateLearningObjectiveText();
    }

    private void UpdateLearningObjectiveText()
    {
        smartLearningModeUIManager.UpdateLearningObjectiveText(arrayLearningObjective[currentProgressIndex]);
    }

    private void MoveProgressBox()
    {
        var moveAmount = GetProgressBoxMoveAmount();

        DeActivateLearningObjectiveTextbox();

        smartLearningModeUIManager.MoveProgressBox(moveAmount, () => 
        {
            ActivatePillarLight();

            ActivateProgressBatteryGauge();

            ActivateLearningObjectiveTextbox();

            // DeActivateLearningObjectiveTextboxAfterDelay();
        });
    }

    private float GetProgressBoxMoveAmount()
    {
        float moveAmount;

        if (currentProgressIndex == arrayLearningObjective.Length - 1)
        {
            // 마지막 학습목표 성공
            moveAmount = PROGRESS_BOX_MOVE_AMOUNT_LAST;
        }
        else
        {
            if (currentProgressIndex == 0)
            {
                moveAmount = PROGRESS_BOX_MOVE_AMOUNT_FIRST;
            }
            else
            {
                moveAmount = PROGRESS_BOX_MOVE_AMOUNT_NORMAL;
            }
        }

        return moveAmount;
    }

    private void UpdateProgressGauge()
    {
        smartLearningModeUIManager.UpdateProgressGauge(PROGRESS_GAUGE_MOVE_AMOUNT_NORMAL, currentProgressIndex);
    }

    public void ActivatePillarLight()
    {
        smartLearningModeUIManager.ActivatePillarLight(currentProgressIndex);
    }

    public void ActivateProgressBatteryGauge()
    {
        smartLearningModeUIManager.ActivateProgressBatteryGauge(currentProgressIndex);
    }

    public void ActivateLearningObjectiveTextbox()
    {
        smartLearningModeUIManager.ActivateLearningObjectiveTextbox();
    }

    public void DeActivateLearningObjectiveTextbox()
    {
        smartLearningModeUIManager.DeActivateLearningObjectiveTextbox();
    }

    // 일정 시간 이후 자동 비활성화, 매 학습목표 업데이트마다 실행될 것
    private void DeActivateLearningObjectiveTextboxAfterDelay()
    {
        if(enumerator != null)
        {
            StopCoroutine(enumerator);

            enumerator = null;
        }

        enumerator = CorDeActivateLearningObjectiveTextboxAfterDelay();

        StartCoroutine(enumerator);
    }

    private IEnumerator CorDeActivateLearningObjectiveTextboxAfterDelay()
    {
        yield return delayDeActivateTextbox;

        DeActivateLearningObjectiveTextbox();
    }

    public void ResetData()
    {
        currentProgressIndex = 0;

        arrayLearningObjective = null;
    }
}
