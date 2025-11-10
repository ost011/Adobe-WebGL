using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LearningModeController : MonoBehaviour
{
    private static LearningModeController instance = null;
    public static LearningModeController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LearningModeController>();
            }

            return instance;
        }
    }



//    public LearningModeUIManager learningModeUIManager;
//    public SafeAreaCustom safeArea;
//    public AttentionPointController attentionPointController;

//    public Transform posUiTop;
//    public Transform posUiRoot;

//    public Vector3 posContainerOrigin;
//    public Vector3 posContainerLocalOrigin;

//    [SerializeField]
//    private NGUIAtlas atlas;
//    [SerializeField]
//    private NGUIAtlas bgAtlas;

//    [Header("Stage info")]
//    [SerializeField]
//    private EnumSets.ProgramLangSetting currentLang;

//    [SerializeField]
//    private string currentLectureKey = ""; // learningmode_0

//    [SerializeField]
//    private string currentStage = ""; // PR_115
//    private string pureStageStr = ""; // 115

//    private Info_Video infoVideo = null;

//    private bool isOptionOpened = false;

//    [Header("카메라 이동지점")]
//    public Transform[] attentionPointsOnPortrait; // 카메라 이동지점들 - 세로형
//    public Transform[] attentionPointsOnLandscape; // 카메라 이동지점들 - 가로형
//    private Transform[] attentionPoints = null;

//    [Space]
//    public GameObject learningModeObjects;

//    public Camera cam;
//    private Ray ray;
//    private RaycastHit hit;

//    [Space]
//    [Header("러닝모드가 출력될 때 Off 되어야할 것들")]
//    private List<GameObject> objectOriginalList = new List<GameObject>();

//    private bool isNotchDevice = false;
//    public bool IsNotchDevice
//    {
//        get => this.isNotchDevice;
//    }

//    [Space]
//    [Header("uicamera eventType 조정용")]
//    public UICamera uiCamera;

//    public bool isActivatedSmartLearningMode = false;

//    [Header("다음 강의 진행여부 관련 ------------")]
//    public AfterSpecificLessonCompleteHandlingModuleInSmartLearningMode afterSpecificLessonCompleteHandlingModuleInSmartLearningMode = null;

//    [Space]
//    public LogInPopUpManager logInPopUpManager;

//    private GameObject curStagePanel = null;

//    private List<string> stages = null;
//    private int listIndexLimit = 0;

//    private bool isLastStage = false;
//    public bool IsLastStage
//    {
//        get => isLastStage;
//    }

//    private const string NEW_CHECK_PREFIX = "NewCheck_"; // new 이미지 출력 유무 저장용 playerPrefs 접두 string
//    private string checkOutNewMarkStr = ""; // 스테이지 '이어하기' 시 홈씬에서의 스테이지 버튼에 붙어있는 'new'마크를 제거하기위함

//    private WaitForSeconds waitDelayActivateEndMenuPopUp = new WaitForSeconds(3f);
//    private WaitForSeconds waitDelayAddCompleteInfo = new WaitForSeconds(1f);

//    private GameObject originalLearningModePrefabFromAssetBundle = null;

//    private IEnumerator loadLearingModeAssetsEnumerator = null;
//    private IEnumerator loadTargetPrefabEnumerator = null;
//    private Coroutine initLearningModeAssetCoroutine = null;
//    private IEnumerator initAtlasEnumerator = null;

//    [SerializeField]
//    private bool isLoadingSmartLearningModeAsset = false;
//    private AssetBundle assetbundle = null;

//    private Action afterExitSmartLearningMode = null;

//    [SerializeField]
//    private bool noNeedToActivateLessonList = false;

//    [SerializeField]
//    private bool isLoadNextVideoState = false;

//    private const string TARGET_PATH_SMART_LEARNING_MODE_LANG_FILE = "smartlearningmodelang";
//    private const string TARGET_PATH_SMART_LEARNING_MODE_PREFAB_FILE = "slmprefab";
//    private const string TARGET_PATH_SMART_LEARNING_MODE_ATLAS_FILE = "slmatlas";

//    private void Awake()
//    {
//        // SetStageList();
//    }

//    // Start is called before the first frame update
//    void Start()
//    {
//        FindingOriginalObjects();

//        InitCSVAssets();

//    }

//    private void InitCSVAssets()
//    {
//        UnityEngine.Debug.Log("---- InitCSVAssets ------");

//        Ui_RoundLoading.ins.LoadingWithBG();

//        // StartCoroutine(LoadCSVAssetsFromLocalAssetBundle());
//        LoadCSVAssetFromLocalAssetBundle();
//    }

//    [Obsolete]
//    IEnumerator LoadCSVAssetsFromLocalAssetBundle()
//    {
//        var assetBundleDownloadPath = Ui_DownloadManager.ins.AssetBundleDownloadPath;

//        var targetPath = DevUtil.Instance.CombinePaths(assetBundleDownloadPath, TARGET_PATH_SMART_LEARNING_MODE_LANG_FILE);

//        var isExist = DevUtil.Instance.IsExistTargetFile(targetPath);

//        if (isExist)
//        {
//            var loadedFileFromLocal = AssetBundle.LoadFromFileAsync(targetPath);
//            yield return loadedFileFromLocal;

//            assetbundle = loadedFileFromLocal.assetBundle;

//            var loadedAsset = assetbundle.LoadAllAssetsAsync();
//            yield return loadedAsset;

//            var assets = loadedAsset.allAssets;

//            StageTitlesAndTextsDBController.Instance.SetCSVFiles(assets);

//            UnloadAssetBundle();

//            Ui_RoundLoading.ins.Stop();
//        }
//        else
//        {
//            Ui_DownloadManager.ins.DownloadAndSaveToLocalWhenTargetFileMissing(EnumSets.AdditionalAssetBundleDownload.SmartLearningModeLang, (isComplete) => {

//                if (isComplete)
//                {
//                    InitCSVAssets();
//                }
//                else
//                {
//                    UnityEngine.Debug.Log("missing, smart learningmode csv 추가 다운로드 실패");

//                    Ui_RoundLoading.ins.Stop();

//                    NetworkController.Instance.ActivateNetworkErrorPopUpWithAction(null, InitCSVAssets);
//                }
//            });
//        }
//    }

//    private async void LoadCSVAssetFromLocalAssetBundle()
//    {
//        var assetBundleDownloadPath = Ui_DownloadManager.ins.AssetBundleDownloadPath;

//        var targetPath = DevUtil.Instance.CombinePaths(assetBundleDownloadPath, TARGET_PATH_SMART_LEARNING_MODE_LANG_FILE);

//        var isExist = DevUtil.Instance.IsExistTargetFile(targetPath);

//        if (isExist)
//        {
//            var task = DevUtil.Instance.GetFileBytes(targetPath);
//            await task;

//            if (task.Result != null)
//            {
//                var loadedBytesFromLocal = task.Result;

//                LoadFromMemory(loadedBytesFromLocal);
//            }
//            else
//            {
//                // null 이 나왔다는 것은 Out of memory 로 인해서 터지는 경우가 많음
//                DevUtil.Instance.OptimizeMemory();

//                LoadCSVAssetFromLocalAssetBundle();
//            }
//        }
//        else
//        {
//            Ui_DownloadManager.ins.DownloadAndSaveToLocalWhenTargetFileMissing(EnumSets.AdditionalAssetBundleDownload.SmartLearningModeLang, (isComplete) => {

//                if (isComplete)
//                {
//                    InitCSVAssets();
//                }
//                else
//                {
//                    UnityEngine.Debug.Log("missing, smart learningmode csv 추가 다운로드 실패");

//                    Ui_RoundLoading.ins.Stop();

//                    NetworkController.Instance.ActivateNetworkErrorPopUpWithAction(null, InitCSVAssets);

//                    //NetworkController.Instance.ActivateNetworkErrorPopUp(() => {

//                    //    InitCSVAssets();
//                    //});
//                }
//            });
//        }
//    }

//    private void LoadFromMemory(byte[] targetBytes)
//    {
//        StartCoroutine(CorLoadFromMemory(targetBytes));
//    }

//    IEnumerator CorLoadFromMemory(byte[] targetBytes)
//    {
//        var assetbundleCreateRequest = AssetBundle.LoadFromMemoryAsync(targetBytes);

//        yield return assetbundleCreateRequest;

//        assetbundle = assetbundleCreateRequest.assetBundle;

//        var loadedAsset = assetbundle.LoadAllAssetsAsync();
//        yield return loadedAsset;

//        var assets = loadedAsset.allAssets;

//        StageTitlesAndTextsDBController.Instance.SetCSVFiles(assets);

//        UnloadAssetBundle();

//        // Ui_RoundLoading.ins.Stop();
//    }

//    /// <summary>
//    /// 스마트학습 출력을 위해서 off 시켜야할 오브젝트들을 찾는다
//    /// </summary>
//    private void FindingOriginalObjects()
//    {
//        var objects = FindObjectsOfType<AutoFindingModule>();

//        objectOriginalList.Clear();

//        for (int i = 0; i < objects.Length; i++)
//        {
//            objectOriginalList.Add(objects[i].GetCurrentGameObejct());
//        }
//    }

//    public void Init()
//    {
//        UnityEngine.Debug.Log("Init ------");

//        isLoadingSmartLearningModeAsset = true;

//        ParentHomeController.Instance.FastHideNaviBtnsSequence();

//        SetCurrentLearningModeReferense();
//        SetLearningModeProgramLang();

//        DeActivateOriginalObjects();

//        learningModeObjects.SetActive(true);

//        this.learningModeUIManager.DeActivateAllPopUp();

//        InitLearningModeAsset();

//        CameraManager.ins.InitCamera();
//        SetUICameraEventType();

//        ChangeAttentionPointsToPortrait();

//        Resources.UnloadUnusedAssets();

//        GameSoundManager.ins.StopBgm();

//        AudioManager.Instance.PlayBGM();

//        ReArrangeTitleBG();

//        ReInitIsActivatedSmartLearningMode();
//    }

//    IEnumerator CorInitLearningModeAsset()
//    {
//        UnityEngine.Debug.Log("start Cor Init Learning Mode Asset  ^^^^^^^^^^^^^^^^^^^");
//        ActivateLoadingPanel();

//        initAtlasEnumerator = CorInitAtlas();

//        initLearningModeAssetCoroutine = StartCoroutine(initAtlasEnumerator);

//        yield return initLearningModeAssetCoroutine;

//        UnityEngine.Debug.Log("start CorLoadTargetPrefabFromLocal  33333");
//        loadTargetPrefabEnumerator = CorLoadTargetPrefabFromLocal();
//        StartCoroutine(loadTargetPrefabEnumerator);
//    }

//    private void InitLearningModeAsset()
//    {
//        ActivateLoadingPanel();

//        LoadAtlasFromLocal(() => {

//            LoadTargetPrefabFromLocal();

//        });
//    }

//    /// <summary>
//    /// 최초 러닝모드를 실행할 시 titleBG의 위치position) 가 (0,1.0,0)로 출력됨, (0,0.9,1)로 조정해야함
//    /// </summary>
//    private void ReArrangeTitleBG()
//    {
//        this.learningModeUIManager.ReArrangeTitleBG();
//    }

//    public void SetCurrentLearningModeStage(string curentTargetKey, string learningModeKey) // learningmode_0 , PR_111
//    {
//        this.currentLectureKey = curentTargetKey;
//        currentStage = learningModeKey;

//        UnityEngine.Debug.Log($"@@@@ SetCurrentLearningModeStage : {currentStage}");
//    }

//    private void SetLearningModeProgramLang()
//    {
//        currentLang = StageTitlesAndTextsDBController.Instance.GetCurrentProgramLangSetting();

//        UnityEngine.Debug.Log($"@@@@ SetLearningModeLang : {currentLang}");
//    }

//    public void SetInfoVideo(Info_Video infoVideo)
//    {
//        UnityEngine.Debug.Log($">> >> >> SetInfoVideo >>>>>>>>>");

//        this.infoVideo = infoVideo;
//    }

//    /// <summary>
//    /// 스마트학습 UI (top ui 가 아님) 을 선택하기위한 ui camera - event type 교체하기
//    /// </summary>
//    private void SetUICameraEventType()
//    {
//        var currentProgram = DevUtil.Instance.GetChosenProgramName();

//        switch (currentProgram)
//        {
//            case EnumSets.ProgramName.Pr:
//                {
//                    // ui camera의 event type 변경하기
//                    this.uiCamera.eventType = UICamera.EventType.UI_2D;
//                }
//                break;
//            case EnumSets.ProgramName.Ae:
//            case EnumSets.ProgramName.Dr:
//                {
//                    // ui camera의 event type 변경하기
//                    this.uiCamera.eventType = UICamera.EventType.UI_3D;
//                }
//                break;
//        }
//    }

//    private void SetStageList()
//    {
//        // to do
//        // this.stages = AppInfo.Instance.GetCurrentSpecificStages();
//        this.stages = new List<string>();

//        for (int i = 0; i < stages.Count; i++)
//        {
//            UnityEngine.Debug.Log($"stages : {stages[i]}");
//        }
//    }

//    [Obsolete]
//    private void DynamicStageLoad()
//    {
//        GameObject stagePanel = Resources.Load("Prefabs_LM/" + currentStage) as GameObject;
//        curStagePanel = NGUITools.AddChild(posUiRoot, stagePanel);

//        // stagePanel.transform.SetParent(posUiRoot); //Setting the parent of a transform which resides in a Prefab Asset is disabled to prevent data corruption (GameObject: 'LmPr372'). 방지를 위해서 주석처리함

//        curStagePanel.transform.name = "ContainerOf" + currentStage.ToString();

//        StagePanelInfo stagePanelInfo = curStagePanel.GetComponent<StagePanelInfo>();

//        stagePanelInfo.Init(posUiTop);
//        stagePanelInfo.SetBGAtlas(bgAtlas);

//        // curLevel = stagePanelInfo.GetMainGameObject();

//        CameraManager.ins.SetBGObject(stagePanelInfo.GetMainBG());

//        if (this.attentionPointController != null)
//        {
//            this.attentionPointController.RePositioningLandscapeAttentionPoints();

//        }
//    }

//    private void DynamicStageLoad(GameObject targetObject)
//    {
//        try
//        {
//            if (atlas == null)
//            {
//                throw new NullReferenceException("필요한 오브젝트들이 null 임");
//            }

//            curStagePanel = NGUITools.AddChild(posUiRoot, targetObject);

//            if (curStagePanel == null)
//            {
//                throw new NullReferenceException("필요한 오브젝트들이 null 임");
//            }

//            curStagePanel.transform.name = "ContainerOf" + currentStage.ToString();

//            StagePanelInfo stagePanelInfo = curStagePanel.GetComponent<StagePanelInfo>();

//            var isNeedExtraBGAtlas = stagePanelInfo.GetIsNeedExtraBGAtlas(); // AE 이지만 PR 형식으로 만들어져서 기존 AE와 같이 BG_kr 형태의 추가 BG 아틀라스가 필요없는 경우를 확인하기

//            if (isNeedExtraBGAtlas && bgAtlas == null)
//            {
//                throw new NullReferenceException("구형 AE, DR형식임 / 필요한 오브젝트들이 null 임");
//            }

//            stagePanelInfo.Init(posUiTop);

//            if (isNeedExtraBGAtlas)
//            {
//                stagePanelInfo.SetBGAtlas(bgAtlas);
//            }
//            else
//            {
//                stagePanelInfo.SetBGAtlas(atlas); // pr 형식은 atlas == bgAtlas
//            }

//            CameraManager.ins.SetBGObject(stagePanelInfo.GetMainBG());

//            if (this.attentionPointController != null)
//            {
//                this.attentionPointController.RePositioningLandscapeAttentionPoints();
//            }
//        }
//        catch (Exception e)
//        {
//            UnityEngine.Debug.Log($"dynamic stage load error : {e.Message}");

//            ActivateGotErrorWhenLoadingSmartlearningModeAssetsPopUp(ExitGameScene);
//        }

//    }

//    [Obsolete]
//    IEnumerator CorLoadTargetPrefabFromLocal()
//    {
//        UnityEngine.Debug.Log("@@@@@ CorLoadTargetPrefabFromLocal @@@@@@@@@@@@");

//        var assetBundleDownloadPath = Ui_DownloadManager.ins.AssetBundleDownloadPath;
//        var targetPath = DevUtil.Instance.CombinePaths(assetBundleDownloadPath, TARGET_PATH_SMART_LEARNING_MODE_PREFAB_FILE);

//        var isExist = DevUtil.Instance.IsExistTargetFile(targetPath);

//        if (isExist)
//        {
//            var assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(targetPath);

//            yield return assetBundleCreateRequest;

//            var assetBundle = assetBundleCreateRequest.assetBundle;

//            var assetbundleRequest = assetBundle.LoadAssetAsync(currentStage);
//            yield return assetbundleRequest;

//            var targetAsset = assetbundleRequest.asset as GameObject;
//            originalLearningModePrefabFromAssetBundle = targetAsset; // obsolete

//            DynamicStageLoad(targetAsset); // obsolete

//            assetBundle.Unload(false);

//            DeActivateLoadingPanel();
//        }
//        else
//        {
//            StopCoroutine(loadTargetPrefabEnumerator);

//            Ui_DownloadManager.ins.DownloadAndSaveToLocalWhenTargetFileMissing(EnumSets.AdditionalAssetBundleDownload.SlmPrefab, (isComplete) => {

//                if (isComplete)
//                {
//                    UnityEngine.Debug.Log("프리팹 다운로드 성공");

//                    loadTargetPrefabEnumerator = null;
//                    loadTargetPrefabEnumerator = CorLoadTargetPrefabFromLocal();
//                    StartCoroutine(loadTargetPrefabEnumerator);
//                }
//                else
//                {
//                    UnityEngine.Debug.Log("추가 다운로드 실패");

//                    DeActivateLoadingPanel();

//                    NetworkController.Instance.ActivateNetworkErrorPopUpInLearningMode(() => {

//                        ResetSmartLearningModeForExit();
//                    });
//                }
//            });
//        }
//    }

//    private async void LoadTargetPrefabFromLocal()
//    {
//        UnityEngine.Debug.Log("LoadTargetPrefabFromLocal -- ");

//        var assetBundleDownloadPath = Ui_DownloadManager.ins.AssetBundleDownloadPath;
//        var targetPath = DevUtil.Instance.CombinePaths(assetBundleDownloadPath, TARGET_PATH_SMART_LEARNING_MODE_PREFAB_FILE);

//        var isExist = DevUtil.Instance.IsExistTargetFile(targetPath);

//        if (isExist)
//        {
//            var task = DevUtil.Instance.GetFileBytes(targetPath);
//            await task;

//            if (task.Result != null)
//            {
//                var loadedBytesFromLocal = task.Result;

//                LoadPrefabFromMemory(loadedBytesFromLocal);
//            }
//            else
//            {
//                // null 이 나왔다는 것은 Out of memory 로 인해서 터지는 경우가 많음
//                DevUtil.Instance.OptimizeMemory();

//                LoadTargetPrefabFromLocal();
//            }
//        }
//        else
//        {
//            Ui_DownloadManager.ins.DownloadAndSaveToLocalWhenTargetFileMissing(EnumSets.AdditionalAssetBundleDownload.SlmPrefab, (isComplete) => {

//                if (isComplete)
//                {
//                    UnityEngine.Debug.Log("프리팹 다운로드 성공");

//                    LoadTargetPrefabFromLocal();
//                }
//                else
//                {
//                    UnityEngine.Debug.Log("추가 다운로드 실패");

//                    DeActivateLoadingPanel();

//                    NetworkController.Instance.ActivateNetworkErrorPopUpInLearningMode(() => {

//                        ResetSmartLearningModeForExit();
//                    });
//                }
//            });
//        }
//    }

//    private void LoadPrefabFromMemory(byte[] targetBytes)
//    {
//        StartCoroutine(CorLoadPrefabFromMemory(targetBytes));
//    }

//    IEnumerator CorLoadPrefabFromMemory(byte[] targetBytes)
//    {
//        UnityEngine.Debug.Log("CorLoadPrefabFromMemory @@@@@@@@@");

//        var assetbundleCreateRequest = AssetBundle.LoadFromMemoryAsync(targetBytes);

//        yield return assetbundleCreateRequest;

//        assetbundle = assetbundleCreateRequest.assetBundle;

//        var assetbundleRequest = assetbundle.LoadAssetAsync(currentStage); // PR_112
//        yield return assetbundleRequest;

//        var targetAsset = assetbundleRequest.asset as GameObject;
//        originalLearningModePrefabFromAssetBundle = targetAsset; // prefab 처럼 인식을 하는 듯

//        DynamicStageLoad(originalLearningModePrefabFromAssetBundle); // CorLoadPrefabFromMemory

//        UnloadAssetBundle();

//        isLoadingSmartLearningModeAsset = false;

//        DeActivateLoadingPanel();

//        SendLogEventWhenSmartLearningStart();
//    }

//    [Obsolete]
//    private void InitAtlas()
//    {
//        string resourceName = "";

//        StringBuilder sb = new StringBuilder();

//        sb.Append("AtlasAssets/");
//        sb.Append(currentStage);

//        switch (currentLang)
//        {
//            case EnumSets.ProgramLangSetting.En:
//                {
//                    resourceName = sb.ToString();
//                }
//                break;
//            case EnumSets.ProgramLangSetting.Ko:
//                {
//                    if (currentStage.Contains("DR"))
//                    {
//                        resourceName = sb.ToString();
//                    }
//                    else
//                    {
//                        //-> AE or PR
//                        sb.Append("_kr");
//                        resourceName = sb.ToString();
//                    }
//                }
//                break;
//            case EnumSets.ProgramLangSetting.Vn:
//                {
//                    sb.Append("_vn");
//                    resourceName = sb.ToString();
//                }
//                break;
//        }

//        // UnityEngine.Debug.Log($"resourceName : {resourceName}");
//        atlas = Resources.Load(resourceName) as NGUIAtlas;
//    }

//    [Obsolete]
//    IEnumerator CorInitAtlas()
//    {
//        UnityEngine.Debug.Log("start cor init atlas %%%%%");

//        string resourceName = "";
//        string resourceBgName = "";

//        StringBuilder sb = new StringBuilder();
//        StringBuilder sbBg = new StringBuilder();

//        sb.Append(currentStage);
//        sbBg.Append(currentStage);

//        switch (currentLang)
//        {
//            case EnumSets.ProgramLangSetting.En:
//                {
//                    resourceName = sb.ToString();

//                    if (!currentStage.Contains("PR"))
//                    {
//                        sbBg.Append("_BG");
//                    }

//                    resourceBgName = sbBg.ToString();
//                }
//                break;
//            case EnumSets.ProgramLangSetting.Ko:
//                {
//                    if (currentStage.Contains("DR"))
//                    {
//                        resourceName = sb.ToString();
//                    }
//                    else
//                    {
//                        //-> AE or PR
//                        sb.Append("_kr");
//                        resourceName = sb.ToString();
//                    }

//                    if (currentStage.Contains("PR"))  // pr 은 일반, 배경 이미지들이 하나로 통합되어있음
//                    {
//                        sbBg.Append("_kr");
//                    }
//                    else
//                    {
//                        if (currentStage.Contains("DR"))
//                        {
//                            sbBg.Append("_BG");
//                        }
//                        else
//                        {
//                            //-> AE 
//                            // resourceName = currentStage.ToString() + "_ko";
//                            sbBg.Append("_BG_kr");
//                        }
//                    }

//                    resourceBgName = sbBg.ToString();
//                }
//                break;
//            case EnumSets.ProgramLangSetting.Vn:
//                {
//                    sb.Append("_vn");
//                    resourceName = sb.ToString();

//                    sbBg.Append("_BG_vn");
//                    resourceBgName = sb.ToString();
//                }
//                break;
//        }

//        // 선택한 프로그램의 아틀라스 에셋번들에 접근하기위함
//        sb.Clear();

//        var currentProgramName = DevUtil.Instance.GetChosenProgramName().ToString().ToLower();

//        sb.Append(TARGET_PATH_SMART_LEARNING_MODE_ATLAS_FILE);
//        sb.Append("_");
//        sb.Append(currentProgramName);

//        var targetFileName = sb.ToString();

//        var assetBundleDownloadPath = Ui_DownloadManager.ins.AssetBundleDownloadPath;
//        var targetPath = DevUtil.Instance.CombinePaths(assetBundleDownloadPath, targetFileName);

//        var isExist = DevUtil.Instance.IsExistTargetFile(targetPath);

//        if (isExist)
//        {
//            var assetbundleCreateRequest = AssetBundle.LoadFromFileAsync(targetPath);
//            yield return assetbundleCreateRequest;

//            var assetBundle = assetbundleCreateRequest.assetBundle;

//            // atlas 초기화 관련
//            var assetbundleRequest = assetBundle.LoadAssetAsync(resourceName);
//            yield return assetbundleRequest;

//            var loadedAsset = assetbundleRequest.asset as NGUIAtlas;

//            this.atlas = loadedAsset;

//            // bg atlas 초기화 관련
//            var bgAssetbundleRequest = assetBundle.LoadAssetAsync(resourceBgName);
//            yield return bgAssetbundleRequest;

//            var bgLoadedAsset = bgAssetbundleRequest.asset as NGUIAtlas;
//            this.bgAtlas = bgLoadedAsset;

//            assetBundle.Unload(false);
//        }
//        else
//        {
//            StopCoroutine(loadLearingModeAssetsEnumerator);

//            RetryDownloadAtlasAssetBundle(currentProgramName);

//            yield break;
//        }
//    }

//    private Action loadAtlasFromLocalAction = null;
//    private async void LoadAtlasFromLocal(Action isReady = null)
//    {
//        loadAtlasFromLocalAction = isReady;

//        string resourceName = "";
//        string resourceBgName = "";

//        StringBuilder sb = new StringBuilder();
//        StringBuilder sbBg = new StringBuilder();

//        sb.Append(currentStage);
//        sbBg.Append(currentStage);

//        switch (currentLang)
//        {
//            case EnumSets.ProgramLangSetting.En:
//                {
//                    resourceName = sb.ToString();

//                    if (!currentStage.Contains("PR"))
//                    {
//                        sbBg.Append("_BG");
//                    }

//                    resourceBgName = sbBg.ToString();
//                }
//                break;
//            case EnumSets.ProgramLangSetting.Ko:
//                {
//                    if (currentStage.Contains("DR"))
//                    {
//                        resourceName = sb.ToString();
//                    }
//                    else
//                    {
//                        //-> AE or PR
//                        sb.Append("_kr");
//                        resourceName = sb.ToString();
//                    }

//                    if (currentStage.Contains("PR"))  // pr 은 일반, 배경 이미지들이 하나로 통합되어있음
//                    {
//                        sbBg.Append("_kr"); // resourceName 과 같은 이름을 가져오지만 쓰지는 않음
//                    }
//                    else
//                    {
//                        if (currentStage.Contains("DR"))
//                        {
//                            sbBg.Append("_BG");
//                        }
//                        else
//                        {
//                            //-> AE 
//                            // resourceName = currentStage.ToString() + "_ko";
//                            sbBg.Append("_BG_kr"); // ae 는 bg 가 따로 분리되어있음
//                        }
//                    }

//                    resourceBgName = sbBg.ToString();
//                }
//                break;
//            case EnumSets.ProgramLangSetting.Vn:
//                {
//                    sb.Append("_vn");
//                    resourceName = sb.ToString();

//                    sbBg.Append("_BG_vn");
//                    resourceBgName = sb.ToString();
//                }
//                break;
//        }

//        string[] resourceNames = new string[] { resourceName, resourceBgName };

//        // 선택한 프로그램의 아틀라스 에셋번들에 접근하기위함
//        sb.Clear();

//        var currentProgramName = DevUtil.Instance.GetChosenProgramName().ToString().ToLower();

//        sb.Append(TARGET_PATH_SMART_LEARNING_MODE_ATLAS_FILE);
//        sb.Append("_");
//        sb.Append(currentProgramName);

//        var targetFileName = sb.ToString();

//        var assetBundleDownloadPath = Ui_DownloadManager.ins.AssetBundleDownloadPath;
//        var targetPath = DevUtil.Instance.CombinePaths(assetBundleDownloadPath, targetFileName);

//        var isExist = DevUtil.Instance.IsExistTargetFile(targetPath);

//        UnityEngine.Debug.Log($"LoadAtlasFromLocal, resourceName : {resourceName} / resourceBgName : {resourceBgName} / isExist : {isExist}");

//        if (isExist)
//        {
//            var task = DevUtil.Instance.GetFileBytes(targetPath);
//            await task;

//            if (task.Result != null)
//            {
//                var loadedBytesFromLocal = task.Result;

//                LoadAtlasFromMemory(loadedBytesFromLocal, resourceNames);
//            }
//            else
//            {
//                // null 이 나왔다는 것은 Out of memory 로 인해서 터지는 경우가 많음
//                DevUtil.Instance.OptimizeMemory();

//                loadAtlasFromLocalAction = null;

//                LoadAtlasFromLocal(LoadTargetPrefabFromLocal);
//            }
//        }
//        else
//        {
//            UnityEngine.Debug.Log($"file not found, targetPath : {targetPath}, retry download");

//            loadAtlasFromLocalAction = null;

//            RetryDownloadAtlasAssetBundle(currentProgramName);
//        }
//    }

//    private void LoadAtlasFromMemory(byte[] targetBytes, string[] resourceNames)
//    {
//        StartCoroutine(CorLoadAtlasFromMemory(targetBytes, resourceNames));
//    }

//    IEnumerator CorLoadAtlasFromMemory(byte[] targetBytes, string[] resourceNames)
//    {
//        if (targetBytes == null)
//        {
//            UnityEngine.Debug.Log("복호화 실패, targetBytes is NULL -=-==-=-=-=-=-=-=");

//            // 복호화 실패 시 null 이 들어옴
//            ActivateGotErrorWhenLoadingSmartlearningModeAssetsPopUp(ExitGameScene);

//            yield break;
//        }

//        UnityEngine.Debug.Log($"@@@@@@@  CorLoadAtlasFromMemory, resourceNames 0 : {resourceNames[0]} / resourceNames 1 : {resourceNames[1]}");

//        var assetbundleCreateRequest = AssetBundle.LoadFromMemoryAsync(targetBytes);

//        yield return assetbundleCreateRequest;

//        assetbundle = assetbundleCreateRequest.assetBundle;

//        // atlas 초기화 관련
//        var assetbundleRequest = assetbundle.LoadAssetAsync(resourceNames[0]);
//        yield return assetbundleRequest;

//        var loadedAsset = assetbundleRequest.asset as NGUIAtlas;

//        this.atlas = loadedAsset;

//        // bg atlas 초기화 관련
//        var bgAssetbundleRequest = assetbundle.LoadAssetAsync(resourceNames[1]);
//        yield return bgAssetbundleRequest;

//        var bgLoadedAsset = bgAssetbundleRequest.asset as NGUIAtlas;
//        this.bgAtlas = bgLoadedAsset;

//        UnloadAssetBundle();

//        loadAtlasFromLocalAction?.Invoke();
//        loadAtlasFromLocalAction = null;

//    }

//    private void RetryDownloadAtlasAssetBundle(string currentProgramName)
//    {
//        var missingFileName = EnumSets.AdditionalAssetBundleDownload.None;

//        switch (currentProgramName)
//        {
//            case "ae":
//                {
//                    missingFileName = EnumSets.AdditionalAssetBundleDownload.SlmAtlas_ae;
//                }
//                break;
//            case "dr":
//                {
//                    missingFileName = EnumSets.AdditionalAssetBundleDownload.SlmAtlas_dr;
//                }
//                break;
//            case "pr":
//                {
//                    missingFileName = EnumSets.AdditionalAssetBundleDownload.SlmAtlas_pr;
//                }
//                break;
//        }

//        Ui_DownloadManager.ins.DownloadAndSaveToLocalWhenTargetFileMissing(missingFileName, (isComplete) => {

//            if (isComplete)
//            {
//                UnityEngine.Debug.Log(" Retry Download isComplete");

//                //loadLearingModeAssetsEnumerator = null;
//                //loadLearingModeAssetsEnumerator = CorInitLearningModeAsset();
//                //StartCoroutine(loadLearingModeAssetsEnumerator);

//                InitLearningModeAsset();

//            }
//            else
//            {
//                UnityEngine.Debug.Log($"Retry Download, 추가 다운로드 실패, {missingFileName}");

//                DeActivateLoadingPanel();

//                NetworkController.Instance.ActivateNetworkErrorPopUpInLearningMode(() => {

//                    ResetSmartLearningModeForExit();
//                });
//            }
//        });
//    }

//    [Obsolete]
//    private void InitBGAtlas()
//    {
//        string resourceName = "";
//        StringBuilder sb = new StringBuilder();

//        sb.Append("AtlasAssets/");
//        sb.Append(currentStage);

//        switch (currentLang)
//        {
//            case EnumSets.ProgramLangSetting.En:
//                {
//                    if (!currentStage.Contains("PR"))
//                    {
//                        sb.Append("_BG");
//                    }

//                    resourceName = sb.ToString();
//                }
//                break;
//            case EnumSets.ProgramLangSetting.Ko:
//                {
//                    if (currentStage.Contains("PR"))  // pr 은 일반, 배경 이미지들이 하나로 통합되어있음
//                    {
//                        sb.Append("_kr");
//                    }
//                    else
//                    {
//                        if (currentStage.Contains("DR"))
//                        {
//                            sb.Append("_BG");
//                        }
//                        else
//                        {
//                            //-> AE 
//                            // resourceName = currentStage.ToString() + "_ko";
//                            sb.Append("_BG_kr");
//                        }
//                    }
//                    resourceName = sb.ToString();
//                }
//                break;
//            case EnumSets.ProgramLangSetting.Vn:
//                {
//                    sb.Append("_BG_vn");
//                    resourceName = sb.ToString();
//                }
//                break;
//        }

//        // UnityEngine.Debug.Log($"BG resourceName : {resourceName}");
//        bgAtlas = Resources.Load(resourceName) as NGUIAtlas;
//    }

//    /// <summary>
//    /// 현재 언어설정 상태에 따라 참조해야할 언어관련 dictionary 들 초기화하기, 매번 언어설정값 비교를 피하기위함
//    /// </summary>
//    private void SetCurrentLearningModeReferense()
//    {
//        StageTitlesAndTextsDBController.Instance.SetCurrentLearningModeReferenseByLanguage();
//    }

//    public void SetTitle(string key)
//    {
//        string titleStr = GetProperTitleText(key);

//        this.learningModeUIManager.SetTitle(titleStr);
//    }

//    public void SetSentenceInGuidePanel(string key, UIEventListener.VoidDelegate func)
//    {
//        string value = GetProperSentenceText(key);

//        this.learningModeUIManager.SetSentenceInGuidePanel(value, func);

//        AudioManager.Instance.PlayFxRandom();
//    }

//    public void SetSentenceInMiddleGuidePanel(string key, int middleGuideYPos)
//    {
//        string value = GetProperSentenceText(key);

//        this.learningModeUIManager.SetSentenceInMiddleGuidePanel(value, middleGuideYPos);

//        AudioManager.Instance.PlayFxRandom();
//    }

//    public void SetSentenceInMiddleGuidePanel(string key, EnumSets.MiddleGuidePos middleGuideYPos)
//    {
//        string value = GetProperSentenceText(key);

//        this.learningModeUIManager.SetSentenceInMiddleGuidePanel(value, middleGuideYPos);

//        AudioManager.Instance.PlayFxRandom();
//    }

//    public void SetSentenceInEndGuidePanel(string key)
//    {
//        AudioManager.Instance.PlayFxEnd();

//        string value = GetProperSentenceText(key);

//        this.learningModeUIManager.SetSentenceInEndGuidePanel(value);

//        CompleteSmartLearningMode(); // 스마트학습 수행완료

//        SendLogEventWhenSmartLearningEnd();

//        HandlingAfterSmartLearningPlayingComplete();
//        // StartCoroutine(CorActivateEndMenuPopUp());
//    }

//    IEnumerator CorActivateEndMenuPopUp()
//    {
//        yield return waitDelayActivateEndMenuPopUp;

//        ActivateEndMenuPopUp(this.infoVideo);
//    }

//    private void ReInitIsActivatedSmartLearningMode()
//    {
//        isActivatedSmartLearningMode = true;
//    }

//    private void CompleteSmartLearningMode()
//    {
//        isActivatedSmartLearningMode = false;
//    }

//    public string GetProperTitleText(string key)
//    {
//        string value = "";

//        value = StageTitlesAndTextsDBController.Instance.GetTitle(key);

//        return value;
//    }

//    public string GetProperSentenceText(string key)
//    {
//        string value = "";

//        value = StageTitlesAndTextsDBController.Instance.GetText(key);

//        return value;
//    }

//    public void DeActivateStartGuidePanel()
//    {
//        this.learningModeUIManager.DeActivateStartGuidePanel();
//    }

//    public void DeActivateMiddleGuidePanel()
//    {
//        this.learningModeUIManager.DeActivateMiddleGuidePanel();
//    }

//    public void DeActivateEndGuidePanel()
//    {
//        this.learningModeUIManager.DeActivateEndGuidePanel();
//    }

//    public void ActivateLastMissionPanel()
//    {
//        this.learningModeUIManager.ActivateLastMissionPanel();
//    }

//    public void ActivateCompletePanel()
//    {
//        this.learningModeUIManager.ActivateCompletePanel();
//    }

//    public void DeActivateLastMissionPanel()
//    {
//        this.learningModeUIManager.DeActivateLastMissionPanel();
//    }

//    public void DeActivateCompletePanel()
//    {
//        this.learningModeUIManager.DeActivateCompletePanel();
//    }

//    public EnumSets.ProgramLangSetting GetCurrentLanguage()
//    {
//        return currentLang;
//    }

//    public NGUIAtlas GetCurrentAtlas()
//    {
//        return atlas;
//    }

//    public NGUIAtlas GetCurrentBGAtlas()
//    {
//        return bgAtlas;
//    }
//    //-------------------------------------------------------
//    // 카메라 이동구문
//    public void MoveCameraViewToSpecificAttentionPoint(int index, bool isTopLimit = true)
//    {
//        UnityEngine.Debug.Log($"* attentionPoints[index].position : {index} , {attentionPoints[index].position}");
//        if (Screen.orientation.Equals(ScreenOrientation.Portrait))
//        {
//            CameraManager.ins.MoveCameraInPortrait(attentionPoints[index].position, 0.5f, isTopLimit);
//        }
//        else
//        {
//            if (Screen.orientation.Equals(ScreenOrientation.Landscape))
//            {
//                CameraManager.ins.MoveCameraInLandscape(attentionPoints[index].position, 0.5f, isTopLimit);
//            }
//        }

//        // UnityEngine.Debug.Log($"pos : {attentionPoints[index].position}");

//    }

//    public void MoveCameraViewToSpecificAttentionPoint(int index, float movingTime, bool isTopLimit = true)
//    {
//        // CameraManager.ins.MoveCamera(attentionPoints[index].position, movingTime);

//        if (Screen.orientation.Equals(ScreenOrientation.Portrait))
//        {
//            CameraManager.ins.MoveCameraInPortrait(attentionPoints[index].position, movingTime, isTopLimit);
//        }
//        else
//        {
//            if (Screen.orientation.Equals(ScreenOrientation.Landscape))
//            {
//                CameraManager.ins.MoveCameraInLandscape(attentionPoints[index].position, movingTime, isTopLimit);
//            }
//        }
//    }

//    public void MoveCameraInLandscapeHeadingToMiddle(int index, float movingTime)
//    {
//        CameraManager.ins.MoveCameraInLandscapeHeadingToMiddleSpot(attentionPoints[index].position, movingTime);
//    }

//    public void MoveCameraViewToSpecificTargetPos(Transform target, EnumSets.FocusSpotOnLandscape focusSpotOnLandscape)
//    {
//        if (Screen.orientation.Equals(ScreenOrientation.Portrait))
//        {
//            CameraManager.ins.MoveCameraInPortrait(target.position, 0.5f);
//        }
//        else
//        {
//            if (Screen.orientation.Equals(ScreenOrientation.Landscape))
//            {
//                CameraManager.ins.MoveCameraInLandscape(target.position, 0.5f, focusSpotOnLandscape);
//            }
//        }
//    }

//    //--------------------------------------------------------
//    // 우측 상단 'X' 버튼에 참조되어있음
//    public void OnClickExitBtn()
//    {
//        // UnityEngine.Debug.Log("OnClickExitBtn");
//        HandlingExitPopUp();
//    }

//    // exit 팝업의 끝내기 버튼에 참조되어있음
//    public void DeActivatePopUp()
//    {
//        this.learningModeUIManager.DeActivatePopUp();
//    }

//    private void HandlingExitPopUp()
//    {
//        this.learningModeUIManager.HandlingExitPopUp();
//        // this.learningModeUIManager.HandlingExitPopUp(currentStage);
//    }

//    [Obsolete]
//    private void ActivateErrorWhenLoadSomethingPopUp()
//    {
//        this.learningModeUIManager.ActivateErrorWhenLoadSomethingPopUp();
//    }

//    public void ExitGameScene()
//    {
//        if (ScreenManager.ins.GetIsPortraitState())
//        {
//            ResetSmartLearningModeForExit();
//        }
//        else
//        {
//            StartCoroutine(CorExitGameScene());
//        }
//    }

//    IEnumerator CorExitGameScene()
//    {
//        ActivateLoadingPanel();

//        ScreenManager.ins.ChangeScreenOrientataion();

//#if UNITY_EDITOR
//        yield return new WaitForEndOfFrame();
//#else
//        yield return new WaitForSeconds(0.7f); // 모바일에서 세로 전환 시간이 필요함
//#endif

//        ResetSmartLearningModeForExit();

//        DeActivateLoadingPanel();
//    }

//    private void ActivateLoadingPanel()
//    {
//        this.learningModeUIManager.ActivateLoadingPanel();
//    }

//    private void DeActivateLoadingPanel()
//    {
//        this.learningModeUIManager.DeActivateLoadingPanel();
//    }

//    private void ActivateLoadingPanelForDBRecording()
//    {
//        this.learningModeUIManager.ActivateLoadingPanelForDBRecording();
//    }

//    private void DeActivateLoadingPanelForDBRecording()
//    {
//        this.learningModeUIManager.DeActivateLoadingPanelForDBRecording();
//    }

//    public void QuitAppFunc()
//    {
//#if UNITY_EDITOR
//        UnityEditor.EditorApplication.isPlaying = false;
//#else
//                Application.Quit();
//#endif
//    }

//    // --------------------------------------------------------------------------------

//    public void ChangeAttentionPointsToPortrait()
//    {
//        attentionPoints = attentionPointsOnPortrait;

//        this.safeArea.SetPortraitMode();
//        UnityEngine.Debug.Log($"ChangeAttentionPoints To Portrait");
//    }

//    public void ChangeAttentionPointsToLandscape()
//    {
//        attentionPoints = attentionPointsOnLandscape;

//        this.safeArea.SetLandscapeMode();
//        UnityEngine.Debug.Log($"ChangeAttentionPoints To Landscape");
//    }

//    // --------------------------------------------------------------------------------

//    /// <summary>
//    /// 학습완료 패널(구체적인 축하이미지는 없고, 버튼만 존재) 띄우기, 스마트학습 전용
//    /// </summary>
//    [Obsolete]
//    private void ActivateEndMenuPopUp()
//    {
//        // 211006, 중간업데이트를 위해서 기존 완료루틴 되살림 -> 주석처리 복구
//        // this.learningModeUIManager.ActivateEndMenuPopUp();

//        afterSpecificLessonCompleteHandlingModuleInSmartLearningMode.CheckCanDoNextSpecificLecture(() =>
//        {
//            // 스마트학습은 다음 스마트학습 버튼이 비활성화된다
//            afterSpecificLessonCompleteHandlingModuleInSmartLearningMode.SetNextBtnActions(ReadyToLoadPlayer, null);

//            ReInitIsActivatedSmartLearningMode(); // 뒤로가기시 exit 팝업을 띄우기위함
//        });
//    }

//    private void ActivateEndMenuPopUp(Info_Video infoVideo)
//    {
//        afterSpecificLessonCompleteHandlingModuleInSmartLearningMode.CheckCanDoNextSpecificLecture(infoVideo, () =>
//        {
//            var isLastLecture = ParentHomeController.Instance.IsCompleteLastLecture(infoVideo);

//            if (isLastLecture)
//            {
//                var isTrialAleadyCompleted = ParentClassController.Instance.IsTrialAleadyCompleted();

//                if (isTrialAleadyCompleted)
//                {
//                    // 트라이얼을 이미 수행완료한 상태
//                    // 일반적인 진행

//                    if (!ParentHomeController.Instance.IsGuestLoginState())
//                    {
//                        SetNextLectureLoadCallback();
//                    }
//                    else
//                    {
//                        afterSpecificLessonCompleteHandlingModuleInSmartLearningMode
//                            .SetNextBtnActionsWithInfoVideo((nextInfoVideo) =>
//                            {
//                                ActivateYouNeedToLoginPopUp();

//                            }, null);

//                        ReInitIsActivatedSmartLearningMode(); // 뒤로가기시 exit 팝업을 띄우기위함
//                    }
//                }
//                else
//                {
//                    // 스마트학습은 다음 스마트학습 버튼이 비활성화된다
//                    afterSpecificLessonCompleteHandlingModuleInSmartLearningMode
//                        .SetNextBtnActionsWithInfoVideo((nextInfoVideo) => {

//                            if (!ParentHomeController.Instance.IsGuestLoginState())
//                            {
//                                ActivateYouHaveToCompleteTutorialFirstPopUp();
//                            }
//                            else
//                            {
//                                ActivateYouNeedToLoginPopUp();
//                            }

//                        }, null);

//                    ReInitIsActivatedSmartLearningMode(); // 뒤로가기시 exit 팝업을 띄우기위함
//                }
//            }
//            else
//            {
//                SetNextLectureLoadCallback();
//            }
//        });

//        #region old
//        /*
//        var isLastLecture = ParentHomeController.Instance.IsCompleteLastLecture(infoVideo);

//        if(isLastLecture)
//        {
//            var isTrialAleadyCompleted = ParentClassController.Instance.IsTrialAleadyCompleted();

//            if(isTrialAleadyCompleted)
//            {
//                // 트라이얼을 이미 수행완료한 상태
//                // 일반적인 진행
//                afterSpecificLessonCompleteHandlingModuleInSmartLearningMode.CheckCanDoNextSpecificLecture(infoVideo, () =>
//                {
//                    SetNextLectureLoadCallback();
//                });
//            }
//            else
//            {
//                afterSpecificLessonCompleteHandlingModuleInSmartLearningMode.CheckCanDoNextSpecificLecture(infoVideo, () =>
//                {
//                    // 스마트학습은 다음 스마트학습 버튼이 비활성화된다
//                    afterSpecificLessonCompleteHandlingModuleInSmartLearningMode
//                        .SetNextBtnActionsWithInfoVideo((nextInfoVideo) => {

//                            ActivateYouHaveToCompleteTutorialFirstPopUp();

//                        }, null);

//                    ReInitIsActivatedSmartLearningMode(); // 뒤로가기시 exit 팝업을 띄우기위함
//                });
//            }
//        }
//        else
//        {
//            afterSpecificLessonCompleteHandlingModuleInSmartLearningMode.CheckCanDoNextSpecificLecture(infoVideo, () =>
//            {
//                SetNextLectureLoadCallback();
//            });
//        }
//        */
//        #endregion
//    }

//    private void SetNextLectureLoadCallback()
//    {
//        afterSpecificLessonCompleteHandlingModuleInSmartLearningMode.SetNextBtnActionsWithInfoVideo((nextInfoVideo) => {

//            if (ParentClassController.Instance.IsOpenedLecture(nextInfoVideo))
//            {
//                ReadyToLoadPlayer(nextInfoVideo);
//            }
//            else
//            {
//                ActivateThisLectureNotYetOpenPopUp();
//            }

//        }, null);  // 스마트학습은 다음 스마트학습 버튼이 비활성화된다

//        ReInitIsActivatedSmartLearningMode(); // 뒤로가기시 exit 팝업을 띄우기위함
//    }

//    [Obsolete]
//    private void ReadyToLoadPlayer(string targetKey)
//    {
//        SetTrueLoadNextVideoState();

//        afterExitSmartLearningMode = () => {

//            LoadNextVideoLesson(targetKey);
//        };

//        // NoNeedToActivateLessonList();

//        ExitGameScene(); // ResetSmartLearningModeForExit 에서 afterExitSmartLearningMode 이벤트를 호출함
//    }

//    private void ReadyToLoadPlayer(Info_Video infoVideo)
//    {
//        afterSpecificLessonCompleteHandlingModuleInSmartLearningMode.OnClickCloseBtn();

//        SetTrueLoadNextVideoState();

//        afterExitSmartLearningMode = () => {

//            LoadNextVideoLesson(infoVideo);
//        };

//        ExitGameScene(); // ResetSmartLearningModeForExit 에서 afterExitSmartLearningMode 이벤트를 호출함
//    }

//    [Obsolete]
//    private async void LoadNextVideoLesson(string targetKey) // video_0
//    {
//        UnityEngine.Debug.Log($"LoadNextVideoLesson !!!! : {targetKey}");

//        if (NetworkController.Instance.IsInternetConnected())
//        {
//            var enumCategory = ParentClassController.Instance.GetEnumCategory(Ui_Stage.ins.ClassCategory);

//            var table = DocsManager.ins.GetTargetVideoTable(enumCategory);

//            if (table.TryGetValue(targetKey, out var infoVideo))
//            {
//                if (Ui_Store.ins != null && Ui_Store.ins.subscriptionCheckerWithRevenueCat.IsSubscribed)
//                {
//                    var task = Ui_Store.ins.subscriptionCheckerWithRevenueCat.GetStillValidSubscription();

//                    await task;
//                }

//                LessonRentalOrOwnManager.Instance.SetInfoVideo(infoVideo);

//                DocsManager.ins.LoadScriptAndAddToDicMsg(infoVideo.Category);

//                var heartConsumeAmount = infoVideo.GetHeartCount() * -1;
//                UnityEngine.Debug.Log($"#@@@ heartConsumeAmount : {heartConsumeAmount}");

//                var needOwnHeartAmount = HeartConsumeInfoManager.Instance.GetVideoHeartCountForOwn() * -1;

//                int[] arrHeartAmount = { heartConsumeAmount, needOwnHeartAmount };

//                // if (LessonRentalOrOwnManager.Instance.IsAnythingPurchasedLesson(targetKey) 
//                if (LessonRentalOrOwnManager.Instance.IsAnythingPurchasedLesson(infoVideo.Category, targetKey)
//                    || DataBaseManager.ins.GetIsFreeStageOrPurchasedStageFromStore(Ui_Lesson.ins.StageIdx))
//                {
//                    // EnterTheLessonOrLearningModeManager.Instance.EnterTheLesson(targetKey, Ui_Player.ins.DissMiss);
//                    // ParentEnterTheLessonOrLearningModeManager.Instance.EnterTheLesson(targetKey, Ui_Player.ins.DissMiss);
//                    ParentEnterTheLessonOrLearningModeManager.Instance.EnterTheLesson(infoVideo, Ui_Player.ins.DissMiss);
//                }
//                else
//                {
//                    var lessonKeys = new string[] { targetKey, infoVideo.GetSmartLearningModeKey() };

//                    var lectureInfo = new SpecificLectureInfo()
//                        .SetInfoVideo(infoVideo)
//                        .SetNeedHeartAmounts(arrHeartAmount);

//                    LessonRentalOrOwnManager.Instance.SetCurrentStudyTypeToLesson();

//                    // LessonRentalOrOwnManager.Instance.ActivateHeartConsumePopUp(targetKey, arrHeartAmount);
//                    LessonRentalOrOwnManager.Instance.ActivateHeartConsumePopUpWithLectureInfo(lectureInfo, () => { // 스마트 학습 진행 후 하트결제하는 루틴

//                        // 월드맵이 켜져있고, 상세 레슨 리스트가 꺼져있는 경우에만 상세 레슨 리스트를 출력한다
//                        // 클래스 화면에서 접근
//                        if (Ui_Lesson.ins.GetIsPanelActivated() && !Ui_LessonList.ins.GetIsPanelActivated())
//                        {
//                            ForceActivateLessonList();
//                        }
//                        else
//                        {
//                            // 홈 화면에서 접근
//                            // 네비바 출력
//                            if (!Ui_Lesson.ins.GetIsPanelActivated() && !Ui_LessonList.ins.GetIsPanelActivated())
//                            {
//                                ParentHomeController.Instance.ShowNaviBtnsSequence();
//                            }
//                        }
//                    });
//                }
//            }
//            else
//            {
//                // "" or "noData" 가 들어오는 경우
//                NetworkController.Instance.ActivateNetworkErrorOneBtnPopUp(); // 스마트학습은 꺼진 상태, 홈 화면 나온상태, 플레이어는 안뜨고 네트워크 에러 팝업만 나옴
//                // NetworkController.Instance.ActivateNetworkErrorPopUp(); // 스마트학습은 꺼진 상태, 홈 화면 나온상태, 플레이어는 안뜨고 네트워크 에러 팝업만 나옴
//            }
//        }
//        else
//        {
//            NetworkController.Instance.ActivateNetworkErrorOneBtnPopUp();
//        }
//    }

//    private async void LoadNextVideoLesson(Info_Video infoVideo) // video_0
//    {
//        UnityEngine.Debug.Log($"LoadNextVideoLesson With InfoVideo !!!! : {infoVideo.Id}");

//        if (NetworkController.Instance.IsInternetConnected())
//        {
//            if (Ui_Store.ins != null && Ui_Store.ins.subscriptionCheckerWithRevenueCat.IsSubscribed)
//            {
//                var task = Ui_Store.ins.subscriptionCheckerWithRevenueCat.GetStillValidSubscription();

//                await task;
//            }

//            var heartConsumeAmount = infoVideo.GetHeartCount() * -1;
//            UnityEngine.Debug.Log($"#@@@ heartConsumeAmount : {heartConsumeAmount}");

//            var needOwnHeartAmount = HeartConsumeInfoManager.Instance.GetVideoHeartCountForOwn() * -1;

//            int[] arrHeartAmount = { heartConsumeAmount, needOwnHeartAmount };

//            // if (LessonRentalOrOwnManager.Instance.IsAnythingPurchasedLesson(targetKey) 
//            if (LessonRentalOrOwnManager.Instance.IsAnythingPurchasedLesson(infoVideo.Category, infoVideo.Id)
//                || DataBaseManager.ins.GetIsFreeStageOrPurchasedStageFromStore(Ui_Lesson.ins.StageIdx))
//            {
//                ParentHomeController.Instance.SetLectureData(infoVideo);

//                ParentEnterTheLessonOrLearningModeManager.Instance.EnterTheLesson(infoVideo, Ui_Player.ins.DissMiss);
//            }
//            else
//            {
//                var lessonKeys = new string[] { infoVideo.Id, infoVideo.GetSmartLearningModeKey() };

//                var lectureInfo = new SpecificLectureInfo()
//                    .SetInfoVideo(infoVideo)
//                    .SetNeedHeartAmounts(arrHeartAmount);

//                LessonRentalOrOwnManager.Instance.SetCurrentStudyTypeToLesson();

//                // LessonRentalOrOwnManager.Instance.ActivateHeartConsumePopUp(targetKey, arrHeartAmount);
//                LessonRentalOrOwnManager.Instance.ActivateHeartConsumePopUpWithLectureInfo(lectureInfo, () => { // 스마트 학습 진행 후 하트결제하는 루틴

//                    // 월드맵이 켜져있고, 상세 레슨 리스트가 꺼져있는 경우에만 상세 레슨 리스트를 출력한다
//                    // 클래스 화면에서 접근
//                    if (Ui_Lesson.ins.GetIsPanelActivated() && !Ui_LessonList.ins.GetIsPanelActivated())
//                    {
//                        ForceActivateLessonList();
//                    }
//                    else
//                    {
//                        // 홈 화면에서 접근
//                        // 네비바 출력
//                        if (!Ui_Lesson.ins.GetIsPanelActivated() && !Ui_LessonList.ins.GetIsPanelActivated())
//                        {
//                            ParentHomeController.Instance.ShowNaviBtnsSequence();
//                        }
//                    }
//                });
//            }
//        }
//        else
//        {
//            NetworkController.Instance.ActivateNetworkErrorOneBtnPopUp();
//        }
//    }

//    // 영상 이어보기 선택 시에는 하단 네비가 나타나면 안된다
//    private void SetTrueLoadNextVideoState()
//    {
//        this.isLoadNextVideoState = true;
//    }

//    private void SetFalseLoadNextVideoState()
//    {
//        this.isLoadNextVideoState = false;
//    }

//    // 다시하기
//    public void RetryStage()
//    {
//        UnityEngine.Debug.Log("RetryStage --------");

//        ActivateLoadingPanel();
//        CameraManager.ins.ResetInit();

//        DeActivateCompletePanel();
//        DeActivateEndGuidePanel();

//        //LoadingController.Instance.ActivateLoadingPanel();

//        Destroy(curStagePanel);

//        curStagePanel = null;

//        DynamicStageLoad(originalLearningModePrefabFromAssetBundle); // retry

//        Resources.UnloadUnusedAssets();

//        CameraManager.ins.ResetCameraPosToUpperLeft();

//        // LoadingController.Instance.DeActivateLoadingPanel();

//        SendLogEventWhenSmartLearningStart();

//        ReInitIsActivatedSmartLearningMode();

//        DeActivateLoadingPanel();
//    }

//    [Obsolete]
//    // 이어하기 
//    public void TryNextStage()
//    {
//        UnityEngine.Debug.Log($"TryNextStage %^&*(");

//        CameraManager.ins.ResetInit();
//        // ResetAddressableObjects();

//        DeActivateCompletePanel();
//        DeActivateEndGuidePanel();

//        // LoadingController.Instance.ActivateLoadingPanel();

//        int index = this.stages.FindIndex(x => x.Equals(pureStageStr));
//        UnityEngine.Debug.Log($"index : {index}");

//        if (index < listIndexLimit)
//        {
//            int tmpIndex = index + 1;

//            this.pureStageStr = this.stages[tmpIndex]; // 112

//            StringBuilder sb = new StringBuilder();

//            // to do
//            // sb.Append(AppInfo.Instance.ChosenProgramName.ToString().ToUpper()); // PR
//            sb.Append("PR");
//            sb.Append("_");
//            sb.Append(this.pureStageStr);

//            currentStage = sb.ToString(); // PR_112
//            UnityEngine.Debug.Log($"currentStage : {currentStage}");

//            Destroy(curStagePanel);

//            curStagePanel = null;

//            InitAtlas();
//            // InitAtlas2();

//            InitBGAtlas();
//            // InitBGAtlas2();

//            DynamicStageLoad();
//            // DynamicStageLoad2();

//            CameraManager.ins.InitCamera();

//            Resources.UnloadUnusedAssets();

//            CameraManager.ins.ResetCameraPosToUpperLeft();

//            // to do
//            // AppInfo.Instance.SetChosenSpecificStage(this.currentStage);
//            CheckIsNewStage();

//            LoadingController.Instance.DeActivateLoadingPanel();

//            if (tmpIndex == listIndexLimit)
//            {
//                this.isLastStage = true;
//            }
//        }
//    }

//    private void CheckIsLastStage()
//    {
//        listIndexLimit = this.stages.Count - 1;

//        UnityEngine.Debug.Log($"listIndexLimit : {listIndexLimit}");

//        int index = this.stages.FindIndex(x => x.Equals(pureStageStr));
//        UnityEngine.Debug.Log($"index : {index}");

//        if (index == this.listIndexLimit)
//        {
//            this.isLastStage = true;
//        }
//    }

//    /// <summary>
//    /// 이전에 해당 스테이지를 살펴본 적이 있는 지 확인, 살펴봤다면 New 상태 끄기
//    /// </summary>
//    private void CheckIsNewStage()
//    {
//        StringBuilder sb = new StringBuilder();

//        sb.Append(NEW_CHECK_PREFIX);
//        sb.Append(currentStage);

//        SetCheckOutNewMarkStr(sb.ToString());

//        int isNewStage = PlayerPrefs.GetInt(this.checkOutNewMarkStr, 1);

//        if (isNewStage == 1)
//        {
//            CheckOutNewMarkStr();
//        }

//    }

//    // new 마크 제거용
//    private void CheckOutNewMarkStr()
//    {
//        PlayerPrefs.SetInt(this.checkOutNewMarkStr, 0);
//        PlayerPrefs.Save();
//    }

//    public void SetCheckOutNewMarkStr(string newMarkStr)
//    {
//        this.checkOutNewMarkStr = newMarkStr;
//    }

//    public Action particleReScalerAction = null; // 이미 complete 패널이 나온 상태에서 화면 전환을 할 경우 대응
//    /// <summary>
//    /// 세로형 <-> 가로형 변환시 팝업 등의 scale 조절하기
//    /// </summary>
//    public void ReScalePopUpsInPortrait()
//    {
//        UnityEngine.Debug.Log("ReScalePopUpsInPortrait ^^^^^");
//        this.learningModeUIManager.ReScalePopUpsInPortrait();

//        particleReScalerAction?.Invoke();
//    }

//    public void ReScalePopUpsInLandscape(float sizeOfLandscapeOrthographics)
//    {
//        this.learningModeUIManager.ReScalePopUpsInLandscape(sizeOfLandscapeOrthographics);

//        particleReScalerAction?.Invoke();
//    }

//    public Action resetAndUpdateAnchorsInUISpriteAction = null;

//    /// <summary>
//    /// uisprite anchors 갱신하기
//    /// </summary>
//    public void CallResetAndUpdateAnchorsInUISprite()
//    {
//        resetAndUpdateAnchorsInUISpriteAction?.Invoke();
//    }

//    public void ResetAndUpdateAnchorInMiddleBubble()
//    {
//        UnityEngine.Debug.Log("???? ResetAndUpdateAnchorInMiddleBubble");

//        this.learningModeUIManager.ResetAndUpdateAnchorInMiddleBubble();
//    }

//    private void ResetSmartLearningModeForExit()
//    {
//        ResetData();

//        StopAllCoroutines();

//        UnloadAssetBundle();

//        CameraManager.ins.ResetCameraPosToUpperLeft();
//        CameraManager.ins.ResetInit();

//        DeActivateStartGuidePanel();
//        DeActivateMiddleGuidePanel();
//        DeActivateEndGuidePanel();
//        DeActivateCompletePanel();
//        DeActivateLastMissionPanel();

//        Destroy(curStagePanel);
//        curStagePanel = null;

//        // Destroying assets is not permitted to avoid data loss. 예외로 인해 Destroy 불가함
//        // CorLoadPrefabFromMemory 에서 originalLearningModePrefabFromAssetBundle 에 에셋번들 요소를 부여하는데
//        // originalLearningModePrefabFromAssetBundle 를 prefab으로 인식하는 듯함
//        // Destroy 대신 null 처리만 함
//        // Destroy(originalLearningModePrefabFromAssetBundle); // ResetSmartLearningModeForExit
//        originalLearningModePrefabFromAssetBundle = null;

//        atlas = null;
//        bgAtlas = null;

//        learningModeObjects.SetActive(false);

//        ActivateOriginalObjects();

//        AudioManager.Instance.StopBGM();

//        GameSoundManager.ins.PlayBGMByMainState();

//        Ui_Player.ins.NeedToActivateLessonList();

//        CompleteSmartLearningMode(); // 스마트학습 수행 도중 끝내는 경우 상정

//        afterExitSmartLearningMode?.Invoke(); // '플레이어를 출력하기'가 대기중이라면 플레이어를 출력한다
//        afterExitSmartLearningMode = null;

//        Resources.UnloadUnusedAssets();
//        GC.Collect();

//        if (isLoadNextVideoState) // 영상 이어보기를 할 때는 하단 네비바 관련 루틴을 작동시키지않는다
//        {
//            SetFalseLoadNextVideoState();

//            ResetNoNeedToActivateLessonListState();
//        }
//        else
//        {
//            // 북마크, 최근 학습등으로 스마트학습에 들어왔고, 스마트학습을 끝낸다면
//            // 클래스를 통해서 스마트학습에 들어왔거나
//            if (Ui_Lesson.ins.GetIsPanelActivated())
//            {
//                if (!noNeedToActivateLessonList)
//                {
//                    ForceActivateLessonList();
//                }
//                else
//                {
//                    ResetNoNeedToActivateLessonListState();

//                    ParentHomeController.Instance.ShowNaviBtnsSequence();
//                }
//            }
//            else
//            {
//                ResetNoNeedToActivateLessonListState();

//                ParentHomeController.Instance.ShowNaviBtnsSequence();
//            }
//        }
//    }

//    private void ForceActivateLessonList()
//    {
//        ParentClassController.Instance.ForceActivateLessonList();
//    }

//    private void ResetData()
//    {
//        currentLectureKey = "";
//        currentStage = "";

//        this.infoVideo = null;
//    }

//    /// <summary>
//    /// 새로운 스마트학습 출력전에 보여야할 것들 
//    /// </summary>
//    private void ActivateOriginalObjects()
//    {
//        for (int i = 0; i < objectOriginalList.Count; i++)
//        {
//            objectOriginalList[i].SetActive(true);
//        }
//    }

//    private void DeActivateOriginalObjects()
//    {
//        for (int i = 0; i < objectOriginalList.Count; i++)
//        {
//            objectOriginalList[i].SetActive(false);
//        }
//    }

//    private Action networkErrorAction = null;
//    public void ActivateNetworkErrorPopUpInLearningMode(Action nextStep)
//    {
//        this.learningModeUIManager.ActivateNetworkErrorPopUpInLearningMode();

//        this.networkErrorAction = nextStep;
//    }

//    private Action loadAssetsErrorAction = null;
//    public void ActivateGotErrorWhenLoadingSmartlearningModeAssetsPopUp(Action nextStep)
//    {
//        this.learningModeUIManager.ActivateLoadAssetsErrorPopUpInLearningMode();

//        this.loadAssetsErrorAction = nextStep;
//    }

//    // 네트워크 에러 팝업에 참조
//    public void OnClickCloseBtnInNetworkErrorPopUp()
//    {
//        this.learningModeUIManager.DeActivateNetworkErrorPopUpInLearningMode();

//        this.networkErrorAction?.Invoke();
//        this.networkErrorAction = null;
//    }

//    // 에셋 로드 에러 팝업에 참조
//    public void OnClickCloseBtnInLoadAssetsErrorPopUp()
//    {
//        this.learningModeUIManager.DeActivateLoadAssetsErrorPopUpInLearningMode();

//        this.loadAssetsErrorAction?.Invoke();
//        this.loadAssetsErrorAction = null;
//    }

//    /// <summary>
//    /// 가로, 세로 오리엔테이션 변경시 가로에서 변경버튼이 안먹는 문제발생함, 문제해결을 위해서 해당 코드 실행하기
//    /// boxcollider가 박힌 sprite를 켰다가 끄면 해당 오류가 발생하지않음
//    /// top camera 에 참조되어있는 uicamera의 input 처리가 제대로 안되서 그런듯함
//    /// 해당 코드는 일단은 임시 해결방안임
//    /// </summary>
//    public void LoadPreventScreenOrientationChangingFreeze()
//    {
//        this.learningModeUIManager.LoadPreventScreenOrientationChangingFreeze();
//    }

//    private void UnloadAssetBundle()
//    {
//        if (this.assetbundle != null)
//        {
//            assetbundle.Unload(false);

//            assetbundle = null;
//        }
//    }


//    //-----------------------------------------------------------------------------------------------------

//    private void HandlingAfterSmartLearningPlayingComplete()
//    {
//        var aleadyCompleted = ParentHomeController.Instance.CheckIsAleadyCompletedSpecificVideoLesson(currentLectureKey);

//        if (aleadyCompleted)
//        {
//            StartCoroutine(CorActivateEndMenuPopUp());
//        }
//        else
//        {
//            if (ParentHomeController.Instance.IsGuestLoginState())
//            {
//                // StartCoroutine(CorActivateEndMenuPopUp());
//                JustAddCompleteInfo();
//            }
//            else
//            {
//                if (NetworkController.Instance.IsInternetConnected())
//                {
//                    StartCoroutine(CorAddCompleteInfoToDB());
//                }
//                else
//                {
//                    NetworkController.Instance.ActivateNetworkErrorOneBtnPopUp();

//                    StartCoroutine(CorActivateEndMenuPopUp());
//                }
//            }
//        }
//    }

//    IEnumerator CorAddCompleteInfoToDB()
//    {
//        yield return waitDelayAddCompleteInfo;

//        ActivateLoadingPanelForDBRecording();

//        // 최초로 세부강의-비디오 시청을 완료함
//        ParentHomeController.Instance.AddSpecificCompletedVideoLessonInfo(currentLectureKey, () => { // learningmode_0 형태로 저장이 필요

//            // UnityEngine.Debug.Log($"add done");

//            DeActivateLoadingPanelForDBRecording();

//            StartCoroutine(CorActivateEndMenuPopUp());
//        });
//    }

//    private void JustAddCompleteInfo()
//    {
//        ParentHomeController.Instance.AddSpecificCompletedVideoLessonInfo(currentLectureKey);

//        StartCoroutine(CorActivateEndMenuPopUp());
//    }

//    /// <summary>
//    /// 단순히 스마트학습을 끄느냐, 다음 영상보기를 하기위해서 스마트학습을 끄느냐에 따라 레슨 리스트를 띄울 지 결정한다
//    /// </summary>
//    public void NoNeedToActivateLessonList()
//    {
//        noNeedToActivateLessonList = true;
//    }

//    public void ResetNoNeedToActivateLessonListState()
//    {
//        noNeedToActivateLessonList = false;
//    }

//    private Stack<Action> stackPopUpCloseMethod = new Stack<Action>();

//    public void AddPopUpStack(Action closeMethod)
//    {
//        if (!this.stackPopUpCloseMethod.Contains(closeMethod))
//        {
//            this.stackPopUpCloseMethod.Push(closeMethod);
//        }
//    }

//    public void RemovePopUpStack(Action closeMethod)
//    {
//        if (this.stackPopUpCloseMethod.Contains(closeMethod))
//        {
//            //var callback = this.stackPopUpCloseMethod.Pop();

//            //callback?.Invoke();
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (isActivatedSmartLearningMode)
//        {
//            if (!isLoadingSmartLearningModeAsset && Input.GetKeyDown(KeyCode.Escape))
//            {
//                HandlingExitPopUp(); // <-- 학습 씬에서는 앱 종료팝업 대신 이전 씬으로 돌아가기 팝업을 출력하기
//            }
//        }
//    }

//    // ----------------------------------------------------------------------

//    private void SendLogEventWhenSmartLearningStart()
//    {
//        if (ParentHomeController.Instance.IsGuestLoginState())
//        {
//            return;
//        }

//        FirebaseLogEventTool.Instance.LogEventWithAdditionalInformation(EnumSets.FirebaseLogEventKey.SmartLearningStart, currentStage);
//    }

//    private void SendLogEventWhenSmartLearningEnd()
//    {
//        if (ParentHomeController.Instance.IsGuestLoginState())
//        {
//            return;
//        }

//        FirebaseLogEventTool.Instance.LogEventWithAdditionalInformation(EnumSets.FirebaseLogEventKey.SmartLearningEnd, currentStage);
//    }

//    private void ActivateYouHaveToCompleteTutorialFirstPopUp()
//    {
//        this.learningModeUIManager.ActivateYouHaveToCompleteTutorialFirstPopUp();
//    }

//    private void ActivateThisLectureNotYetOpenPopUp()
//    {
//        this.learningModeUIManager.ActivateThisLectureNotYetOpenPopUp();
//    }

//    private void ActivateYouNeedToLoginPopUp()
//    {
//        logInPopUpManager.ActivatePopUp();
//    }
}