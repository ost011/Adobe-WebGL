using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.UI.Extensions;

public class LessonManager : MonoBehaviour
{
    public EnumSets.StageType stageType = EnumSets.StageType.S1;

    private string stageTypeStr = "";
    public string StageTypeStr => this.stageTypeStr;

    public LessonModule[] lessonModules;

    [Space]
    public HorizontalScrollSnap horizontalScrollSnap;

    [SerializeField]
    private int currentLectureSequence = 0; // 현재 '연구 중' 인 레슨 인덱스

    private Action<string[]> addRecentTryingLectureEvent = null;

    
    private void Awake()
    {
        Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        GetSpecialPrizeItemPos();

        SetOrdinaryPrizeItemPos();
    }

    private void OnEnable()
    {
        UpdateCurrentScrollViewPos();
    }

    private void Init()
    {
        stageTypeStr = stageType.ToString().ToLower();

        for (int i = 0; i < lessonModules.Length; i++)
        {
            lessonModules[i].SetStageType(this.stageType);

            lessonModules[i].Init();

            lessonModules[i].SetRecentTryingLectureEvent(AddRecentTryingLecture);
        }
    }

    // 레슨 리스트가 켜질 때마다 현재 '연구 중' 인 레슨이 중앙으로 오게하기위함
    private void UpdateCurrentScrollViewPos()
    {
        StartCoroutine(CorUpdateCurrentScrollViewPos());
    }

    IEnumerator CorUpdateCurrentScrollViewPos()
    {
        yield return new WaitForEndOfFrame();

        horizontalScrollSnap.GoToScreen(this.currentLectureSequence);
    }

    public void SetAddRecentTryingLectureEvent(Action<string[]> addRecentTryingLectureEvent)
    {
        this.addRecentTryingLectureEvent = addRecentTryingLectureEvent;
    }

    public void LoadInitLessonState()
    {
        // CustomDebug.LogWithColor("LoadInitLessonState ---", CustomDebug.ColorSet.Yellow);

        var stageKey = stageType.ToString().ToLower();

        var completedTable = UserManager.Instance.GetSpecificStageCompletedInfo(stageKey);

        if(completedTable != null)
        {
            InitLessonState(completedTable);
        }
        else
        {
            InitFirstLessonState();
        }
    }

    private void InitLessonState(Dictionary<string, int> stageCompleteTable)
    {
        var completedCount = stageCompleteTable.Count;

        for (int i = 0; i < completedCount; i++)
        {
            var kvp = stageCompleteTable.ElementAt(i);

            var target = lessonModules[i];

            target.SetMarkState(kvp.Value); // 완료 or skip 한 인덱스의 것들만 변경
            target.EnableBtn();
            target.DeActivateHaloMark();
        }

        if(completedCount < lessonModules.Length)
        {
            var target = lessonModules[completedCount];

            // 다음차례 레슨 버튼 활성화 - halo 표시하기
            target.EnableBtn();
            target.DeActivateAllStateMarks();
            target.ActivateHaloMark();

            this.currentLectureSequence = completedCount;

            for (int i = completedCount + 1; i < lessonModules.Length; i++)
            {
                var lockTarget = lessonModules[i];

                lockTarget.ActivateLockMark();
            }
        }
        else
        {
            if(completedCount == lessonModules.Length)
            {
                // 모든 강의 클리어
                // 화면은 제일 처음 버튼을 바라보도록 하기
                this.currentLectureSequence = 0;
            }
        }
    }

    // s1, s2, s3 최초 시도의 경우, stageCompleteTable이 null 이기때문에
    // 완료 정보를 확인할 방법이 없음
    // 각 스테이지 별 첫 레슨은 '연구 중', 그 외 레슨은 모두 'Lock' 세팅을 해야함
    private void InitFirstLessonState()
    {
        for (int i = 0; i < lessonModules.Length; i++)
        {
            var target = lessonModules[i];

            target.DeActivateAllStateMarks();

            if (i == 0)
            {
                target.EnableBtn();
                target.ActivateHaloMark();

                continue;
            }

            target.ActivateLockMark();
        }

        this.currentLectureSequence = 0;
    }

    private void AddRecentTryingLecture(string[] lectureInfo)
    {
        this.addRecentTryingLectureEvent?.Invoke(lectureInfo);
    }

    // 마지막 강의 상품, 스페셜 상품을 제외한 일반 강의의 상품 구역 세팅하기
    public void SetOrdinaryPrizeItemPos()
    {
        var table = AppInfo.Instance.roOrdinaryPrizeItemInfoTable;

        foreach (var item in table)
        {
            // 기존 스페셜 상품 UI를 세팅하는 로직을 그대로 사용
            // 아이콘만 띄우면되고, 상품 지급 로직은 상품성격에 맞춰서 알아서 돌아갈 것임
            SetSpecialPrizeItemPos(item.Key, item.Value);
        }
    }

    public void GetSpecialPrizeItemPos()
    {
        var table = AppInfo.Instance.prizeItemInfoTable;

        var specialItems = from specialItem in table
                           where specialItem.Value.special
                           select specialItem;

        if (specialItems.Count() > 0)
        {
            for (int i = 0; i < specialItems.Count(); i++)
            {
                var kvp = specialItems.ElementAt(i);

                CustomDebug.Log($"--- Get SpecialPrizeItemPos, special name in lessonManager : {kvp.Value.name}");

                SetSpecialPrizeItemPos(kvp.Key, kvp.Value);
            }
        }
    }

    private void SetSpecialPrizeItemPos(string prizeItemInfoId, PrizeItemInfo prizeItemInfo)
    {
        var stageType = prizeItemInfo.stagePos;
        var lessonType = prizeItemInfo.lessonPos;
        CustomDebug.Log($"Set SpecialPrizeItemPos, stageType: {stageType} / lessonType : {lessonType}");

        var targets = from target in lessonModules
                      where target.StageTypeStr.Equals(stageType) && target.LessonTypeStr.Equals(lessonType)
                      select target;

        if (targets.Count() > 0)
        {
            CustomDebug.Log($"second, targets.ElementAt(0) name : {targets.ElementAt(0).name}");

            targets.ElementAt(0).SetSpecialPrizeItemPos(prizeItemInfoId, prizeItemInfo);
        }
    }
}
