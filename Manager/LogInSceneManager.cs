using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;

public struct PathJsLibData
{
    public string path;
    public string objectName;
    public string callbackName;
    public string fallbackName;
}

public class LogInSceneManager : MonoBehaviour
{
    private static LogInSceneManager instance = null;
    public static LogInSceneManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LogInSceneManager>();
            }

            return instance;
        }
    }

    public LogInController loginController;

    [Space]
    public AgreeTermsModule agreeTermsModule;

    [Space]
    public TMP_InputField inputFieldMobile;
    public TMP_InputField inputFieldPassword;
    public TMP_InputField inputFieldSendPasswordResetEmail;

    public TextMeshProUGUI textOutput;

    [Space]
    public GameObject panelLogin;

    [Space]
    public GameObject objAuthErrorMsg;
    public TextMeshProUGUI textAuthError;

    [Space]
    public GameObject popUpError;

    [Space]
    [Header("vesion")]
    public TextMeshProUGUI textVersion;

    private StringBuilder sb = new StringBuilder();

    private const string FIND_PASSWORD_STR = "forgot-password";

    private const int MINIMUM_MOBILE_ID_COUNT = 11;
    private const int MINIMUM_MOBILE_PASSWORD_COUNT = 6;

    private const string SIGN_IN_TEST_MOBILE_STR = "01012345678";
    private const string SIGN_IN_TEST_PASSWORD_STR = "098765";

    // Start is called before the first frame update
    void Start()
    {
        CheckTermsUpdated();

        Init();
    }

    private void Init()
    {
        CheckAppVesion();

        SoundManager.Instance.StopBGM();
    }

    private void CheckAppVesion()
    {
        var version = Application.version;

        CustomDebug.LogWithColor($"version : {version}", CustomDebug.ColorSet.Magenta);

        textVersion.text = version;
    }

    public void OnClickSignUpBtn()
    {
#if UNITY_EDITOR
        CustomDebug.LogError("OnClick SignUp Btn, WEBGL 에서 수행하기바람");

#elif UNITY_WEBGL

        // FirebaseAuthController.Instance.CreateUserWithEmailAndPassword("adobetest@test.com", inputFieldPassword.text);
        WebWindowController.OpenTargetWindow(AppInfo.BASE_URL);
#endif
    }

    public void OnClickSignInBtn()
    {
#if UNITY_EDITOR
        CustomDebug.Log("OnClickSignInBtn");

#elif SIGNINTEST
        CustomDebug.Log("SIGNINTEST");

#else
        if(!CheckAuthFieldDataIsValid())
        {
            return;
        }
#endif

        DeActivateAuthErrorMsg();

        ActivateLoadingPanel();

#if UNITY_EDITOR
       
        SignInOnEditor();

#elif SIGNINTEST
        this.loginController.SignInWithMobileNumberAndPassword(SIGN_IN_TEST_MOBILE_STR, SIGN_IN_TEST_PASSWORD_STR);

#else
        this.loginController.SignInWithMobileNumberAndPassword(inputFieldMobile.text, inputFieldPassword.text);

#endif
    }

    private bool CheckAuthFieldDataIsValid()
    {
        var isValid = true;

        if (string.IsNullOrEmpty(inputFieldMobile.text) || string.IsNullOrEmpty(inputFieldPassword.text))
        {
            ActivateAuthErrorMsg(EnumSets.AuthErrorCode.NotEnoughData);

            isValid = false;
        }
        else
        {
            if(inputFieldMobile.text.Length != MINIMUM_MOBILE_ID_COUNT)
            {
                ActivateAuthErrorMsg(EnumSets.AuthErrorCode.NotEnoughIDCount);

                isValid = false;
            }
            else
            {
                if (inputFieldPassword.text.Length < MINIMUM_MOBILE_PASSWORD_COUNT)
                {
                    ActivateAuthErrorMsg(EnumSets.AuthErrorCode.NotEnoughPassCount);

                    isValid = false;
                }
            }
        }

        return isValid;
    }

    private void SignInOnEditor()
    {
        CustomDebug.Log("Sign In OnEditor---");

        var tmpUserInfo = new UserInfo();

        tmpUserInfo.comeFromGuest = false;
        tmpUserInfo.createTimestamp = 1653037983973;
        tmpUserInfo.email = "editorTest@test.com";
        tmpUserInfo.emailVerified = true;
        tmpUserInfo.lastSignInTimestamp = 1653318949690;
        tmpUserInfo.mobile = "01027927777";

        var tmpPlayingTimeTable = new Dictionary<string, int>()
        {
            {"ai", 0 },
            {"photoShop", 155 }
        };

        tmpUserInfo.playingTime = tmpPlayingTimeTable;

        //var termsAgreeInfoTable = new Dictionary<string, int>()
        //{
        //    { "isAgree", 1 },
        //    {"termsVersion", 1 }
        //};

        //tmpUserInfo.termsAgreeInfos = termsAgreeInfoTable;

        tmpUserInfo.termsAgree = 1;
        
        var tmpPrizeProgramTable = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, PrizeItem>>>>();

        Dictionary<string, Dictionary<string, Dictionary<string, PrizeItem>>> prizeItemTable = new Dictionary<string, Dictionary<string, Dictionary<string, PrizeItem>>>();

        var prizeTable = new Dictionary<string, PrizeItem>();

        PrizeItem prizeItem = new PrizeItem();
        prizeItem.receive = true;
        prizeItem.receiveDate = 0;
        prizeItem.prizeKey = "pz5";
        prizeItem.type = "editor-까까오 쿠폰";
        prizeItem.targetEmailAddress = "none";
        prizeItem.category = "special";

        prizeTable.Add("-NAhc1kviGFydf_WnHIO", prizeItem);

        /*
        PrizeItem prizeItem3 = new PrizeItem();
        prizeItem3.receive = false;
        prizeItem3.receiveDate = 0;
        prizeItem3.type = "어!도비티콘 다꾸 스티커";
        prizeItem3.targetEmailAddress = "none";
        prizeItem3.prizeKey = "pz3";
        prizeItem3.category = "normal";

        prizeTable.Add("-NAhc1kviGFydf_Wnndi", prizeItem3);

        var prizeTable2 = new Dictionary<string, PrizeItem>();
        PrizeItem prizeItem2 = new PrizeItem();
        prizeItem2.receive = false;
        prizeItem2.receiveDate = 0;
        prizeItem2.prizeKey = "pz1";
        prizeItem2.type = "editor-Q 템플릿";
        prizeItem2.targetEmailAddress = "none";
        prizeItem2.category = "normal";

        prizeTable2.Add("-NAhc1kviGFydf_WnHTT", prizeItem2);
        */

        var lessonTable = new Dictionary<string, Dictionary<string,PrizeItem>>();
        lessonTable.Add("l1", prizeTable);
        // lessonTable.Add("l9", prizeTable2);

        prizeItemTable.Add("s1", lessonTable);

        tmpPrizeProgramTable.Add(AppInfo.Instance.GetProgramInfo(), prizeItemTable);
        
        tmpUserInfo.prizeItems = tmpPrizeProgramTable;
        
        var tmpProgramTable = new Dictionary<string,Dictionary<string, Dictionary<string, int>>>();
        var tmpStageCompletedTable = new Dictionary<string, Dictionary<string, int>>();
        
        var tmpSubCompletedTable = new Dictionary<string, int>();
        tmpSubCompletedTable.Add("l1", 1);
        tmpSubCompletedTable.Add("l2", 1);
        tmpSubCompletedTable.Add("l3", 1);
        tmpSubCompletedTable.Add("l4", 1);
        tmpSubCompletedTable.Add("l5", 1);
        tmpSubCompletedTable.Add("l6", 1);
        tmpSubCompletedTable.Add("l7", 1);
        tmpSubCompletedTable.Add("l8", 1);
        tmpSubCompletedTable.Add("l9", 1);

        tmpStageCompletedTable.Add("s1", tmpSubCompletedTable);

        var tmpSubS2CompletedTable = new Dictionary<string, int>();
        tmpSubS2CompletedTable.Add("l1", 1);
        tmpSubS2CompletedTable.Add("l2", -1);
        tmpSubS2CompletedTable.Add("l3", -1);
        tmpSubS2CompletedTable.Add("l4", -1);
        tmpSubS2CompletedTable.Add("l5", -1);
        tmpSubS2CompletedTable.Add("l6", -1);

        tmpStageCompletedTable.Add("s2", tmpSubS2CompletedTable);

        tmpProgramTable.Add(AppInfo.Instance.GetProgramInfo(), tmpStageCompletedTable);

        tmpUserInfo.stageCompletedInfos = tmpProgramTable;
        
        tmpUserInfo.uid = "7P62gzVIoERz3eg5c9yNN8PI9DZ0";
        tmpUserInfo.visitingCount = 1;

        var tmpRecentTryingTable = new Dictionary<string, string>()
        {
            {"photoShop_s1_l1", "인터페이스-시작하기" },
            {"photoShop_s1_l2", "인터페이스-작업환경 둘러보기" }
        };

        tmpUserInfo.recentTryingLecture = tmpRecentTryingTable;

        UserManager.Instance.CreateUserInfoOnEditor(tmpUserInfo);
    }

    private void SignInGuestOnEditor()
    {
        CustomDebug.Log("Sign In Guest On Editor >g>g>g>>");

        var tmpUserInfo = new UserInfo();

        tmpUserInfo.comeFromGuest = true;
        tmpUserInfo.createTimestamp = 1653037983973;
        tmpUserInfo.email = "editorTest@GuestTest.com";
        tmpUserInfo.emailVerified = true;
        tmpUserInfo.lastSignInTimestamp = 1653318949690;
        tmpUserInfo.mobile = "01027927777";

        var tmpPlayingTimeTable = new Dictionary<string, int>()
        {
            {"photoShop", 155 }
        };

        tmpUserInfo.playingTime = tmpPlayingTimeTable;

        //var termsAgreeInfoTable = new Dictionary<string, int>()
        //{
        //    { "isAgree", 1 },
        //    {"termsVersion", 1 }
        //};

        //tmpUserInfo.termsAgreeInfos = termsAgreeInfoTable;
        tmpUserInfo.termsAgree = 1;

        // tmpUserInfo.uid = "7P62gzVIoERz3eg5c9yNN8PI9DZ0";
        tmpUserInfo.uid = "xvGv7GtAoXVEUPTrkKagMndIFDp1";
        tmpUserInfo.visitingCount = 1;

        UserManager.Instance.CreateUserInfoOnEditor(tmpUserInfo);
    }

    public void OnClickSignOutBtn()
    {
        this.loginController.SignOut();
    }

    public void OnClickSendPasswordResetEmailBtn()
    {
        this.loginController.SendPasswordResetEmail(inputFieldSendPasswordResetEmail.text);
    }

    public void DisplaySuccessInfo(string info)
    {
        textOutput.color = Color.white;
        textOutput.text = info;

        CustomDebug.Log($"display success info : {info}   -----------");
    }

    public void DisplayFailInfo(string error)
    {
        // Debug.LogError($"1 Display fail info, error: {error}");

        //var msg = GetFirebaseErrorMsg(error);

        this.textOutput.color = Color.red;
        this.textOutput.text = error;

        CustomDebug.LogError($"2 Display fail info : {error}");
    }

    public void DisplayUserInfo(string text)
    {
        textOutput.color = Color.green;
        textOutput.text = text;
    }

    public void TryGettingUserDBWithUID(FirebaseUserModified initUserInfos)
    {
        PathJsLibData userJsLibData = new PathJsLibData();

        userJsLibData.path = DevUtil.Instance.GetTargetPath(EnumSets.DBParentType.Users, initUserInfos.uid);
        userJsLibData.objectName = this.transform.name;
        userJsLibData.callbackName = nameof(GetTargetUserDataSucceeded);
        userJsLibData.fallbackName = nameof(GetTargetUserDataFailed);

        FirebaseDatabaseController.Instance.GetTargetData(userJsLibData);
    }

    // GetTargetData 의 성공 콜백
    private void GetTargetUserDataSucceeded(string snapShotData)
    {
        if (snapShotData.Equals("none")) // 게스트 로그인 (익명로그인) 시도
        {
            var firebaseUser = this.loginController.CurrentFirebaseUser;

            // 유저 데이터 없음
            CustomDebug.Log($"??? 유저 데이터 없음 (게스트-익명 로그인) , { firebaseUser.uid}");

            var playingTimeTable = new Dictionary<string, int>
            {
                {"photoShop" , 0 },
                {"ai" , 0 }
            };

            var tmpTable = new Dictionary<string, object>();

            tmpTable.Add("uid", firebaseUser.uid);
            tmpTable.Add("createTimestamp", Convert.ToInt64(firebaseUser.createdAt));
            tmpTable.Add("playingTime", playingTimeTable);

            // tmpTable.Add("email", firebaseUser.email);
            // tmpTable.Add("emailVerified", firebaseUser.emailVerified);
            // tmpTable.Add("lastSignInTimestamp", firebaseUser.lastLoginAt);

            var jsonStr = JsonConvert.SerializeObject(tmpTable);

            // Debug.Log($"jsonStr : {jsonStr}");

            FirebaseDatabaseController.Instance.SetTagetData(
                DevUtil.Instance.GetTargetPath(EnumSets.DBParentType.Users,firebaseUser.uid), jsonStr, (isSuccess) => {

                    CustomDebug.Log($">>> set user data success ?? : {isSuccess}");

                    if (isSuccess == 0)
                    {
                        // 게스트 유저 정보, db에 삽입 실패
                        // appinfo, guestSignIn 리셋은 동의팝업이 꺼질 때 이루어짐

                        DeActivateLoadingPanel();

                        ActivateAuthErrorMsg(EnumSets.AuthErrorCode.Default, "게스트 유저정보 생성 실패");
                    }
                    else
                    {
                        // 게스트 유저 정보, db에 삽입 성공
                        CustomDebug.Log("게스트 유저 정보, db에 삽입 성공");

                        UserManager.Instance.CreateGuestUserInfo(jsonStr);
                    }

                    // DeActivateLoadingPanel();
                });
        }
        else
        {
           // 간혹 게스트 로그인정보가 리셋되지않고 그대로 들고오는 경우가 있음
           // 현재 firebaseUser 가 익명인증 상태라면 강제로 로그아웃 시켜버리기
            if(this.loginController.CurrentFirebaseUser.isAnonymous) // v1.2.4
            {
                CustomDebug.Log("게스트 로그인정보가 리셋되지않고 그대로 들고오는 경우가 발생, 강제로 로그아웃시키기");

                ActivateLoadingPanel();

                loginController.SignOut();

                return;
            }

            // 221205, 김형철 추가
            // 해당 메소드는 최초 로그인만을 가정하여 uid 를 통해 얻은 유저정보를 토대로 로컬 userinfo를 생성하는데
            // 글리치를 이용해 해당 메소드를 계속 실행시키는 경우가 있음
            // 이미 userinfo 가 있다면 CreateUserInfo 못하도록 막음
            if(!UserManager.Instance.IsUserInfoNull()) // v1.2.5
            {
                // 글리치 사용 시 아무것도 못하게막음
                ActivateLoadingPanel();

                return;
            }

            UserManager.Instance.CreateUserInfo(snapShotData);

            UserManager.Instance.UpdateVisitingCount();
            UserManager.Instance.UpdateLastSignInDate();

            #region stored User Infos
            /*
             >>> stored User Infos : 
            {
            "comeFromGuest":1,
            "createTimestamp":1653037983973,
            "email":"trick5786@naver.com",
            "emailVerified":false,
            "lastSignInTimestamp":1653318949690,
            "mobile":"01027920000",
            "playingTime":777,
            "prizeItems":{
                "s1":{
                    "l3":{
                    "receive":0,
                    "receiveDate":"none",
                    "targetEmailAddress":"none",
                    "type":"none",
                    "whatLessonSequence":"l5"
                    },
                    "l5":{
                    "receive":1,
                    "receiveDate":"1655383198448",
                    "targetEmailAddress":"trick5786@naver.com",
                    "type":"어떤어떤 기프티콘",
                    "whatLessonSequence":"l5"
                    }
                }
            },
            "stageCompletedInfos":{
                "s1":{
                "l1":1,
                "l2":1,
                "l3":1
                }
            },
            termsAgreeInfos:
            {
                isAgree :1,
                termsVersion : 1
            }
            "uid":"tY7mCbrvxUbnbn0ar9ebP965MAx2",
            "visitingCount":9
            }
             */
            #endregion
        }
    }


    // GetTargetData 의 실패 콜백
    private void GetTargetUserDataFailed(string error)
    {
        CustomDebug.LogError($">>> GetTargetUserDataFailed : {error} |||||");

        var parsedError = DevUtil.Instance.GetFirebaseError(error);

        DeActivateLoadingPanel();

        if (AppInfo.Instance.IsGuestLogin)
        {
            // ActivateAuthErrorMsg(EnumSets.AuthErrorCode.Default, "게스트 로그인을 위한 정보 찾기 실패");
            ActivateAuthErrorMsg(EnumSets.AuthErrorCode.Default, parsedError.code);
        }
        else
        {
            ActivateAuthErrorMsg(EnumSets.AuthErrorCode.UserNotFound);
        }
        
    }

    //--------------------------------------
    public void MoveToHomeScene()
    {
        CustomDebug.Log("Home 으로 이동");

        StartCoroutine(CorMoveToHomeScene());
        // SceneManager.LoadScene(1);
    }

    IEnumerator CorMoveToHomeScene()
    {
        // 전환되는 시간이 가끔씩 너무 오래걸림
        // progress 출력하기
        var async = SceneManager.LoadSceneAsync(1);

        yield return async;
    }

    //--------------------------------------
    public void ActivateLoadingPanel()
    {
        LoadingManager.Instance.ActivateLoading();
    }

    public void DeActivateLoadingPanel()
    {
        CustomDebug.Log(" ----> Try De Activate LoadingPanel");

        LoadingManager.Instance.DeActivateLoading();
    }

    public void ActivateLoginPanel()
    {
        panelLogin.SetActive(true);
    }

    public void ActivateAuthErrorMsg(EnumSets.AuthErrorCode errorCode, string msg = "")
    {
        ActivateAuthErrorMsg();

        switch (errorCode)
        {
            case EnumSets.AuthErrorCode.UserNotFound:
                {
                    SetAuthErrorMsg("유저 정보를 찾을 수 없습니다");
                }
                break;
            case EnumSets.AuthErrorCode.InvalidEmail:
                {
                    // webgl 컨텐츠에서는 해당 에러가 뜰 일이 없음
                    // 웹에서 제대로 입력을 받아야함
                    SetAuthErrorMsg("어도비 - 이메일 형식이 아닙니다");
                }
                break;
            case EnumSets.AuthErrorCode.WrongPassword:
                {
                    SetAuthErrorMsg("해당 아이디에 맞는 비밀번호가 아닙니다");
                }
                break;
            case EnumSets.AuthErrorCode.Default:
                {
                    SetAuthErrorMsg($"{msg} 오류, 고객센터에 문의바랍니다");
                }
                break;
            case EnumSets.AuthErrorCode.NotEnoughData:
                {
                    SetAuthErrorMsg("아이디 / 비밀번호에 필요한 정보가 없습니다");
                }
                break;
            case EnumSets.AuthErrorCode.NotEnoughIDCount:
                {
                    SetAuthErrorMsg("아이디 최소 글자 수가 부족합니다");
                }
                break;
            case EnumSets.AuthErrorCode.NotEnoughPassCount:
                {
                    SetAuthErrorMsg("비밀번호 최소 글자 수가 부족합니다");
                }
                break;
        }

        LoadingManager.Instance.DeActivateLoading();
    }

    private void ActivateAuthErrorMsg()
    {
        this.objAuthErrorMsg.SetActive(true);
    }

    private void DeActivateAuthErrorMsg()
    {
        if(this.objAuthErrorMsg.activeSelf)
        {
            this.objAuthErrorMsg.SetActive(false);
        }
    }

    private void SetAuthErrorMsg(string msg)
    {
        this.textAuthError.text = msg;
    }

    public void CheckTermsUpdated()
    {
        this.loginController.CheckTermsUpdated();
    }

    public void ActivateOrdinaryTermsAgreePanel()
    {
        this.agreeTermsModule.ActivateAgreeTermsPanelWhenTermsUpdated();
    }

    public void OnClickTryingGuestSignIn()
    {
        AppInfo.Instance.ComeThroughGuestLogin();

#if UNITY_EDITOR
        SignInGuestOnEditor();

#elif UNITY_WEBGL

        this.agreeTermsModule.ActivateAgreeTermsPanelWhenTryingGuestSignIn();
#endif
    }

    public void OnClickFindPWBtn()
    {

#if UNITY_EDITOR
        CustomDebug.LogWithColor("Try Find Pw ", CustomDebug.ColorSet.Cyan);

#elif UNITY_WEBGL
        var targetUrl = AppInfo.Instance.GetProperURL(FIND_PASSWORD_STR);

        WebWindowController.OpenTargetWindow(targetUrl);
#endif

    }


    //-----------------------------------------
    public void OnClickExitBtn()
    {
        // window close
        CustomDebug.Log("Exit!");

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
        WebWindowController.CloseWindow();
#endif
    }

    public void TryingSignInAnonymously()
    {
        this.loginController.TryingSignInAnonymously();
    }

    //-------------------------------------------
    public void ActivateErrorPopUp()
    {
        popUpError.SetActive(true);
    }

    public void DeActivateErrorPopUp()
    {
        popUpError.SetActive(false);
    }
}

