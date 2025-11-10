using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DocsManager : MonoBehaviour
{
    private static DocsManager instance = null;
    public static DocsManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DocsManager>();
            }

            return instance;
        }
    }

    private IReadOnlyDictionary<string, LearningModeInfo> roLearningModeInfoTable = null;

    private const string LEARNING_MODE_INFO_JSON_FILE_PATH_STR = "Documents/LearningModeInfo";

    public void LoadTextFilesWhileInitiatingApp()
    {
        InitLearningModeInfoTables();
    }

    public void InitLearningModeInfoTables()
    {
        InitLearningObjectiveTable();
    }

    private void InitLearningObjectiveTable()
    {
        var json = DevUtil.Instance.GetJsonStringFromResources(LEARNING_MODE_INFO_JSON_FILE_PATH_STR);

        var learningModeInfoTable = DevUtil.Instance.GetTargetObjectFromJson<Dictionary<string, LearningModeInfo>>(json);

        //var json = textFileReader.GetJsonStringFromResources(LEARNING_MODE_INFO_JSON_FILE_PATH_STR);

        //var learningModeInfoTable = textFileReader.JsonToObject<Dictionary<string, LearningModeInfo>>(json);

        this.roLearningModeInfoTable = DevUtil.Instance.AsReadOnly(learningModeInfoTable);
    }

    // Get Table------------------------------------------

    public IReadOnlyDictionary<string, LearningModeInfo> GetLearningModeInfoTable()
    {
        return this.roLearningModeInfoTable;
    }

    public LearningModeInfo GetTargetLearningModeInfo(string smartLearningName)
    {
        try
        {
            return this.roLearningModeInfoTable[smartLearningName];
        }
        catch(Exception e)
        {
            CustomDebug.LogError($"GetTargetLearningModeInfo error : {e.Message}");

            return null;
        }
    }
}
