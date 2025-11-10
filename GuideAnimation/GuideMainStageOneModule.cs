using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GuideMainStageOneModule : MonoBehaviour
{
    public GuideTemplate[] guideModules;
    private Queue<GuideTemplate> queueGuideModule = new Queue<GuideTemplate>();

    [Space]
    [Header("Main Btns -----")]
    public GameObject nextBtn;

    [SerializeField]
    private GuideTemplate currentGuideModule = null;

    private Action announceStageOneIsDoneCallback = null;

    private void OnDestroy()
    {
        this.announceStageOneIsDoneCallback = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        for (int i = 0; i < guideModules.Length; i++)
        {
            guideModules[i].SetTimeToLoadNextChapterCallback(LoadNextChapter);
            guideModules[i].SetTimeToShowNextBtn(ShowNextBtn);

            queueGuideModule.Enqueue(guideModules[i]);
        }

        SetFirstChapter();
    }


    private void SetFirstChapter()
    {
        currentGuideModule = queueGuideModule.Dequeue();

        currentGuideModule.LoadFirstStep();
    }

    private void LoadNextChapter()
    {
        if (queueGuideModule.Count > 0)
        {
            currentGuideModule = queueGuideModule.Dequeue();

            currentGuideModule.LoadFirstStep();
        }
        else
        {
            CustomDebug.Log("가이드 스테이지 1 종료, 스테이지 2 출력하기");

            this.gameObject.SetActive(false);

            this.announceStageOneIsDoneCallback?.Invoke();
        }
    }

    private void ShowNextBtn()
    {
        ActivateNextBtn();
    }

    private void ActivateNextBtn()
    {
        nextBtn.SetActive(true);
    }

    private void DeActivateNextBtn()
    {
        nextBtn.SetActive(false);
    }

    public void OnClickNextStepBtn()
    {
        DeActivateNextBtn();

        currentGuideModule.NextStep();
    }

    public void SetAnnounceStageOneIsDoneCallback(Action isDone)
    {
        this.announceStageOneIsDoneCallback = isDone;
    }
}
