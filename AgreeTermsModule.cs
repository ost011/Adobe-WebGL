using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AgreeTermsModule : MonoBehaviour
{
    public GameObject body;

    public GameObject[] topTexts; // 0 ordinary, 1 guest
    public GameObject bottomText;

    [Space]
    public TextMeshProUGUI textTerms;

    [Space]
    public Toggle toggleAgree;

    [Space]
    public Button btnOk;

    private StringBuilder sb = new StringBuilder();

    private const string SLASH_STR = "/";
    private const string TERMS_DIR_STR = "terms";
    private const string TERMS_VERSION_STR = "termsVersion";

    private const string TERMS_USER_KEY = "termsAgree";

    // Start is called before the first frame update
    void Start()
    {

    }

    private void ActivateAgreeTermsPanel()
    {
        body.SetActive(true);
    }

    public void DeActivateAgreeTermsPanel()
    {
        body.SetActive(false);
    }

    private void AllDeActivateTexts()
    {
        for (int i = 0; i < topTexts.Length; i++)
        {
            topTexts[i].SetActive(false);
        }

        bottomText.SetActive(false);
    }

    public void ActivateAgreeTermsPanelWhenTermsUpdated()
    {
        CustomDebug.Log("Activate Agree Terms Panel When Terms Updated >>>");

        AllDeActivateTexts();

        topTexts[0].SetActive(true);

        ShowTermsText();
    }

    public void ActivateAgreeTermsPanelWhenTryingGuestSignIn()
    {
        CustomDebug.Log("ActivateAgreeTermsPanel When Trying Guest SignIn");

        AllDeActivateTexts();

        // 손님으로 입장하기 관련 텍스트 출력
        topTexts[1].SetActive(true);
        bottomText.SetActive(true);

        ShowGuestTermsText();
    }

    private void ShowTermsText()
    {
        var termsDataOnServer = AppInfo.Instance.GetTermsTextData();

        // CustomDebug.Log($"ShowTermsText, termsDataOnServer : {termsDataOnServer}");

        if (termsDataOnServer.Equals(string.Empty))
        {
            LoadingManager.Instance.ActivateLoading();

            LoadTermsTexts();
        }
        else
        {
            LoadingManager.Instance.ForceDeActivateLoading();

            textTerms.text = termsDataOnServer;

            ActivateAgreeTermsPanel();
        }
    }

    // 게스트 전용
    private void ShowGuestTermsText()
    {
        AppInfo.Instance.SetTermsServerVersion(777);

        var termsDataOnServer = AppInfo.Instance.GetTermsTextData();

        // CustomDebug.Log($"ShowGuestTermsText, termsDataOnServer : {termsDataOnServer}");

        if (termsDataOnServer.Equals(string.Empty))
        {
            LoadingManager.Instance.ActivateLoading();

            LoadTermsTexts();
        }
        else
        {
            textTerms.text = termsDataOnServer;

            ActivateAgreeTermsPanel();
        }
    }

    private void LoadTermsTexts()
    {
        CustomDebug.Log("LoadTermsTexts >>");

        PathJsLibData pathJsLibData = new PathJsLibData();

        pathJsLibData.path = GetNewTermsFileName(); // "termsVersion-1.txt";
        pathJsLibData.objectName = this.transform.name;
        pathJsLibData.callbackName = nameof(SetTermsTexts);
        pathJsLibData.fallbackName = nameof(GetTermsTextFailed);

        FirebaseStorageController.Instance.DownloadTargetFile(pathJsLibData);
    }

    private void SetTermsTexts(string base64Code)
    {
        LoadingManager.Instance.ForceDeActivateLoading();

        var bytes = Convert.FromBase64String(base64Code);

        var text = Encoding.UTF8.GetString(bytes);

        AppInfo.Instance.SetTermsTextData(text);

        // CustomDebug.Log($"SetTermsTexts => {text}");

        textTerms.text = text;

        ActivateAgreeTermsPanel();
    }

    private void GetTermsTextFailed(string errorText)
    {
        CustomDebug.LogError($"GetTermsText Failed : {errorText}");

        LoadingManager.Instance.DeActivateLoading();

        // 동의 관련 텍스트 받아오는 게 실패하면 일단 홈씬으로 보내기
        LogInSceneManager.Instance.MoveToHomeScene();
    }

    private string GetNewTermsFileName()
    {
        sb.Clear();

        sb.Append(TERMS_DIR_STR);
        sb.Append(SLASH_STR);
        sb.Append(TERMS_VERSION_STR);
        sb.Append("-");
        sb.Append(AppInfo.Instance.TermsServerVersion);
        sb.Append(".txt");

        return sb.ToString();
    }

    public void OnClickAgreeBtn()
    {
        CustomDebug.Log($"OnClickAgreeBtn >>>>>>>>> try guest sign in? > {AppInfo.Instance.IsGuestLogin}");

        CheckSignInTypeWhenClickAgreeBtn();
    }

    private void CheckSignInTypeWhenClickAgreeBtn()
    {
        if(!AppInfo.Instance.IsGuestLogin)
        {
            UpdateUserTermsVersion();
        }
        else
        {
            // 게스트 로그인시 '동의' 버튼을 누르면 익명로그인 시도
            LoadingManager.Instance.ActivateLoading();

            LogInSceneManager.Instance.TryingSignInAnonymously();
            // LogInSceneManager.Instance.MoveToHomeScene();
        }
    }

    private void UpdateUserTermsVersion()
    {
        LoadingManager.Instance.ActivateLoading();

        var path = DevUtil.Instance.GetTargetPath(EnumSets.DBParentType.Users, UserManager.Instance.GetUID());

        CustomDebug.Log($"path : {path}");

        var termsVersion = AppInfo.Instance.TermsServerVersion;

        var tmpTable = new Dictionary<string, int>()
        {
                {TERMS_USER_KEY, termsVersion }
        };

        var valueJson = DevUtil.Instance.GetJson(tmpTable);
        // CustomDebug.Log($"valueJson : {valueJson}");

        FirebaseDatabaseController.Instance.UpdateJsonWithCallback(path, valueJson, WhenUpateTermsVersionSucceeded, WhenUpateTermsVersionFailed);
    }

    private void WhenUpateTermsVersionSucceeded()
    {
        CustomDebug.Log("When Upate TermsVersion Succeeded  :D");

        LogInSceneManager.Instance.MoveToHomeScene();

    }

    private void WhenUpateTermsVersionFailed()
    {
        CustomDebug.LogError("When Upate TermsVersion Failed >>>>>> :<");

        LoadingManager.Instance.DeActivateLoading();

        LogInSceneManager.Instance.ActivateErrorPopUp();
    }

    private string GetTargetPath()
    {
        sb.Clear();

        sb.Append(UserManager.Instance.GetUID());
        sb.Append(SLASH_STR);
        sb.Append("termsAgreeInfos");

        return sb.ToString();
    }

    public void OnClickCloseBtn()
    {
        ResetData();

        if (AppInfo.Instance.IsGuestLogin)
        {
            AppInfo.Instance.ResetData();
        }
        else
        {
            ForceSignOut();
        }
    }

    private void ForceSignOut()
    {
        // FirebaseAuthController.Instance.SetCallbackObject("LogInController");

        FirebaseAuthController.Instance.SignOut();
    }

    //--------------------------
    public void CheckIsAgreeToggleState(Toggle toggle)
    {
        if(toggle.isOn)
        {
            this.btnOk.interactable = true;
        }
        else
        {
            this.btnOk.interactable = false;
        }
    }

    private void ResetData()
    {
        this.btnOk.interactable = false;

        this.toggleAgree.isOn = false;
    }
}
