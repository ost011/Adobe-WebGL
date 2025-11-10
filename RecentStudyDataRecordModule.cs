using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Text;
using System;
using System.Collections.Specialized;

public class RecentStudyDataRecordModule : MonoBehaviour
{
    private OrderedDictionary orderedRecentTryingLectureTable = new OrderedDictionary();

    private StringBuilder sb = new StringBuilder();

    private bool isInit = false;

    private const string RECENT_TRYING_LECTURE_KEY = "recentTryingLecture";
    private const string UNDER_LINE = "_";
    private const int LIMIT_RECENT_TRYING_LECTURE_COUNTS = 15;

    // Start is called before the first frame update
    void Start()
    {
        // Init();
    }

    private void Init()
    {
        if (!isInit)
        {
            InitRecentTryingLectureTable();

            isInit = true;
        }
    }

    private void InitRecentTryingLectureTable()
    {
        var tmpRecentTryingLectureTable = UserManager.Instance.GetRecentTryingLectureTable();

        this.orderedRecentTryingLectureTable.Clear();

        if (tmpRecentTryingLectureTable == null)
        {
            CustomDebug.Log("tmpRecentTryingLectureTable is null");

            return;
        }

        for (int i = 0; i < tmpRecentTryingLectureTable.Count; i++)
        {
            var tmp = tmpRecentTryingLectureTable.ElementAt(i);

            this.orderedRecentTryingLectureTable.Add(tmp.Key, tmp.Value);

            CustomDebug.LogWithColor($"orderedRecentTryingLectureTable -> {tmp.Key} / {tmp.Value}", CustomDebug.ColorSet.Cyan);
        }
    }

    public void UpdateRecentTryingLectureInfos(string[] lectureInfo)
    {
        Init();

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
                CustomDebug.Log($"orderedRecentTryingLectureTable.Contains({key})!");

                return;
            }

            if (this.orderedRecentTryingLectureTable.Count == LIMIT_RECENT_TRYING_LECTURE_COUNTS)
            {
                // 15개까지만 저장해야하므로 제일 첫번째 것 (제일 오래된 것)을 지운다

                // this.orderedRecentTryingLectureTable.RemoveAt(this.orderedRecentTryingLectureTable.Count-1);
                this.orderedRecentTryingLectureTable.RemoveAt(0);
            }

            // this.orderedRecentTryingLectureTable.Insert(0, key, value);
            this.orderedRecentTryingLectureTable.Insert(this.orderedRecentTryingLectureTable.Count, key, value);
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

        sb.Append(AppInfo.Instance.GetProgramInfo());
        sb.Append(UNDER_LINE);
        sb.Append(lectureInfo[0]);
        sb.Append(UNDER_LINE);
        sb.Append(lectureInfo[1]);

        return sb.ToString();
    }
}
