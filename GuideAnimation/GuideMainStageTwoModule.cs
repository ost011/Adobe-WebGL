using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public struct GuideTextBoxStatus
{
    private bool isNeedToShow;
    public bool IsNeedToShow => this.isNeedToShow;

    private int characterType;
    public int CharacterType { get => characterType; }

    private int emotion;
    public int Emotion { get => emotion; }

    private string sentence;
    public string Sentence { get => sentence; }

    private bool isNeedToShowYoutubeArea;
    public bool IsNeedToShowYoutubeArea => this.isNeedToShowYoutubeArea;

    public void SetNoNeedToShow()
    {
        this.isNeedToShow = false;
    }

    public void SetNeedToShow()
    {
        this.isNeedToShow = true;
    }

    public void SetCharacterType(int characterType)
    {
        this.characterType = characterType;
    }

    public void SetCharacteremotion(int emotion)
    {
        this.emotion = emotion;
    }

    public void SetSentence(string sentence)
    {
        this.sentence = sentence;
    }

    public void SetNoNeedToShowYoutubeArea()
    {
        this.isNeedToShowYoutubeArea = false;
    }

    public void SetIsNeedToShowYoutubeArea()
    {
        this.isNeedToShowYoutubeArea = true;
    }
}

public struct GuideTextBoxSentences
{
    public int character;
    public int emotion;
    public string sentence;
}

// textAsset 인 guideStageTwoTextBox.json 참고
public class GuideMainStageTwoModule : MonoBehaviour
{
    public GameObject body;

    [Space]
    public GuideStageTwoTextBoxModule guideStageTwoTextBoxModule;

    [Space]
    [Header("스테이지 2 세부 요소들 -----")]

    public GameObject[] sequences;

    [SerializeField]
    private int currentStage2SequenceCount = 0;

    // seqeunce 2 -------------------
    [Space]
    public GameObject[] scrollGuideArrows; // 0 left to right  / 1 right to left
    public int targetScrollIndex = 0;

    [Space]
    public ScrollRect mainScrollRect;
    public HorizontalScrollSnap horizontalScrollSnap;

    private int currentChapterIndex = 0; // 현재 챕터를 가리키는 인덱스
    private int currentViewIndex = 0; // 현재 진행된 멘트 인덱스, 챕터 변경 시 0 으로 초기화

    private GuideTextBoxStatus status;

    // key : 0 ~ 8 / value : list[~]
    private Dictionary<int, List<GuideTextBoxSentences>> sentenceTable = null;

    private Action completeGuideEvent = null;

    private void OnDestroy()
    {
        completeGuideEvent = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();

    }

    private void Init()
    {
        guideStageTwoTextBoxModule.SetCheckNextStepStatusCallback(GetCurrentViewStatus);

        SetReadyToShowNextSequence();
    }

    public void SetSentenceTable(Dictionary<int, List<GuideTextBoxSentences>> table)
    {
        this.sentenceTable = table;
    }

    public void SetCompleteGuideEvent(Action callback)
    {
        this.completeGuideEvent = callback;
    }

    private GuideTextBoxStatus GetCurrentViewStatus(int currentPressedNextBtnCount)
    {
        try
        {
            var targetList = GetProperList();

            this.currentViewIndex = currentPressedNextBtnCount;

            var guideElement = targetList.ElementAt(this.currentViewIndex);

            this.status = GetValidStatus(guideElement);

            return this.status;
        }
        catch (ArgumentOutOfRangeException listError)
        {
            CustomDebug.Log($"참조 리스트 범위 초과, listError : {listError.Message}");

            if (this.currentChapterIndex == 7)
            {
                CustomDebug.Log($"male 에서 female 로 변경되는 순간 - 텍스트 박스-연속 출력이 필요함");

                // male 에서 female 로 변경되는 순간 
                // - 텍스트 박스-연속 출력이 필요함

                CountUpCurrentChapterIndex();

                this.currentViewIndex = 0;

                guideStageTwoTextBoxModule.SetForceReInitCurrentPressedNextBtnCount();

                var lastList = GetProperList();

                var target = lastList.ElementAt(this.currentViewIndex);

                this.status = GetValidStatus(target);
            }
            else
            {
                // 텍스트 박스를 꺼야할 때
                CustomDebug.Log($"텍스트 박스를 꺼야할 때 --");

                SetNoNeedToShowStatus();

                CountUpCurrentChapterIndex();
            }

            return this.status;
        }
        catch (Exception e)
        {
            CustomDebug.LogError($"GetCurrentViewStatus, error : {e.Message}");

            SetNoNeedToShowStatus();

            return status;
        }

    }

    private void SetNoNeedToShowStatus()
    {
        this.status = new GuideTextBoxStatus();

        this.status.SetNoNeedToShow();
    }

    private List<GuideTextBoxSentences> GetProperList()
    {
        if (this.sentenceTable.TryGetValue(this.currentChapterIndex, out var list))
        {
            return list;
        }
        else
        {
            return null;
        }
    }

    private GuideTextBoxStatus GetValidStatus(GuideTextBoxSentences value)
    {
        var status = new GuideTextBoxStatus();

        status.SetNeedToShow();

        status.SetSentence(value.sentence);
        status.SetCharacterType(value.character);
        status.SetCharacteremotion(value.emotion);

        if(this.currentChapterIndex == 7 && this.currentViewIndex == 4 )
        {
            status.SetIsNeedToShowYoutubeArea();
        }
        else
        {
            status.SetNoNeedToShowYoutubeArea();
        }

        return status;
    }

    private void CountUpCurrentChapterIndex()
    {
        ++this.currentChapterIndex;
    }

    public void ActivateBody()
    {
        this.body.SetActive(true);
    }

    public void DeActivateBody()
    {
        this.body.SetActive(false);
    }

    //---------------------------------------------

    private void SetReadyToShowNextSequence()
    {
        this.guideStageTwoTextBoxModule.SetShowNextSequence(ShowNextSequence);
    }

    public void ShowNextSequence()
    {
        try
        {
            sequences[this.currentStage2SequenceCount].SetActive(true);
        }
        catch(Exception e)
        {
            CustomDebug.LogWithColor($"더이상 보여줄 튜토리얼이 없음, 종료하기", CustomDebug.ColorSet.Red);

            LoadCompleteGuide();
        }
    }

    private void LoadCompleteGuide()
    {
        this.completeGuideEvent?.Invoke();
    }

    // 스테이지 2 세부 시퀀스 진행관련
    public void CompleteCurrentSequence()
    {
        DeActivatePreSequence();

        CountUpSequenceCount();

        ActivateTextBox();
    }

    private void DeActivatePreSequence()
    {
        try
        {
            sequences[this.currentStage2SequenceCount].SetActive(false);
        }
        catch (Exception e)
        {
            // to do 
            // 가이드 종료
            CustomDebug.LogError($"DeActivatePreSequence error : {e.Message}");
        }
    }


    public void ActivateTextBox()
    {
        guideStageTwoTextBoxModule.ForceLoadStep();
    }

    private void CountUpSequenceCount()
    {
        ++this.currentStage2SequenceCount;
    }

    public void OnSelectionPageChanged(int what)
    {
        if (what < targetScrollIndex)
        {
            ActivateRightToLeftArrow();
        }
        else
        {
            if (what > targetScrollIndex)
            {
                ActivateLeftToRightArrow();
            }
            else
            {
                this.mainScrollRect.enabled = false;
                this.horizontalScrollSnap.enabled = false;

                AllDeActivateScrollGuideArrows();

                CompleteCurrentSequence();
            }
        }
    }

    private void AllDeActivateScrollGuideArrows()
    {
        for (int i = 0; i < scrollGuideArrows.Length; i++)
        {
            scrollGuideArrows[i].SetActive(false);
        }
    }

    private void ActivateLeftToRightArrow()
    {
        AllDeActivateScrollGuideArrows();

        scrollGuideArrows[0].SetActive(true);
    }

    private void ActivateRightToLeftArrow()
    {
        AllDeActivateScrollGuideArrows();

        scrollGuideArrows[1].SetActive(true);
    }

    public void SetTargetScrollingIndex(int index)
    {
        this.targetScrollIndex = index;
    }

    public void OnClickSkipBtn()
    {
        this.completeGuideEvent?.Invoke();
    }
}
