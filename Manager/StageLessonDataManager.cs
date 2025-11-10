using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;

public struct StageData
{
    public int stageIndex;
    public int lessonIndex;
    public bool prizeStage;

    public string stageTitle;
    public string lessonTitle;

    private Color32 thumnailColor;

    public Color32 GetThumnailColor()
    {
        return thumnailColor;
    }

    public void SetThumnailColor(Color32 color)
    {
        this.thumnailColor = color;
    }
}

public class StageLessonDataManager : MonoBehaviour
{
    private static StageLessonDataManager instance = null;
    public static StageLessonDataManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<StageLessonDataManager>();
            }

            return instance;
        }
    }

    public TextAsset textAssetAllStageData;
    public TextAsset textAssetLessonTitles = null; // SmartLearningMode Scene 에서 참조 중
    public ThumnailStorage thumnailStorage;

    // 차후에 readOnly table로 바꿀 것
    private Dictionary<int, List<StageData>> allFixedStageTable = null;
    private IReadOnlyDictionary<int, List<StageData>> roAllFixedStageTable = null;

    // program / stage / lesson / title
    private Dictionary<string, Dictionary<string, Dictionary<string, string>>> lessonTitleTable = null;
    public IReadOnlyDictionary<string, Dictionary<string, Dictionary<string, string>>> roLessonTitleTable = null;

    private StringBuilder sb = new StringBuilder();

    private const string PREFIX_S = "s";
    private const string PREFIX_L = "l";


    private void Awake()
    {
        InitFixedStageLessonTable();

        InitLessonTitleTable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void InitFixedStageLessonTable()
    {
        var jsonStr = textAssetAllStageData.text;

        allFixedStageTable = DevUtil.Instance.GetTargetObjectFromJson<Dictionary<int, List<StageData>>>(jsonStr);

        roAllFixedStageTable = DevUtil.Instance.AsReadOnly(allFixedStageTable);

        CustomDebug.LogWithColor($"-- table count : {allFixedStageTable.Count}", CustomDebug.ColorSet.Magenta);
    }

    private void InitLessonTitleTable()
    {
        if(textAssetLessonTitles != null)
        {
            var jsonStr = textAssetLessonTitles.text;

            lessonTitleTable = DevUtil.Instance.GetTargetObjectFromJson<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(jsonStr);

            roLessonTitleTable = DevUtil.Instance.AsReadOnly(lessonTitleTable);
        }
    }

    public string GetLessonTitle(string stageType, string lessonType)
    {
        var lessonTitle = "";

        var programInfo = AppInfo.Instance.GetProgramInfo();

        if(lessonTitleTable.TryGetValue(programInfo, out var stageTable))
        {
            if(stageTable.TryGetValue(stageType, out var lessonTable))
            {
                if(lessonTable.TryGetValue(lessonType, out var title))
                {
                    lessonTitle = title;
                }
            }
        }

        return lessonTitle;
    }

    public Sprite GetProperThumnail(int[] stageAndLessonInfos)
    {
        var thumnail = this.thumnailStorage.GetProperThumnail(stageAndLessonInfos);

        return thumnail;
    }

    public string GetStageKey(int stageIndex)
    {
        sb.Clear();

        sb.Append(PREFIX_S);
        sb.Append(stageIndex + 1); // s1, s2, s3...

        return sb.ToString();
    }

    public string GetLessonKey(int lessonIndex)
    {
        sb.Clear();

        sb.Append(PREFIX_L);
        sb.Append(lessonIndex + 1); // l1, l2, l3...

        return sb.ToString();
    }

    public int GetProgramTotalLessonCount()
    {
        var totalCount = 0;

        for (int i = 0; i < allFixedStageTable.Count; i++)
        {
            totalCount += allFixedStageTable.ElementAt(i).Value.Count;
        }

        CustomDebug.Log($"GetProgramTotalLessonCount : {totalCount}");

        return totalCount;
    }

    public int GetStageTotalCount(int stageIndex)
    {
        var totalCount = 0;

        if(this.allFixedStageTable.TryGetValue(stageIndex, out var list))
        {
            totalCount = list.Count;
        }

        return totalCount;
    }

    public bool IsThisLastLessonOfCurrentStage(int stageValue, int lessonValue)
    {
        var isThisLastLessonOfCurrentStage = false;

        var stageIndex = stageValue - 1;

        if (this.allFixedStageTable.TryGetValue(stageIndex, out var list))
        {
            if(lessonValue >= list.Count)
            {
                isThisLastLessonOfCurrentStage = true;
            }
        }
        else
        {
            // 존재하지 않는 stageIndex

            CustomDebug.Log($"존재하지 않는 stageIndex : {stageIndex}");

            return isThisLastLessonOfCurrentStage;
        }

        return isThisLastLessonOfCurrentStage;
    }

    public IReadOnlyDictionary<int, List<StageData>> GetReadOnlyFixedStageTable()
    {
        return this.roAllFixedStageTable;
    }

    /// <summary>
    /// 스킵도 포함해서 스테이지를 완료했는 지 확인
    /// </summary>
    public bool IsCompletedStage(int stageIndex)
    {
        var targetStageKey = GetStageKey(stageIndex);

        var targetStageCompleteTable = UserManager.Instance.GetSpecificStageCompletedInfo(targetStageKey);

        if(targetStageCompleteTable == null)
        {
            return false;
        }
        else
        {
            var totalTargetStageCompleteCount = GetStageTotalCount(stageIndex);

            if (totalTargetStageCompleteCount == targetStageCompleteTable.Count)
            {
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// 스킵없이 스테이지를 완료했는 지 확인
    /// </summary>
    public bool IsCompletedStageActually(int stageIndex)
    {
        var targetStageKey = GetStageKey(stageIndex);

        var targetStageCompleteTable = UserManager.Instance.GetSpecificStageCompletedInfo(targetStageKey);

        if (targetStageCompleteTable == null)
        {
            return false;
        }
        else
        {
            var totalTargetStageCompleteCount = GetStageTotalCount(stageIndex);

            var datas = from data in targetStageCompleteTable
                        where data.Value == 1 // 완전 완료한 것들만 
                        select data;

            if (totalTargetStageCompleteCount == datas.Count())
            {
                return true;
            }

            return false;
        }
    }
}
