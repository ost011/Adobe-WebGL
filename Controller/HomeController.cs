using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarksAssets.FullscreenWebGL;
using System.Linq;

using status = MarksAssets.FullscreenWebGL.FullscreenWebGL.status;
using navigationUI = MarksAssets.FullscreenWebGL.FullscreenWebGL.navigationUI;
using UnityEngine.SceneManagement;
using System.Text;
using UnityEditor;

public class HomeController : MonoBehaviour
{
    private static HomeController instance = null;
    public static HomeController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<HomeController>();
            }

            return instance;
        }
    }


    public HomeUIManager uiManager;
    public SmartLearningModeController slmController;

    public UpperLineController upperLineController;

    public VideoPlayerController videoPlayerController;

    [Obsolete]
    [Space]
    public TutorialModalManager tutorialModalManager;

    [Space]
    public StageController stageController;

    [Space]
    public LogInControllerInHome loginController;

    [Space]
    public PopUpModule[] popUps;
    private IPopUp currentOpenedPopUp = null;
    private Stack<IPopUp> stackPopUp = new Stack<IPopUp>();

    [Space]
    [SerializeField]
    private EnumSets.LobbyType lobbyType = EnumSets.LobbyType.LoginLobby;
    [SerializeField]
    private EnumSets.PanelType panelType = EnumSets.PanelType.ChapterBtnPanel;

    private Stack<EnumSets.PanelType> backwardStack = new Stack<EnumSets.PanelType>();

    private Action afterCommonPopUpDeActivated = null;

    private HomeControl homeControl = null;

    private StringBuilder sb = new StringBuilder();

    private const string SCENE_NAME_SMART_LEARNING_MODE = "SmartLearningMode";
    
    private void Awake()
    {
        InitHomeControl();
    }

    private void InitHomeControl()
    {
        if (!AppInfo.Instance.IsGuestLogin) // 게스트 로그인이 아니라면
        {
            this.homeControl = new HomeControl(new SignedInType());
        }
        else
        {
            this.homeControl = new HomeControl(new GuestType());
        }

        this.homeControl.InitView(this.uiManager);
        this.homeControl.RecordingUserPlayingTime();

    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        SetUpperUserEmailText();
        SetUpperExtraFuncBtnText();

        // tutorialModalManager.ActivateModal();

        // 스마트학습 씬에 DocsManager 추가 221012 오승택
        // DocsManager.Instance.LoadTextFilesWhileInitiatingApp();

        AppInfo.Instance.GetPrizeItemInfosFromServer();

#if UNITY_EDITOR
        UserManager.Instance.RemoveOldPrizeItemInfos(); // editor
#endif

        // 튜토리얼 진행여부에 따라 GuideAnimationManager에서 BGM 선택-재생함
        // SoundManager.Instance.PlayBGM(EnumSets.BGMType.Lobby);

        // 학습 플레이 도중 뒤로가기 통해 홈씬오게 됐을 때 레슨리스트를 띄우기 위해 추가 220923 오승택
        CheckNeedToShowLessonListWhenLoadedFromSmartLearningMode();

        // 혹시나 레이아웃 조절 루틴이 안돌아갈 것을 생각해서
        // fullscreen bool 값을 false 로 재조정
        WebWindowController.SetNormalScreenState();
    }

    public void OnClickOpenChapterBtn()
    {
        SetLobbyType(EnumSets.LobbyType.MainLobby);
        SetPanelType(EnumSets.PanelType.ChapterBtnPanel);

        ActivateTargetPanel();
    }

    public void OnClickOpenLessonBtn()
    {
        SetLobbyType(EnumSets.LobbyType.MainLobby);
        SetPanelType(EnumSets.PanelType.LessonMapPanel);

        ActivateTargetPanel();
    }

    public void OnClickLoadVideoLectureModeBtn()
    {
        SetLobbyType(EnumSets.LobbyType.MainLobby);
        SetPanelType(EnumSets.PanelType.VideoLecturePanel);

        ActivateTargetPanel();
    }

    /*
    public void OnClickLoadSmartLearningModeBtn()
    {
        SetLobbyType(EnumSets.LobbyType.MainLobby);
        SetPanelType(EnumSets.PanelType.SmartLearningModePanel);

        ActivateTargetPanel();
        
        this.upperLineController.ActivateFullScreenBtn();
    }
    */

    public void OnClickOpenRewardPanelBtn()
    {
        SetLobbyType(EnumSets.LobbyType.MainLobby);
        SetPanelType(EnumSets.PanelType.RewardBtnPanel);

        ActivateTargetPanel();
    }

    private void SetLobbyType(EnumSets.LobbyType lobbyType)
    {
        this.lobbyType = lobbyType;
    }

    private void SetPanelType(EnumSets.PanelType panelType)
    {
        this.panelType = panelType;
    }

    private void ActivateTargetPanel()
    {
        this.uiManager.ActivateTargetPanel();
    }

    private void AllDeActivateMainLobbyPanels()
    {
        this.uiManager.AllDeActivateMainLobbyPanels();
    }

    public void ForceDeActivateLoginLobbyPanel()
    {
        this.uiManager.ForceDeActivateLoginLobbyPanel();
    }

    public EnumSets.LobbyType GetCurrentLobbyType()
    {
        return this.lobbyType;
    }

    public EnumSets.PanelType GetCurrentPanelType()
    {
        return this.panelType;
    }

    public void AddPanelDataToBackwardStack(EnumSets.PanelType panelType)
    {
        this.backwardStack.Push(panelType);

        Debug.Log($"------- AddPanelDataToBackwardStack : {panelType} / count : {this.backwardStack.Count}");

        CheckNeedToShowBackwardBtn();
    }

    private void CheckNeedToShowBackwardBtn()
    {
        if (this.backwardStack.Count > 1)
        {
            this.upperLineController.ActivateBackwardBtn();
        }
        else
        {
            if (this.backwardStack.Count == 1)
            {
                var currentStackedItem = this.backwardStack.Peek();

                if (currentStackedItem.Equals(EnumSets.PanelType.ChapterBtnPanel))
                {
                    this.upperLineController.DeActivateBackwardBtn();
                }
            }
        }
    }

    public void OnClickBackwardBtn()
    {

        DeActivateStackedPanel();
        //if (this.slmController.GetIsSmartLearningActivated())
        //{
        //    // 스마트학습 진행 중이니 스마트학습을 끌 지 여부 묻는 팝업 출력

        //    this.slmController.ActivateAskingExitSmartLearningPopUp();
        //}
        //else
        //{
        //    DeActivateStackedPanel();
        //}
    }

    private void CheckWhatPanelShowing(EnumSets.PanelType nextTargetPanel)
    {
        switch (nextTargetPanel)
        {
            case EnumSets.PanelType.ChapterBtnPanel:
                {
                    OnClickOpenChapterBtn();
                }
                break;
            case EnumSets.PanelType.LessonMapPanel:
                {
                    OnClickOpenLessonBtn();
                }
                break;
            case EnumSets.PanelType.SmartLearningModePanel:
                {
                    OnClickLoadSmartLearningModeBtn();
                }
                break;
            case EnumSets.PanelType.RewardBtnPanel:
                {
                    OnClickOpenRewardPanelBtn();
                }
                break;
        }
    }

    private void CheckNeedToDeActivateFullScreenBtn(EnumSets.PanelType currentActivatedPanel)
    {
        if (panelType.Equals(EnumSets.PanelType.SmartLearningModePanel))
        {
            this.upperLineController.DeActivateFullScreenBtn();
        }
    }

    private EnumSets.PanelType GetCurrentStackedPanel()
    {
        var activatedPanel = this.backwardStack.Pop();

        return activatedPanel;
    }

    public void SetScreenReSize()
    {
        Screen.fullScreen = !Screen.fullScreen;

        Screen.SetResolution(1920, 1080, Screen.fullScreen);
    }

    // ------------------------------------------------
    // FullScreen 세팅 관련

    public RectTransform slmView;

    public void SetFullScreenMode()
    {
        // 상단 전체화면 버튼 끄고
        // 스마트학습 뷰 확장
        // 하단 학습목표 뷰 비활성화

        EnterFullScreenMode();
    }

    private void EnterFullScreenMode()
    {
        if (FullscreenWebGL.isFullscreen())
        {
            return;
        }

        FullscreenWebGL.requestFullscreen((stat) => {

            if (stat.Equals(status.Success))
            {
                SetFullScreenUIView();
            }
        }, navigationUI.hide);
    }

    private void SetFullScreenUIView()
    {
        this.upperLineController.DeActivatePanelUpperLine();

        this.slmController.DeActivatePanelLectureInfo();

        slmView.offsetMax = Vector2.zero;
        slmView.offsetMin = Vector2.zero;
    }

    public void SetOrdinaryScreenMode()
    {
        ExitFullScreenMode();
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
        this.upperLineController.ActivatePanelUpperLine();

        this.slmController.ActivatePanelLectureInfo();
        this.slmController.DeActivateSettingBtnOnFullScreen();

        this.slmController.DeActivateSettingPopUpOnFullScreenMode();

        slmView.offsetMax = new Vector2(0, -120); // right - top
        slmView.offsetMin = new Vector2(0, 120); // left - bottom
    }

    private void SubsribeFullScreenChangedEvent()
    {
        if (FullscreenWebGL.isFullscreenSupported())
        {
            FullscreenWebGL.subscribeToFullscreenchangedEvent();

            FullscreenWebGL.onfullscreenchange += CheckFullScreenState;
        }
    }

    private void CheckFullScreenState()
    {
        if (FullscreenWebGL.isFullscreen())
        {
            // SetFullScreenMode();
            Debug.Log("화면이 커졌다");
        }
        else
        {
            SetOridinaryUIView();
        }
    }

    //------------------------------------------
    // video 세팅 관련

    public void SetWhatVideoPlaying(string videoFileName)
    {
        this.videoPlayerController.OpenVideoFile(videoFileName);
    }

    //------------------------------------------
    private void SetUpperUserEmailText()
    {
        var userEmailStr = this.homeControl.GetUserEmailValue();
        // var userEmailStr = UserManager.Instance.GetUserEmail();

        this.uiManager.SetUpperUserEmailText(userEmailStr);
    }

    private void SetUpperExtraFuncBtnText()
    {
        var textExtraFuncBtnType = this.homeControl.GetExtraFuncBtnType();

        this.uiManager.SetUpperExtraFuncBtnType(textExtraFuncBtnType);
    }

    // 스테이지 선택 버튼에 참조
    public void SetWhatStageIndex(StageModule stageModule)
    {
        this.stageController.SetTargetStageIndex(stageModule.stageType);

        this.stageController.InitSpecificStageView();
    }

    //----------------------------------------------------------------
    // 로그아웃 관련
    public void OnClickUserIdBtn()
    {
        // to do : guest로그인이면 계정 연동하기 버튼,
        // 로그인상태면 로그아웃 버튼 띄우기
        this.uiManager.ActivateLogOutPanel();
    }

    // ExtraFunc 에서 참조 중 / 로그인 상태 -> 로그아웃, 게스트 상태 -> ReAsk -> 계정연동 / 취소
    public void OnClickLogOutBtn()
    {
        this.homeControl.CheckNeedToShowReAskPopUpWhenUserDoExtraFunc();
    }

    public void OnClickExitBtnInLogOutPanel()
    {
        this.uiManager.DeActivateLogOutPanel();
    }

    public void OnClickConversionBtn()
    {
        LoadingManager.Instance.ActivateLoading();

        this.homeControl.OnClickConversionBtn();
    }

    //--------------------------------------------------------
    // 스마트학습 관련
    public void OnClickLoadSmartLearningModeBtn()
    {
        //var specificSmartLearningName = "";

        //specificSmartLearningName = "PhotoShop_2";  // tmp

        //CurrentSmartLearningInfo.Instance.InitializeTargetSmartLearningInfo(specificSmartLearningName);

        //SceneManager.LoadScene(SCENE_NAME_SMART_LEARNING_MODE);

#region loading SLM
        //GetIsTargetSmartLearningExist(specificSmartLearningName, (isTargetSmartLearningExist) =>
        //{
        //    CustomDebug.Log($"isTargetSmartLearningExist : {isTargetSmartLearningExist}");

        //    try
        //    {
        //        if (isTargetSmartLearningExist)
        //        {
        //            SetLobbyType(EnumSets.LobbyType.MainLobby);
        //            SetPanelType(EnumSets.PanelType.SmartLearningModePanel);

        //            ActivateTargetPanel();

        //            this.upperLineController.ActivateFullScreenBtn();

        //            Debug.Log($"target SmartLearning Name : {specificSmartLearningName}");

        //            // ActivateTargetLearningMode(specificSmartLearningName);
        //            InitializeTargetSmartLearning();

        //            this.slmController.SetActionOnClickExitSmartLearning(DeActivateStackedPanel);
        //        }
        //        else
        //        {
        //            Debug.Log($"{specificSmartLearningName} prefab does not exist in Resources folder");

        //            // Activate Load Error Popup, …
        //        }
        //    }
        //    catch(Exception e)
        //    {
        //        CustomDebug.LogError($"OnClickLoadSmartLearningModeBtn error :{e.Message}");
        //    }

        //});
#endregion
    }

    public void LoadSLMLesson(string[] lessonInfos)
    {
        CurrentSmartLearningInfo.Instance.InitializeTargetSmartLearningInfo(lessonInfos, () =>
        {
            StartCoroutine(CorMoveToSmartLearningModeScene());  // SmartLearningModeController 실행됨
        });
    }

    private IEnumerator CorMoveToSmartLearningModeScene()
    {
        LoadingManager.Instance.ActivateLoading();

        var async = SceneManager.LoadSceneAsync(SCENE_NAME_SMART_LEARNING_MODE);

        yield return async;
    }

    private void DeActivateStackedPanel()
    {
        var currentActivatedPanel = GetCurrentStackedPanel();

        CheckNeedToDeActivateFullScreenBtn(currentActivatedPanel);

        Debug.Log($"current Activated Panel : {currentActivatedPanel}");

        AllDeActivateMainLobbyPanels();

        var nextTargetPanel = GetCurrentStackedPanel();
        Debug.Log($"next Target Panel : {nextTargetPanel}");

        CheckWhatPanelShowing(nextTargetPanel);
    }

    public void ActivateCommonErrorPopUp(EnumSets.ErrorType errorType)
    {
        this.uiManager.ActivateCommonErrorPopUp(errorType);
    }

    public void SetAfterCommonErrorPopUpDeActivated(Action callback)
    {
        this.afterCommonPopUpDeActivated = callback;
    }

    public void DeActivateCommonErrorPopUp()
    {
        this.uiManager.DeActivateCommonErrorPopUp();

        this.afterCommonPopUpDeActivated?.Invoke();
        this.afterCommonPopUpDeActivated = null;
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#elif UNITY_WEBGL

        // '나가기' 를 통해서 종료하는 것은
        // beforeunload, unload 가 발동되지않는 듯하다
        AppInfo.Instance.CheckOnRefreshWhenGuestSignedIn();

        AppInfo.Instance.WhenAppClosed();

        WebWindowController.CloseWindow();
#endif
    }

    //-------------------------------------------------------

    public void ActivatePopUp(EnumSets.PopUpType popUpType)
    {
        var targetPopUp = from data in this.popUps
                          where data.popUpType.Equals(popUpType)
                          select data;

        if (targetPopUp.Count() > 0)
        {
            IPopUp openedPopUp = targetPopUp.ElementAt(0);

            if (this.stackPopUp.Contains(openedPopUp))
            {
                return;
            }

            this.stackPopUp.Push(openedPopUp);

            openedPopUp.ActivatePopUp();

            CustomDebug.Log($"this.stackPopUp count : {this.stackPopUp.Count}");
        }
    }

    public void DeActivatePopUp()
    {
        if (this.stackPopUp.Count > 0)
        {
            this.currentOpenedPopUp = this.stackPopUp.Pop();

            this.currentOpenedPopUp.DeActivatePopUp();

            this.currentOpenedPopUp = null;

            CustomDebug.Log($"this.stackPopUp count 2 : {this.stackPopUp.Count}");
        }
    }

    //-------------------------------------------------------

    public void StopRecordingUserPlayingTime()
    {
        homeControl.StopRecordingUserPlayingTime();
    }

    public void TryingSignOut()
    {
        this.loginController.SignOut(WhenSignOutSucceeded, WhenSignOutFailed);
    }

    public void WhenSignOutSucceeded()
    {
        CustomDebug.Log("WhenSignOut Succeeded in home scene");

        UserManager.Instance.ResetData();

        MoveToLoginScene();
    }

    public void WhenSignOutFailed()
    {
        CustomDebug.LogError("WhenSignOutFailed in home scene");
    }

    //--------------------------------------------
    private void MoveToLoginScene()
    {
        StartCoroutine(CorMoveToLoginScene());
    }

    IEnumerator CorMoveToLoginScene()
    {
        var async = SceneManager.LoadSceneAsync(0);

        yield return async;
    }

    // guestType, OnClickExtraFuncBtn 에서 사용 중
    private void OnSuccessTryingSignUpFromGuest(string idToken)
    {
        CustomDebug.Log($"On Success Trying SignUpFromGuest : {idToken}");

        LoadingManager.Instance.DeActivateLoading();

        // to do
        // 받은 idToken 을 url 파라미터로 전달하기
        // 'https://bdm-adobe-jinhyuk.vercel.app/register?token={idToken}' 파라미터 값"

        var path = DevUtil.Instance.GetJustCombine2Parts(AppInfo.REGISTER_WEB_PAGE_FOR_GUEST, idToken);

        TryingSignOut();

        WebWindowController.OpenTargetWindow(path);

        AppInfo.Instance.ResetData();
    }

    // guestType, OnClickExtraFuncBtn 에서 사용 중
    private void OnFailedTryingSignUpFromGuest(string errorMsg)
    {
        CustomDebug.LogError($"On Failed TryingSignUpFromGuest : {errorMsg}");

        LoadingManager.Instance.DeActivateLoading();

        ActivateCommonErrorPopUp(EnumSets.ErrorType.AuthError);
    }

    private void SucceededGettingPrizeItemInfo(string data)
    {
        // CustomDebug.Log($"Succeeded, GettingPrizeItemInfo : {data}");

        AppInfo.Instance.SetPrizeItemInfo(data);

        UserManager.Instance.RemoveOldPrizeItemInfos(); // webgl
    }

    private void FailedGettingPrizeItemInfo(string error)
    {
        CustomDebug.LogError($"Failed GettingPrizeItemInfo : {error}");
    }

    // 뒤로가기로 나왔을 때 보여줘야할 레슨리스트 관련
    private void CheckNeedToShowLessonListWhenLoadedFromSmartLearningMode()
    {
        var stageType = UserManager.Instance.GetUserLastPlayingStageType();

        if (!string.IsNullOrEmpty(stageType))  // 뒤로가기 했었다
        {
            var stageTypeEnumValues = DevUtil.Instance.GetEnumValues<EnumSets.StageType>();

            var data = from datas in stageTypeEnumValues
                       where datas.ToString().ToLower().Equals(stageType)
                       select datas;

            UserManager.Instance.ResetLastPlayingStageType();

            if (data.Count() > 0)
            {
                var enumStageType = data.ElementAt(0);

                OnClickOpenLessonBtn();

                this.stageController.SetTargetStageIndex(enumStageType);

                this.stageController.InitSpecificStageView();
            }
        }
    }

    //-------------------------------------------------------------------
    //public void SetSpecialPrizeItemPos(PrizeItemInfo prizeItemInfo)
    //{
    //    this.stageController.SetSpecialPrizeItemPos(prizeItemInfo);
    //}
}
