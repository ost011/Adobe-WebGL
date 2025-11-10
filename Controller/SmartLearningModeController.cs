using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using status = MarksAssets.FullscreenWebGL.FullscreenWebGL.status;
using navigationUI = MarksAssets.FullscreenWebGL.FullscreenWebGL.navigationUI;
using MarksAssets.FullscreenWebGL;
using UnityEngine.SceneManagement;
using System.Linq;
using DanielLochner.Assets.SimpleZoom;
using UnityEngine.UI;
using AOT;

public class SmartLearningModeController : MonoBehaviour
{
    private static SmartLearningModeController instance = null;
    public static SmartLearningModeController Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<SmartLearningModeController>();
            }

            return instance;
        }
    }

    public SmartLearningModeUIManager uIManager;
    public DocsManager docsManager;
    public LearningObjectiveManager learningObjectiveManager;
    public UpperLineControllerInSLMMode upperLineControllerInSLMMode;
    public StageCompleteRecorder stageCompleteRecorder;
    public RecentStudyDataRecordModule recentStudyDataRecordModule;
    public SLMPopUpManager slMPopUpManager;
    public Transform parentObjOfSLMPrefab;
    public LogInControllerInSLM loginController;

    [Space]
    public CharacterSpineHandlingModule characterSpineHandlingModule;

    //[Space]
    //public HandlingStageBtnOnTableOfContents[] arrayHandlingStageBtnOnTableOfContents;
    
    private StringBuilder sb = new StringBuilder();
    private GameObject currentSLMObject = null;
    private LearningModeInfo learningModeInfo = null;
    private SLMControllModule slmControllModule = null;
    private SimpleZoom simpleZoom = null;
    private RewardModuleInSLM rewardModule = null;
    private List<RewardModule> listRewardModule = new List<RewardModule>();


    public Button tmpCompleteLessonBtn; // for test

    private bool isCurrentLessonLastLesson = false;


    // 강의 클리어 시점에서 DB업데이트하는데 필요한 < rewardName / { randomKey, category } >
    private Dictionary<string, string[]> rewardInfoTable = new Dictionary<string, string[]>();

    // stageCompletedInfos에 넣을 값임과 동시에 DBUpdate 시도하던 게 스킵인지 클리어인지 판단용
    private int currentSkipCompleteValueForDBUpdate = 0;  // -1 : skip, 1 : complete

    private WaitForSeconds delayWhileUnitaskTokenBeCancelled = new WaitForSeconds(1f);

    private Action afterSpecificPopUpDeActivated = null;
    private Action afterCommonPopUpDeActivated = null;

    private bool isNeedToDetectFullScreenStateNotUsingFullScreenWebGLBecauseNotSupported = false; // FullScreenWebGL이 지원되지 않아 다른 방식으로 감지해야 하는가
    private bool wasItFullScreen = false;
    private IEnumerator exitFullScreenEnumerator = null;
    // private static Action onPressEscapeKeycode = null;

    private const string SLASH_STR = "/";
    private const string SMART_LEARNING_UNDER_RESOURCES_PATH_STR = "SLMPrefabs/";
    private const string SCENE_NAME_HOME = "Home";

    private const string GUEST_UPPER_EMAIL_TEXT = "손님으로 입장";
    private const string GUEST_FULL_SCREEN_EMAIL_TEXT = "손님으로 입장";
    private const string GUEST_EXTRA_FUNC_TEXT = "계정 연동하기";
    private const int GUEST_EXTRA_FUNC_SPRITE_INDEX = 0;

    private const string SIGN_IN_EXTRA_FUNC_TEXT = "로그아웃";
    private const int SIGN_IN_EXTRA_FUNC_SPRITE_INDEX = 1;

    [Space]
    public DragAndDropModule dragAndDropModuleOnSettingBtn;

    // Start is called before the first frame update
    void Awake()
    {
        Init();
    }
    
    void Start()
    {
        SoundManager.Instance.PlayBGM(EnumSets.BGMType.SLM);
    }

    private void Update()
    {
        if(wasItFullScreen && Screen.fullScreenMode.Equals(FullScreenMode.Windowed) && !Screen.fullScreen)
        {
            // 직전에 풀스크린이었다가 현재 풀스크린 해제 상태가 되었다

            CheckExitFullScreenRoutine();
        }
    }

    void OnDestroy()
    {
        StopRecordingLessonPlayingTime();

        CurrentSmartLearningInfo.Instance.ResetData();

        RemoveKeyboardInputEventListener();

        UnSubsribeFullScreenChangedEvent();
    }

    private void Init()
    {
        docsManager.InitLearningModeInfoTables();

        InitSmartLearningMode();

#if UNITY_WEBGL && !UNITY_EDITOR

        // 에디터 환경에서 이 구문을 실행하면 SmartLearningController 스크립트가 꺼짐
        InitJsLibForSmartLearningMode();
#endif
        // onPressEscapeKeycode = SetOrdinaryScreenMode;

        // WebWindowController.AddEventListenerOnPressEscapeKeycode(OnPressEscapeCallback);

        SubsribeFullScreenChangedEvent();

        SetUserEmailText();

        SetExtraFuncDisplay();

        CheckNeedToDetectFullScreenStateNotUsingFullScreenWebGLBecauseNotSupported();
    }

    private void SetUserEmailText()
    {
        SetUpperUserEmailText();

        SetFullScreenPopUpUserEmailText();
    }

    private void SetUpperUserEmailText()
    {
        var msg = "";

        if (AppInfo.Instance.IsGuestLogin)
        {
            msg = GUEST_UPPER_EMAIL_TEXT;
        }
        else
        {
            msg = UserManager.Instance.GetUserEmail();
        }

        this.uIManager.SetUpperUserEmailText(msg);
    }

    private void SetFullScreenPopUpUserEmailText()
    {
        var msg = "";

        if (AppInfo.Instance.IsGuestLogin)
        {
            msg = GUEST_FULL_SCREEN_EMAIL_TEXT;
        }
        else
        {
            msg = UserManager.Instance.GetUserEmail();
        }

        this.uIManager.SetFullScreenPopUpUserEmailText(msg);
    }

    private void SetExtraFuncDisplay()
    {
        SetExtraFuncTextBG();

        SetExtraFuncText();
    }

    private void SetExtraFuncTextBG()
    {
        var index = -1;

        if (AppInfo.Instance.IsGuestLogin)
        {
            index = GUEST_EXTRA_FUNC_SPRITE_INDEX;
        }
        else
        {
            index = SIGN_IN_EXTRA_FUNC_SPRITE_INDEX;
        }

        this.uIManager.SetExtraFuncTextBG(index);
    }

    private void SetExtraFuncText()
    {
        var msg = "";

        if (AppInfo.Instance.IsGuestLogin)
        {
            msg = GUEST_EXTRA_FUNC_TEXT;
        }
        else
        {
            msg = SIGN_IN_EXTRA_FUNC_TEXT;
        }

        this.uIManager.SetExtraFuncText(msg);
    }

    private void AddKeyboardInputEventListener()
    {
#if UNITY_EDITOR

        CustomDebug.Log("AddKeyboardInputEventListener");
#elif UNITY_WEBGL

        WebWindowController.AddEventListenerHandleOnPressSpecificInputKey();

        WebWindowController.AddEventListenerHandleOnPreventEventFromBrowser();
#endif
    }

    private void RemoveKeyboardInputEventListener()
    {
        WebWindowController.RemoveEventListenerHandleOnPressSpecificInputKey();

        WebWindowController.RemoveEventListenerHandleOnPreventEventFromBrowser();

        // WebWindowController.RemoveEventListenerOnPressEscapeKeycode();
    }

    // 레슨리스트에서 넘어올 경우, 이번 연구 건너뛰기로 스킵한 경우, 다음 강의로 이동한 경우, 학습진도표로 강의에 진입한 경우 실행
    public void InitSmartLearningMode()
    {
        var smartLearningName = CurrentSmartLearningInfo.Instance.GetCurrentSmartLearningName();

        // this.learningModeInfo = CurrentSmartLearningInfo.Instance.GetLearningModeInfo();
        this.learningModeInfo = docsManager.GetTargetLearningModeInfo(smartLearningName);

        SetBottomUIDisplay();

        ResetMiddleGuideBubbleColor();
        
        DeActivateAllCharactersOnComplete();

        InitializeTargetSmartLearning(smartLearningName);
    }

    // Init에 참조되어 있음
    private void InitJsLibForSmartLearningMode()
    {
        AddKeyboardInputEventListener();

        WebWindowController.SetConfirmPopUpDescriptionMessage(Constant.REFRESH_POPUP_DESCRIPTION_STR);
    }

    private void SetBottomUIDisplay()
    {
        if (CurrentSmartLearningInfo.Instance.IsNeedToActivateBottomSkipBtn())
        {
            this.uIManager.ActivateBottomUIBGWithSkipBtn();

            this.uIManager.EnableSkipLectureBtnOnFullScreenPopUp();
        }
        else
        {
            this.uIManager.ActivateBottomUIBGWithoutSkipBtn();

            this.uIManager.DisableSkipLectureBtnOnFullScreenPopUp();
        }
    }

    public void ActivatePanelLectureInfo()
    {
        this.uIManager.ActivatePanelLectureInfo();
    }

    public void DeActivatePanelLectureInfo()
    {
        this.uIManager.DeActivatePanelLectureInfo();
    }

    public void OnClickUserIdBtn()
    {
        this.uIManager.ActivateLogOutPanel();
    }

    // ExtraFunc 에서 참조 중 / 로그인 상태 -> 로그아웃, 게스트 상태 -> 계정연동 팝업
    public void OnClickExtraFuncBtn()
    {
        if (AppInfo.Instance.IsGuestLogin)
        {
            ActivateSpecificPopUp(EnumSets.SLMPopUpType.Conversion);
        }
        else
        {
            // FirebaseAuthController.Instance.SignOut();
            TrySignOut();
        }
    }

    // ---------------------------------------------------------------
    // 계정 연동, 로그아웃 관련

    // 계정 연동 진행 중 끝까지 진행하지 않는다면 데이터 유실된다는 안내 팝업의 '연동하기' 버튼에 참조되어 있음
    public void TrySignUp()
    {
        // 게스트 로그인 시 익명로그인을 통해서 로그인, 필요한 정보만 db에 저장함
        // 연동시 GetIdToken() 을 통해 얻은 idToken을 이용해서 서버사이드 렌더링(파베 admin sdk)을 거쳐
        // 실제 계정으로 승급, 풀 데이터를 익명 로그인-uid에 저장함

        CustomDebug.Log("계정 연동하기");

#if UNITY_EDITOR
        CustomDebug.Log("TrySignUp In Editor");

#elif UNITY_WEBGL && !UNITY_EDITOR
        LoadingManager.Instance.ActivateLoading();

        PathJsLibData pathJsLibData = new PathJsLibData();

        pathJsLibData.objectName = "SLMController";
        pathJsLibData.callbackName = "OnSuccessTryingSignUpFromGuest";
        pathJsLibData.fallbackName = "OnFailedTryingSignUpFromGuest";

        FirebaseAuthController.Instance.GetIdToken(pathJsLibData);
#endif
    }

    // 강의 완료 팝업 - 계정 연동하기 버튼
    public void OnClickSignUpBtnInLessonCompletePopUp()
    {
        ActivateSpecificPopUp(EnumSets.SLMPopUpType.Conversion);

        SetCallbackAfterSpecificPopUpDeActivated(() => 
        {
            ActivateVerbalCloudNextToSignUpBtnInLessonCompletePopUp();

            ActivateAllSpriteMaskUnderCompleteCharacters();
        });

        AllDeActivateSpriteMaskUnderCompleteCharacters();

        DeActivateVerbalCloudNextToSignUpBtnInLessonCompletePopUp();
    }

    // 웹지엘 종료팝업 - 계정 연동하기 버튼에 참조되어 있음
    public void OnClickSignUpBtnInExitWebGLPopUp()
    {
        ActivateSpecificPopUp(EnumSets.SLMPopUpType.Conversion);
    }

    private void TrySignOut()
    {
        this.loginController.SignOut();

        CustomDebug.Log("TrySignOut");
    }

    // 강의 클리어 팝업 - 계정 연동 버튼, OnClickExtraFuncBtn 에서 사용 중
    private void OnSuccessTryingSignUpFromGuest(string idToken)
    {
        CustomDebug.Log($"On Success Trying SignUpFromGuest : {idToken}");

        LoadingManager.Instance.DeActivateLoading();

        // to do
        // 받은 idToken 을 url 파라미터로 전달하기
        // 'https://bdm-adobe-jinhyuk.vercel.app/register?token={idToken}' 파라미터 값"

        var path = DevUtil.Instance.GetJustCombine2Parts(AppInfo.REGISTER_WEB_PAGE_FOR_GUEST, idToken);

        TrySignOut();

        WebWindowController.OpenTargetWindow(path);

        AppInfo.Instance.ResetData();
    }

    // 강의 클리어 팝업 - 계정 연동 버튼, OnClickExtraFuncBtn 에서 사용 중
    private void OnFailedTryingSignUpFromGuest(string errorMsg)
    {
        CustomDebug.LogError($"On Failed TryingSignUpFromGuest : {errorMsg}");

        LoadingManager.Instance.DeActivateLoading();

        // to do 
        // 예외 처리바람
    }

    public void SucceededSignOut()
    {
        CustomDebug.Log("SucceededSignOut");

        UserManager.Instance.ResetData();

        SceneManager.LoadScene(0);
    }

    public void WhenSignOutFailed()
    {
        CustomDebug.Log("WhenSignOutFailed");
    }

    public void OnClickExitBtnInLogOutPanel()
    {
        this.uIManager.DeActivateLogOutPanel();
    }

    // Full Screen ----------------------------------------------------
    private void CheckNeedToDetectFullScreenStateNotUsingFullScreenWebGLBecauseNotSupported()
    {
        if (!FullscreenWebGL.isFullscreenSupported())
        {
            isNeedToDetectFullScreenStateNotUsingFullScreenWebGLBecauseNotSupported = true;
        }
    }

    private void SubsribeFullScreenChangedEvent()
    {
        if (FullscreenWebGL.isFullscreenSupported())
        {
            FullscreenWebGL.subscribeToFullscreenchangedEvent();

            FullscreenWebGL.onfullscreenchange += CheckFullScreenState;
        }
    }

    private void UnSubsribeFullScreenChangedEvent()
    {
        if (FullscreenWebGL.isFullscreenSupported())
        {
            FullscreenWebGL.unsubscribeToFullscreenchangedEvent();

            FullscreenWebGL.onfullscreenchange -= CheckFullScreenState;
        }
    }

    private void CheckFullScreenState()
    {
        if (FullscreenWebGL.isFullscreen())
        {
            HandleOnScreenChangedToFullScreen();
        }
        else
        {
            // esc 나 설정창의 버튼을 통해서 전체화면 해제를 하므로
            // 여기에서 레이아웃 조절 루틴 수행이 되도록 함
            WebWindowController.SetNormalScreenState();

            SetOridinaryUIView();

            DeActivateSettingBtnOnFullScreen();

            DeActivateSettingPopUpOnFullScreenMode();

            this.uIManager.SetSmartLearningYPosOffsetOnExitFullScreen();
        }
    }

    public void SetFullScreenMode()
    {
        // 전체화면 들어가는 루틴은 1개 (상단 전체화면 버튼)
        // 레이아웃 조절 루틴을 중지시켜야하므로 전체화면으로 들어가기전에 SetFullScreenState 호출
        WebWindowController.SetFullScreenState();

        // 상단 전체화면 버튼 끄고
        // 스마트학습 뷰 확장
        // 하단 학습목표 뷰 비활성화

        EnterFullScreenMode();
    }

    private void EnterFullScreenMode()
    {
        if (!isNeedToDetectFullScreenStateNotUsingFullScreenWebGLBecauseNotSupported)
        {
            EnterFullScreenModeUsingFullScreenWebGL();
        }
        else
        {
            JustEnterFullScreenMode();
        }
    }

    private void EnterFullScreenModeUsingFullScreenWebGL()
    {
        if (FullscreenWebGL.isFullscreen())
        {
            return;
        }

        FullscreenWebGL.requestFullscreen((stat) =>
        {
            if (stat.Equals(status.Success))
            {
                CustomDebug.Log($"requestFullscreen Success");
            }
        }, navigationUI.hide);
    }

    private void JustEnterFullScreenMode()
    {
        StartCoroutine(CorJustEnterFullScreenMode());
    }

    private IEnumerator CorJustEnterFullScreenMode()
    {
        if (Screen.fullScreen)
        {
            yield break;
        }

        Screen.fullScreen = true;

        yield return new WaitUntil(() => Screen.fullScreen.Equals(true));

        HandleOnScreenChangedToFullScreen();

        wasItFullScreen = true;
    }

    private void HandleOnScreenChangedToFullScreen()
    {
        try
        {
            ActivateSettingBtnOnFullScreen();

            this.uIManager.SetSmartLearningYPosOffsetOnFullScreen();

            SetFullScreenUIView();
        }
        catch (Exception e)
        {
            CustomDebug.LogError($"HandleOnScreenChangedToFullScreen : {e.Message}");
        }
    }

    // 상/하단 UI 패널 끄기
    private void SetFullScreenUIView()
    {
        this.upperLineControllerInSLMMode.DeActivatePanelUpperLine();

        DeActivatePanelLectureInfo();
    }

    public void SetOrdinaryScreenMode()
    {
        if (!isNeedToDetectFullScreenStateNotUsingFullScreenWebGLBecauseNotSupported)
        {
            ExitFullScreenMode();
        }
        else
        {
            CheckExitFullScreenRoutine();
        }   
    }

    private void CheckExitFullScreenRoutine()
    {
        if (exitFullScreenEnumerator == null && wasItFullScreen)
        {
            CustomDebug.Log("exitFullScreenEnumerator == null && wasItFullScreen");

            exitFullScreenEnumerator = CorExitFullScreenMode();

            StartCoroutine(exitFullScreenEnumerator);
        }
    }

    private IEnumerator CorExitFullScreenMode()
    {
        CustomDebug.Log("Entered CorExitFullScreenMode");
        
        Screen.fullScreenMode = FullScreenMode.Windowed;

        CustomDebug.Log("Screen.fullScreenMode = FullScreenMode.Windowed");

        yield return new WaitUntil(() => Screen.fullScreenMode.Equals(FullScreenMode.Windowed));

        CustomDebug.Log("now Screen.fullScreenMode.Equals(FullScreenMode.Windowed)");

        yield return new WaitUntil(() => !Screen.fullScreen);

        CustomDebug.Log("now screen.fullScreen == false");

        HandleOnExitFullScreen();
    }

    private void HandleOnExitFullScreen()
    {
        SetOridinaryUIView();

        DeActivateSettingBtnOnFullScreen();

        DeActivateSettingPopUpOnFullScreenMode();

        this.uIManager.SetSmartLearningYPosOffsetOnExitFullScreen();

        wasItFullScreen = false;

        exitFullScreenEnumerator = null;

        WebWindowController.SetNormalScreenState();

        CustomDebug.Log("~exit full screen Routine End~");
    }

    private void ExitFullScreenMode()
    {
        if (!FullscreenWebGL.isFullscreen())
        {
            return;
        }

        FullscreenWebGL.exitFullscreen(stat => {

            if (stat.Equals(status.Success))
            {
                SetOridinaryUIView();
            }
        });
    }

    private void SetOridinaryUIView()
    {
        this.upperLineControllerInSLMMode.ActivatePanelUpperLine();

        ActivatePanelLectureInfo();
        DeActivateSettingBtnOnFullScreen();

        DeActivateSettingPopUpOnFullScreenMode();

        //slmView.offsetMax = new Vector2(0, -100); // right - top
        //slmView.offsetMin = new Vector2(0, 140); // left - bottom
    }

    //[MonoPInvokeCallback(typeof(Action))]
    //private static void OnPressEscapeCallback()
    //{
    //    onPressEscapeKeycode?.Invoke();

    //    onPressEscapeKeycode = null;
    //}

    public void ActivateSettingPopUpOnFullScreenMode()
    {
        if (!dragAndDropModuleOnSettingBtn.ItCanDrag)
        {
            this.uIManager.ActivateSettingPopUpOnFullScreenMode();
        }
    }

    public void DeActivateSettingPopUpOnFullScreenMode()
    {
        this.uIManager.DeActivateSettingPopUpOnFullScreenMode();
    }

    public void ActivateSettingBtnOnFullScreen()
    {
        this.uIManager.ActivateSettingBtnOnFullScreen();
    }

    public void DeActivateSettingBtnOnFullScreen()
    {
        this.uIManager.DeActivateSettingBtnOnFullScreen();
    }

    // 나가기 팝업의 '예' 버튼에 참조되어 있음
    public void ExitWebGL()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
        WebWindowController.CloseWindow();
#endif
    }

    // 스마트학습 마무리 화면 출력
    public void ActivateExitPanelOnLessonComplete()
    {
        ActivateSpineCharacterOnComplete();

        SoundManager.Instance.PlayFX(EnumSets.FxType.LessonComplete);

        CheckNeedtoActivateExtraBtnsOnCompletePanel();

        this.uIManager.ActivateExitPanelOnLessonComplete();

        ActivateCommonDarkBG();
    }

    // 다시하기, 다음 강의로 이동, 나가기 버튼에 참조되어 있음
    public void DeActivateExitPanelOnLessonComplete()
    {
        this.uIManager.DeActivateExitPanelOnLessonComplete();

        DeActivateCommonDarkBG();
    }

    private void CheckNeedtoActivateExtraBtnsOnCompletePanel()
    {
        CheckNeedToActivateMoveNextButton();

        CheckNeedToActivateStageClearTitleOnLessonCompletePanel();

        CheckNeedToActivateGetRewardButton();

        CheckNeedToActivateConversionNoticeVerbalCloud();
    }

    private void CheckNeedToActivateMoveNextButton()
    {
        if (isCurrentLessonLastLesson)
        {
            this.uIManager.DeActivateMoveNextLessonBtnOnLessonCompletePanel();
        }
        else
        {
            this.uIManager.ActivateMoveNextLessonBtnOnLessonCompletePanel();
        }
    }

    private void CheckNeedToActivateStageClearTitleOnLessonCompletePanel()
    {
        if (isCurrentLessonLastLesson)
        {
            var stageClearTitleText = GetStageClearText();

            this.uIManager.ActivateStageClearTitleOnLessonCompletePanel(stageClearTitleText);
        }
        else
        {
            this.uIManager.DeActivateStageClearTitleOnLessonCompletePanel();
        }
    }

    public void CheckNeedToActivateGetRewardButton()
    {
        try
        {
            var isNeedToActivateGetRewardButton = false;

            var stageType = CurrentSmartLearningInfo.Instance.GetCurrentStageType();
            var lessonType = CurrentSmartLearningInfo.Instance.GetCurrentLessonType();

            var table = AppInfo.Instance.prizeItemInfoTable;

            var specificPrizeItem = from data in table
                                    where data.Value.stagePos.Equals(stageType) && data.Value.lessonPos.Equals(lessonType)
                                    select data;

            if (specificPrizeItem.Count() > 0)
            {
                if (!UserManager.Instance.GetIsUserReceivedReward(specificPrizeItem))
                {
                    isNeedToActivateGetRewardButton = true;

                    CustomDebug.Log("유저는 현재 레슨의 경품을 받지 않았다");
                }
                else
                {
                    CustomDebug.Log("유저는 현재 레슨의 경품을 받았다");
                }
            }
            else
            {
                CustomDebug.Log("경품을 주지 않는 레슨");
            }

            if (isNeedToActivateGetRewardButton)
            {
                if (AppInfo.Instance.IsGuestLogin)
                {
                    ActivateSignUpBtnOnLessonCompletePanel();
                }
                else
                {
                    // GetRewardNameArrayOfUnReceivedRewards() 수행 전에 유저의 prizeItem 틀은 잡혔고, 그 중 receive == false인 항목만 찾음
                    var arrayRewardNameArrayOfUnReceivedRewards = GetRewardNameArrayOfUnReceivedRewards(stageType, lessonType);

                    CustomDebug.Log($"강의 끝난 시점에 해당 레슨에서 아직 안받은 경품 개수 : {arrayRewardNameArrayOfUnReceivedRewards.Length}");

                    for (int i = 0; i < arrayRewardNameArrayOfUnReceivedRewards.Length; i++)
                    {
                        // 해당 레슨의 안받은 경품 배열을 돌면서
                        var rewardName = arrayRewardNameArrayOfUnReceivedRewards[i];

                        var unReceivedPrizeItem = from data in table
                                                  where data.Value.name.Equals(rewardName)
                                                  select data;

                        if (unReceivedPrizeItem.Count() > 0)
                        {
                            // rewardName이 같은 prizeItemInfo를 찾았다, 이름이 같은 것은 1개뿐
                            var prizeInfoId = unReceivedPrizeItem.ElementAt(0).Key;
                            var prizeItemInfo = unReceivedPrizeItem.ElementAt(0).Value;

                            var randomKey = GetSpecificRandomKey(rewardName);

                            string[] arrayLessonInfosWithPrizeInfo = { stageType, lessonType, prizeInfoId, randomKey };

                            SetRewardModuleInfo(arrayLessonInfosWithPrizeInfo, prizeItemInfo);
                        }
                    }

                    ActivateGetRewardBtnOnLessonCompletePanel();
                }
            }
            else
            {
                DeActivateGetRewardBtnOnLessonCompletePanel();

                DeActivateSignUpBtnOnLessonCompletePanel();
            }
        }
        catch(Exception e)
        {
            CustomDebug.LogError($"CheckNeedToActivateGetRewardButton error : {e.Message}");
        }
    }


    private void CheckNeedToActivateConversionNoticeVerbalCloud()
    {
        if (IsNeedToActivateConversionNoticeVerbalCloudOnLessonComplete())
        {
            ActivateVerbalCloudNextToSignUpBtnInLessonCompletePopUp();
        }
        else
        {
            DeActivateVerbalCloudNextToSignUpBtnInLessonCompletePopUp();
        }
    }

    private bool IsNeedToActivateConversionNoticeVerbalCloudOnLessonComplete()
    {
        var result = false;

        if (AppInfo.Instance.IsGuestLogin)
        {
            var stageType = CurrentSmartLearningInfo.Instance.GetCurrentStageType();

            if (stageType.Equals("s1"))
            {
                var lessonType = CurrentSmartLearningInfo.Instance.GetCurrentLessonType();

                if (lessonType.Equals("l1") || lessonType.Equals("l9"))
                {
                    result = true;
                }
            }
        }

        return result;
    }

    private string GetStageClearText()
    {
        var stageValue = CurrentSmartLearningInfo.Instance.GetCurrentStageValue();

        sb.Clear();

        sb.Append("Stage ");
        sb.Append(stageValue);
        sb.Append(".");
        sb.Append("\n");
        sb.Append("클리어 성공!");

        var text = sb.ToString();

        return text;
    }

    public void OnCompleteSmartLearning()
    {
        // 스마트학습 완료 시 캐릭터 스파인 객체들 강제 비활성화하기
        ForceAllDeActivateCharacters(); // 220906, khc 추가

        // 231116, khc 수정, 관리자페이지 용 kpi 기록은 이제 하지않음
        // TryUpdateKpiDataOnLessonComplete();

        StopRecordingLessonPlayingTime();

        this.uIManager.PlayLastAnimationOnSmartLearningEnd(() =>
        {
            LoadingManager.Instance.ActivateLoading();

            if (IsNeedToUpdateFirebaseUserDBOnCompleteLesson())
            {
                currentSkipCompleteValueForDBUpdate = 1;

#if UNITY_WEBGL && !UNITY_EDITOR
                UpdateFirebaseUserDBRoutineOnLessonEnd(CallbackOnUpdateDBSuccessWhenLessonComplete);

#elif UNITY_EDITOR
                CallbackOnUpdateDBSuccessWhenLessonComplete();

#endif
            }
            else
            {
                CustomDebug.Log("강의 끝나고 DB 업데이트 해야할 것이 없다");

                LoadingManager.Instance.DeActivateLoading();

                ActivateExitPanelOnLessonComplete();
            }
        });
    }

    // OnCompleteSmartLearning, OnClickSkipCurrentSmartLearningBtn 에도 참조되어 있음
    private void UpdateFirebaseUserDBRoutineOnLessonEnd(Action callbackOnSuccess)
    {
        var updateTable = GetTableToUpdateDBOnSkipOrCompleteLesson();

        var jsonType = DevUtil.Instance.GetJson(updateTable);

        CustomDebug.Log($"강의 끝나고 DB 업데이트 해야할 것이 있다 : \n{jsonType}");

        var mainPath = DevUtil.Instance.GetTargetPath(EnumSets.DBParentType.Users, UserManager.Instance.GetUID());

        FirebaseDatabaseController.Instance.UpdateJsonWithCallback(mainPath, jsonType, callbackOnSuccess, CallbackOnErrorUpdatingDB);
    }

    private void CallbackOnUpdateDBSuccessWhenLessonComplete()
    {
        LoadingManager.Instance.DeActivateLoading();

        stageCompleteRecorder.SetLocalDBValueOnCompleteSpecificLesson();

        UpdateDefaultPrizeItemToTargetLocalDB();

        ActivateExitPanelOnLessonComplete();
    }

    private void CallbackOnUpdateDBSuccessWhenLessonSkip()
    {
        stageCompleteRecorder.SetLocalDBValueOnSkipSpecificLesson();

        UpdateDefaultPrizeItemToTargetLocalDB();

        RoutineOnSkipLesson();
    }

    // 강의 완료시 kpi용 데이터를 쏴야 하는가
    private void TryUpdateKpiDataOnLessonComplete()
    {
        this.stageCompleteRecorder.TryUpdateKpiDataOnLessonComplete();
    }

    // 강의 스킵시 kpi용 데이터를 쏴야 하는가
    private void TryUpdateKpiDataOnLessonSkip()
    {
        this.stageCompleteRecorder.TryUpdateKpiDataOnLessonSkip();
    }

    private void CallbackOnErrorUpdatingDB()
    {
        LoadingManager.Instance.DeActivateLoading();

        ActivateSpecificPopUp(EnumSets.SLMPopUpType.UpdatingDBError);
    }

    private void UpdateDefaultPrizeItemToTargetLocalDB()
    {
        try
        {
            for(int i = 0; i < rewardInfoTable.Count; i++)
            {
                var stageType = CurrentSmartLearningInfo.Instance.GetCurrentStageType();
                var lessonType = CurrentSmartLearningInfo.Instance.GetCurrentLessonType();

                var rewardName = rewardInfoTable.ElementAt(i).Key;

                var randomKey = rewardInfoTable.ElementAt(i).Value[0];
                var category = rewardInfoTable.ElementAt(i).Value[1];

                var prizeData = new string[] { stageType, lessonType, rewardName, randomKey, category };

                UserManager.Instance.AddPrizeDataTemplateAfterLessonSkipOrComplete(prizeData);
            }
        }
        catch(Exception e)
        {
            CustomDebug.LogError($"UpdateDefaultPrizeItemToTargetLocalDB error : {e.Message}");
        }
    }

    private bool IsNeedToSetDefaultPrizeItemsToDB()
    {
        try
        {
            var result = false;

            var stageType = CurrentSmartLearningInfo.Instance.GetCurrentStageType();
            var lessonType = CurrentSmartLearningInfo.Instance.GetCurrentLessonType();
            
            var table = AppInfo.Instance.prizeItemInfoTable;

            var specificPrizeItem = from data in table
                                    where data.Value.stagePos.Equals(stageType) && data.Value.lessonPos.Equals(lessonType)
                                    select data;

            if (specificPrizeItem.Count() > 0) // 경품을 주는 레슨
            {
                // 해당 레슨에서 틀이 기록안된 경품의 개수
                var unRecordedRewardCountOfCurrentLesson = UserManager.Instance.GetCountOfCurrentLessonUnRecordedPrizeItemUnderUserDB(specificPrizeItem);

                if (unRecordedRewardCountOfCurrentLesson > 0)
                {
                    result = true;
                }
            }
            
            return result;
        }
        catch(Exception e)
        {
            CustomDebug.LogError($"IsNeedToSetDefaultPrizeItemsToDB error : {e.Message}");

            return false;
        }
    }

    private bool IsNeedToUpdateFirebaseUserDBOnCompleteLesson()
    {
        var result = false;

        if (IsNeedToSetDefaultPrizeItemsToDB() || stageCompleteRecorder.IsCurrentLessonNeedToSetDBValueOnComplete())
        {
            result = true;
        }

        return result;
    }

    private bool IsNeedToUpdateFirebaseUserDBOnSkipLesson()
    {
        var result = false;

        if (IsNeedToSetDefaultPrizeItemsToDB() || stageCompleteRecorder.IsCurrentLessonNeedsToSetDBValueOnSkip())
        {
            result = true;
        }

        return result;
    }

    // userDB 업데이트를 위한 UID 하위 path, value pair
    private Dictionary<string, object> GetTableToUpdateDBOnSkipOrCompleteLesson()
    {
        var updateTable = new Dictionary<string, object>();

        if (IsNeedToSetDefaultPrizeItemsToDB())
        {
            var stageType = CurrentSmartLearningInfo.Instance.GetCurrentStageType();
            var lessonType = CurrentSmartLearningInfo.Instance.GetCurrentLessonType();

            var lessonInfos = new string[] { stageType, lessonType };

            var table = AppInfo.Instance.prizeItemInfoTable;

            var specificPrizeItem = from data in table
                                    where data.Value.stagePos.Equals(stageType) && data.Value.lessonPos.Equals(lessonType)
                                    select data;
            
            var listOfRewardNameUnRecorded = UserManager.Instance.GetRewardNameListOfUnRecordedPrizeItemOfCurrentLesson(lessonInfos, specificPrizeItem);

            for (int i = 0; i < listOfRewardNameUnRecorded.Count; i++)
            {
                var rewardName = listOfRewardNameUnRecorded.ElementAt(i);

                var tablePathPrizeItemPair = UserManager.Instance.GetInitialRewardInfoAndDefaultPrizeItemPair(lessonInfos, rewardName);

                if(tablePathPrizeItemPair != null)
                {
                    var prizeItemPath = tablePathPrizeItemPair.ElementAt(0).Key[0];
                    var randomKey = tablePathPrizeItemPair.ElementAt(0).Key[1];
                    var category = tablePathPrizeItemPair.ElementAt(0).Key[2];

                    var prizeItem = tablePathPrizeItemPair.ElementAt(0).Value;

                    updateTable.Add(prizeItemPath, prizeItem);

                    var arrayRewardInfo = new string[] { randomKey, category };

                    rewardInfoTable.Add(rewardName, arrayRewardInfo);
                }
                else
                {
                    CustomDebug.LogError($"tablePathPrizeItemPair == null, stageType, lessonType, rewardName 모두 일치하는 prizeItemInfo 항목이 없다.");
                }
            }

            CustomDebug.Log($"prizeItems 틀 {listOfRewardNameUnRecorded.Count}개 만들기");
        }

        if (stageCompleteRecorder.IsCurrentLessonNeedToSetDBValueOnComplete())
        {
            var stageCompletedInfosSubPath = stageCompleteRecorder.GetTargetStageCompletedInfosPathStr();

            updateTable.Add(stageCompletedInfosSubPath, currentSkipCompleteValueForDBUpdate);

            CustomDebug.Log($"stageCompletedInfos 값 업데이트 => {currentSkipCompleteValueForDBUpdate}");
        }

        return updateTable;
    }

    public void ActivateVerbalCloudNextToSignUpBtnInLessonCompletePopUp()
    {
        this.uIManager.ActivateVerbalCloudNextToSignUpBtnInLessonCompletePopUp();
    }

    public void DeActivateVerbalCloudNextToSignUpBtnInLessonCompletePopUp()
    {
        this.uIManager.DeActivateVerbalCloudNextToSignUpBtnInLessonCompletePopUp();
    }

    // 다음 강의로 이동에 참조되어 있음
    public void LoadNextSmartLearningMode()
    {
        Destroy(currentSLMObject);

        ForceAllDeActivateCharacters();

        DeActivateMiddleGuidePanel();

        ResetData();

        CurrentSmartLearningInfo.Instance.InitializeNextSmartLearningInfo(() =>
        {
            DeActivateCurrentActivatedPopUp();

            InitSmartLearningMode();
        });
    }

    public void QuitSmartLearningModeAndGoBackToHomeScene()
    {
        LoadingManager.Instance.ActivateLoading();

        DeActivateAllCharactersOnComplete();

        QuitSmartLearningMode();

        ReturnBackToHomeScene();
    }

    public void QuitSmartLearningMode()
    {
        Destroy(currentSLMObject);

        ForceAllDeActivateCharacters();

        DeActivateMiddleGuidePanel();

        ResetData();

        CurrentSmartLearningInfo.Instance.ResetData();
    }

    // 뒤로가기 - 예, 강의 완료 팝업 - 나가기 버튼에 참조되어 있음
    public void QuitSmartLearningAndGoBackToLessonList()
    {
        // 대시보드 플레이시간, 학습관리 플레이시간 기록 필요 or SLMStageSequence OnDisable때?

        var stageType = CurrentSmartLearningInfo.Instance.GetCurrentStageType();
        var lessonType = CurrentSmartLearningInfo.Instance.GetCurrentLessonType();

        UserManager.Instance.SetUserLastPlayingStageType(stageType); // 뒤로가기, 강의완료팝업 - 나가기 시에 레슨리스트로 복귀하기 위한 세팅

        DeActivateCurrentActivatedPopUp();

        QuitSmartLearningModeAndGoBackToHomeScene();
    }

    // 하단 '이번 연구 건너뛰기' - 예 버튼에 참조되어 있음
    public void OnClickSkipCurrentSmartLearningBtn()
    {
        StartCoroutine(CorSkipCurrentLesson());

        #region
        //        DeActivateCurrentActivatedPopUp();

        //        LoadingManager.Instance.ActivateLoading();

        //        TryUpdateKpiDataOnLessonSkip();

        //        StopRecordingLessonPlayingTime();

        //        if (IsNeedToUpdateFirebaseUserDBOnSkipLesson())
        //        {
        //            currentSkipCompleteValueForDBUpdate = -1;

        //#if UNITY_WEBGL && !UNITY_EDITOR
        //            UpdateFirebaseUserDBRoutineOnLessonEnd(CallbackOnUpdateDBSuccessWhenLessonSkip);

        //#elif UNITY_EDITOR
        //            CallbackOnUpdateDBSuccessWhenLessonSkip();
        //#endif
        //        }
        //        else
        //        {
        //            CustomDebug.Log("강의 끝나고 DB 업데이트 해야할 것이 없다");

        //            RoutineOnSkipLesson();
        //        }
        #endregion
    }

    private IEnumerator CorSkipCurrentLesson()
    {
        StopRecordingLessonPlayingTime();

        LoadingManager.Instance.ActivateLoading();

        // 231116, khc 수정, 관리자페이지 용 kpi 기록은 이제 하지않음
        // TryUpdateKpiDataOnLessonSkip();

        yield return delayWhileUnitaskTokenBeCancelled; // playingTime recorder cancel 기다리기

        DeActivateCurrentActivatedPopUp();

        if (IsNeedToUpdateFirebaseUserDBOnSkipLesson())
        {
            currentSkipCompleteValueForDBUpdate = -1;

#if UNITY_WEBGL && !UNITY_EDITOR
            UpdateFirebaseUserDBRoutineOnLessonEnd(CallbackOnUpdateDBSuccessWhenLessonSkip);

#elif UNITY_EDITOR
            CallbackOnUpdateDBSuccessWhenLessonSkip();
#endif
        }
        else
        {
            CustomDebug.Log("강의 끝나고 DB 업데이트 해야할 것이 없다");

            RoutineOnSkipLesson();
        }
    }

    private void RoutineOnSkipLesson()
    {
        LoadingManager.Instance.DeActivateLoading();

        if (isCurrentLessonLastLesson)
        {
            ForceAllDeActivateCharacters();

            ActivateExitPanelOnLessonComplete(); // 스킵했을 때 클리어 팝업이 뜨는 경우는 마지막 강의일 경우만
        }
        else
        {
            LoadNextSmartLearningMode();
        }
    }

    // updateDBErrorPopUp - 재시도 에 참조되어 있음
    public void OnClickRetryUpdateDBOnLessonEnd()
    {
        DeActivateSpecificPopUp(EnumSets.SLMPopUpType.UpdatingDBError);

        RetryUpdateFirebaseUserDBOnLessonEnd();
    }

    private void RetryUpdateFirebaseUserDBOnLessonEnd()
    {
        Action callback = null;

        LoadingManager.Instance.ActivateLoading();

        if(currentSkipCompleteValueForDBUpdate == 1)
        {
            CustomDebug.Log("DB에 강의 완료값 업데이트 도중 에러 발생했었음");
            
            callback = CallbackOnUpdateDBSuccessWhenLessonComplete;
        }
        else
        {
            if(currentSkipCompleteValueForDBUpdate == -1)
            {
                CustomDebug.Log("DB에 강의 스킵값 업데이트 도중 에러 발생했었음");

                callback = CallbackOnUpdateDBSuccessWhenLessonSkip;
            }
            else
            {
                ActivateSpecificPopUp(EnumSets.SLMPopUpType.UpdatingDBError);

                CustomDebug.LogError($"RetryUpdateDBRoutine error, currentSkipCompleteValueForDBUpdate : {currentSkipCompleteValueForDBUpdate}");

                return;
            }
        }

        UpdateFirebaseUserDBRoutineOnLessonEnd(callback);
    }

    // Close Panel의 다시하기에 참조되어 있음
    public void RetrySameSmartLearning()
    {
        // StartCoroutine(CorRetrySameSmartLearing());

        Destroy(currentSLMObject);

        ResetMiddleGuideBubbleColor();

        DeActivateMiddleGuidePanel();

        currentSLMObject = null;

        DeActivateAllCharactersOnComplete();

        this.uIManager.ResetData();

        learningObjectiveManager.ResetData();

        var smartLearningName = CurrentSmartLearningInfo.Instance.GetCurrentSmartLearningName();

        ActivateTargetLearningMode(smartLearningName);
    }

    public void ActivateTargetLearningMode(string smartLearningName)
    {
        InitializeTargetSmartLearning(smartLearningName);
    }

    #region no need to yield return
    //IEnumerator CorRetrySameSmartLearing()
    //{
    //    Destroy(currentSLMObject);

    //    currentSLMObject = null;

    //    this.uIManager.ResetData();

    //    learningObjectiveManager.ResetData();

    //    yield return delayRetrySmartLearning;   // 스마트학습 말미에 있는 GuidePanel의 OnDisable과 재시작할 때 0번째의 GuidePanel의 OnEnable 타이밍 충돌 회피용

    //    ActivateTargetLearningMode();
    //}
#endregion

    private void InitializeTargetSmartLearning(string smartLearningName)
    {
        try
        {
            var prefabPathStr = GetPrefabFilePathStr(smartLearningName);

            currentSLMObject = Instantiate(Resources.Load(prefabPathStr), parentObjOfSLMPrefab) as GameObject;
            
            simpleZoom = currentSLMObject.GetComponent<SimpleZoom>();

            this.uIManager.SetCurrentSimpleZoomAndInitializeMiddleGuidePanelFadeModule(simpleZoom);

            this.uIManager.SetCurrentSimpleZoomAndInitializeSpineFadeModules(simpleZoom);

            currentSLMObject.transform.SetAsFirstSibling();

            InitializeStageCompleteRecorder();

            InitializeLearningModeInfo();

            SetProperActionInSLMControllModule();

            CheckCurrentLessonLastOfStage();

            ResetRewardInfo();

            // 231116, khc 수정, 최근 시청기록을 기록하는 것을 봉인 - 홈페이지 출력용이므로
            // UpdateRecentTryingLectureTable();
        }
        catch (Exception e)
        {
            Debug.LogError($"error loading smartlearningmode : {e.Message}");

            ActivateSpecificPopUp(EnumSets.SLMPopUpType.LoadPrefabError);
        }
    }

    public void UpdateLearningObjective(int currentProgressIndex)
    {
        this.learningObjectiveManager.LoadNextLearningObjective(currentProgressIndex);
    }

    private void InitializeLearningModeInfo()
    {
        this.learningObjectiveManager.InitializeLearningObjectiveInfo(learningModeInfo);
    }

    private void InitializeStageCompleteRecorder()
    {
        StopRecordingLessonPlayingTime();

        this.stageCompleteRecorder.ResetData();

        CheckCurrentLessonCompleteState();

        // 231116, khc 수정, 플레잉타임 기록루틴을 봉인
        // this.stageCompleteRecorder.RecordPlayingTime();
    }

    private void UpdateRecentTryingLectureTable()
    {
        var stageType = CurrentSmartLearningInfo.Instance.GetCurrentStageType();
        var lessonType = CurrentSmartLearningInfo.Instance.GetCurrentLessonType();
        var lessonTitleStr = StageLessonDataManager.Instance.GetLessonTitle(stageType, lessonType);

        var lessonInfos = new string[] { stageType, lessonType, lessonTitleStr };

        recentStudyDataRecordModule.UpdateRecentTryingLectureInfos(lessonInfos);
    }

    public void StopRecordingLessonPlayingTime()
    {
        this.stageCompleteRecorder.ForceCancelRecordingPlayingTimeTask();
    }

    private void CheckCurrentLessonCompleteState()
    {
        var stageType = CurrentSmartLearningInfo.Instance.GetCurrentStageType();
        var lessonType = CurrentSmartLearningInfo.Instance.GetCurrentLessonType();

        var stageValue = CurrentSmartLearningInfo.Instance.GetCurrentStageValue();
        var lessonValue = CurrentSmartLearningInfo.Instance.GetCurrentLessonValue();

        var stageLessonTypes = new string[] { stageType, lessonType };
        var stageLessonValues = new int[] { stageValue, lessonValue };

        this.stageCompleteRecorder.SetCurrentLessonInfo(stageLessonTypes, stageLessonValues);

        this.stageCompleteRecorder.CheckCurrentLessonCompleteState();
    }

    private void SetProperActionInSLMControllModule()
    {
        slmControllModule = currentSLMObject.GetComponent<SLMControllModule>();

        slmControllModule.SetSpecificLearningObjectiveSuccessCallback(UpdateLearningObjective);

        slmControllModule.SetCallbackSettingSentenceInMiddleGuidePanel(SetSentenceInMiddleGuidePanel);

        // tmp complete lesson test
        tmpCompleteLessonBtn.onClick.RemoveAllListeners();
        tmpCompleteLessonBtn.onClick.AddListener(slmControllModule.CompleteSmartLearning);
        // tmp complete lesson test
    }

    private void CheckCurrentLessonLastOfStage()
    {
        var stageValue = CurrentSmartLearningInfo.Instance.GetCurrentStageValue();
        var lessonValue = CurrentSmartLearningInfo.Instance.GetCurrentLessonValue();
        
        isCurrentLessonLastLesson = StageLessonDataManager.Instance.IsThisLastLessonOfCurrentStage(stageValue, lessonValue);
    }

    // 경품------------------------------------------

    // 클로즈 패널의 '선물 받기' 버튼에 참조돼 있음
    public void OnClickGetRewardButton()
    {
        this.rewardModule.OnClickGettingGift();
    }

    private void SetRewardModuleInfo(string[] arrayLessonInfosWithPrizeInfo, PrizeItemInfo prizeItemInfo)
    {
        if (rewardModule == null)
        {
            rewardModule = gameObject.AddComponent<RewardModuleInSLM>();
        }

        rewardModule.SetRewardModuleInfoTable(arrayLessonInfosWithPrizeInfo, prizeItemInfo);
    }

    private string GetSpecificRandomKey(string rewardName)
    {
        var randomKey = "";

        var stageType = CurrentSmartLearningInfo.Instance.GetCurrentStageType();
        var lessonType = CurrentSmartLearningInfo.Instance.GetCurrentLessonType();

        var specificPrizeItemTable = UserManager.Instance.GetSpecificGottenPrizeItemInfos(stageType, lessonType);

        var tableUnReceivedRewardFromCurrentLesson = from data in specificPrizeItemTable
                                                     where data.Value.type.Equals(rewardName)
                                                     select data;

        if(tableUnReceivedRewardFromCurrentLesson.Count() > 0)
        {
            randomKey = tableUnReceivedRewardFromCurrentLesson.ElementAt(0).Key;
        }

        return randomKey;
    }

    private string[] GetRewardNameArrayOfUnReceivedRewards(string stageType, string lessonType)
    {
        string[] arrayRewardNames = null;

        var specificPrizeItemTable = UserManager.Instance.GetSpecificGottenPrizeItemInfos(stageType, lessonType);

        var tableUnReceivedRewardFromCurrentLesson = from data in specificPrizeItemTable
                                                     where !data.Value.receive
                                                     select data;

        if (tableUnReceivedRewardFromCurrentLesson.Count() > 0)
        {
            var countUnReceivedReward = tableUnReceivedRewardFromCurrentLesson.Count();

            arrayRewardNames = new string[countUnReceivedReward];

            for (int i = 0; i < countUnReceivedReward; i++)
            {
                arrayRewardNames[i] = tableUnReceivedRewardFromCurrentLesson.ElementAt(i).Value.type;

                CustomDebug.Log($"arrayRewardNames[{i}] : {arrayRewardNames[i]}");
            }
        }
        else
        {
            CustomDebug.LogError($"여기로 들어올 일 없음");
        }

        return arrayRewardNames;
    }

    public void ActivateGetRewardBtnOnLessonCompletePanel()
    {
        this.uIManager.ActivateGetRewardBtnOnLessonCompletePanel();
    }

    public void DeActivateGetRewardBtnOnLessonCompletePanel()
    {
        this.uIManager.DeActivateGetRewardBtnOnLessonCompletePanel();
    }

    public void ActivateSignUpBtnOnLessonCompletePanel()
    {
        this.uIManager.ActivateSignUpBtnOnLessonCompletePanel();
    }

    public void DeActivateSignUpBtnOnLessonCompletePanel()
    {
        this.uIManager.DeActivateSignUpBtnOnLessonCompletePanel();
    }

    // middleGuidePanel------------------------------

    public string GetProperSentenceText(string guideLabelKey)
    {
        string value = "";

        value = learningModeInfo.middleGuideSentence[guideLabelKey] as string;

        return value;
    }

    public void SetSentenceInMiddleGuidePanel(string guidePanelKey)
    {
        string value = GetProperSentenceText(guidePanelKey);

        this.uIManager.SetSentenceInMiddleGuidePanel(value);
    }

    public void ActivateMiddleGuidePanel()
    {
        this.uIManager.ActivateMiddleGuidePanel();
    }

    public void ActivateMiddleGuidePanelWithPos(int middleGuideXPos, int middleGuideYPos)
    {
        this.uIManager.ActivateMiddleGuidePanelWithPos(middleGuideXPos, middleGuideYPos);
    }

    [Obsolete]
    public void ActivateTipMiddleGuidePanelWithPos(float middleGuideXPos, float middleGuideYPos)
    {
        this.uIManager.ActivateTipMiddleGuidePanelWithPos(middleGuideXPos, middleGuideYPos);
    }

    public void SetMiddleGuideBubbleTexture(int index)
    {
        this.uIManager.SetMiddleGuideBubbleTexture(index);
    }

    public void SetMiddleGuideBubblePivot(float xValue, float yValue)
    {
        this.uIManager.SetMiddleGuideBubblePivot(xValue, yValue);
    }

    public void EnableButtonInMiddleGuideBubble()
    {
        this.uIManager.EnableButtonInMiddleGuideBubble();
    }

    public void DisableButtonInMiddleGuideBubble()
    {
        this.uIManager.DisableButtonInMiddleGuideBubble();
    }

    public void SetMiddleGuideBubblePadding(int[] arrayPaddingValues)
    {
        this.uIManager.SetMiddleGuideBubbleVerticalLayoutPadding(arrayPaddingValues);
    }

    // middleGuidePanel의 Button에 참조되어 있음
    public void SkipMiddleGuidePanel()
    {
        if(slmControllModule != null && ! simpleZoom.IsZoomedInState())
        {
            slmControllModule.OnClickSkipBtnOnMiddleGuidePanel();

            DeActivateMiddleGuidePanel();

            DisableImageComponentInMiddleGuidePanel();

            DisableMiddleGuidePanelVerticalLayoutGroup();
        }
    }

    public void DeActivateMiddleGuidePanel()
    {
        this.uIManager.DeActivateMiddleGuidePanel();
    }

    public void DisableImageComponentInMiddleGuidePanel()
    {
        this.uIManager.DisableImageComponentInMiddleGuidePanel();
    }

    public void ActivateUpperQuizTitleImage()
    {
        this.uIManager.ActivateUpperQuizTitleImage();
    }

    public void ActivateUpperGameTitleImage()
    {
        this.uIManager.ActivateUpperGameTitleImage();
    }

    public void ActivateUpperTipTitleImage()
    {
        this.uIManager.ActivateUpperTipTitleImage();
    }

    public void DeActivateMiddleGuideUpperTitleImages()
    {
        this.uIManager.DeActivateMiddleGuideUpperTitleImages();
    }

    public void EnableMiddleGuidePanelVerticalLayoutGroup()
    {
        this.uIManager.EnableMiddleGuidePanelVerticalLayoutGroup();
    }

    public void DisableMiddleGuidePanelVerticalLayoutGroup()
    {
        this.uIManager.DisableMiddleGuidePanelVerticalLayoutGroup();
    }

    public void SetMiddleGuideBubbleAlpha(float alpha)
    {
        this.uIManager.SetMiddleGuideBubbleAlpha(alpha);
    }

    public void ResetMiddleGuideBubbleColor()
    {
        this.uIManager.ResetMiddleGuideBubbleColor();
    }

    public SimpleZoom GetCurrentSimpleZoom()
    {
        return this.simpleZoom;
    }

    public RewardModule GetRewardModule()
    {
        return this.rewardModule;
    }

    public List<RewardModule> GetRewardModuleList()
    {
        return this.listRewardModule;
    }

    // PopUps-----------------------------------------

    public void ActivateSpecificPopUp(int enumOrder)
    {
        var popUpType = (EnumSets.SLMPopUpType)enumOrder;

        this.slMPopUpManager.ActivatePopUp(popUpType);
    }

    public void ActivateSpecificPopUp(EnumSets.SLMPopUpType popUpType)
    {
        this.slMPopUpManager.ActivatePopUp(popUpType);
    }

    public void DeActivateSpecificPopUp(int enumOrder)
    {
        var popUpType = (EnumSets.SLMPopUpType)enumOrder;

        this.slMPopUpManager.DeActivatePopUp(popUpType);

        afterSpecificPopUpDeActivated?.Invoke();
        afterSpecificPopUpDeActivated = null;
    }

    public void DeActivateSpecificPopUp(EnumSets.SLMPopUpType popUpType)
    {
        this.slMPopUpManager.DeActivatePopUp(popUpType);

        afterSpecificPopUpDeActivated?.Invoke();
        afterSpecificPopUpDeActivated = null;
    }

    public void SetCallbackAfterSpecificPopUpDeActivated(Action callback)
    {
        this.afterSpecificPopUpDeActivated = callback;
    }

    // 현재 활성화된 팝업과 CommonDarkBG도 같이 꺼짐
    public void DeActivateCurrentActivatedPopUp()
    {
        this.slMPopUpManager.DeActivateCurrentActivatedPopUp();

        afterSpecificPopUpDeActivated?.Invoke();
        afterSpecificPopUpDeActivated = null;
    }

    public void ActivateCommonErrorPopUp(EnumSets.ErrorType errorType)
    {
        this.slMPopUpManager.ActivateCommonErrorPopUp(errorType);
    }

    public void SetCallbackAfterCommonErrorPopUpDeActivated(Action callback)
    {
        this.afterCommonPopUpDeActivated = callback;
    }

    // CommonErrorPopUp의 Confirm 버튼에 참조되어 있음
    public void DeActivateCommonErrorPopUp()
    {
        var popUpType = EnumSets.SLMPopUpType.CommonError;

        this.slMPopUpManager.DeActivatePopUp(popUpType);

        afterCommonPopUpDeActivated?.Invoke();
        afterCommonPopUpDeActivated = null;
    }

    public void ActivateExitWebGLPopUp()
    {
        if (!AppInfo.Instance.IsGuestLogin)
        {
            this.uIManager.ActivateExitWebGLPopUpWhenSignedIn();
        }
        else
        {
            this.uIManager.ActivateExitWebGLPopUpWhenGuest();
        }

        ActivateCommonDarkBG();
    }

    public void DeActivateExitWebGLPopUp()
    {
        this.uIManager.DeActivateExitWebGLPopUp();

        DeActivateCommonDarkBG();
    }

    public void ReturnBackToHomeScene()
    {
        StartCoroutine(CorMoveToHomeScene());
    }

    private IEnumerator CorMoveToHomeScene()
    {
        //CustomDebug.LogError("CorMoveToHomeScene wait until 1s");
        //yield return new WaitForSeconds(1f);
        //CustomDebug.LogError("CorMoveToHomeScene 1s delayed");
        var async = SceneManager.LoadSceneAsync(SCENE_NAME_HOME);

        yield return async;
    }

    private string GetPrefabFilePathStr(string smartLearningPrefabName)
    {
        sb.Clear();
        sb.Append(SMART_LEARNING_UNDER_RESOURCES_PATH_STR);
        sb.Append(smartLearningPrefabName);

        var prefabPathStr = sb.ToString();

        return prefabPathStr;
    }

    // 스마트학습 클리어 캐릭터 출력 ------------------------------------

    public void ActivateSpineCharacterOnComplete()
    {
        var lessonValue = CurrentSmartLearningInfo.Instance.GetCurrentLessonValue();

        var indexCharacterOnComplete = lessonValue % 2;

        this.uIManager.ActivateCharacterOnComplete(indexCharacterOnComplete);
    }

    public void DeActivateAllCharactersOnComplete()
    {
        this.uIManager.DeActivateAllCharactersOnComplete();
    }

    public void ActivateAllSpriteMaskUnderCompleteCharacters()
    {
        this.uIManager.ActivateAllSpriteMaskUnderCompleteCharacters();
    }

    // 컴플리트 패널의 캐릭터 스파인이 경품 팝업을 뚫고 나와 마스크를 껐다 켜줌
    public void AllDeActivateSpriteMaskUnderCompleteCharacters()
    {
        this.uIManager.AllDeActivateSpriteMaskUnderCompleteCharacters();
    }

    // ----------------------------
    public void ActivateCommonDarkBG()
    {
        this.uIManager.ActivateCommonDarkBG();
    }

    public void DeActivateCommonDarkBG()
    {
        this.uIManager.DeActivateCommonDarkBG();
    }

    // CommonBG-DarkBg의 DelayOpenNextStep에 참조되어 있음
    public void SetZoomableOnCurrentSimpleZoom()
    {
        if (simpleZoom)
        {
            this.simpleZoom.SetZoomable();
        }
    }

    // CommonBG-DarkBg의 UnityEventOnDisable에 참조되어 있음
    public void SetUnZoomableOnCurrentSimpleZoom()
    {
        if (simpleZoom)
        {
            this.simpleZoom.SetUnZoomable();
        }
    }

    private void ResetData()
    {
        currentSLMObject = null;

        uIManager.ResetData();

        learningObjectiveManager.ResetData();

        learningModeInfo = null;

        slmControllModule = null;

        simpleZoom = null;
    }

    private void ResetRewardInfo()
    {
        rewardInfoTable.Clear();

        if (rewardModule != null)
        {
            Destroy(rewardModule);

            rewardModule = null;
        }
    }

    //------------------------------------------------------------------
    // 캐릭터 스파인 활성화 관련 로직

    public void DeActivateCharacter(EnumSets.CharacterType characterType)
    {
        this.characterSpineHandlingModule.DeActivateCharacter(characterType);
    }

    public void ActivateFemaleCharacter(EnumSets.CharacterSpineSpecialEmotion emotion, EnumSets.CharacterPos pos)
    {
        this.characterSpineHandlingModule.ActivateFemaleCharacter(emotion, pos);
    }

    public void ActivateMaleCharacter(EnumSets.CharacterSpineSpecialEmotion emotion, EnumSets.CharacterPos pos)
    {
        this.characterSpineHandlingModule.ActivateMaleCharacter(emotion, pos);
    }

    public void ActivateFemaleCharacter(EnumSets.CharacterSpineSpecialEmotion emotion, Vector2[] posInfo)
    {
        this.characterSpineHandlingModule.ActivateFemaleCharacter(emotion, posInfo);
    }

    public void ActivateMaleCharacter(EnumSets.CharacterSpineSpecialEmotion emotion, Vector2[] posInfo)
    {
        this.characterSpineHandlingModule.ActivateMaleCharacter(emotion, posInfo);
    }

    /// <summary>
    /// 현재 활성화된 모든 캐릭터 스파인들 비활성화하기
    /// </summary>
    public void ForceAllDeActivateCharacters()
    {
        this.characterSpineHandlingModule.ForceAllDeActivateCharacters();
    }
}

