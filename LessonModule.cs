using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LessonModule : MonoBehaviour
{
    public Button button;

    [Space]
    public GameObject[] stateMarks; // 0 completeMark, 1 skipMark, 2 lockMark 
    public GameObject haloMark; // 지금 이 레슨을 수행할 차례 표시

    [Space]
    public EnumSets.StageType stageType;

    [Space]
    public EnumSets.LessonType lessonType;

    [Space]
    public TextMeshProUGUI lessonTitle;

    [Space]
    public SpecialRewardModule specialRewardModule;

    private string stageTypeStr = "";
    public string StageTypeStr => stageTypeStr;

    private string lessonTypeStr = "";
    public string LessonTypeStr => this.lessonTypeStr;

    private string lessonTitleStr = "";

    private Action<string[]> recentTyringLectureEvent = null;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void SetRecentTryingLectureEvent(Action<string[]> recentTyringLectureEvent)
    {
        this.recentTyringLectureEvent = recentTyringLectureEvent;
    }

    public void ActivateCompleteMark()
    {
        this.stateMarks[0].SetActive(true);
    }

    public void ActivateSkipMark()
    {
        this.stateMarks[1].SetActive(true);
    }

    public void SetMarkState(int completeState)
    {
        DeActivateAllStateMarks();

        if (completeState == 1) // 완료
        {
            ActivateCompleteMark();
        }
        else
        {
            if(completeState == -1) // 스킵
            {
                ActivateSkipMark();
            }
        }
    }

    public void EnableBtn()
    {
        this.button.interactable = true;

        // CustomDebug.LogWithColor($"EnableBtn, {this.button.interactable}", CustomDebug.ColorSet.Cyan);
    }

    public void DeActivateAllStateMarks()
    {
        for (int i = 0; i < stateMarks.Length; i++)
        {
            stateMarks[i].SetActive(false);
        }
    }

    public void ActivateHaloMark()
    {
        this.haloMark.SetActive(true);
    }

    public void DeActivateHaloMark()
    {
        this.haloMark.SetActive(false);
    }

    public void ActivateLockMark()
    {
        this.stateMarks[2].SetActive(true);
    }

    public void SetStageType(EnumSets.StageType stageType)
    {
        this.stageType = stageType;
    }

    public void Init()
    {
        this.stageTypeStr = stageType.ToString().ToLower();
        this.lessonTypeStr = lessonType.ToString().ToLower();

        // CustomDebug.Log($"lesson module init, {this.stageTypeStr} / this.lessonTypeStr : {this.lessonTypeStr}");

        this.lessonTitleStr = this.lessonTitle.text;
    }

    public void OnClickLessonBtn()
    {
        var lessonInfos = new string[] { this.stageTypeStr, this.lessonTypeStr, this.lessonTitleStr };

        this.recentTyringLectureEvent?.Invoke(lessonInfos);

        HomeController.Instance.LoadSLMLesson(lessonInfos);
    }

    public void SetSpecialPrizeItemPos(string prizeItemInfoId, PrizeItemInfo prizeItemInfo)
    {
         this.specialRewardModule.SetSpecialPrizeItemPos(prizeItemInfoId, prizeItemInfo);
    }
}
