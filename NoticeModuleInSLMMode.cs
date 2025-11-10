using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticeModuleInSLMMode : NoticeModule
{
    public override void ActivatePopUp()
    {
        base.ActivatePopUp();
        
        SmartLearningModeController.Instance.ActivateCommonDarkBG();
    }

    public override void DeActivatePopUp()
    {
        base.DeActivatePopUp();
        
        SmartLearningModeController.Instance.DeActivateCommonDarkBG();
    }
}
