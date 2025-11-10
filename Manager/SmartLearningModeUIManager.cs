using DanielLochner.Assets.SimpleZoom;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SmartLearningModeUIManager : MonoBehaviour
{
    public RectTransform[] arrySlmOffsets;
    public GameObject panelLectureInfo;  // bottom
    public GameObject popUpAskingGoBackToHomeScene;               // 학습 진행 중 나가기 버튼 클릭 시 출력되는 팝업(나가기, 아니오)
    public GameObject popUpAskingSkipCurrentLecture;
    public GameObject popUpSetting;
    public GameObject logoutPanel;

    [Space]
    [Header("ExitWebGLPopUpSets-----------")]
    public GameObject popUpAskingExitWebGL;
    public GameObject[] arrayExitWebGLDescriptions; // 0 : signin, 1 : guest
    public GameObject signUpBtnInExitWebGLPopUp;
    public Image imagePopUpAskingExitWebGL;

    [Space]
    [Header("Top")]
    public TextMeshProUGUI upperUserMailText;
    public ChangeObjectImage extraFuncTextBG;
    public TextMeshProUGUI upperExtraFuncText;
    public TextMeshProUGUI fullScreenPopUpUserMailText;

    [Space]
    public CursorImageChanger cursorImageChanger;

    [Space]
    [Header("PopUps-----------------------------")]
    public GameObject popUpConversionFromGuestToLogin;
    public GameObject loadSLMErrorPopUp;
    public GameObject updateDBErrorPopUp;
    public GameObject popUpcommonError;
    public TextMeshProUGUI textCommonError;

    [Space]
    [Header("MiddleGuidePanel-----------------------")]
    public GameObject middleGuidePanel;
    public TextMeshProUGUI textMiddleGuide;
    public GameObject imageGameTitle;
    public GameObject imageQuizTitle;
    public GameObject imageTipTitle;
    public Sprite[] guideBubbleSprites = null; // 리소스 순서가 EnumSets.MiddleGuideBubbleType 과 순서가 같아야함
    public SpineFadeModule[] arraySpineFadeModules;

    private MiddleGuidePanelFadeModule middleGuidePanelFadeModule;
    private Image middleGuideBubbleImage;
    private RectTransform middleGuideRectTransform;
    private VerticalLayoutGroup middleGuideVerticalLayoutGroup;
    private Button buttonInMiddleGuideImage;

    #region normal, game/quiz bubble obj distinguished
    //[Space]
    //[Header("NormalMiddleGuidePanel-------------------------")]
    //public GameObject panelMiddleGuide;
    //public TextMeshProUGUI textInNormalMiddleGuide;
    //private Image imageMiddleGuideBubble;
    //private HorizontalLayoutGroup middleGuideHorizontalLayoutGroup2;

    //[Space]
    //[Header("GameQuizTypeMiddleGuidePanel-------------------------")]
    //public GameObject panelGameQuizTypeMiddleGuide;
    //public TextMeshProUGUI textInGameQuizTypeMiddleGuide;
    //public GameObject gameTitleImage;
    //public GameObject quizTitleImage;

    //private Image imageGameQuizTypeMiddleGuideBubble;
    //private HorizontalLayoutGroup gameQuizTypeMiddleGuideHorizontalLayoutGroup;
    //private Button buttonInGameQuizTypeMiddleGuideBubble;
    #endregion

    [Space]
    [Header("Bottom LearningObjective--------------------------")]
    public GameObject bGWithSkipBtn;
    public GameObject bGWithoutSkipBtn;

    public TextMeshProUGUI textLearningObjective;
    public Transform progressBox;
    public Transform truck;
    public GameObject learningObjectiveTextbox;
    public Transform[] imagesBlockingProgressGauge;
    public GameObject[] progressBatteryGauges;
    public GameObject[] pillarLights;

    [Space]
    [Header("FullScreen------------------")]
    public GameObject popUpSettingOnFullScreen;
    public GameObject objSettingBtnOnFullScreen;
    public Button skipCurrentLectureOnFullScreenPopUp;

    [Space]
    [Header("강의 클리어")]
    public CompleteCharacterModule completedCharacterModule;
    public GameObject[] arraySpriteMasksUnderCharacter;
    public GameObject exitPanelOnLessonComplete;   // 학습 완료 시 종료 창(다시하기, 다음강의로 이동, 나가기)
    // public GameObject exitPanelOnLastLesson;
    public GameObject btnGetReward;
    public GameObject btnSignUp;
    public GameObject btnMoveNextLesson;
    public GameObject objstageClearTitle;
    public GameObject verbalCloud;
    public TextMeshProUGUI textStageClearTitle;

    [Space]
    public GameObject commonDarkBG;

    private WaitForSeconds termTipBubbleSetPosition = new WaitForSeconds(0.01f);
    private WaitForSeconds termWhileHandlingMiddleGuideReset = new WaitForSeconds(0.002f);

    private const float Y_POS_OFFSET_ON_FULLSCREEN = -20f;

    // progress Amount===========================
    private const float PROGRESS_BOX_ORIGIN_X_LOCAL_POS = -439f;// -957f;
    private const float PROGRESS_TRUCK_ORIGIN_X_LOCAL_POS = 531f;
    private const float PROGRESS_GAUGE_BACKWARD_VALUE = 169f;
    private const float LAST_ANIMATION_MOVE_AMOUNT = 300f;

    private const float EXIT_POPUP_HEIGHT_SIGN_IN = 383f;
    private const float EXIT_POPUP_HEIGHT_GUEST = 465f;


    // Start is called before the first frame update
    void Awake()
    {
        Init();
    }

    private void Init()
    {
        middleGuideVerticalLayoutGroup = middleGuidePanel.GetComponent<VerticalLayoutGroup>();

        middleGuideBubbleImage = middleGuidePanel.GetComponent<Image>();

        buttonInMiddleGuideImage = middleGuidePanel.GetComponent<Button>();

        middleGuidePanelFadeModule = middleGuidePanel.GetComponent<MiddleGuidePanelFadeModule>();

        middleGuideRectTransform = middleGuideBubbleImage.GetComponent<RectTransform>();
    }

    public void ActivateBottomUIBGWithoutSkipBtn()
    {
        if (! bGWithoutSkipBtn.activeSelf)
        {
            bGWithSkipBtn.SetActive(false);

            bGWithoutSkipBtn.SetActive(true);
        }
    }

    public void ActivateBottomUIBGWithSkipBtn()
    {
        if (!bGWithSkipBtn.activeSelf)
        {
            bGWithoutSkipBtn.SetActive(false);

            bGWithSkipBtn.SetActive(true);
        }
    }

    public void EnableSkipLectureBtnOnFullScreenPopUp()
    {
        skipCurrentLectureOnFullScreenPopUp.interactable = true;
    }

    public void DisableSkipLectureBtnOnFullScreenPopUp()
    {
        skipCurrentLectureOnFullScreenPopUp.interactable = false;
    }

    public void SetSmartLearningYPosOffsetOnFullScreen()
    {
        // this.slmOffset.localPosition = new Vector2(0, Y_POS_OFFSET_ON_FULLSCREEN);

        for(int i = 0; i < arrySlmOffsets.Length; i++)
        {
            arrySlmOffsets[i].localPosition = new Vector2(0, Y_POS_OFFSET_ON_FULLSCREEN);
        }
    }

    public void SetSmartLearningYPosOffsetOnExitFullScreen()
    {
        // this.slmOffset.localPosition = Vector2.zero;

        for (int i = 0; i < arrySlmOffsets.Length; i++)
        {
            arrySlmOffsets[i].localPosition = Vector2.zero;
        }
    }

    public void SetUpperUserEmailText(string text)
    {
        upperUserMailText.text = text;
    }

    public void SetExtraFuncTextBG(int index)
    {
        this.extraFuncTextBG.ChangeImageOfTargetObject(index);
    }

    public void SetExtraFuncText(string text)
    {
        upperExtraFuncText.text = text;
    }

    public void ActivateConversionFromGuestToLoginPopUp()
    {
        popUpConversionFromGuestToLogin.SetActive(true);
    }

    public void DeActivateConversionFromGuestToLoginPopUp()
    {
        popUpConversionFromGuestToLogin.SetActive(false);
    }

    public void ActivateVerbalCloudNextToSignUpBtnInLessonCompletePopUp()
    {
        verbalCloud.SetActive(true);
    }

    public void DeActivateVerbalCloudNextToSignUpBtnInLessonCompletePopUp()
    {
        verbalCloud.SetActive(false);
    }

    public void SetFullScreenPopUpUserEmailText(string text)
    {
        fullScreenPopUpUserMailText.text = text;
    }

    public void ActivatePanelLectureInfo()
    {
        this.panelLectureInfo.SetActive(true);
    }

    public void DeActivatePanelLectureInfo()
    {
        this.panelLectureInfo.SetActive(false);
    }

    public void ActivateSettingBtnOnFullScreen()
    {
        objSettingBtnOnFullScreen.SetActive(true);
    }

    public void DeActivateSettingBtnOnFullScreen()
    {
        objSettingBtnOnFullScreen.SetActive(false);
    }

    public void ActivateLogOutPanel()
    {
        logoutPanel.SetActive(true);
    }

    public void DeActivateLogOutPanel()
    {
        logoutPanel.SetActive(false);
    }

    public void ActivateSettingPopUpOnFullScreenMode()
    {
        popUpSettingOnFullScreen.SetActive(true);
    }

    public void DeActivateSettingPopUpOnFullScreenMode()
    {
        popUpSettingOnFullScreen.SetActive(false);
    }

    public void ActivateAskingGoBackToHomeScenePopUp()
    {
        popUpAskingGoBackToHomeScene.SetActive(true);
    }

    public void DeActivateAskingGoBackToHomeScenePopUp()
    {
        popUpAskingGoBackToHomeScene.SetActive(false);
    }

    public void ActivateExitWebGLPopUpWhenSignedIn()
    {
        imagePopUpAskingExitWebGL.rectTransform.sizeDelta = new Vector2(imagePopUpAskingExitWebGL.rectTransform.rect.width, EXIT_POPUP_HEIGHT_SIGN_IN);

        DeActivateAllExitWebGLPopUpDescriptions();

        arrayExitWebGLDescriptions[0].SetActive(true);

        signUpBtnInExitWebGLPopUp.SetActive(false);

        popUpAskingExitWebGL.SetActive(true);
    }

    public void ActivateExitWebGLPopUpWhenGuest()
    {
        imagePopUpAskingExitWebGL.rectTransform.sizeDelta = new Vector2(imagePopUpAskingExitWebGL.rectTransform.rect.width, EXIT_POPUP_HEIGHT_GUEST);

        DeActivateAllExitWebGLPopUpDescriptions();

        arrayExitWebGLDescriptions[1].SetActive(true);

        signUpBtnInExitWebGLPopUp.SetActive(true);

        popUpAskingExitWebGL.SetActive(true);
    }

    public void DeActivateExitWebGLPopUp()
    {
        popUpAskingExitWebGL.SetActive(false);
    }

    private void DeActivateAllExitWebGLPopUpDescriptions()
    {
        for(int i = 0; i < arrayExitWebGLDescriptions.Length; i++)
        {
            arrayExitWebGLDescriptions[i].SetActive(false);
        }
    }

    public void ActivateSkipCurrentLecturePopUp()
    {
        popUpAskingSkipCurrentLecture.SetActive(true);
    }

    public void DeActivateSkipCurrentLecturePopUp()
    {
        popUpAskingSkipCurrentLecture.SetActive(false);
    }

    public void ActivateSettingsPopUp()
    {
        popUpSetting.SetActive(true);
    }

    public void DeActivateSettingsPopUp()
    {
        popUpSetting.SetActive(false);
    }

    public void ActivateExitPanelOnLessonComplete()
    {
        exitPanelOnLessonComplete.SetActive(true);
    }

    public void DeActivateExitPanelOnLessonComplete()
    {
        exitPanelOnLessonComplete.SetActive(false);
    }

    public void ActivateMoveNextLessonBtnOnLessonCompletePanel()
    {
        btnMoveNextLesson.SetActive(true);
    }

    public void DeActivateMoveNextLessonBtnOnLessonCompletePanel()
    {
        btnMoveNextLesson.SetActive(false);
    }

    public void ActivateStageClearTitleOnLessonCompletePanel(string text)
    {
        this.textStageClearTitle.text = text;

        this.objstageClearTitle.SetActive(true);
    }

    public void DeActivateStageClearTitleOnLessonCompletePanel()
    {
        this.objstageClearTitle.SetActive(false);
    }

    public void ActivateGetRewardBtnOnLessonCompletePanel()
    {
        btnGetReward.SetActive(true);
    }

    public void DeActivateGetRewardBtnOnLessonCompletePanel()
    {
        btnGetReward.SetActive(false);
    }

    public void ActivateSignUpBtnOnLessonCompletePanel()
    {
        btnSignUp.SetActive(true);
    }

    public void DeActivateSignUpBtnOnLessonCompletePanel()
    {
        btnSignUp.SetActive(false);
    }

    // Error PopUps------------------------------------
    public void ActivateLoadingErrorPopUp()
    {
        loadSLMErrorPopUp.SetActive(true);
    }

    public void DeActivateLoadingErrorPopUp()
    {
        loadSLMErrorPopUp.SetActive(false);
    }

    public void ActivateUpdateDBErrorPopUp()
    {
        updateDBErrorPopUp.SetActive(true);
    }

    public void DeActivateUpdateDBErrorPopUp()
    {
        updateDBErrorPopUp.SetActive(false);
    }

    public void ActivateCommonErrorPopUp(string msg)
    {
        this.textCommonError.text = msg;

        this.popUpcommonError.SetActive(true);
    }

    public void DeActivateCommonErrorPopUp()
    {
        this.popUpcommonError.SetActive(false);
    }

    // middle guide panel--------------------------------------------------------------
    public void SetSentenceInMiddleGuidePanel(string sentence)
    {
        textMiddleGuide.text = sentence;
    }

    public void ActivateMiddleGuidePanelWithPos(int middleGuideXPos, int middleGuideYPos)
    {
        StartCoroutine(CorActivateMiddleGuidePanelWithPos(middleGuideXPos, middleGuideYPos));

        #region not coroutine
        //middleGuidePanel.transform.localPosition = new Vector2(middleGuideXPos, middleGuideYPos);

        //ActivateMiddleGuidePanel();

        //HandleMiddleGuideBubbleSizeFittingOnActivated(EnableMiddleGuidePanelVerticalLayoutGroup, EnableImageComponentInMiddleGuidePanel);

        //var simpleZoom = SmartLearningModeController.Instance.GetCurrentSimpleZoom();

        //var transparencyValue = simpleZoom.GetAlphaValueFromCurrentZoom();

        //SetMiddleGuideBubbleAlpha(transparencyValue);
        #endregion
    }

    [Obsolete]
    public void ActivateTipMiddleGuidePanelWithPos(float middleGuideXPos, float middleGuideYPos)
    {
        StartCoroutine(CorActivateTipMiddleGuidePanelWithPos(middleGuideXPos, middleGuideYPos));
    }

    private IEnumerator CorActivateMiddleGuidePanelWithPos(int middleGuideXPos, int middleGuideYPos)
    {
        middleGuidePanel.transform.localPosition = new Vector2(middleGuideXPos, middleGuideYPos);

        HandleMiddleGuideBubbleSizeFittingOnActivated(EnableMiddleGuidePanelVerticalLayoutGroup, EnableImageComponentInMiddleGuidePanel);

        var simpleZoom = SmartLearningModeController.Instance.GetCurrentSimpleZoom();

        var transparencyValue = simpleZoom.GetAlphaValueFromCurrentZoom();

        yield return termWhileHandlingMiddleGuideReset;

        SetMiddleGuideBubbleAlpha(transparencyValue);

        ActivateMiddleGuidePanel();

        #region origin 말풍선 오브젝트를 미리 키워주고 있었음
        //middleGuidePanel.transform.localPosition = new Vector2(middleGuideXPos, middleGuideYPos);

        //ActivateMiddleGuidePanel();

        //HandleMiddleGuideBubbleSizeFittingOnActivated(EnableMiddleGuidePanelVerticalLayoutGroup, EnableImageComponentInMiddleGuidePanel);

        //var simpleZoom = SmartLearningModeController.Instance.GetCurrentSimpleZoom();

        //var transparencyValue = simpleZoom.GetAlphaValueFromCurrentZoom();

        //yield return termWhileHandlingMiddleGuideReset;

        //SetMiddleGuideBubbleAlpha(transparencyValue);
        #endregion
    }

    [Obsolete]
    private IEnumerator CorActivateTipMiddleGuidePanelWithPos(float middleGuideXPos, float middleGuideYPos)
    {
        yield return StartCoroutine(CorHandleMiddleGuideBubbleSizeFittingOnActivated(EnableMiddleGuidePanelVerticalLayoutGroup, EnableImageComponentInMiddleGuidePanel));

        ActivateMiddleGuidePanel();

        var simpleZoom = SmartLearningModeController.Instance.GetCurrentSimpleZoom();

        var transparencyValue = simpleZoom.GetAlphaValueFromCurrentZoom();

        yield return termTipBubbleSetPosition;

        SetMiddleGuideBubbleAlpha(transparencyValue);

        var xPosOffset = (middleGuideBubbleImage.rectTransform.rect.width / 2f);
        var yPosOffset = (middleGuideBubbleImage.rectTransform.rect.height / 2f);

        middleGuidePanel.transform.localPosition = new Vector2(middleGuideXPos + xPosOffset, middleGuideYPos + yPosOffset);
    }

    public void ActivateMiddleGuidePanel()
    {
        if (! middleGuidePanel.activeSelf)
        {
            middleGuidePanel.SetActive(true);
        }
    }

    public void DeActivateMiddleGuidePanel()
    {
        if (middleGuidePanel.activeSelf)
        {
            middleGuidePanel.SetActive(false);
        }
    }

    public void ActivateUpperQuizTitleImage()
    {
        if(imageQuizTitle.activeSelf)
        {
            return;
        }

        DeActivateMiddleGuideUpperTitleImages();

        imageQuizTitle.SetActive(true);
    }

    public void ActivateUpperGameTitleImage()
    {
        if (imageGameTitle.activeSelf)
        {
            return;
        }

        DeActivateMiddleGuideUpperTitleImages();

        imageGameTitle.SetActive(true);
    }

    public void ActivateUpperTipTitleImage()
    {
        if (imageTipTitle.activeSelf)
        {
            return;
        }

        DeActivateMiddleGuideUpperTitleImages();

        imageTipTitle.SetActive(true);
    }

    public void DeActivateMiddleGuideUpperTitleImages()
    {
        imageQuizTitle.SetActive(false);

        imageGameTitle.SetActive(false);

        imageTipTitle.SetActive(false);
    }

    public void SetMiddleGuideBubbleTexture(int index)
    {
        middleGuideBubbleImage.sprite = this.guideBubbleSprites[index];
    }

    public void SetMiddleGuideBubblePivot(float xValue, float yValue)
    {
        middleGuideRectTransform.pivot = new Vector2(xValue, yValue);
    }

    public void EnableImageComponentInMiddleGuidePanel()
    {
        middleGuideBubbleImage.enabled = true;
    }

    public void DisableImageComponentInMiddleGuidePanel()
    {
        middleGuideBubbleImage.enabled = false;
    }

    //public void SetMiddleGuideBubblePadding(int top, int bottom)
    //{
    //    middleGuideHorizontalLayoutGroup.padding.top = top;

    //    middleGuideHorizontalLayoutGroup.padding.bottom = bottom;
    //}

    public void SetMiddleGuideBubbleVerticalLayoutPadding(int[] arrayPaddingValues)
    {
        middleGuideVerticalLayoutGroup.padding.left = arrayPaddingValues[0];

        middleGuideVerticalLayoutGroup.padding.right = arrayPaddingValues[1];

        middleGuideVerticalLayoutGroup.padding.top = arrayPaddingValues[2];
        
        middleGuideVerticalLayoutGroup.padding.bottom = arrayPaddingValues[3];

        middleGuideVerticalLayoutGroup.spacing = arrayPaddingValues[4];
    }

    //public void EnableMiddleGuidePanelHorizontalLayoutGroup()
    //{
    //    middleGuideHorizontalLayoutGroup.enabled = true;
    //}

    //public void DisableMiddleGuidePanelHorizontalLayoutGroup()
    //{
    //    middleGuideHorizontalLayoutGroup.enabled = false;
    //}

    public void EnableMiddleGuidePanelVerticalLayoutGroup()
    {
        middleGuideVerticalLayoutGroup.enabled = true;
    }

    public void DisableMiddleGuidePanelVerticalLayoutGroup()
    {
        middleGuideVerticalLayoutGroup.enabled = false;
    }

    private void HandleMiddleGuideBubbleSizeFittingOnActivated(Action handleHorizontalLayoutGroup, Action handleImage)
    {
        StartCoroutine(CorHandleMiddleGuideBubbleSizeFittingOnActivated(handleHorizontalLayoutGroup, handleImage));
    }

    // 자식 텍스트 크기가 그 길이에 따라 정해지고 난 후 말풍선 이미지 앵커 잡을 시간 확보용, 드르륵 방지
    private IEnumerator CorHandleMiddleGuideBubbleSizeFittingOnActivated(Action handleLayoutGroup, Action handleImage)
    {
        yield return termWhileHandlingMiddleGuideReset;

        handleLayoutGroup.Invoke();

        yield return termWhileHandlingMiddleGuideReset;

        handleImage.Invoke();
    }

    #region HandleMiddleGuideBubbleSizeFittingOnActivated 하나로 퉁침
    //private void HandleMiddleGuideBubbleSizeFittingOnActivated()
    //{
    //    StartCoroutine(CorHandleMiddleGuideBubbleSizeFittingOnActivated());
    //}

    //private IEnumerator CorHandleMiddleGuideBubbleSizeFittingOnActivated()
    //{
    //    yield return new WaitForEndOfFrame();

    //    EnableImageComponentInMiddleGuidePanel();

    //    yield return new WaitForEndOfFrame();

    //    EnableMiddleGuidePanelHorizontalLayoutGroup();
    //}

    //private void HandleGameQuizTypeMiddleGuideBubbleSizeFittingOnActivated()
    //{
    //    StartCoroutine(CorHandleGameQuizTypeMiddleGuideBubbleSizeFittingOnActivated());
    //}

    //private IEnumerator CorHandleGameQuizTypeMiddleGuideBubbleSizeFittingOnActivated()
    //{
    //    yield return new WaitForEndOfFrame();

    //    EnableImageComponentInGameQuizTypeMiddleGuidePanel();

    //    yield return new WaitForEndOfFrame();

    //    EnableGameQuizTypeMiddleGuidePanelHorizontalLayoutGroup();
    //}
    #endregion

    public void EnableButtonInMiddleGuideBubble()
    {
        buttonInMiddleGuideImage.enabled = true;
    }

    public void DisableButtonInMiddleGuideBubble()
    {
        buttonInMiddleGuideImage.enabled = false;
    }

    public void SetMiddleGuideBubbleAlpha(float alpha)
    {
        middleGuidePanelFadeModule.Fade(alpha);
    }

    public void ResetMiddleGuideBubbleColor()
    {
        middleGuidePanelFadeModule.ResetMiddleGuideBubbleColor();
    }

    public void SetCurrentSimpleZoomAndInitializeMiddleGuidePanelFadeModule(SimpleZoom simpleZoom)
    {
        middleGuidePanelFadeModule.SetCurrentSimpleZoomAndInitialize(simpleZoom);
    }

    public void SetCurrentSimpleZoomAndInitializeSpineFadeModules(SimpleZoom simpleZoom)
    {
        for(int i = 0; i < arraySpineFadeModules.Length; i++)
        {
            arraySpineFadeModules[i].SetCurrentSimpleZoomAndInitialize(simpleZoom);
        }
    }

    // 학습 목표-----------------------------------------------------

    public void UpdateLearningObjectiveText(string text)
    {
        textLearningObjective.text = text;
    }

    public void ActivateProgressBatteryGauge(int progressIndex)
    {
        if (progressIndex < progressBatteryGauges.Length)
        {
            progressBatteryGauges[progressIndex].SetActive(true);
        }
    }

    public void ActivatePillarLight(int progressIndex)
    {
        if(progressIndex < pillarLights.Length)
        {
            pillarLights[progressIndex].SetActive(true);
        }
    }
    
    public void MoveProgressBox(float moveAmount, Action onBoxMoveComplete = null)
    {
        progressBox
        .DOLocalMoveX(moveAmount, 1f).SetRelative(true)
        .OnComplete(() =>
        {
            onBoxMoveComplete?.Invoke();
        });
    }

    public void JustMoveProgressBox(float moveAmount)
    {
        progressBox
        .DOLocalMoveX(moveAmount, 1f).SetRelative(true);
    }

    public void ActivateLearningObjectiveTextbox()
    {
        learningObjectiveTextbox.SetActive(true);
    }

    public void DeActivateLearningObjectiveTextbox()
    {
        learningObjectiveTextbox.SetActive(false);
    }

    public void UpdateProgressGauge(float moveAmount, int progressIndex)
    {
        imagesBlockingProgressGauge[progressIndex]
        .DOLocalMoveX(moveAmount, 1f).SetRelative(true)
        .OnComplete(() =>
        {
            imagesBlockingProgressGauge[progressIndex].gameObject.SetActive(false);
        });
    }

    public void PlayLastAnimationOnSmartLearningEnd(Action onLastAnimationComplete = null)
    {
        DeActivateLearningObjectiveTextbox();

        JustMoveProgressBox(LAST_ANIMATION_MOVE_AMOUNT);

        MoveTruck(onLastAnimationComplete);
    }

    public void MoveTruck(Action onLastAnimationComplete = null)
    {
        truck.DOLocalMoveX(LAST_ANIMATION_MOVE_AMOUNT, 1f).SetRelative(true)
        .OnComplete(() => 
        {
            onLastAnimationComplete?.Invoke();
        });
    }

    private void ResetBottomObjects()
    {
        DeActivateAllPillarLights();

        DeActivateAllProgressBatteryGauges();

        ResetBottomProgressBox();

        ResetBottomProgressGauge();

        ResetBottomTruck();

        DeActivateLearningObjectiveTextbox();
    }

    private void DeActivateAllPillarLights()
    {
        foreach(var lights in pillarLights)
        {
            lights.SetActive(false);
        }
    }

    private void DeActivateAllProgressBatteryGauges()
    {
        foreach (var lights in progressBatteryGauges)
        {
            lights.SetActive(false);
        }
    }

    private void ResetBottomProgressBox()
    {
        progressBox.DOLocalMoveX(PROGRESS_BOX_ORIGIN_X_LOCAL_POS, 0f);
    }

    private void ResetBottomProgressGauge()
    {
        foreach (var blockingGaugeImages in imagesBlockingProgressGauge)
        {
            if(blockingGaugeImages.gameObject.activeSelf == false)
            {
                blockingGaugeImages.DOLocalMoveX(- PROGRESS_GAUGE_BACKWARD_VALUE, 0f).SetRelative(true);

                blockingGaugeImages.gameObject.SetActive(true);
            }
        }
    }

    private void ResetBottomTruck()
    {
        truck.DOLocalMoveX(PROGRESS_TRUCK_ORIGIN_X_LOCAL_POS, 0f);
    }

    // 스마트학습 클리어 캐릭터 출력 ------------------------------------
    public void ActivateCharacterOnComplete(int index)
    {
        completedCharacterModule.ActivateCharacterOnComplete(index);
    }

    public void DeActivateAllCharactersOnComplete()
    {
        completedCharacterModule.DeActivateAllCharactersOnComplete();
    }

    public void ActivateAllSpriteMaskUnderCompleteCharacters()
    {
        for (int i = 0; i < arraySpriteMasksUnderCharacter.Length; i++)
        {
            arraySpriteMasksUnderCharacter[i].SetActive(true);
        }
    }

    public void AllDeActivateSpriteMaskUnderCompleteCharacters()
    {
        for (int i = 0; i < arraySpriteMasksUnderCharacter.Length; i++)
        {
            arraySpriteMasksUnderCharacter[i].SetActive(false);
        }
    }

    // ---------------------------------------
    public void ActivateCommonDarkBG()
    {
        commonDarkBG.SetActive(true);
    }

    public void DeActivateCommonDarkBG()
    {
        commonDarkBG.SetActive(false);
    }

    public void ChangeCursorToDefaultOne()
    {
        cursorImageChanger.ChangeCursorToDefaultOne();
    }

    public void ResetData()
    {
        ResetBottomObjects();
    }
}