using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class GuideAnimationManager : MonoBehaviour
{
    public GameObject canvasGuideAnimation;

    [Space]
    public GameObject prefabGuideStageOne;
    public GameObject prefabGuideStageTwo;

    public Transform parent;

    private GameObject guideStageOneBody = null;
    private GameObject guideStageTwoBody = null;

    private GuideMainStageOneModule guideMainStageOneModule;

    //---------------------------------------------------------

    private GuideMainStageTwoModule guideMainStageTwoModule;

    [Space]
    public TextAsset guideStageTwoTextBoxSentences;

    private Dictionary<int, List<GuideTextBoxSentences>> sentenceTable = null;

    private const string TUTORIAL_ALEADY_SEEN_STR = "tutorialModalNoMoreSee";

    // Start is called before the first frame update
    void Start()
    {
        CheckNoMoreState();
    }

    private void InitStageOne()
    {
        guideMainStageOneModule.SetAnnounceStageOneIsDoneCallback(LoadNextStageTwo);
    }

    private void InitStageTwo()
    {
        guideMainStageTwoModule.SetSentenceTable(this.sentenceTable);

        guideMainStageTwoModule.SetCompleteGuideEvent(CompleteGuide);
    }

    private void InitTexts()
    {
        try
        {
            this.sentenceTable = DevUtil.Instance.GetTargetObjectFromJson<Dictionary<int, List<GuideTextBoxSentences>>>(guideStageTwoTextBoxSentences.text);
        }
        catch(Exception e)
        {
            CustomDebug.LogError($"InitTexts error : {e.Message}");
        }
    }

    private void CheckNoMoreState()
    {
        if(PlayerPrefs.HasKey(TUTORIAL_ALEADY_SEEN_STR))
        {
            // 키가 있다는 것은 이미 튜토리얼을 했다는 것
            SoundManager.Instance.PlayBGM(EnumSets.BGMType.Lobby);

            return;
        }

        CreateGuideAllAssets();

        ActivateGuide();

        InitStageOne();

        InitTexts();

        InitStageTwo();

        SoundManager.Instance.PlayBGM(EnumSets.BGMType.Tutorial);
    }

    private void CreateGuideAllAssets()
    {
        this.guideStageOneBody = Instantiate(prefabGuideStageOne, parent);

        this.guideMainStageOneModule = this.guideStageOneBody.GetComponent<GuideMainStageOneModule>();

        this.guideStageTwoBody = Instantiate(prefabGuideStageTwo, parent);

        this.guideMainStageTwoModule = this.guideStageTwoBody.transform.GetChild(0).GetComponent<GuideMainStageTwoModule>();
    }

    private void CreateGuideStageTwoAsset()
    {
        this.guideStageTwoBody = Instantiate(prefabGuideStageTwo, parent);

        this.guideMainStageTwoModule = this.guideStageTwoBody.transform.GetChild(0).GetComponent<GuideMainStageTwoModule>();
    }

    private void LoadNextStageTwo()
    {
        this.guideMainStageTwoModule.ActivateBody();
        this.guideMainStageTwoModule.ActivateTextBox();
    }

    private void ActivateGuide()
    {
        canvasGuideAnimation.SetActive(true);
    }

    private void DeActivateGuide()
    {
        canvasGuideAnimation.SetActive(false);
    }

    private void CompleteGuide()
    {
        // 가이드 종료

        SaveTutorialAleadyDoneRecord();

        DestroyGuideBody();

        DeActivateGuide();

        SoundManager.Instance.PlayBGM(EnumSets.BGMType.Lobby);
    }

    private void DestroyGuideBody()
    {
        if(this.parent.childCount > 0)
        {
            for (int i = 0; i < this.parent.childCount; i++)
            {
                Destroy(this.parent.GetChild(i).gameObject);
            }
        }
    }

    private void SaveTutorialAleadyDoneRecord()
    {
        PlayerPrefs.SetInt(TUTORIAL_ALEADY_SEEN_STR, 1);
        PlayerPrefs.Save();
    }

    // 이미 가이드 애니메이션을 완료했지만
    // 특정 루트를 통해서 다시 가이드를 출력해야할 때 호출
    public async void ForceActivateGuide()
    {
        // LoadingManager.Instance.ActivateLoading();

        SoundManager.Instance.PlayBGM(EnumSets.BGMType.Tutorial);

        CreateGuideStageTwoAsset();

        ActivateGuide();

        InitTexts();

        await UniTask.Delay(50); // 아래 init 을 하기위한 최소시간 확보를 위함
        
        InitStageTwo();

        LoadNextStageTwo();

        //await UniTask.Delay(1000);

        //LoadingManager.Instance.DeActivateLoading();
    }
}
