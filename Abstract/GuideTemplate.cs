using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class GuideTemplate : MonoBehaviour
{
    public GameObject[] sequences;
    protected Queue<GameObject> queueSequence = new Queue<GameObject>();

    protected GameObject preStep = null;
    protected GameObject currentStep = null;

    private Action timeToLoadNextChapter = null;
    private Action timeToShowNextBtn = null;

    protected virtual void Start()
    {
        Init();
    }

    private void Init()
    {
        for (int i = 0; i < sequences.Length; i++)
        {
            queueSequence.Enqueue(sequences[i]);
        }
    }

    protected void SetCurrentStep()
    {
        if(queueSequence.Count > 0)
        {
            currentStep = queueSequence.Dequeue();
        }
    }

    protected void SetPreStep()
    {
        preStep = currentStep;
    }

    protected void HidePreStep()
    {
        if(preStep != null)
        {
            preStep.SetActive(false);

            preStep = null;
        }
    }

    protected void ShowCurrentStep()
    {
        if(this.currentStep != null)
        {
            this.currentStep.SetActive(true);
        }
    }
    
    protected bool IsReadyToShowLocalSequence()
    {
        if(this.queueSequence.Count > 0)
        {
            return true;
        }

        return false;
    }

    public virtual void NextStep()
    {
        if (IsReadyToShowLocalSequence())
        {
            LoadNextStep();
        }
        else
        {
            HidePreStep();

            this.timeToLoadNextChapter?.Invoke();
        }
    }

    protected void LoadNextStep()
    {
        HidePreStep();

        SetCurrentStep();

        ShowCurrentStep();

        SetPreStep();
    }

    public void SetTimeToLoadNextChapterCallback(Action callback)
    {
        this.timeToLoadNextChapter = callback;
    }

    public void SetTimeToShowNextBtn(Action callback)
    {
        this.timeToShowNextBtn = callback;
    }

    // 외부에서 참조 중
    public void ShowNextBtn()
    {
        timeToShowNextBtn?.Invoke();
    }

    public virtual void LoadFirstStep()
    {
        SetCurrentStep();

        ShowCurrentStep();

        SetPreStep();
    }

}