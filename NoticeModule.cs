using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using TMPro;
using Vuplex.WebView;

public struct noticeData
{
    public long creationDate;
    public string title;
    public bool show;
}

public class NoticeModule : MonoBehaviour
{
    public GameObject popUpNotice;

    [Space]
    [Header("공지사항 세부 내용")]
    public TextMeshProUGUI textSpecificNoticeTitle;
    public Transform parentPrefabSpecificNotice;
    public GameObject prefabSpecificNotice;

    [Space]
    [Header("공지사항 버튼 내용")]
    public Transform parentPrefabSpecificBtn;
    public GameObject prefabSpecificBtn;

    [Space]
    public GameObject updateMark;
    private bool isNeedToShowUpdateMark = false;

    [Header("웹뷰 출력 용")]
    public CanvasWebViewPrefab webViewModule;

    private NoticeBtnModule[] noticeBtnModules = null;

    private string currentNoticeKey = "";

    protected bool isCreatedNoticeObjects = false; // 이미 공지사항 오브젝트들이 만들어졌나 확인

    private StringBuilder sb = new StringBuilder();

    private const string MAIN_PATH_STR = "notices";
    private const string SLASH_STR = "/";
    private const string FIRST_NOTICE_CREATE_DATE_STR = "firstNoticeCreateDate";



    // Start is called before the first frame update
    void Start()
    {
        // 씬이 로딩될 때 공지사항 정보를 가져오고, 뷰를 세팅함
        // 공지사항 아이콘 옆 뱃지 출력을 위함

        // 231116, khc 수정, 공지사항 출력 로직을 봉인
        // GetNoticeDatas();

    }

    public virtual void ActivatePopUp()
    {
        popUpNotice.SetActive(true);

        // 231116, khc 수정, 공지사항 출력 로직을 봉인
        // start 단에서 실패할 경우에 대비해서 다시 시도
        // GetNoticeDatas();
    }

    public virtual void DeActivatePopUp()
    {
        popUpNotice.SetActive(false);

        ResetData();
    }

    private void GetNoticeDatas()
    {
#if UNITY_EDITOR

        CustomDebug.Log("try GetNoticeDatas in editor----->");

#elif UNITY_WEBGL

        CustomDebug.Log("try GetNoticeDatas in UNITY_WEBGL -------");

        var count = AppInfo.Instance.GetNoticeDataCount();

        CustomDebug.Log($"count : {count}");

        if (count == -1)
        {
            LoadingManager.Instance.ActivateLoading();

            PathJsLibData pathJsLibData = new PathJsLibData();

            pathJsLibData.path = DevUtil.Instance.GetTargetPath(EnumSets.DBParentType.Info, MAIN_PATH_STR);
            pathJsLibData.objectName = this.gameObject.name;
            pathJsLibData.callbackName = nameof(InitNoticeView);
            pathJsLibData.fallbackName = nameof(OnFailedToGetData);

            CustomDebug.Log($"pathJsLibData.path : {pathJsLibData.path}");

            FirebaseDatabaseController.Instance.GetTargetData(pathJsLibData);
        }
        else
        {
            // ActivatePopUp(), 공지사항을 열었을 때 이미 noticeDatas 가 있다는 것은
            // 필요한 공지사항 데이터를 얻었다는 것임

            if (isCreatedNoticeObjects)
            {
                // 여기에서 업데이트 마크 상태값 처리를 수행함

                UpdateNoticeUpdateMarkState();
            }
            else
            {
                // 다른 Scene 이동, 로그아웃 -> 로그인 등을 하는 경우도 상정해서 여기서 다시 재생성함
                CheckNeedToShowUpdateMark();

                InitTitleNoticeBtns();

                InitFirstNoticeInfo();
            }
        }
#endif
    }

    private void InitNoticeView(string json)
    {
        try
        {
            CustomDebug.Log($"json in InitNoticeView : {json}");

            LoadingManager.Instance.DeActivateLoading();

            var convertedData = DevUtil.Instance.GetTargetObjectFromJson<Dictionary<string, noticeData>>(json);

            if (convertedData != null)
            {
                // CustomDebug.Log($"Init Notice View : {convertedData}");

                foreach (var item in convertedData)
                {
                    CustomDebug.Log($"notice item : {item.Key}");
                }

                // show == 1 인 공지사항만 보여주기

                var validDatas = from data in convertedData
                                 where data.Value.show
                                 select data;

                if(validDatas.Count() <= 0)
                {
                    return;
                }

                // this.noticeDatas = validDatas.OrderByDescending(x => x.date).ToList();
                var tmpTable = validDatas.OrderByDescending(x => x.Value.creationDate).ToDictionary(p => p.Key, p => p.Value);

                AppInfo.Instance.SetNoticeData(tmpTable); 

                //foreach (var item in noticeDatas)
                //{
                //    CustomDebug.Log($"sorted item : {item.path}");
                //}

               // CustomDebug.Log("-------------------------------------------------------------");

                CheckNeedToShowUpdateMark();

                InitTitleNoticeBtns();

                InitFirstNoticeInfo();

                // TryingGetFirstNoticeImage();
            }
        }
        catch(Exception e)
        {
            CustomDebug.LogError($"InitNoticeView error : {e.Message}");
        }
        
    }

    private void CheckNeedToShowUpdateMark()
    {
        try
        {
            var localFirstNoticeCreateDateStr = PlayerPrefs.GetString(FIRST_NOTICE_CREATE_DATE_STR, "0");

            var localFirstNoticeCreateDate = Convert.ToInt64(localFirstNoticeCreateDateStr);

            var serverFirstNoticeCreateDate = AppInfo.Instance.GetFirstNoticeCreateDate();

            if (serverFirstNoticeCreateDate > localFirstNoticeCreateDate) // 마크를 보여줘야한다
            {
                isNeedToShowUpdateMark = true;

                ShowUpdateMark();
            }
            else
            {
                HideUpdateMark();
            }
        }
        catch(Exception e)
        {
            CustomDebug.LogError($"CheckNeedToShowUpdateMark error : {e.Message}");
        }
    }

    /// <summary>
    /// 공지사항을 열었을 때, 업데이트 마크 상태값을 재조정해야하는 지 확인하고 재조정하기
    /// </summary>
    private void UpdateNoticeUpdateMarkState()
    {
        if(isNeedToShowUpdateMark)
        {
            var serverFirstNoticeCreateDate = AppInfo.Instance.GetFirstNoticeCreateDate();

            CustomDebug.Log($"- serverFirstNoticeCreateDate : {serverFirstNoticeCreateDate}");

            if(serverFirstNoticeCreateDate == -1)
            {
                return;
            }

            PlayerPrefs.SetString(FIRST_NOTICE_CREATE_DATE_STR, serverFirstNoticeCreateDate.ToString());
            PlayerPrefs.Save();

            isNeedToShowUpdateMark = false;

            HideUpdateMark();
        }
    }

    private void ShowUpdateMark()
    {
        updateMark.SetActive(true);
    }

    private void HideUpdateMark()
    {
        updateMark.SetActive(false);
    }

    private void InitTitleNoticeBtns()
    {
        try
        {
            var noticeDataCount = AppInfo.Instance.GetNoticeDataCount();

            noticeBtnModules = new NoticeBtnModule[noticeDataCount];

            CustomDebug.Log($"this.noticeDatas.Count : {noticeDataCount}");

            for (int i = 0; i < noticeDataCount; i++)
            {
                var o = Instantiate(this.prefabSpecificBtn, this.parentPrefabSpecificBtn);

                var btnModule = o.GetComponent<NoticeBtnModule>();

                noticeBtnModules[i] = btnModule;

                // var item = this.noticeDatas.ElementAt(i);
                var itemTable = AppInfo.Instance.roNoticeDataTable.ElementAt(i);

                btnModule.SetTitle(itemTable.Value.title);
                btnModule.SetPath(itemTable.Key);

                btnModule.SetOnClickCallback(OnClickSpecificNoticeOpenBtn);
            }

            isCreatedNoticeObjects = true;
        }
        catch(Exception e)
        {
            CustomDebug.LogError($"InitTitleNoticeBtns error : {e.Message}");
        }
    }

    /// <summary>
    /// 최초로 공지사항 이미지 얻기
    /// </summary>
    [Obsolete]
    private void TryingGetFirstNoticeImage()
    {
        try
        {
            // 제일 처음 공지사항 내용은 다운받아서 보여주기

            // this.currentNoticeKey = AppInfo.Instance.roNoticeDataTable.ElementAt(0).Value.image;

            CustomDebug.Log($"currentNoticeKey : {currentNoticeKey}");

            LoadingManager.Instance.ActivateLoading();

            //DownloadNoticeTex(nameof(InitFirstNoticeInfo));
            DownloadNoticeTex(InitFirstNoticeInfo);
        }
        catch(Exception e)
        {
            CustomDebug.LogError($"Trying GetFirstNoticeImage error {e.Message}");
        }
    }

    [Obsolete]
    private void DownloadNoticeTex(string methodName)
    {
        CustomDebug.Log($"try DownloadNoticeTex -> {methodName}");

        PathJsLibData pathJsLibData = new PathJsLibData();

        pathJsLibData.path = GetTargetPath(this.currentNoticeKey);
        pathJsLibData.objectName = this.gameObject.name;
        pathJsLibData.callbackName = methodName;
        pathJsLibData.fallbackName = nameof(OnFailedToGetData);

        FirebaseStorageController.Instance.DownloadTargetFile(pathJsLibData);
        
    }

    [Obsolete]
    private void DownloadNoticeTex(Action<string> callback)
    {
        CustomDebug.Log($"try DownloadNoticeTex, callback-> {callback}");

        PathJsLibData pathJsLibData = new PathJsLibData();

        pathJsLibData.path = GetTargetPath(this.currentNoticeKey);
        pathJsLibData.objectName = this.gameObject.name;
        
        pathJsLibData.fallbackName = nameof(OnFailedToGetData);

        FirebaseStorageController.Instance.DownloadTargetFileWithCallback(pathJsLibData, callback);
    }
    
    private void InitFirstNoticeInfo()
    {
        // 제일 첫 공지사항 내용과 관련있는 제일 첫 버튼의 색상을 바꾼다
        var currentNoticeBtnModule = this.noticeBtnModules.ElementAt(0);

        currentNoticeBtnModule.UpdateBtnImageColorToSelectedColor();

        SetSpecificNoticeTitle(currentNoticeBtnModule.TitleStr);

        LoadHtmlByWebViewModule(currentNoticeBtnModule.Path);
    }

    [Obsolete]
    private void InitFirstNoticeInfo(string base64Code)
    {
        #region
        /*
        try
        {
            // CustomDebug.Log($"AddNoticeImageToTable 2 : {base64Code}");

            var o = Instantiate(prefabSpecificNotice, parentPrefabSpecificNotice);

            var module = o.GetComponent<SpecificNoticeInfoModule>();

            // CustomDebug.Log($"this.currentNoticeKey : {this.currentNoticeKey} / module: {module}");

            this.noticeTexTable.Add(this.currentNoticeKey, module);

            module.InstantLoadNotice(base64Code);

            // 제일 첫 공지사항 내용과 관련있는 제일 첫 버튼의 색상을 바꾼다
            var currentNoticeBtnModule = this.noticeBtnModules.ElementAt(0);

            currentNoticeBtnModule.UpdateBtnImageColorToSelectedColor();

            SetSpecificNoticeTitle(currentNoticeBtnModule.TitleStr);
        }
        catch (Exception e)
        {
            CustomDebug.LogError($"AddNoticeImageToTable error {e.Message}");
        }
        */
        #endregion
    }

    private void OnFailedToGetData(string error)
    {
        CustomDebug.LogError($"OnFailedToGetData , {error}");

        AllDeActivateBtnSelectedState(); // 공지사항 tex 다운받는 걸 실패할경우 버튼 색을 idle 로 되돌린다

        LoadingManager.Instance.DeActivateLoading();
    }
    
    public string GetTargetPath(string targetKey)
    {
        sb.Clear();

        sb.Append(MAIN_PATH_STR);
        sb.Append(SLASH_STR);
        sb.Append(targetKey);
        // sb.Append(PNG_STR);

        return sb.ToString();
    }

    private void OnClickSpecificNoticeOpenBtn(NoticeBtnModule whatBtnModule)
    {
        try
        {
            AllDeActivateBtnSelectedState();

            // AllDeActivateSpecificNoticeTex();

            SetSpecificNoticeTitle(whatBtnModule.TitleStr);

            whatBtnModule.UpdateBtnImageColorToSelectedColor();

            // 웹뷰 출력
            LoadHtmlByWebViewModule(whatBtnModule.Path);

            #region 공지사항을 이미지로 불러오는 형태, 웹뷰로 대체함
            /*
            if (noticeTexTable.TryGetValue(whatBtnModule.Path, out var noticeInfoModule))
            {
                noticeInfoModule.ActivateItem();
            }
            else
            {
                // 아직 관련 공지사항 tex가 없다면

                // tex 다운로드가 필요하다!
                LoadingManager.Instance.ActivateLoading();

                this.currentNoticeKey = whatBtnModule.Path;

                //DownloadNoticeTex(nameof(InitTargetNoticeInfo));
                DownloadNoticeTex(InitTargetNoticeInfo);
            }*/
        #endregion
    }
        catch (Exception e)
        {
            CustomDebug.LogError($"OnClickSpecificNoticeOpenBtn error : {e.Message}");
        }
    }

    private async void LoadHtmlByWebViewModule(string path)
    {
        CustomDebug.Log($"LoadHtmlByWebViewModule : {path}");

        // LoadingManager.Instance.ActivateLoading();

        await this.webViewModule.WaitUntilInitialized();

        this.webViewModule.WebView.LoadUrl(path);

        // LoadingManager.Instance.DeActivateLoading();
    }

    [Obsolete]
    private void InitTargetNoticeInfo(string base64Code)
    {
        #region
        /*
        try
        {
            CustomDebug.Log($"InitTargetNoticeInfo ");

            var o = Instantiate(prefabSpecificNotice, parentPrefabSpecificNotice);

            var module = o.GetComponent<SpecificNoticeInfoModule>();

            CustomDebug.Log($"this.currentNoticeKey : {this.currentNoticeKey} / module: {module}");

            this.noticeTexTable.Add(this.currentNoticeKey, module);

            module.InstantLoadNotice(base64Code);
        }
        catch (Exception e)
        {
            CustomDebug.LogError($"AddNoticeImageToTable error {e.Message}");

            AllDeActivateBtnSelectedState();
        }
        */
        #endregion
    }

    private void SetSpecificNoticeTitle(string title)
    {
        textSpecificNoticeTitle.text = title;
    }

    private void AllDeActivateBtnSelectedState()
    {
        if(noticeBtnModules == null)
        {
            return;
        }

        for (int i = 0; i < this.noticeBtnModules.Length; i++)
        {
            this.noticeBtnModules.ElementAt(i).UpdateBtnImageColorToIdleColor();
        }
    }

    [Obsolete]
    private void AllDeActivateSpecificNoticeTex()
    {
        #region
        /*
        for (int i = 0; i < this.noticeTexTable.Count; i++)
        {
            this.noticeTexTable.ElementAt(i).Value.DeActivateItem();
        }
        */
        #endregion
    }

    private void ResetData()
    {
        this.currentNoticeKey = string.Empty;
    }
}
