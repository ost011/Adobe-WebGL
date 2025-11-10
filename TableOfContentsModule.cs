using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using System.Text;
using System;

/// <summary>
/// 목차 관련 모듈
/// </summary>
public class TableOfContentsModule : MonoBehaviour
{
    public Transform[] parentLessonParts;
    public GameObject prefabLessonPart;

    [Space]
    [Header("목차 뷰 관련 --------")]

    //[Space]
    //[Header("Top")]
    //public Image imageStageTab;
    //public Sprite[] stageTabSprites;

    [Space]
    [Header("Middle")]
    public GameObject darkBgTableOfContents;
    public GameObject bgTableOfContents;
    public RectTransform bodyTableOfContents;

    [Header("Lower")]
    public GameObject[] openCloseBtns; // 0 open , 1 close

    [Space]
    public HandlingStageBtnOnTableOfContents[] stageBtns;

    [Space]
    public GameObject[] arryLesson; // 레슨들이 출력될 메인 뷰(Content)들

    [Space]
    public SelectedLessonInfoOnTableOfContents selectedLessonInfoOnTableOfContents;

    [Space]
    [Header("PopUp -----")]
    public GameObject popUpGoSmartLearningMode;
    

    private int[] stageAndLessonInfos = new int[2]; // 데이터 초기화가 가능해서 사용하고있음

    private StringBuilder sb = new StringBuilder();
    private List<LessonDataOnTableOfContents> listLessonDataOnTableOfContent = new List<LessonDataOnTableOfContents>();
    private WaitForSeconds delayWhileCancelRecordingPlayingTime = new WaitForSeconds(1f);

    private const float INIT_Y_POS = 690f;
    private const float OPEN_Y_POS = -140f;

    private const float TWEEN_DURATION = 0.2f;

    private const string PREFIX_S = "s";

    // Start is called before the first frame update
    void Start()
    {
        ResetData();

        CreateTableOfContents();
    }
    
    private void CreateTableOfContents()
    {
        // var allFixedStageTable = StageLessonDataManager.Instance.allFixedStageTable;
        var allFixedStageTable = StageLessonDataManager.Instance.GetReadOnlyFixedStageTable();

        for (int i = 0; i < allFixedStageTable.Count; i++)
        {
            var kvp = allFixedStageTable.ElementAt(i);

            var list = kvp.Value;

            stageBtns[i].InitTotalLessonCount(list.Count);

            for (int j = 0; j < list.Count; j++)
            {
                var o = Instantiate(prefabLessonPart, parentLessonParts[i]);

                var module = o.GetComponent<LessonDataOnTableOfContents>();

                module.Init(list.ElementAt(j));

                listLessonDataOnTableOfContent.Add(module);

                // module.SetOnClickEvent(ShowSpecificLessonInfo);
                module.SetOnClickEvent((stageData, stageCompleteState) =>
                {
                    ShowSpecificLessonInfo(stageData, stageCompleteState);

                    AllDeActivateLessonItemEdgeBg();
                });
            }
        }
    }

    private void AllDeActivateLessonItemEdgeBg()
    {
        foreach(var lessonDataOnTableOfContents in listLessonDataOnTableOfContent)
        {
            lessonDataOnTableOfContents.DeActivateEdgeBG();
        }
    }

    private void ShowSpecificLessonInfo(StageData stageData, int stageCompleteState)
    {
        this.stageAndLessonInfos[0] = stageData.stageIndex;
        this.stageAndLessonInfos[1] = stageData.lessonIndex;

        this.selectedLessonInfoOnTableOfContents.SetCurrentState(stageCompleteState);

        this.selectedLessonInfoOnTableOfContents.ShowSelectedLessonInfo(stageData, stageAndLessonInfos);
    }

    public void LoadCurrentStageLessonInfo()
    {
        // 기획서 기준, 현재 수행 중인 스마트학습의 스테이지를 기준으로 
        // 목차 뷰를 띄워야하지만
        // 지금은 무조건 스테이지 1을 기준으로 최초 뷰 설정을 한다
        // 필요에 따라 고치기

        // to do
        // 현재 수강 중인 스테이지 - 레슨 강의 정보를 

        // 상세정보에 띄우기

        var currentStageIndex = CurrentSmartLearningInfo.Instance.GetCurrentStageValue() - 1;

        LoadTargetStageLessonInfo(currentStageIndex);
    }
    
    // 목차의 스테이지 버튼에 참조
    public void ForceLoadStageLessonInfo(int index)
    {
        LoadTargetStageLessonInfo(index);
    }

    private void LoadTargetStageLessonInfo(int stageIndex)
    {
        ResetData();

        AllSetIdleStageBtns();
        AllDeActivateLessonLists();

        this.selectedLessonInfoOnTableOfContents.HideSelectedLessonInfo();

        var target = arryLesson[stageIndex];

        target.SetActive(true);

        UpdateTargetStageBtn(stageIndex);
    }

    private void UpdateTargetStageBtn(int stageIndex)
    {
        var stageKey = StageLessonDataManager.Instance.GetStageKey(stageIndex);

        var table = UserManager.Instance.GetSpecificStageCompletedInfo(stageKey);

        var progressCount = 0;

        if(table != null)
        {
            //var datas = from data in table
            //            where data.Value != -1
            //            select data;

            //progressCount = datas.Count();

            // 220726 스킵이나 클리어도 모두 프로그레스 진행
            progressCount = table.Count();
        }
        
        stageBtns[stageIndex].SetCurrentCompletedLessonCount(progressCount);
        stageBtns[stageIndex].SetSelectedState();
        stageBtns[stageIndex].SetStageView();

        //imageStageTab.sprite = stageTabSprites[stageIndex];
    }

    private void AllSetIdleStageBtns()
    {
        for (int i = 0; i < stageBtns.Length; i++)
        {
            stageBtns[i].SetIdleState();
        }
    }

    private void AllDeActivateLessonLists()
    {
        for (int i = 0; i < arryLesson.Length; i++)
        {
            arryLesson[i].SetActive(false);
        }
    }

    public void OpenTableOfContents()
    {
        bodyTableOfContents.DOKill();

        AcitvateDarkBG();
        ActivateTableOfContentsBG();

        ActivateCloseBtn();

        SoundManager.Instance.PlayFX(EnumSets.FxType.TableOfContentsOpen);

        bodyTableOfContents.DOLocalMoveY(OPEN_Y_POS, TWEEN_DURATION, true).SetEase(Ease.Linear).OnComplete(()=> {

            LoadCurrentStageLessonInfo();
        });
    }

    private void AcitvateDarkBG()
    {
        darkBgTableOfContents.SetActive(true);
    }

    public void CloseTableOfContents()
    {
        ResetData();

        this.selectedLessonInfoOnTableOfContents.HideSelectedLessonInfo();

        AllDeActivateLessonLists();

        bodyTableOfContents.DOKill();

        ActivateOpenBtn();

        bodyTableOfContents.DOLocalMoveY(INIT_Y_POS, TWEEN_DURATION, true).SetEase(Ease.Linear).OnComplete(()=> {

            DeActivateDarkBG();
            DeActivateTableOfContentsBG();
            // SmartLearningModeController.Instance.DeActivateCommonDarkBG();
        });
    }

    private void QuickCloseTableOfContents()
    {
        ResetData();

        this.selectedLessonInfoOnTableOfContents.HideSelectedLessonInfo();

        AllDeActivateLessonLists();

        bodyTableOfContents.DOKill();

        ActivateOpenBtn();

        bodyTableOfContents.DOLocalMoveY(INIT_Y_POS, 0f, true).SetEase(Ease.Linear).OnComplete(() => {

            DeActivateDarkBG();
            DeActivateTableOfContentsBG();
            // SmartLearningModeController.Instance.DeActivateCommonDarkBG();
        });
    }

    private void ActivateTableOfContentsBG()
    {
        bgTableOfContents.SetActive(true);
    }

    private void DeActivateTableOfContentsBG()
    {
        bgTableOfContents.SetActive(false);
    }

    private void DeActivateDarkBG()
    {
        darkBgTableOfContents.SetActive(false);
    }

    private void ActivateOpenBtn()
    {
        AllDeActivateOpenCloseBtns();

        openCloseBtns[0].SetActive(true);
    }

    private void ActivateCloseBtn()
    {
        AllDeActivateOpenCloseBtns();

        openCloseBtns[1].SetActive(true);
    }

    private void AllDeActivateOpenCloseBtns()
    {
        for (int i = 0; i < openCloseBtns.Length; i++)
        {
            openCloseBtns[i].SetActive(false);
        }
    }

    //-----------------------------------------
    public void ActivatePopUpGoSmartLearningMode()
    {
        this.popUpGoSmartLearningMode.SetActive(true);
    }

    public void DeActivatePopUpGoSmartLearningMode()
    {
        this.popUpGoSmartLearningMode.SetActive(false);
    }

    // popUpGoSmartLearningMode 의 'Go' 버튼에 참조되어있음
    public void LoadSelectedSmartLearningMode()
    {
        // 선택한 강의로 진입
        // this.stageAndLessonInfos 배열에 들어가있는 스테이지, 레슨 정보 활용
        // 특정 레슨버튼을 눌렀을 때 this.stageAndLessonInfos 에 스테이지, 레슨 인덱스 정보가 부여된다

        // to do
        // 특정 레슨으로 진입!!

        StartCoroutine(CorLoadSelectedSmartLearningMode());
    }

    IEnumerator CorLoadSelectedSmartLearningMode()
    {
        LoadingManager.Instance.ActivateLoading();

        SmartLearningModeController.Instance.StopRecordingLessonPlayingTime();

        yield return delayWhileCancelRecordingPlayingTime;

        SmartLearningModeController.Instance.QuitSmartLearningMode();

        CurrentSmartLearningInfo.Instance.InitializeTargetSmartLearningInfo(this.stageAndLessonInfos);

        QuickCloseTableOfContents();

        DeActivatePopUpGoSmartLearningMode();

        LoadingManager.Instance.DeActivateLoading();

        SmartLearningModeController.Instance.InitSmartLearningMode();
    }

    private void ResetData()
    {
        Array.Clear(stageAndLessonInfos, 0, 2);
    }
}
