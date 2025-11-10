using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System;
using Newtonsoft.Json;
using FirebaseWebGL.Scripts.Objects;
using FirebaseWebGL.Examples.Utils;

public class DevUtil
{
    public static DevUtil Instance = null;

    private StringBuilder sb = new StringBuilder();

    private const string ACCOUNTS_STR = "accounts";
    private const string USERS_STR = "users";
    private const string INFO_STR = "info";

    private const string SLASH_STR = "/";

    private const string OPERATING_SYSTEM_WINDOW_PREFIX = "Win";
    private const string OPERATING_SYSTEM_MAC_PREFIX = "Mac";


    static DevUtil()
    {
        Instance = new DevUtil();
    }

    private DevUtil()
    {

    }

    public string GetTargetPath(EnumSets.DBParentType dBParentType, string target)
    {
        var parentType = "";

        switch(dBParentType)
        {
            case EnumSets.DBParentType.Users:
                {
                    parentType = USERS_STR;
                }
                break;
            case EnumSets.DBParentType.Accounts:
                {
                    parentType = ACCOUNTS_STR;
                }
                break;
            case EnumSets.DBParentType.Info:
                {
                    parentType = INFO_STR;
                }
                break;
        }

        sb.Clear();

        sb.Append(parentType);
        sb.Append(SLASH_STR);
        sb.Append(target);

        return sb.ToString();
    }

    public string GetTargetPathInSpecificUser(string target)
    {
        sb.Clear();

        sb.Append(UserManager.Instance.GetUID());
        sb.Append(SLASH_STR);
        sb.Append(target);

        return sb.ToString();
    }

    public string GetTargetPath2Parts(string root, string sub)
    {
        sb.Clear();

        sb.Append(root);
        sb.Append(SLASH_STR);
        sb.Append(sub);

        return sb.ToString();
    }

    public string GetJustCombine2Parts(string root, string sub)
    {
        sb.Clear();

        sb.Append(root);
        sb.Append(sub);

        return sb.ToString();
    }


    // 이메일 주소 유효성 검사
    public bool CheckIsValidEmailAddress(string emailAddress)
    {
        CustomDebug.Log($"Check Is Valid Email Address : {emailAddress}");

        bool result = false;

        if (string.IsNullOrEmpty(emailAddress))
        {
            result = false;
        }
        else
        {
            result = Regex.IsMatch(emailAddress, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,5}|[0-9]{1,3})(\]?)$");
        }

        return result;
    }

    public long GetUTCNowTimeStamp()
    {
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var utcNow = DateTime.Now.ToUniversalTime();

        var timeSpan = utcNow - epoch;

        var longType = Convert.ToInt64(timeSpan.TotalMilliseconds);

        return longType;
    }

    public DateTime GetLocalTimeFromUTCTimeStamp(long timeStamp)
    {
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var localCreated = epoch.AddMilliseconds(timeStamp).ToLocalTime(); // 1970.1.1 으로부터 얼만큼 지났는가, 현재 DateTime 개체의 값을 현지 시간으로 변환

        return localCreated;
    }

    public long RevertUniversalTimeToLongType(DateTime utcTime)
    {
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var tmp = utcTime - epoch;

        var longType = Convert.ToInt64(tmp.TotalMilliseconds);

        return longType;
    }

    public string GetJson(object target)
    {
        return JsonConvert.SerializeObject(target);
    }
    
    public T GetTargetObjectFromJson<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }

    public string GetJsonStringFromResources(string filePath)
    {
        var jsonFile = Resources.Load(filePath) as TextAsset;

        var json = jsonFile.text;

        return json;
    }

    public FirebaseError GetFirebaseError(string error)
    {
        var parsedError = StringSerializationAPI.Deserialize(typeof(FirebaseError), error) as FirebaseError;

        return parsedError;
    }

    // ----------------------------------------------------------------------------------------------
    // 컬렉션 요소들, 읽기전용으로 만들기

    /// <summary>
    /// 딕셔너리 -> 읽기전용 변환 
    /// </summary>
    public ReadOnlyDictionary<Tkey, Tvalue> AsReadOnly<Tkey, Tvalue>(IDictionary<Tkey, Tvalue> dictionary)
    {
        return new ReadOnlyDictionary<Tkey, Tvalue>(dictionary);
    }

    /// <summary>
    /// 리스트 -> 읽기전용 변환
    /// </summary>
    public ReadOnlyCollection<T> AsReadOnly<T>(IList<T> collection)
    {
        return new ReadOnlyCollection<T>(collection);
    }

    // ------------------------------------------------------------------------------------------------

    public IEnumerable<T> GetEnumValues<T>()
    {
        return (IEnumerable<T>)Enum.GetValues(typeof(T));
    }

    // 유저 운영체제 파악하기
    public EnumSets.OperatingSystem GetUserOperatingSystem()
    {
        var result = EnumSets.OperatingSystem.Win;

        var os = SystemInfo.operatingSystem;

        if (os.Contains(OPERATING_SYSTEM_MAC_PREFIX))
        {
            result = EnumSets.OperatingSystem.Mac;
        }

        return result;
    }
}