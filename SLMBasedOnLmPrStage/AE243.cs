using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AE243 : SLMStageSequence
{
    // Start is called before the first frame update
    private string TITLE_TEXT_KEY = "LM_AE_243";
    private string[] arrPrSequenceKey = { "LEARNING_AE_243_0", "LEARNING_AE_243_1", "LEARNING_AE_243_2", "LEARNING_AE_243_3", "LEARNING_AE_243_4" };

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        Init();
    }

    private void Init()
    {
        // LearningModeController.Instance.SetTitle(TITLE_TEXT_KEY);
        // LearningModeController.Instance.SetSentenceInGuidePanel(arrPrSequenceKey[0], LoadSequence);

        //// LearningModeController.Instance.MoveCameraViewToSpecificAttentionPoint(0);
    }
}
