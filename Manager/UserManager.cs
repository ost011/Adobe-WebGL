using System;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.Specialized;
using System.Threading;
using Cysharp.Threading.Tasks;

public class UserManager 
{
    public static UserManager Instance = null;

    private UserInfo userInfo = null;

    // playingTime 기록관련
    private int lastestPlayingTime = 0; // 분단위

    private bool isRecordingUserPlayingTimeTask = false;

    // 유저가 뒤로가기 누르기 전 플레이하던 강의 스테이지
    private string lastPlayingStageType;

    private CancellationTokenSource cancellationTokenSource = null;
    
    private bool isSetPreventDualLogin = false;

    private string loginRandomKey = "something"; // device Unique id
    private const string ADDRESS_STR = "loginRandomKey";

    private StringBuilder sb = new StringBuilder();
    
    private const string SLASH_STR = "/";
    private const string PRIZE_ITEMS_STR = "prizeItems";

    // 경품 카테고리 상수
    private const string PRIZE_CATEGORY_NORMAL = "normal";
    private const string PRIZE_CATEGORY_SPECIAL = "special";

    static UserManager()
    {
        Instance = new UserManager();
    }

    private UserManager()
    {

    }

    // 에디터 로그인에 사용 중
    public void CreateUserInfoOnEditor(UserInfo userInfo)
    {
        CustomDebug.Log("CreateUserInfo ---");

        this.userInfo = userInfo;
        // this.userInfo.RemoveOldPrizeItemInfos();
        
        CheckTermsVersion();
    }

    public void CreateUserInfo(string snapShotData)
    {
        // CustomDebug.Log($"Create UserInfo > : {snapShotData}");

        try
        {
            var data = JsonConvert.DeserializeObject<UserInfo>(snapShotData);

            if (data != null)
            {
                CustomDebug.Log($"data.uid : { data.uid}");

                this.userInfo = data;
                // this.userInfo.RemoveOldPrizeItemInfos();

                CheckTermsVersion();
#if UNITY_EDITOR

#elif UNITY_WEBGL

                 FirebaseFunctionsController.Instance.CountUpKPIData("loginCount");
#endif
            }
        }
        catch(Exception e)
        {
            CustomDebug.LogError($"CreateUserInfo error : {e.Message}");
        }
    }

    public void CreateGuestUserInfo(string snapShotData)
    {
        CustomDebug.Log($"Create Guest UserInfo > : {snapShotData}");

        try
        {
            var data = JsonConvert.DeserializeObject<UserInfo>(snapShotData);

            if (data != null)
            {
                CustomDebug.Log($"data.uid : { data.uid}");

                this.userInfo = data;

#if UNITY_EDITOR

#elif UNITY_WEBGL

                 FirebaseFunctionsController.Instance.CountUpKPIData("guestLoginCount");

#endif
                LogInSceneManager.Instance.MoveToHomeScene();
            }
        }
        catch (Exception e)
        {
            CustomDebug.LogError($"CreateUserInfo error : {e.Message}");
        }
    }

    private void CheckTermsVersion()
    {
        var serverTermsVersion = AppInfo.Instance.TermsServerVersion;
        var localTermsVersion = GetUserTermsVersion();

        CustomDebug.Log($"--> CheckTermsVersion >> localTermsVersion : {localTermsVersion} / serverTermsVersion : {serverTermsVersion}");

        if (localTermsVersion >= serverTermsVersion)
        {
            LogInSceneManager.Instance.MoveToHomeScene();
        }
        else
        {
            // 재동의 요청 팝업 출력
            LogInSceneManager.Instance.ActivateOrdinaryTermsAgreePanel();
        }
    }

    private void LoadNextStep()
    {
        CustomDebug.Log("LoadNextStep !");
        // Debug.Log("LoadNextStep");

        //this.nextStep?.Invoke();
        //this.nextStep = null;
    }

    public string GetUserEmail()
    {
        return this.userInfo.email;
    }

    public string GetUID()
    {
        return this.userInfo.uid;
    }

    public string GetMoblieNumber()
    {
        return this.userInfo.mobile;
    }

    public Dictionary<string, int> GetSpecificStageCompletedInfo(string stageKey)
    {
        if(this.userInfo.stageCompletedInfos == null)
        {
            return null;
        }

        if(this.userInfo.stageCompletedInfos.TryGetValue(AppInfo.Instance.GetProgramInfo(), out var programTable))
        {
            if (programTable.TryGetValue(stageKey, out var table))
            {
                return table;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    public Dictionary<string, Dictionary<string, int>> GetStageCompletedInfo()
    {
        if (this.userInfo.stageCompletedInfos.TryGetValue(AppInfo.Instance.GetProgramInfo(), out var programTable))
        {
            return programTable;
        }
        else
        {
            return null;
        }
    }

    public void AddDataInStageCompletedInfo(string key, Dictionary<string, int> value)
    {
        if(this.userInfo.stageCompletedInfos == null)
        {
            // 아예 NULL 이었을 때

            this.userInfo.stageCompletedInfos = new Dictionary<string,Dictionary<string, Dictionary<string, int>>>();

            var tmpTable = new Dictionary<string, Dictionary<string, int>>();
            tmpTable.Add(key, value);

            this.userInfo.stageCompletedInfos.Add(AppInfo.Instance.GetProgramInfo(), tmpTable);
        }
        else
        {
            if(this.userInfo.stageCompletedInfos.TryGetValue(AppInfo.Instance.GetProgramInfo(), out var programTable))
            {
                if (!programTable.ContainsKey(key))
                {
                    // 테이블에 없는 키 밸류가 들어왔을 때

                    programTable.Add(key, value);
                }
                else
                {
                    // 있으면 교체

                    programTable[key] = value;
                }
            }
            else
            {
                var tmpTable = new Dictionary<string, Dictionary<string, int>>();
                tmpTable.Add(key, value);

                this.userInfo.stageCompletedInfos.Add(AppInfo.Instance.GetProgramInfo(), tmpTable);
            }
        }
    }

    /// <returns>key : 난수, valule : PrizeItem 인 테이블을 리턴함</returns>
    public Dictionary<string, PrizeItem> GetSpecificGottenPrizeItemInfos(string stageType, string lessonType)
    {
        if(this.userInfo.prizeItems == null)
        {
            CustomDebug.Log($"{stageType} => GetSpecificGottenPrizeItemInfos, this.userInfo.prizeItemTable == null");

            return null;
        }

        if(this.userInfo.prizeItems.TryGetValue(AppInfo.Instance.GetProgramInfo(), out var programTable))
        {
            if (programTable.TryGetValue(stageType, out var lessonTable))
            {
                if(lessonTable.TryGetValue(lessonType, out var specificPrizeItemTable))
                {
                    return specificPrizeItemTable;
                }
            }
        }
        else
        {
            return null;
        }

        CustomDebug.LogWarning($"^^ Get Specific user's Gotten Prize ItemInfos, no {stageType} / no {lessonType}");

        return null;
    }

    // 220921 - 김형철 추가
    // 유저 상품정보에 난수 키 추가로 인해 해당 코드 추가함
    public bool GetIsUserReceivedReward(IEnumerable<KeyValuePair<string, PrizeItemInfo>> specificPrizeItem)
    {
        // 기본적으로 lesson 당 하나의 상품만 존재함
        // s3 l6 의 경우 일반, 특별 선물이 같이 존재함
        // 같은 스테이지, 레슨 위치를 공유하는 다수의 상품정보가 있을 수 있다

        var isReceived = false;

        // 일단 다수라 할지라도 같은 스테이지, 레슨 위치를 공유하니 첫번째 인덱스의 값을 사용하기
        var stageType = specificPrizeItem.ElementAt(0).Value.stagePos;
        var lessonType = specificPrizeItem.ElementAt(0).Value.lessonPos;
        
        if (this.userInfo.prizeItems == null)
        {
            CustomDebug.Log($"{specificPrizeItem.ElementAt(0).Value} => GetIsUserReceivedReward, this.userInfo.prizeItemTable == null");

            return isReceived;
        }

        if (this.userInfo.prizeItems.TryGetValue(AppInfo.Instance.GetProgramInfo(), out var programTable))
        {
            if (programTable.TryGetValue(stageType, out var subTable))
            {
                if (subTable.TryGetValue(lessonType, out var specificPrizeItemTable))
                {
                    // 기본적으로 lesson 당 하나의 상품만 존재함
                    if (specificPrizeItemTable.Count == 1)
                    {
                        if (specificPrizeItemTable.ElementAt(0).Value.receive)
                        {
                            isReceived = true;
                        }
                    }
                    else
                    {
                        // s3 l6 의 경우 일반, 특별 선물이 같이 존재함
                        // 하지만 문제는 2개 이상이 될 수 있다는 것
                        foreach (var item in specificPrizeItemTable)
                        {
                            // 하나라도 안받았으면 바로 리턴
                            if(!item.Value.receive)
                            {
                                return false;
                            }
                            else
                            {
                                isReceived = true;
                            }    
                        }
                    }
                }
            }
        }

        CustomDebug.Log($"^^ GetIsUserReceivedReward : {isReceived}");

        return isReceived;
    }

    /// <summary>
    /// 해당 강의에서 아직 유저 prizeItems 틀이 기록 안된 개수 얻기
    /// </summary>
    public int GetCountOfCurrentLessonUnRecordedPrizeItemUnderUserDB(IEnumerable<KeyValuePair<string, PrizeItemInfo>> specificPrizeItem)
    {
        var recordedRewardCountOfCurrentLesson = 0;
        var totalRewardCountCurrentLessonGive = specificPrizeItem.Count(); // 해당 강의가 원래 주는 경품 개수
        
        // 일단 다수라 할지라도 같은 스테이지, 레슨 위치를 공유하니 첫번째 인덱스의 값을 사용하기
        var stageType = specificPrizeItem.ElementAt(0).Value.stagePos;
        var lessonType = specificPrizeItem.ElementAt(0).Value.lessonPos;
        
        if (this.userInfo.prizeItems == null)
        {
            CustomDebug.Log($"this.userInfo.prizeItemTable == null, totalRewardCountCurrentLessonGive : {totalRewardCountCurrentLessonGive}");

            return totalRewardCountCurrentLessonGive;
        }

        if (this.userInfo.prizeItems.TryGetValue(AppInfo.Instance.GetProgramInfo(), out var programTable))
        {
            if (programTable.TryGetValue(stageType, out var subTable))
            {
                if (subTable.TryGetValue(lessonType, out var specificPrizeItemTable))
                {
                    recordedRewardCountOfCurrentLesson = specificPrizeItemTable.Count;
                }
            }
        }

        var unReceivedRewardCount = totalRewardCountCurrentLessonGive - recordedRewardCountOfCurrentLesson;

        CustomDebug.Log($"^^ GetUnRecordedRewardCountOfCurrentLesson : {unReceivedRewardCount}");

        return unReceivedRewardCount;
    }

    /// <returns>해당 레슨이 주는 경품 중 userDB에 존재하지 않는 type(name)의 리스트만 반환</returns>
    public List<string> GetRewardNameListOfUnRecordedPrizeItemOfCurrentLesson(string[] lessonIfnos, IEnumerable<KeyValuePair<string, PrizeItemInfo>> specificPrizeItems)
    {
        var stageType = lessonIfnos[0];
        var lessonType = lessonIfnos[1];

        var rewardTypeList = new List<string>();
        
        foreach(var specificPrizeItem in specificPrizeItems)
        {
            rewardTypeList.Add(specificPrizeItem.Value.name);

            CustomDebug.Log($"prizeItemInfos에서 찾은 해당 레슨이 줘야할 모든 경품 : {specificPrizeItem.Value.name}");
        }

        var gottenPrizeTable = GetSpecificGottenPrizeItemInfos(stageType, lessonType); // 해당 스테이지, 레슨의 실질적으로 기록되어 있는 (randomKey / prizeItem)

        //if(gottenPrizeTable == null)
        //{
        //    return rewardTypeList;
        //}

        for(int i = 0; i < gottenPrizeTable.Count; i++)
        {
            if (rewardTypeList.Contains(gottenPrizeTable.ElementAt(i).Value.type)) // 이미 기록되어 있다
            {
                var specificType = gottenPrizeTable.ElementAt(i).Value.type;

                rewardTypeList.Remove(specificType);

                CustomDebug.Log($"{specificType} is removed because it already exists in userDB");
            }
        }

        return rewardTypeList;
    }

    public void UpdateVisitingCount()
    {
        var currentVisitingCount = this.userInfo.visitingCount;

        var nextVisitingCount = ++currentVisitingCount;

        var subPath = DevUtil.Instance.GetTargetPathInSpecificUser("visitingCount");

        var path = DevUtil.Instance.GetTargetPath(EnumSets.DBParentType.Users, subPath);

        FirebaseDatabaseController.Instance.SetTagetData(path, nextVisitingCount.ToString(), (isSuccess) => { 
        
            if(isSuccess == 0)
            {
                CustomDebug.LogError($"update visitingCount Error");
            }
            else
            {
                this.userInfo.visitingCount = nextVisitingCount;

                CustomDebug.Log($">> this.userInfo.visitingCount : {this.userInfo.visitingCount}");
            }
        });
    }

    public void UpdatePrizeData(RewardModule rewardModule, PrizeItem prizeItem)
    {
        CustomDebug.Log($">> UpdatePrizeData, stageType : {rewardModule.StageTypeStr} / lessonType : {rewardModule.lessonType} / prizeItem : {prizeItem.receiveDate} / {prizeItem.type}");

        if(this.userInfo.prizeItems.TryGetValue(AppInfo.Instance.GetProgramInfo(), out var programTable))
        {
            var stageStr = rewardModule.StageTypeStr;
            var lessonStr = rewardModule.LessonTypeStr;
            var randomKey = rewardModule.RandomKey;

            if (programTable.TryGetValue(stageStr, out var subTable))
            {
                var datas = from data in subTable
                            where data.Key.Equals(lessonStr)
                            select data;

                if (datas.Count() > 0)
                {
                    CustomDebug.Log($"datas count : {datas.Count()}");

                    var subLessonTable = subTable[lessonStr];

                    // test
                    for (int i = 0; i < subLessonTable.Count; i++)
                    {
                        CustomDebug.Log($"subLessonTable key : {subLessonTable.ElementAt(i).Key} / value : {subLessonTable.ElementAt(i).Value}");
                    }

                    if(subLessonTable.ContainsKey(randomKey))
                    {
                        subLessonTable[randomKey] = prizeItem;

                        CustomDebug.Log("ContainsKey");
                    }
                    else
                    {
                        subLessonTable.Add(randomKey, prizeItem);

                        CustomDebug.Log("1");
                    }
                }
                else
                {
                    var specificPriztItemTable = new Dictionary<string, PrizeItem>();

                    specificPriztItemTable.Add(randomKey, prizeItem);

                    subTable.Add(lessonStr, specificPriztItemTable);

                    CustomDebug.Log("2");
                }
            }
            else
            {
                var subLessonTable = new Dictionary<string, PrizeItem>();

                subLessonTable.Add(randomKey, prizeItem);

                var lessonTable = new Dictionary<string, Dictionary<string, PrizeItem>>();

                lessonTable.Add(lessonStr, subLessonTable);

                programTable.Add(stageStr, lessonTable);

                CustomDebug.Log("3");
            }
        }
    }

    public void AddPrizeDataTemplateAfterLessonSkipOrComplete(string[] prizeData)
    {
        try
        {
            var stageType = prizeData[0];
            var lessonType = prizeData[1];
            var rewardName = prizeData[2];
            var randomKey = prizeData[3];
            var category = prizeData[4];
            // prizeKey를 안넣는 이유 : 221004 기준으로 유니티 컨텐츠 안에서는 해당 키를 딱히 사용하지 않으므로 해당 메소드에서는 딱히 추가하지 않음
            // 추후 필요시에 추가하기

            var prizeItem = GetDefaultPrizeItem(rewardName, category);

            CustomDebug.Log($">>{stageType} / {lessonType} UpdatePrizeDataOnLessonEnd, rewardName : {rewardName} / randomKey : {randomKey}");

            var subLessonTable = new Dictionary<string, PrizeItem>();

            subLessonTable.Add(randomKey, prizeItem);

            if (this.userInfo.prizeItems == null)
            {
                var stageTable = new Dictionary<string, Dictionary<string, Dictionary<string, PrizeItem>>>();

                var lessonTable = new Dictionary<string, Dictionary<string, PrizeItem>>();

                lessonTable.Add(lessonType, subLessonTable);

                stageTable.Add(stageType, lessonTable);

                InitPrizeItemTable(stageTable);

                CustomDebug.Log("this.userInfo.prizeItems was null");

                return;
            }

            if (this.userInfo.prizeItems.TryGetValue(AppInfo.Instance.GetProgramInfo(), out var programTable))
            {
                if (programTable.TryGetValue(stageType, out var stageTable))
                {
                    if (stageTable.TryGetValue(lessonType, out var lessonTable))
                    {
                        var datas = from data in lessonTable
                                    where data.Key.Equals(randomKey)
                                    select data;

                        if (datas.Count() > 0) // 무조건 1개 존재함
                        {
                            lessonTable[randomKey] = prizeItem;
                        }
                        else
                        {
                            lessonTable.Add(randomKey, prizeItem);
                        }
                    }
                    else
                    {
                        stageTable.Add(lessonType, subLessonTable);
                    }
                }
                else
                {
                    var tmpLessonTable = new Dictionary<string, Dictionary<string, PrizeItem>>();

                    tmpLessonTable.Add(lessonType, subLessonTable);

                    programTable.Add(stageType, tmpLessonTable);
                }
            }
            else
            {
                // this.userInfo.prizeItems 은 존재한다 (이전에 포토샵, ai 상품정보가 있는 경우가 있을 수 있다)
                // 그러나 현재 수행하려하는 프로그램 정보테이블은 없는 경우 새로 생성한다

                var stageTable = new Dictionary<string, Dictionary<string, Dictionary<string, PrizeItem>>>();

                var lessonTable = new Dictionary<string, Dictionary<string, PrizeItem>>();

                lessonTable.Add(lessonType, subLessonTable);

                stageTable.Add(stageType, lessonTable);

                this.userInfo.AddDataToPrizeItemTable(stageTable);
            }

            CustomDebug.Log("UpdatePrizeDataOnLessonEnd Done~~~!!!");
        }
        catch(Exception e)
        {
            CustomDebug.LogError($"error msg UpdatePrizeDataOnLessonEnd : {e.Message}");
        }
    }

    public void UpdatePrizeDataToDB(Action<bool> isReady)
    {
        CustomDebug.Log("UpdatePrizeData >>");

        //sb.Clear();

        //sb.Append(this.userInfo.uid);
        //sb.Append(SLASH_STR);
        //sb.Append("prizeItems");

        var subPath = GetTargetPath("prizeItems");

        var fullPath = DevUtil.Instance.GetTargetPath(EnumSets.DBParentType.Users, subPath);

        var jsonType = DevUtil.Instance.GetJson(this.userInfo.prizeItems);

        FirebaseDatabaseController.Instance.SetTagetData(fullPath, jsonType, (isSuccess) => { 

            if(isSuccess == 1)
            {
                isReady?.Invoke(true);
            }
            else
            {
                CustomDebug.LogError($"UpdatePrizeData Error : {fullPath} / {jsonType}");

                isReady?.Invoke(false);
            }
        });
    }

    public void UpdateLastSignInDate()
    {
        var subPath = GetTargetPath("lastSignInTimestamp");

        var path = DevUtil.Instance.GetTargetPath(EnumSets.DBParentType.Users, subPath);

        var lastSignInTimeStampStr = DevUtil.Instance.GetUTCNowTimeStamp();

        FirebaseDatabaseController.Instance.SetTagetData(path, lastSignInTimeStampStr.ToString(), (isSuccess) => {

            if (isSuccess == 0)
            {
                CustomDebug.LogError($"Update Last SignIn Date Error");
            }
            else
            {
                this.userInfo.lastSignInTimestamp = lastSignInTimeStampStr;

                CustomDebug.Log($">> this.userInfo.lastSignInTimestamp : {this.userInfo.lastSignInTimestamp}");
            }
        });
    }

    public string GetTargetPath(string target)
    {
        sb.Clear();

        sb.Append(this.userInfo.uid);
        sb.Append(SLASH_STR);
        sb.Append(target);

        return sb.ToString();
    }

    public int GetUserTermsVersion()
    {
        return this.userInfo.termsAgree;

        //if(this.userInfo.termsAgreeInfos.TryGetValue("termsVersion", out var localVersion))
        //{
        //    return localVersion;
        //}
        //else
        //{
        //    return 1;
        //}
    }

    public void ResetData()
    {
        this.userInfo = null;
    }

    public Dictionary<string, string> GetRecentTryingLectureTable()
    {
        return this.userInfo.recentTryingLecture;
    }

    public void UpdateLocalRecentTryingLecture(OrderedDictionary recentLectureTable)
    {
        var tmpTable = new Dictionary<string, string>();

        var tmpEnumerator = recentLectureTable.GetEnumerator();

        while (tmpEnumerator.MoveNext())
        {
            tmpTable.Add(tmpEnumerator.Key as string, tmpEnumerator.Value as string);

           // CustomDebug.LogWithColor($"after remove, tableRecentTryingLecture -> {tmpEnumerator.Key} / {tmpEnumerator.Value}", CustomDebug.ColorSet.Magenta);
        }

        this.userInfo.SetRecentTryingLecture(tmpTable);
    }

    public void RecordingUserPlayingTime()
    {
        CustomDebug.Log($"Try UserManager, RecordingUserPlayingTime, isRecordingUserPlayingTimeTask : {isRecordingUserPlayingTimeTask}---------------");

        if (!isRecordingUserPlayingTimeTask)
        {
            CustomDebug.Log("recordUserPlayingTimeTask go");

            cancellationTokenSource = new CancellationTokenSource();

            this.lastestPlayingTime = this.userInfo.playingTime[AppInfo.Instance.GetProgramInfo()];
            
            RecordingUserPlayingTimeTask().Forget();

            isRecordingUserPlayingTimeTask = true;
        }
        else
        {
            CustomDebug.Log("Aleady recording user playing time");
        }
    }

    private async UniTaskVoid RecordingUserPlayingTimeTask()
    {
        while(true)
        {
            if(!cancellationTokenSource.IsCancellationRequested)
            {
                await UniTask.Delay(60000, cancellationToken: cancellationTokenSource.Token); // 60초

                ++this.lastestPlayingTime;

                CustomDebug.Log($"lastestPlayingTime : {lastestPlayingTime} , try write time in user db");

                UpdateUserPlayingTime();
            }
            else
            {
                CustomDebug.Log("CancellationRequested ...");

                break;
            }
        }
    }

    public void ForceCancelRecordingUserPlayingTimeTask()
    {
        CustomDebug.Log(">> ForceCancelRecordingUserPlayingTimeTask --- ");

        if(isRecordingUserPlayingTimeTask)
        {
            cancellationTokenSource.Cancel();

            cancellationTokenSource.Dispose();

            isRecordingUserPlayingTimeTask = false;

            // 김형철 수정, 221116 - 매 분마다 유저 db 에 기록하므로 지금 시점에서는 기록을 할 필요없음
            // UpdateUserPlayingTime();
        }
    }

    private void UpdateUserPlayingTime()
    {
        var subPath = DevUtil.Instance.GetTargetPath2Parts("playingTime", AppInfo.Instance.GetProgramInfo());

        var refPath = DevUtil.Instance.GetTargetPath(EnumSets.DBParentType.Users, GetUID());

        CustomDebug.Log($"Finished, lastestPlayingTime : {lastestPlayingTime} / refPath : {refPath}");

        var tmpTable = new Dictionary<string, int>
        {
            {subPath, this.lastestPlayingTime}
        };

        var json = DevUtil.Instance.GetJson(tmpTable);

        FirebaseDatabaseController.Instance.JustUpdate(refPath, json);
    }

    private void InitPrizeItemTable(Dictionary<string, Dictionary<string, Dictionary<string, PrizeItem>>> stageTable)
    {
        this.userInfo.InitPrizeItemTable();

        this.userInfo.AddDataToPrizeItemTable(stageTable);
    }

    /// <returns>key[0] : prizeItemPath, key[1] : randomKey, key[2] : category / value : defaultPrizeItem</returns>
    public Dictionary<string[], PrizeItem> GetInitialRewardInfoAndDefaultPrizeItemPair(string[] lessonInfos, string rewardName)
    {
        var resultTable = new Dictionary<string[], PrizeItem>();

        var stageType = lessonInfos[0];
        var lessonType = lessonInfos[1];

        var randomKey = FirebaseDatabaseController.Instance.GetPushKey();

        var prizeItemInfoTable = AppInfo.Instance.prizeItemInfoTable;

        var specificPrizeItem = from data in prizeItemInfoTable
                                where data.Value.stagePos.Equals(stageType) && data.Value.lessonPos.Equals(lessonType) && data.Value.name.Equals(rewardName)
                                select data;

        if(specificPrizeItem.Count() > 0)
        {
            var category = "";

            if(specificPrizeItem.ElementAt(0).Value.special)
            {
                category = PRIZE_CATEGORY_SPECIAL;
            }
            else
            {
                category = PRIZE_CATEGORY_NORMAL;
            }

            var prizeKey = specificPrizeItem.ElementAt(0).Key;

            var prizeItemInfos = new string[] { rewardName, prizeKey, category };

            var defaultPrizeItem = GetDefaultPrizeItem(prizeItemInfos);

            var prizeItemSubPath = GetPathToSpecificPrizeItem(lessonInfos, randomKey);

            string[] arrayPathRandomKey = new string[] { prizeItemSubPath, randomKey, category };

            resultTable.Add(arrayPathRandomKey, defaultPrizeItem);

            return resultTable;
        }
        else
        {
            return null;
        }
    }

    public PrizeItem GetDefaultPrizeItem(string rewardName, string category)
    {
        PrizeItem prizeItem = new PrizeItem();

        prizeItem.receive = false;
        prizeItem.receiveDate = -1;
        prizeItem.type = rewardName;
        prizeItem.prizeKey = "유니티 안에서는 안씀"; // 221004 기준
        prizeItem.targetEmailAddress = "none";
        prizeItem.category = category;

        return prizeItem;
    }

    // 0 : rewardName, 1 : prizeKey, 2 : category
    public PrizeItem GetDefaultPrizeItem(string[] prizeItemInfos)
    {
        var rewardName = prizeItemInfos[0];
        var prizeKey = prizeItemInfos[1];
        var category = prizeItemInfos[2];

        PrizeItem prizeItem = new PrizeItem();

        prizeItem.receive = false;
        prizeItem.receiveDate = -1;
        prizeItem.type = rewardName;
        prizeItem.prizeKey = prizeKey;
        prizeItem.targetEmailAddress = "none";
        prizeItem.category = category;

        return prizeItem;
    }

    private string GetPathToSpecificPrizeItem(string[] lessonInfos, string randomKey)
    {
        var stageType = lessonInfos[0];
        var lessonType = lessonInfos[1];

        var programInfoUnderPrizeitems = DevUtil.Instance.GetTargetPath2Parts(PRIZE_ITEMS_STR, AppInfo.Instance.GetProgramInfo());  // prizeItems / photoShop

        var stageLessonPath = DevUtil.Instance.GetTargetPath2Parts(stageType, lessonType);  // s1 / l2

        var lessonRandomKeyPath = DevUtil.Instance.GetTargetPath2Parts(stageLessonPath, randomKey);  // s1/l2 / randomKey

        // users / UID / prizeItems / photoShop / s1 / l1 / randomKey / contents
        var fullPath = DevUtil.Instance.GetTargetPath2Parts(programInfoUnderPrizeitems, lessonRandomKeyPath);

        return fullPath;
    }

    // ------------------------------------------------------------------------------
    // 유저가 뒤로가기했을 때 보여줘야 할 레슨리스트 관련
    public void SetUserLastPlayingStageType(string stageType)
    {
        lastPlayingStageType = stageType;
    }

    public string GetUserLastPlayingStageType()
    {
        return this.lastPlayingStageType;
    }

    public void ResetLastPlayingStageType()
    {
        lastPlayingStageType = "";
    }

    //--------------------------------------------------
    // 이중 로그인 관련 처리 루틴
    private string GetPushKey()
    {
        var randomKeyChild = DevUtil.Instance.GetTargetPath2Parts(ADDRESS_STR, "key");

        var subPath = DevUtil.Instance.GetTargetPathInSpecificUser(randomKeyChild);

        var path = DevUtil.Instance.GetTargetPath(EnumSets.DBParentType.Users, subPath);

        var pushKey = FirebaseDatabaseController.Instance.GetJustPushKey(path);

        CustomDebug.Log($"----> push Key : {pushKey}");

        return pushKey;
    }

    public void UpdateLoginRandomKeyAddress()
    {
        CustomDebug.Log("--- UpdateLoginRandomKeyAddress");

        if(isSetPreventDualLogin)
        {
            return;
        }

        this.loginRandomKey = GetPushKey();
        CustomDebug.Log($"loginRandomKey : {loginRandomKey}");

        var randomKeyChild = DevUtil.Instance.GetTargetPath2Parts(ADDRESS_STR, "key");

        var path = DevUtil.Instance.GetTargetPath(EnumSets.DBParentType.Users, GetUID());

        var tmpTable = new Dictionary<string, string>();
        tmpTable.Add(randomKeyChild, this.loginRandomKey);

        var json = DevUtil.Instance.GetJson(tmpTable);

        FirebaseDatabaseController.Instance.UpdateJsonWithCallback(path, json, ListeningAddressChanged, HandlingFailed);
    }

    private void ListeningAddressChanged()
    {
        isSetPreventDualLogin = true;

        var path = DevUtil.Instance.GetTargetPathInSpecificUser(ADDRESS_STR);

        var mainPath = DevUtil.Instance.GetTargetPath(EnumSets.DBParentType.Users, path);
        
        FirebaseDatabaseController.Instance.ListeningValueChanged(mainPath, HandlingEventWhenAnotherSameUserSignedIn);
    }

    public void HandlingEventWhenAnotherSameUserSignedIn()
    {
        CustomDebug.Log(">>>> [UserManager] Handling Event When Another Same User SignedIn");

        HomeController.Instance.OnClickLogOutBtn();
    }

    public void RemoveListeningAddressChanged()
    {
        isSetPreventDualLogin = false;

        var path = DevUtil.Instance.GetTargetPathInSpecificUser(ADDRESS_STR);

        var mainPath = DevUtil.Instance.GetTargetPath(EnumSets.DBParentType.Users, path);

        FirebaseDatabaseController.Instance.RemoveListeningValueChanged(mainPath);
    }

    private void HandlingFailed()
    {
        CustomDebug.LogError("UpdateJsonWithCallback Error" );
    }

    public bool IsUserInfoNull()
    {
        var isNull = true;

        if(this.userInfo != null)
        {
            isNull = false;
        }

        return isNull;

    }

    public void RemoveOldPrizeItemInfos()
    {
        this.userInfo.RemoveOldPrizeItemInfos();
    }
}
