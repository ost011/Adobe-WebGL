using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Text;
using System;
using System.Collections.Specialized;

public class StageController : MonoBehaviour
{
    public TextMeshProUGUI textSpecificStageCurriculum;

    [Space]
    [Header("Top Progress")]
    public Image imgBigProgress;
    public Image imgSmallProgress;
    public Transform progressArrow;
    public TextMeshProUGUI textProgress;

    [Space]
    public GameObject[] lessonArray;
    private LessonManager[] lessonManagers = null;

    private int whatStageOpened = -1;
    private string whatStageStr = "";

    [Space]
    public TextMeshProUGUI[] stageCurriculumTexts;
    private string[] stageCurriculumNames = new string[3];

    private OrderedDictionary orderedRecentTryingLectureTable = new OrderedDictionary();

    private StringBuilder sb = new StringBuilder();

    private const string RECENT_TRYING_LECTURE_KEY = "recentTryingLecture";
    private const string UNDER_LINE = "_";
    private const int LIMIT_RECENT_TRYING_LECTURE_COUNTS = 15;

    private const float PROGRESS_ARROW_DEGREE = 89f;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        lessonManagers = new LessonManager[lessonArray.Length];

        for (int i = 0; i < lessonArray.Length; i++)
        {
            lessonManagers[i] = lessonArray[i].GetComponent<LessonManager>();

            lessonManagers[i].LoadInitLessonState();

            lessonManagers[i].SetAddRecentTryingLectureEvent(UpdateRecentTryingLectureInfos);
        }

        for (int i = 0; i < stageCurriculumTexts.Length; i++)
        {
            stageCurriculumNames[i] = stageCurriculumTexts[i].text;
        }

        InitRecentTryingLectureTable();
    }

    private void InitRecentTryingLectureTable()
    {
        var tmpRecentTryingLectureTable = UserManager.Instance.GetRecentTryingLectureTable();

        this.orderedRecentTryingLectureTable.Clear();

        // CustomDebug.LogWithColor("-----------------------------------------", CustomDebug.ColorSet.Cyan);

        if(tmpRecentTryingLectureTable == null)
        {
            CustomDebug.Log("tmpRecentTryingLectureTable is null");

            return;
        }

        for (int i = 0; i < tmpRecentTryingLectureTable.Count; i++)
        {
            var tmp = tmpRecentTryingLectureTable.ElementAt(i);

            this.orderedRecentTryingLectureTable.Add(tmp.Key, tmp.Value);

            // CustomDebug.LogWithColor($"recent table -> {tmp.Key} / {tmp.Value}", CustomDebug.ColorSet.Cyan);
        }

        // CustomDebug.LogWithColor("-----------------------------------------", CustomDebug.ColorSet.Cyan);
    }

    public void SetTargetStageIndex(EnumSets.StageType stageType)
    {
        this.whatStageOpened = (int)stageType;

        this.whatStageStr = stageType.ToString().ToLower();
    }

    private void ResetData()
    {
        this.whatStageOpened = -1;
    }

    public void InitSpecificStageView()
    {
        try
        {
            DeActivateAllLessonObjects();

            lessonArray[this.whatStageOpened].SetActive(true);

            var curriculumStr = stageCurriculumNames[this.whatStageOpened];

            textSpecificStageCurriculum.text = curriculumStr; //.Replace("\n", " ");

            var completedTable = UserManager.Instance.GetSpecificStageCompletedInfo(this.whatStageStr);

            if (completedTable != null)
            {
                var datas = from data in completedTable
                            where data.Value == 1
                            select data;

                float totalLessonCount = StageLessonDataManager.Instance.GetStageTotalCount(this.whatStageOpened);
                // CustomDebug.LogWithColor($"UpdateProgressItems, totalLessonCount : {totalLessonCount}", CustomDebug.ColorSet.Green);

                var progress = datas.Count() / totalLessonCount;
                // CustomDebug.LogWithColor($"UpdateProgressItems, progress : {progress}", CustomDebug.ColorSet.Green);

                SetProgressValue(progress);
            }
            else
            {
                CustomDebug.LogError($"{this.whatStageStr} completedTable is null");

                SetProgressValue(0);
            }
        }
        catch(Exception e)
        {
            CustomDebug.LogError($"InitSpecificStageView error : {e.Message}");

            SetProgressValue(0);
        }
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

    private void DeActivateAllLessonObjects()
    {
        for (int i = 0; i < lessonArray.Length; i++)
        {
            lessonArray[i].SetActive(false);
        }
    }

    public void ReInitLessonMarks()
    {
        for (int i = 0; i < lessonManagers.Length; i++)
        {
            lessonManagers[i].LoadInitLessonState();
        }
    }

    // 제일 최근에 본 학습이 테이블의 첫번째 위치로 들어감
    private void UpdateRecentTryingLectureInfos(string[] lectureInfo)
    {
        var key = GetRecentTryingLectureKey(lectureInfo);
        var value = lectureInfo[2];

        CustomDebug.Log($"lectureInfo, : {lectureInfo[0]} / {key} / {lectureInfo[2]}");

        // 최초로 최근학습을 기록함
        if (this.orderedRecentTryingLectureTable.Count == 0)
        {
            this.orderedRecentTryingLectureTable.Add(key, value);

            CustomDebug.Log($"first, UpdateRecentTryingLectureInfos : {key} / {value}");
        }
        else
        {
            if (this.orderedRecentTryingLectureTable.Contains(key))
            {
                // 테이블의 제일 첫번째 순서에 넣기위해서 기존 값은 삭제
                this.orderedRecentTryingLectureTable.Remove(key);

                // test
                CustomDebug.Log($"Remove done : {key}");

                var tmpEnumerator = this.orderedRecentTryingLectureTable.GetEnumerator();

                while(tmpEnumerator.MoveNext())
                {
                    CustomDebug.LogWithColor($"after remove, tableRecentTryingLecture -> {tmpEnumerator.Key} / {tmpEnumerator.Value}", CustomDebug.ColorSet.Magenta);
                }
            }

            if(this.orderedRecentTryingLectureTable.Count == LIMIT_RECENT_TRYING_LECTURE_COUNTS)
            {
                // 15개까지만 저장해야하므로 제일 첫번째 것 (제일 오래된 것)을 지운다

                // this.orderedRecentTryingLectureTable.RemoveAt(this.orderedRecentTryingLectureTable.Count-1);
                this.orderedRecentTryingLectureTable.RemoveAt(0);
            }
            
            // this.orderedRecentTryingLectureTable.Insert(0, key, value);
            this.orderedRecentTryingLectureTable.Insert(this.orderedRecentTryingLectureTable.Count, key, value);

            // var jsonType = DevUtil.Instance.GetJson(this.orderedRecentTryingLectureTable);

            // CustomDebug.Log($"jsonType : {jsonType}");
        }

        CustomDebug.Log($"UpdateRecentTryingLectureInfos : {key} / {value} / count : {this.orderedRecentTryingLectureTable.Count}");

        UserManager.Instance.UpdateLocalRecentTryingLecture(this.orderedRecentTryingLectureTable);

#if UNITY_EDITOR

        // test

        var tmpEnumerator2 = this.orderedRecentTryingLectureTable.GetEnumerator();

        while (tmpEnumerator2.MoveNext())
        {
            CustomDebug.LogWithColor($"final tableRecentTryingLecture -> {tmpEnumerator2.Key} / {tmpEnumerator2.Value}", CustomDebug.ColorSet.Magenta);
        }

#elif UNITY_WEBGL
        var jsonType = DevUtil.Instance.GetJson(this.orderedRecentTryingLectureTable);

        var subPath = DevUtil.Instance.GetTargetPathInSpecificUser(RECENT_TRYING_LECTURE_KEY);

        var finalPath = DevUtil.Instance.GetTargetPath(EnumSets.DBParentType.Users, subPath);

        FirebaseDatabaseController.Instance.SetTagetData(finalPath, jsonType);
#endif

    }

    private string GetRecentTryingLectureKey(string[] lectureInfo)
    {
        sb.Clear();

        sb.Append(AppInfo.Instance.GetProgramInfo()); // photoShop
        sb.Append(UNDER_LINE);
        sb.Append(lectureInfo[0]); // s1
        sb.Append(UNDER_LINE);
        sb.Append(lectureInfo[1]); // l3

        return sb.ToString();
    }

    //public void SetSpecialPrizeItemPos(PrizeItemInfo prizeItemInfo)
    //{
    //    var stagePos = prizeItemInfo.stagePos;
    //    CustomDebug.Log($"first, stagePos : {stagePos}");

    //    //test debug
    //    for (int i = 0; i < lessonManagers.Length; i++)
    //    {

    //    }

    //    var targets = from target in lessonManagers
    //                  where target.StageTypeStr.Equals(stagePos)
    //                  select target;

    //    if(targets.Count() > 0)
    //    {
    //        targets.ElementAt(0).SetSpecialPrizeItemPos(prizeItemInfo);
    //    }
    //}
}
