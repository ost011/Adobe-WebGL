using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class StageModule : MonoBehaviour
{
    public EnumSets.StageType stageType;
    private string stageTypeStr = "";
    private int stageIndex = 0;

    [Space]
    public Image imgBigProgress;
    public Image imgSmallProgress;
    public Transform progressArrow;
    public TextMeshProUGUI textProgress;

    [Space]
    public GameObject currentTryingHalo; // 현재 시도 중인 스테이지 halo
    [Space]
    public GameObject[] stageStates; // lock, complete, trying

    [Space]
    public Button btnStageOpen;

    private const float PROGRESS_ARROW_DEGREE = 89f;

    // Start is called before the first frame update
    void Start()
    {
        // 진행률은 테이블을 통해서 확인
        // lock, 진행여부는 이전 stage 테이블의 완료 개수를 보고 확인하기

        Init();
    }

    private void Init()
    {
        this.stageTypeStr = this.stageType.ToString().ToLower();

        this.stageIndex = Convert.ToInt32(this.stageType);

        CheckProgress();
        CheckStageState();
    }

    public void CheckProgress()
    {
        try
        {
            var stageCompleteTable = UserManager.Instance.GetSpecificStageCompletedInfo(this.stageTypeStr);

            if (stageCompleteTable == null)
            {
                throw new NullReferenceException("target stage complete table is null");
            }
           
            if(StageLessonDataManager.Instance.IsCompletedStageActually(this.stageIndex))
            {
                SetProgressValue(1);
            }
            else
            {
                var currentStageCompleteCount = GetActualCompleteCount(stageCompleteTable);

                var totalStageCompleteCount = StageLessonDataManager.Instance.GetStageTotalCount(this.stageIndex);

                var progressValue = (float)currentStageCompleteCount / totalStageCompleteCount;

                SetProgressValue(progressValue);
            }

        }
        catch(Exception e)
        {
            CustomDebug.LogWithColor($"{stageType}, no Show Progress", CustomDebug.ColorSet.Red);

            SetProgressValue(0);
        }
    }

    private int GetActualCompleteCount(Dictionary<string, int> stageCompleteTable)
    {
        var completeCount = 0;

        var datas = from data in stageCompleteTable
                    where data.Value == 1
                    select data;

        completeCount = datas.Count();

        return completeCount;
    }

    private void SetProgressValue(float value)
    {
        // ex) 7 / 10 의 값이 넘어올 예정

        imgBigProgress.fillAmount = value;
        imgSmallProgress.fillAmount = value;

        textProgress.text = (value * 100).ToString("F0");

        var degree = (-2 * PROGRESS_ARROW_DEGREE * value) + PROGRESS_ARROW_DEGREE;

        progressArrow.localEulerAngles = Vector3.forward * degree; // 86 (0) ~ -86 (100)
    }

    private void CheckStageState()
    {
        try
        {
            var preStageIndex = this.stageIndex - 1;

            if(preStageIndex == -1)
            {
                throw new IndexOutOfRangeException("this is 1st stage");
            }

            // s1이 아닌 상태

            AllDeActivateStageStates();
            
            HideHalo();

            var stageCompleteTable = UserManager.Instance.GetSpecificStageCompletedInfo(this.stageTypeStr);

            if (stageCompleteTable == null)
            {
                // 해당 스테이지를 완료한 기록은 없다
                // 해당 스테이지를 진행 중인 지 확인이 필요함
                // 이전 스테이지를 완료했는가를 보고 해당 스테이지 진행 중인지 파악 가능

                if (StageLessonDataManager.Instance.IsCompletedStage(preStageIndex))
                {
                    // 이전 스테이지를 완료했다는 것
                    UpdateStageState(EnumSets.StageState.Trying);

                    ActivateStageOpenBtn();

                    ShowHalo();
                }
                else
                {
                    UpdateStageState(EnumSets.StageState.Lock);
                }
            }
            else
            {
                if (StageLessonDataManager.Instance.IsCompletedStageActually(this.stageIndex))
                {
                    // 해당 스테이지를 완료했다는 것
                    UpdateStageState(EnumSets.StageState.Complete);
                }
                else
                {
                    UpdateStageState(EnumSets.StageState.Trying);

                    ShowHalo();
                }
                    
                ActivateStageOpenBtn();
            }
        }
        catch (IndexOutOfRangeException e)
        {
            // s1 인 상태이다 , s1은 제일 첫 스테이지이므로 lock 이 없다
            // s1 은 항상 스테이지 진입버튼이 활성화되어있다

           // CustomDebug.LogWithColor($"CheckStageState, IndexOutOfRangeException - {this.stageType} / {e.Message}", CustomDebug.ColorSet.Magenta);

            AllDeActivateStageStates();

            HideHalo();

            var stageCompleteTable = UserManager.Instance.GetSpecificStageCompletedInfo(this.stageTypeStr);

            if (stageCompleteTable == null)
            {
                UpdateStageState(EnumSets.StageState.Trying);

                ShowHalo();
            }
            else
            {
                if(StageLessonDataManager.Instance.IsCompletedStageActually(this.stageIndex))
                {
                    UpdateStageState(EnumSets.StageState.Complete);
                }
                else
                {
                    UpdateStageState(EnumSets.StageState.Trying);

                    ShowHalo();
                }
            }
        }
    }

    private void ActivateStageOpenBtn()
    {
        btnStageOpen.interactable = true;
    }

    private void AllDeActivateStageStates()
    {
        for (int i = 0; i < stageStates.Length; i++)
        {
            stageStates[i].SetActive(false);
        }
    }

    private void ShowHalo()
    {
        this.currentTryingHalo.SetActive(true);
    }

    private void HideHalo()
    {
        this.currentTryingHalo.SetActive(false);
    }

    private void UpdateStageState(EnumSets.StageState stageState)
    {
        GameObject target = null;

        switch(stageState)
        {
            case EnumSets.StageState.Lock:
                {
                    target = stageStates[0];
                }
                break;
            case EnumSets.StageState.Complete:
                {
                    target = stageStates[1];
                }
                break;
            case EnumSets.StageState.Trying:
                {
                    target = stageStates[2];
                }
                break;

        }

        if(target != null)
        {
            target.SetActive(true);
        }
    }
}
