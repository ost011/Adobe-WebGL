using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SLMPsS3L6 : SLMStageSequence
{
    public GameObject[] additionalSequence;
    private Queue<GameObject> queueAdditional = new Queue<GameObject>();

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        Init();
    }

    private void Init()
    {
        for (int i = 0; i < additionalSequence.Length; i++)
        {
            queueAdditional.Enqueue(additionalSequence[i]);
        }
    }

    protected override void SetSpecificSequence(Action isReady = null)
    {
        CustomDebug.LogWithColor("child SetSpecificSequence", CustomDebug.ColorSet.Cyan);

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
            CustomDebug.LogWithColor("child queuePr empty", CustomDebug.ColorSet.Magenta);

            achieveSubjectCount = -1;

            ChangeSequenceToAdditionalSequence();
        }
    }

    private void ChangeSequenceToAdditionalSequence()
    {
        if (queueAdditional.Count > 0)
        {
            CustomDebug.Log("queueAdditional count morethan 0,queueSpecific = queueAdditional >>> ------");

            queueSpecific = queueAdditional;

            LoadNextStep();
        }
        else
        {
            CustomDebug.Log("queue count is 0, End of SLM, show complete view ------");

            CompleteSmartLearningMode();
        }
    }
}
