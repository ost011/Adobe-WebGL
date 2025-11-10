using UnityEngine;
using Newtonsoft.Json;
using System;


public class LogInController : MonoBehaviour
{
    private FirebaseUserModified firebaseUser;
    public FirebaseUserModified CurrentFirebaseUser => this.firebaseUser;

    private string thisObjectName = "";

    private bool isAleadySignedIn = false;

    private string pass = "";

    private Action afterSignOutSuccessCallback = null;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            Debug.LogError("The code is not running on a WebGL build; as such, the Javascript functions will not be recognized.");

            return;
        }

        LogInSceneManager.Instance.ActivateLoadingPanel();

        thisObjectName = this.gameObject.name;

        FirebaseAuthController.Instance.ForceAwake();

       
    }

    public void SignInWithEmailAndPassword(string email, string pass)
    {
        if (!isAleadySignedIn)
        {
            FirebaseAuthController.Instance.SignInWithEmailAndPassword(email, pass);
        }
        else
        {
            // 자동로그인 되었지만
            // 이메일 인증을 안했을경우에
            // sign in 버튼을 눌렀을 때
            // 지금은 안씀
            FirebaseAuthController.Instance.SignOut();

            SetAfterSignOutSuccessCallback(() => {

                LogInSceneManager.Instance.ActivateLoadingPanel();

                // 다시 파베 - 이메일 인증 정보를 받아야하므로
                FirebaseAuthController.Instance.SignInWithEmailAndPassword(email, pass);
            });
        }
    }

    // 전화번호와 비밀번호로 인증을 시도
    // 해당 전화번호와 연결된 이메일 계정을 가져오기
    public void SignInWithMobileNumberAndPassword(string mobileNumber, string pass)
    {
        this.pass = pass;

        PathJsLibData pathJsLibData = new PathJsLibData();

        pathJsLibData.path = DevUtil.Instance.GetTargetPath(EnumSets.DBParentType.Accounts, mobileNumber);
        pathJsLibData.objectName = this.transform.name;
        pathJsLibData.callbackName = nameof(GetTargetAccountDataSucceeded);
        pathJsLibData.fallbackName = nameof(GetTargetAccountDataFailed);

        FirebaseDatabaseController.Instance.GetTargetData(pathJsLibData);
    }

    private void GetTargetAccountDataSucceeded(string snapShotData)
    {
        if (snapShotData.Equals("none"))
        {
            // 해당 전화번호가 db에 없음
            CustomDebug.LogError("해당 전화번호가 db에 없음");

            LogInSceneManager.Instance.ActivateAuthErrorMsg(EnumSets.AuthErrorCode.UserNotFound);

            LogInSceneManager.Instance.DeActivateLoadingPanel();
        }
        else
        {
            // 해당 전화번호와 같이있는 UID 를 가지고 email 정보로 얻기
            var uid = JsonConvert.DeserializeObject<string>(snapShotData);
            
            CustomDebug.Log($"Get Target Account Data Succeeded, uid : {uid}");

            GetEmailAddressInSpecificUser(uid);
        }
    }

    private void GetTargetAccountDataFailed(string data)
    {
        Debug.LogError($"GetTargetAccountData Failed : {data}");

        this.pass = "";

        LogInSceneManager.Instance.ActivateAuthErrorMsg(EnumSets.AuthErrorCode.UserNotFound);
    }

    private void GetEmailAddressInSpecificUser(string uid)
    {
        PathJsLibData pathJsLibData = new PathJsLibData();

        var subPath = DevUtil.Instance.GetTargetPath2Parts(uid, "email");

        pathJsLibData.path = DevUtil.Instance.GetTargetPath(EnumSets.DBParentType.Users, subPath);
        pathJsLibData.objectName = this.transform.name;
        pathJsLibData.callbackName = nameof(GetTargetEmailDataSucceeded);
        pathJsLibData.fallbackName = nameof(GetTargetAccountDataFailed);

        FirebaseDatabaseController.Instance.GetTargetData(pathJsLibData);
    }

    private void GetTargetEmailDataSucceeded(string snapShotData)
    {
        if (snapShotData.Equals("none"))
        {
            // 해당 email이 개별 유저 db에 없음

            CustomDebug.LogError("해당 email이 개별 유저 db에 없음");

            LogInSceneManager.Instance.ActivateAuthErrorMsg(EnumSets.AuthErrorCode.UserNotFound);

            LogInSceneManager.Instance.DeActivateLoadingPanel();
        }
        else
        {
            // 해당 전화번호와 같이있는 UID 를 가지고 email 정보로 얻기
            var email = JsonConvert.DeserializeObject<string>(snapShotData);

            CustomDebug.Log($"Get Target EmailData Succeeded, email : {email}");

            SignInWithEmailAndPassword(email, this.pass);
        }
    }

    private void SetAfterSignOutSuccessCallback(Action isReady)
    {
        this.afterSignOutSuccessCallback = isReady;
    }

    public void SignOut()
    {
        FirebaseAuthController.Instance.SignOut();
    }

    public void TryingSignInAnonymously()
    {
        PathJsLibData pathJsLibData = new PathJsLibData();

        pathJsLibData.objectName =  this.gameObject.name;
        pathJsLibData.callbackName = nameof(SucceededSignIn); // 사실상 쓰이지않음, AuthStateChanged 에서 최종적으로 처리됨
        pathJsLibData.fallbackName = nameof(WhenSignInFailed);

        FirebaseAuthController.Instance.SignInAnonymously(pathJsLibData);
    }

    private void DisplaySuccessInfo(string info)
    {
        LogInSceneManager.Instance.DisplaySuccessInfo(info);
    }

    private void WhenSignInFailed(string error)
    {
        LogInSceneManager.Instance.DeActivateLoadingPanel();

        var parsedError = DevUtil.Instance.GetFirebaseError(error);

        Debug.LogError($"WhenSignInFailed, FirebaseError -> code : {parsedError.code} / details : {parsedError.details} / msg : {parsedError.message}");

        HandlingAuthError(parsedError.code);
    }

    private void HandlingAuthError(string code)
    {
        // auth/user-not-found 유저정보 못찾음
        // auth/invalid-email 이메일 형식이 아님
        // auth/wrong-password 잘못된 비밀번호

        switch (code)
        {
            case "auth/user-not-found":
                {
                    LogInSceneManager.Instance.ActivateAuthErrorMsg(EnumSets.AuthErrorCode.UserNotFound);
                }
                break;
            case "auth/invalid-email":
                {
                    LogInSceneManager.Instance.ActivateAuthErrorMsg(EnumSets.AuthErrorCode.InvalidEmail);
                }
                break;
            case "auth/wrong-password":
                {
                    LogInSceneManager.Instance.ActivateAuthErrorMsg(EnumSets.AuthErrorCode.WrongPassword);
                }
                break;
            default:
                {
                    LogInSceneManager.Instance.ActivateAuthErrorMsg(EnumSets.AuthErrorCode.Default, code);
                }
                break;
        }
    }

    private void WhenSignOutFailed(string error)
    {
        LogInSceneManager.Instance.DeActivateLoadingPanel();

        LogInSceneManager.Instance.DisplayFailInfo(error);

        this.afterSignOutSuccessCallback = null;
    }

    private void DisplayFailInfo(string error)
    {
        LogInSceneManager.Instance.DisplayFailInfo(error);

        this.afterSignOutSuccessCallback = null;
    }

    private void SucceededSignOut(string info)
    {
        CustomDebug.Log($"login scene, DisplayUser Signed Out >> : {info}");

        this.isAleadySignedIn = false;
        this.firebaseUser = null;

        LogInSceneManager.Instance.ActivateLoginPanel();

        LoadingManager.Instance.ForceDeActivateLoading();

        LoadAfterSignOutSuccessCallback();
    }

    private void LoadAfterSignOutSuccessCallback()
    {
        this.afterSignOutSuccessCallback?.Invoke();
        this.afterSignOutSuccessCallback = null;
    }

    private void SucceededSignIn(string originalUserData)
    {
        LogInSceneManager.Instance.ActivateLoadingPanel();

        isAleadySignedIn = true;

        // Debug.Log($"1 DisplayUserInfo >> : {originalUserData}");

        this.firebaseUser = JsonConvert.DeserializeObject<FirebaseUserModified>(originalUserData);

        LogInSceneManager.Instance.TryGettingUserDBWithUID(this.firebaseUser);

        /*
        // 익명(게스트) 로그인인 지 확인하기
        if (this.firebaseUser.isAnonymous)
        {
            CustomDebug.Log($"isAnonymous >> : {this.firebaseUser.isAnonymous}");

            // 유저 db에 생성일자만 기록하기
            LogInSceneManager.Instance.TryGettingUserDBWithUID(this.firebaseUser);
        }
        else
        {
            // 이메일 인증 요구가 필요하지 않을 때의 로직 ---------------------
            
            var testText = $"Email: {this.firebaseUser.email}, UserId: {this.firebaseUser.uid}, EmailVerified: {this.firebaseUser.emailVerified}";

            CustomDebug.Log($"2 DisplayUserInfo >> : {testText}");

            // LogInSceneManager.Instance.DisplayUserInfo(testText);

            // CustomDebug.Log($"이메일 인증 로직은 임시 통과, 유저 정보 가져오기 수행하기");

            LogInSceneManager.Instance.TryGettingUserDBWithUID(this.firebaseUser);

            // return;

            #region 이메일 인증 요구가 필요할 때 주석풀기 ---------------------

            //if (!firebaseUser.emailVerified)
            //{
            //    // to do
            //    // 이메일 인증 요구 요청팝업 띄우기
            //    CustomDebug.Log($"이메일 인증 요구 요청팝업 띄우기 -----");

            //    LogInSceneManager.Instance.DeActivateLoadingPanel();
            //}
            //else
            //{
            //    LogInSceneManager.Instance.TryGettingUserDBWithUID(this.firebaseUser);

            //}
            #endregion
        }
        */
    }

    public void SendPasswordResetEmail(string targetEmailAddress)
    {
        Debug.Log($"비밀번호 변경 이메일 보내기");

        FirebaseAuthController.Instance.SendPasswordResetEmail(targetEmailAddress);

    }

    private void WhenWebLoginSucceeded(string tmpUID)
    {
        CustomDebug.Log($"WhenWebLoginSucceeded, tmpUID : {tmpUID}");
    }

    public void CheckTermsUpdated()
    {
#if UNITY_EDITOR
        GetTermsServerVersionSucceeded("1");

#elif UNITY_WEBGL
        PathJsLibData pathJsLibData = new PathJsLibData();

        pathJsLibData.path = DevUtil.Instance.GetTargetPath(EnumSets.DBParentType.Info, "termsVersion");
        pathJsLibData.objectName = this.transform.name;
        pathJsLibData.callbackName = nameof(GetTermsServerVersionSucceeded);
        pathJsLibData.fallbackName = nameof(GetTermsServerVersionFailed);

        FirebaseDatabaseController.Instance.GetTargetData(pathJsLibData);
#endif
    }

    private void GetTermsServerVersionSucceeded(string snapShotData)
    {
#if UNITY_EDITOR
        var serverTermsVersion = 1;
#elif UNITY_WEBGL
        var serverTermsVersion = JsonConvert.DeserializeObject<int>(snapShotData);
#endif
        CustomDebug.Log($"Get Terms Server Version Succeeded : {serverTermsVersion} / type : {serverTermsVersion.GetType()}");

        AppInfo.Instance.SetTermsServerVersion(serverTermsVersion);

        LoadingManager.Instance.DeActivateLoading();
    }

    private void GetTermsServerVersionFailed(string errorData)
    {
        CustomDebug.LogError($"Get Terms ServerVersion Failed : {errorData}");

        AppInfo.Instance.SetTermsServerVersion(0);

        LoadingManager.Instance.DeActivateLoading();
    }
}
