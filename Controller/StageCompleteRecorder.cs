using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;


public struct DashboardData
{
    public string program;
    public string key;  // lessonCompleteCount, firstCompleteStageCount/stageType, playingTime, allStageComplete
    public float playingTime; // 플레이타임, 분
}

public struct StudyData
{
    public string program;
    public string stageType;
    public string lessonType;
    public float playingTime; // 플레이타임, 분
    public bool isFirstComplete; // 강의 첫 클리어했는가
    public bool isFirstSkip;  // 현재 레슨을 스킵한게 처음인가
    public bool isFirstCompleteWhichSkipped; // 스킵했던 강의를 클리어했는가
}

public class StageCompleteRecorder : MonoBehaviour
{
    private string stageType = "";
    private string lessonType = "";
    private int stageValue = 0;
    private int lessonValue = 0;
    
    private int playingTimeSec = 0;
    // private int lastPlayingTimeSec = 0; // 스킵같은 경우 바로 리셋이 되기 때문에 따로 저장
    private bool isRecordingPlayingTime = false;
    private CancellationTokenSource cancellationTokenSource = null;

    private StringBuilder sb = new StringBuilder();

    private Action onFailedSettingDBValue = null;

    private enum CurrentLessonCompleteState
    {
        AlreadyPlayed,
        Skipped,
        NotFirstTimePlayingCurrentStageButCurrentLessonIsFirstTime,
        FirstTimePlayingCurrentStage,
        None
    }

    // 레슨이 시작될 때 한번만 체크함
    private CurrentLessonCompleteState currentLessonCompleteState = CurrentLessonCompleteState.None;

    private const int STAGE_COMPLETE_DB_VALUE = 1;
    private const int STAGE_SKIP_DB_VALUE = -1;
    private const string STAGE_COMPLETED_INFOS_PATH_STR = "stageCompletedInfos";

    private const string KEY_DASHBOARD_LESSON_COMPLETE = "lessonCompleteCount";
    private const string KEY_DASHBOARD_FIRST_COMPLETE_STAGE = "firstCompleteStageCount";
    private const string KEY_DASHBOARD_PLAYING_TIME = "playingTime";
    private const string KEY_DASHBOARD_ALL_STAGE_COMPLETE = "allStageComplete";


    public void SetDBValueOnCompleteLesson(Action onFailed = null)
    {
        if (IsCurrentLessonNeedToSetDBValueOnComplete())
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            onFailedSettingDBValue = onFailed;

            SetLessonCompleteValueToFirebaseDB(STAGE_COMPLETE_DB_VALUE, SetLocalDBValueOnCompleteSpecificLesson);
#elif UNITY_EDITOR
            SetLocalDBValueOnCompleteSpecificLesson();
#endif
        }
        else
        {
            CustomDebug.Log("No need to Set DB Value on Complete, so do nothing");
        }
    }

    public void SetLessonCompleteValueToFirebaseDB(int value, Action isReady = null)
    {
        try
        {
            var subPath = GetDBSubPath();

            var fullPathStr = DevUtil.Instance.GetTargetPath(EnumSets.DBParentType.Users, subPath);

            FirebaseDatabaseController.Instance.SetTagetData(fullPathStr, value.ToString(), (isSuccess) =>
            {
                if (isSuccess == 1)
                {
                    isReady?.Invoke();
                }
                else
                {
                    throw new Exception();
                }
            });
        }
        catch (Exception e)
        {
            CustomDebug.LogError($"error seting FireBaseDB On Complete SLM : {e.Message}");

            onFailedSettingDBValue?.Invoke();

            onFailedSettingDBValue = null;
        }
    }

    public void SetLocalDBValueOnCompleteSpecificLesson()
    {
        try
        {
            AddDataInStageCompletedInfo(STAGE_COMPLETE_DB_VALUE);
        }
        catch (Exception e)
        {
            CustomDebug.LogError($"error seeint localDB On Complete SLM : {e.Message}");

            onFailedSettingDBValue?.Invoke();

            onFailedSettingDBValue = null;
        }
    }

    public void SetDBValueOnSkipLesson(Action onCompleteSettingDBValue = null, Action onFailed = null)
    {
        if (IsCurrentLessonNeedsToSetDBValueOnSkip())
        {
            onFailedSettingDBValue = onFailed;

#if UNITY_WEBGL && !UNITY_EDITOR
            SetLessonCompleteValueToFirebaseDB(STAGE_SKIP_DB_VALUE, () =>
            {
                SetLocalDBValueOnSkipSpecificLesson(onCompleteSettingDBValue);
            });
#elif UNITY_EDITOR
            SetLocalDBValueOnSkipSpecificLesson(onCompleteSettingDBValue);
#endif
        }
        else
        {
            onCompleteSettingDBValue?.Invoke(); // quit slm, tmp
        }
    }

    public void SetLocalDBValueOnSkipSpecificLesson(Action isReady = null)
    {
        try
        {
            AddDataInStageCompletedInfo(STAGE_SKIP_DB_VALUE);

            isReady?.Invoke();
        }
        catch (Exception e)
        {
            CustomDebug.LogError($"error setting localdb on skip : {e.Message}");

            onFailedSettingDBValue?.Invoke();

            onFailedSettingDBValue = null;
        }
    }

    private void AddDataInStageCompletedInfo(int value)
    {
        var specificStageCompletedInfo = UserManager.Instance.GetSpecificStageCompletedInfo(stageType);

        if (specificStageCompletedInfo == null)
        {
            specificStageCompletedInfo = new Dictionary<string, int>();

            specificStageCompletedInfo.Add(lessonType, value);
        }
        else
        {
            specificStageCompletedInfo[lessonType] = value;
        }

        UserManager.Instance.AddDataInStageCompletedInfo(stageType, specificStageCompletedInfo);

        // CustomDebug.Log("AddDataInStageCompletedInfo End>>>>>>");
    }

    public void CheckCurrentLessonCompleteState()
    {
        var specificStageCompletedInfo = UserManager.Instance.GetSpecificStageCompletedInfo(stageType);

        if (specificStageCompletedInfo != null)
        {
            if (specificStageCompletedInfo.ContainsKey(lessonType))
            {
                if (specificStageCompletedInfo[CurrentSmartLearningInfo.Instance.GetCurrentLessonType()] == STAGE_COMPLETE_DB_VALUE)
                {
                    // 기존에 클리어했음

                    currentLessonCompleteState = CurrentLessonCompleteState.AlreadyPlayed;
                }
                else
                {
                    if (specificStageCompletedInfo[CurrentSmartLearningInfo.Instance.GetCurrentLessonType()] == STAGE_SKIP_DB_VALUE)
                    {
                        // 플레이 도중 나갔었음, -1

                        currentLessonCompleteState = CurrentLessonCompleteState.Skipped;
                    }
                }
            }
            else
            {
                // 해당 레슨 첫 플레이

                currentLessonCompleteState = CurrentLessonCompleteState.NotFirstTimePlayingCurrentStageButCurrentLessonIsFirstTime;
            }
        }
        else
        {
            // 해당 스테이지, 레슨 모두 첫 플레이

            currentLessonCompleteState = CurrentLessonCompleteState.FirstTimePlayingCurrentStage;
        }

        CustomDebug.Log($"currentLessonCompleteState : {currentLessonCompleteState}");
    }

    public void TryUpdateKpiDataOnLessonComplete()
    {
        // 무조건 : 대/강, 대/플, 학/누, 학/플
        // 조건부 : 대/스, 대/전, 학/완
        TryUpdateStudyDataPlayingTime(); // 학/플

        TryUpdateDashboardPlayingTime(); // 대/플

        TryUpdateDashboardDataAllStageComplete(); // 대/전

        TryUpdateDashboardDataLessonComplete(); // 대/강

        TryUpdateDashboardDataStageFirstComplete(); // 대/스

        TryUpdateStudyDataLessonAccumulatedComplete(); // 학/누

        TryUpdateStudyDataLessonComplete(); // 학/완

        this.playingTimeSec = 0;
    }

    public void TryUpdateKpiDataOnLessonSkip()
    {
        // 무조건 : 대/플, 학/플, 학/누
        // 조건부 : 대/스, 대/전, 학/스
        TryUpdateStudyDataPlayingTime(); // 학/플

        TryUpdateDashboardPlayingTime(); // 대/플

        TryUpdateDashboardDataAllStageComplete(); // 대/전

        TryUpdateDashboardDataStageFirstComplete(); // 대/스

        TryUpdateStudyDataLessonAccumulatedComplete(); // 학/누

        TryUpdateStudyDataLessonSkip(); // 학/스

        this.playingTimeSec = 0;
    }

    // 대시보드 ------------------------------------------------------------------------------------
    // 대시보드 / 강의완료, 스킵제외, 반복 포함
    private void TryUpdateDashboardDataLessonComplete()
    {
        var dashboardData = new DashboardData();

        dashboardData.program = AppInfo.Instance.GetProgramInfo();
        dashboardData.key = KEY_DASHBOARD_LESSON_COMPLETE;
        dashboardData.playingTime = -1;

        var dashboardDataJsonStr = DevUtil.Instance.GetJson(dashboardData);

#if UNITY_WEBGL && !UNITY_EDITOR
        FirebaseFunctionsController.Instance.RecordStageDashboardData(dashboardDataJsonStr);

#endif
    }

    // 대시보드 / 스테이지 최초 완료, 마지막 레슨까지 모두 스킵/클리어 기록이 생긴 첫 순간(다 스킵해도 인정)
    private void TryUpdateDashboardDataStageFirstComplete()
    {
        var stageIndex = this.stageValue - 1;

        if (!StageLessonDataManager.Instance.IsCompletedStage(stageIndex))
        {
            var specificStageTotalLessonCount = StageLessonDataManager.Instance.GetStageTotalCount(stageIndex);

            var specificStageCompletedInfo = UserManager.Instance.GetSpecificStageCompletedInfo(stageType);

            if (specificStageCompletedInfo != null)
            {
                var lessonCountUserTookOfSpecificStage = specificStageCompletedInfo.Count; // 해당 스테이지에서 유저가 들은 레슨의 수

                if (lessonCountUserTookOfSpecificStage == specificStageTotalLessonCount - 1 && !specificStageCompletedInfo.ContainsKey(lessonType))
                {
                    // 해당 스테이지는 해당 레슨빼고는 다 들은 상태(스킵/클리어 포함)

                    var dashboardData = new DashboardData();

                    dashboardData.program = AppInfo.Instance.GetProgramInfo();
                    dashboardData.key = DevUtil.Instance.GetTargetPath2Parts(KEY_DASHBOARD_FIRST_COMPLETE_STAGE, stageType);
                    dashboardData.playingTime = -1;

                    var dashboardDataJsonStr = DevUtil.Instance.GetJson(dashboardData);

#if UNITY_WEBGL && !UNITY_EDITOR
                    FirebaseFunctionsController.Instance.RecordStageDashboardData(dashboardDataJsonStr);

#endif
                    CustomDebug.Log($"대시보드, firstCompleteStageCount : {dashboardDataJsonStr}");
                }
            }
        }
    }

    /// <summary>
    /// 대시보드 / 플레이타임
    /// </summary>
    private void TryUpdateDashboardPlayingTime()
    {
        var dashboardData = new DashboardData();

        dashboardData.program = AppInfo.Instance.GetProgramInfo();
        dashboardData.key = KEY_DASHBOARD_PLAYING_TIME;
        dashboardData.playingTime = (float)this.playingTimeSec / 60;

        var dashboardDataJsonStr = DevUtil.Instance.GetJson(dashboardData);

#if UNITY_WEBGL && !UNITY_EDITOR
        FirebaseFunctionsController.Instance.RecordStageDashboardData(dashboardDataJsonStr);

#endif

        CustomDebug.Log($"대시보드, playingTime : {dashboardData.playingTime}m, {playingTimeSec}s");
    }

    // 대시보드 / 전체완료, 모든 강의에 -1 or 1값이 들어가졌을 때의 첫 순간만
    private void TryUpdateDashboardDataAllStageComplete()
    {
        var stageIndex = this.stageValue - 1;

        if (!StageLessonDataManager.Instance.IsCompletedStage(stageIndex))
        {
            var specificStageTotalLessonCount = StageLessonDataManager.Instance.GetStageTotalCount(stageIndex);

            var specificStageCompletedInfo = UserManager.Instance.GetSpecificStageCompletedInfo(stageType);

            if (specificStageCompletedInfo != null)
            {
                var lessonCountUserTookOfSpecificStage = specificStageCompletedInfo.Count; // 해당 스테이지에서 유저가 들은 레슨의 수

                if (lessonCountUserTookOfSpecificStage == specificStageTotalLessonCount - 1 && !specificStageCompletedInfo.ContainsKey(lessonType))
                {
                    // 해당 스테이지는 해당 레슨빼고는 다 들은 상태(스킵/클리어 포함)

                    var programTotalLessonCount = StageLessonDataManager.Instance.GetProgramTotalLessonCount();

                    var programTable = UserManager.Instance.GetStageCompletedInfo();

                    var wholeLessonCountUserTookOfCurrentProgram = 0;

                    for (int i = 0; i < programTable.Count; i++)
                    {
                        var specificLessonCountUserTook = programTable.ElementAt(i).Value.Count; // 해당 스테이지에서 유저가 들은 레슨 수

                        wholeLessonCountUserTookOfCurrentProgram += specificLessonCountUserTook;
                    }

                    if (wholeLessonCountUserTookOfCurrentProgram == programTotalLessonCount - 1)
                    {
                        // 해당 레슨만 들으면 해당 프로그램 모든 레슨을 들은 것

                        var dashboardData = new DashboardData();

                        dashboardData.program = AppInfo.Instance.GetProgramInfo();
                        dashboardData.key = KEY_DASHBOARD_ALL_STAGE_COMPLETE;
                        dashboardData.playingTime = -1;

                        var dashboardDataJsonStr = DevUtil.Instance.GetJson(dashboardData);

#if UNITY_WEBGL && !UNITY_EDITOR
                        FirebaseFunctionsController.Instance.RecordStageDashboardData(dashboardDataJsonStr);

#endif

                        CustomDebug.Log($"대시보드, allStageComplete : {dashboardDataJsonStr} ");
                    }
                }
            }
        }
    }

    // 학습관리 ------------------------------------------------------------------------------------
    // 학습관리 / 누적완료, 스킵/반복포함
    private void TryUpdateStudyDataLessonAccumulatedComplete()
    {
        var studyData = new StudyData();

        studyData.program = AppInfo.Instance.GetProgramInfo();
        studyData.stageType = this.stageType;
        studyData.lessonType = this.lessonType;
        studyData.playingTime = -1;
        studyData.isFirstComplete = false;
        studyData.isFirstSkip = false;
        studyData.isFirstCompleteWhichSkipped = false;

        var studyDataJsonStr = DevUtil.Instance.GetJson(studyData);

#if UNITY_WEBGL && !UNITY_EDITOR
        FirebaseFunctionsController.Instance.RecordStageStudyKPIData(studyDataJsonStr);

#endif
    }

    // 학습관리 / 완료, stageCompletedInfos 내 값이 -1이거나 없는데 complete 했을 때
    private void TryUpdateStudyDataLessonComplete()
    {
        if (!currentLessonCompleteState.Equals(CurrentLessonCompleteState.AlreadyPlayed))
        {
            var studyData = new StudyData();

            studyData.program = AppInfo.Instance.GetProgramInfo();
            studyData.stageType = this.stageType;
            studyData.lessonType = this.lessonType;
            studyData.isFirstComplete = true;
            studyData.isFirstSkip = false;
            studyData.playingTime = -1;

            if (currentLessonCompleteState.Equals(CurrentLessonCompleteState.Skipped))
            {
                studyData.isFirstCompleteWhichSkipped = true;

                CustomDebug.Log("스킵했던 강의를 처음 완료했다");
            }
            else
            {
                studyData.isFirstCompleteWhichSkipped = false;

                CustomDebug.Log("플레이 기록이 없던 강의를 처음 완료했다");
            }
            
            var studyDataJsonStr = DevUtil.Instance.GetJson(studyData);

#if UNITY_WEBGL && !UNITY_EDITOR
            FirebaseFunctionsController.Instance.RecordStageStudyKPIData(studyDataJsonStr);

#endif

            CustomDebug.Log("학습관리, firstCompleteCount");
        }
    }

    /// <summary>
    /// 학습관리 / 플레이타임
    /// </summary>
    private void TryUpdateStudyDataPlayingTime()
    {
        var studyData = new StudyData();

        studyData.program = AppInfo.Instance.GetProgramInfo();
        studyData.stageType = this.stageType;
        studyData.lessonType = this.lessonType;
        studyData.playingTime = (float) this.playingTimeSec / 60;
        studyData.isFirstComplete = false;
        studyData.isFirstSkip = false;
        studyData.isFirstCompleteWhichSkipped = false;

        var studyDataJsonStr = DevUtil.Instance.GetJson(studyData);

#if UNITY_WEBGL && !UNITY_EDITOR
        FirebaseFunctionsController.Instance.RecordStageStudyKPIData(studyDataJsonStr);

#endif

        CustomDebug.Log($"학습관리, playingTime : {studyData.playingTime}m, {playingTimeSec}s");
    }

    // 학습관리 / 스킵, stageCompleteValue가 기록되지 않은 상태에서 스킵했을 때
    private void TryUpdateStudyDataLessonSkip()
    {
        if (!( currentLessonCompleteState.Equals(CurrentLessonCompleteState.AlreadyPlayed) || currentLessonCompleteState.Equals(CurrentLessonCompleteState.Skipped) ))
        {
            // 플레이 경험이 없을 때

            var studyData = new StudyData();

            studyData.program = AppInfo.Instance.GetProgramInfo();
            studyData.stageType = this.stageType;
            studyData.lessonType = this.lessonType;
            studyData.playingTime = -1;
            studyData.isFirstComplete = false;
            studyData.isFirstSkip = true;
            studyData.isFirstCompleteWhichSkipped = false;

            var studyDataJsonStr = DevUtil.Instance.GetJson(studyData);

#if UNITY_WEBGL && !UNITY_EDITOR
            FirebaseFunctionsController.Instance.RecordStageStudyKPIData(studyDataJsonStr);

#endif

            CustomDebug.Log("학습관리, skipCount");
        }
    }

    public void RecordPlayingTime()
    {
        CustomDebug.Log($"RecordPlayingTime, isRecordingPlayingTime : {isRecordingPlayingTime}---------------");

        if (!isRecordingPlayingTime)
        {
            cancellationTokenSource = new CancellationTokenSource();

            RecordingUserPlayingTimeTask().Forget();

            isRecordingPlayingTime = true;
        }
    }

    private async UniTaskVoid RecordingUserPlayingTimeTask()
    {
        while (true)
        {
            if (!cancellationTokenSource.IsCancellationRequested)
            {
                await UniTask.Delay(1000, cancellationToken: cancellationTokenSource.Token);

                ++this.playingTimeSec;

                // CustomDebug.Log($"playingTimeSec : {playingTimeSec}");
            }
            else
            {
                CustomDebug.Log("CancellationRequested ...");

                break;
            }
        }
    }

    public void ForceCancelRecordingPlayingTimeTask()
    {
        CustomDebug.Log(">> ForceCancelRecordingUserPlayingTimeTask --- ");

        if (isRecordingPlayingTime)
        {
            cancellationTokenSource.Cancel();

            cancellationTokenSource.Dispose();

            isRecordingPlayingTime = false;
        }
    }

    public string GetTargetStageCompletedInfosPathStr()
    {
        var stageLessonPath = DevUtil.Instance.GetTargetPath2Parts(stageType, lessonType);  // s1 / l1

        var programInfoUnderPrizeitems = DevUtil.Instance.GetTargetPath2Parts(STAGE_COMPLETED_INFOS_PATH_STR, AppInfo.Instance.GetProgramInfo());  // stageCompletedInfos / photoShop

        var fullPath = DevUtil.Instance.GetTargetPath2Parts(programInfoUnderPrizeitems, stageLessonPath);

        return fullPath;
    }

    private string GetDBSubPath()
    {
        var subPath = "";

        sb.Clear();

        sb.Append(UserManager.Instance.GetUID());
        sb.Append("/");
        sb.Append(STAGE_COMPLETED_INFOS_PATH_STR);
        sb.Append("/");
        sb.Append(AppInfo.Instance.GetProgramInfo());
        sb.Append("/");
        sb.Append(CurrentSmartLearningInfo.Instance.GetCurrentStageType());
        sb.Append("/");
        sb.Append(CurrentSmartLearningInfo.Instance.GetCurrentLessonType());

        subPath = sb.ToString();

        return subPath;
    }

    public bool IsCurrentLessonNeedToSetDBValueOnComplete()
    {
        var result = false;

        if (!this.currentLessonCompleteState.Equals(CurrentLessonCompleteState.AlreadyPlayed))
        {
            result = true;
        }

        return result;
    }

    public bool IsCurrentLessonNeedsToSetDBValueOnSkip()
    {
        var result = false;

        if (currentLessonCompleteState.Equals(CurrentLessonCompleteState.FirstTimePlayingCurrentStage) || currentLessonCompleteState.Equals(CurrentLessonCompleteState.NotFirstTimePlayingCurrentStageButCurrentLessonIsFirstTime))
        {
            result = true;
        }

        return result;
    }

    public void SetCurrentLessonInfo(string[] stageLessonInfos, int[] stageLessonValues)
    {
        this.stageType = stageLessonInfos[0];
        this.lessonType = stageLessonInfos[1];

        this.stageValue = stageLessonValues[0];
        this.lessonValue = stageLessonValues[1];

        CustomDebug.Log($"SetCurrentLessonInfo, {stageType}, {lessonType}, {stageValue}, {lessonValue}");
    }

    public void ResetData()
    {
        this.stageType = "";
        this.lessonType = "";

        this.stageValue = 0;
        this.lessonValue = 0;

        this.playingTimeSec = 0;

        this.cancellationTokenSource = null;

        this.isRecordingPlayingTime = false;
    }
}
