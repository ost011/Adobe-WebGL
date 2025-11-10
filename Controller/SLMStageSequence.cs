using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class SLMStageSequence : MonoBehaviour
{
    // public UISprite spriteMainBG;

    public GameObject[] prZeroSequence;
    private Queue<GameObject> queuePRZero = new Queue<GameObject>();

    public GameObject[] prOneSequence;
    private Queue<GameObject> queuePROne = new Queue<GameObject>();

    public GameObject[] prTwoSequence;
    private Queue<GameObject> queuePRTwo = new Queue<GameObject>();

    public GameObject[] prThreeSequence;
    private Queue<GameObject> queuePRThree = new Queue<GameObject>();

    public GameObject[] prFourSequence;
    private Queue<GameObject> queuePRFour = new Queue<GameObject>();

    [Space]
    public SLMControllModule slmControllModule;

    protected Queue<GameObject> queueSpecific = new Queue<GameObject>(); // 각 시퀸스별로 큐를 가지고있음
    protected Queue<Queue<GameObject>> queuePR = new Queue<Queue<GameObject>>(); // 시퀸스 큐들을 가지고있음

    protected GameObject preStep = null;
    protected GameObject currentStep = null;

    [SerializeField]
    protected int achieveSubjectCount = -1; // 달성한 학습목표 개수

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Init();
        InitQueue();
    }

    private void Init()
    {
        // Debug.Log("parent init");
        // language = LearningModeController.Instance.GetCurrentLanguage();

        SetMainBGAltas();
    }

    private void InitQueue()
    {
        // Debug.Log("InitQueue === ");

        for (int i = 0; i < prZeroSequence.Length; i++)
        {
            queuePRZero.Enqueue(prZeroSequence[i]);
        }
        queuePR.Enqueue(queuePRZero);

        for (int i = 0; i < prOneSequence.Length; i++)
        {
            queuePROne.Enqueue(prOneSequence[i]);
        }
        queuePR.Enqueue(queuePROne);

        for (int i = 0; i < prTwoSequence.Length; i++)
        {
            queuePRTwo.Enqueue(prTwoSequence[i]);
        }
        queuePR.Enqueue(queuePRTwo);

        for (int i = 0; i < prThreeSequence.Length; i++)
        {
            queuePRThree.Enqueue(prThreeSequence[i]);
        }
        queuePR.Enqueue(queuePRThree);

        for (int i = 0; i < prFourSequence.Length; i++)
        {
            queuePRFour.Enqueue(prFourSequence[i]);
        }
        queuePR.Enqueue(queuePRFour);

        SetSpecificSequence(() => 
        {
            ShowNextStep();
        });
    }

    private void SetMainBGAltas()
    {
        // Debug.Log("SetMainBGAltas");
        // spriteMainBG.atlas = LearningModeController.Instance.GetCurrentAtlas();
    }

    protected void LoadSequence(GameObject gameObject)
    {
        LoadNextStep();
    }

    public void LoadNextStep()
    {
        // Debug.Log("Load Next Step~~~");

        SetPreStep();

        ShowNextStep();
    }

    protected void ShowNextStep()
    {
        if (currentStep != null)
        {
            currentStep.SetActive(false);
        }

        if (queueSpecific.Count > 0)
        {
            currentStep = queueSpecific.Dequeue();

            currentStep.SetActive(true);

            DeActivePreStep();

            CheckIsAchievedGoal();
        }
        else
        {
            DeActivePreStep();

            SetSpecificSequence(() => {

                ShowNextStep();
            });
        }
    }

    protected void CheckIsAchievedGoal()
    {
        try
        {
            if (queueSpecific.Count == 0)
            {
                // 마지막 스텝을 띄운 상태 -> 해당 학습목표를 달성했다라는 의미
                this.slmControllModule.ExecuteSpecificLearningObjectiveSuccessCallback(this.achieveSubjectCount);
            }
        }
        catch(Exception e)
        {
            CustomDebug.LogError($"CheckAchieveSubject Error : {e.Message} / where : {this.gameObject.transform.name}");
        }
    }

    protected void SetPreStep()
    {
        if (currentStep != null)
        {
            preStep = currentStep;

            // currentStep.SetActive(false);
            currentStep = null;
        }
    }

    protected void DeActivePreStep()
    {
        preStep?.SetActive(false);
        preStep = null;
    }

    protected virtual void SetSpecificSequence(Action isReady = null)
    {
        CustomDebug.LogWithColor("parent SetSpecificSequence", CustomDebug.ColorSet.Yellow);

        if (queuePR.Count > 0)
        {
            queueSpecific = queuePR.Dequeue();

            // 메인큐에서 타겟 시퀀스를 가져올때마다 달성 목표 카운트를 1 올린다
            // 타겟 시퀀스를 완료했을 때 어떤 학습목표를 띄워야할 지 결정하기위함
            achieveSubjectCount++; 

            isReady?.Invoke();
        }
        else
        {
            CustomDebug.Log("parent queuePr empty, End of SLM");

            CompleteSmartLearningMode();
        }
    }

    /// <summary>
    /// 줌인-아웃시 NGUI 슬라이더 - Thumb가 인식이 안되는 문제있음, 임시방편으로 줌인-아웃할때마다 Thumb 오브젝트를 껐다 켜는 것으로 갱신하도록함
    /// </summary>
    public virtual void ResetThumbObject()
    {
        // 껐다 켜면 Thumb의 박스콜라이더 2D를 인식함 /
        // Thumb 상위 Slider 에 박스콜라이더 2D를 넣어서 테스트해봤으나 터치하는 순간 value 값이 1이 되어버림
        // CustomDebug.Log("parent ResetThumb");
    }

    protected void CompleteSmartLearningMode()
    {
        slmControllModule.CompleteSmartLearning();
    }
}
