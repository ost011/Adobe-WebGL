using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AE263 : SLMStageSequence
{
    // Start is called before the first frame update
    private string TITLE_TEXT_KEY = "LM_AE_263";
    private string[] arrPrSequenceKey = { "LEARNING_AE_263_0", "LEARNING_AE_263_1", "LEARNING_AE_263_2" };

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        Init();
    }

    private void Init()
    {
        //LearningModeController.Instance.SetTitle(TITLE_TEXT_KEY);
        //LearningModeController.Instance.SetSentenceInGuidePanel(arrPrSequenceKey[0], LoadSequence);

        // LearningModeController.Instance.MoveCameraViewToSpecificAttentionPoint(0);
    }
}
