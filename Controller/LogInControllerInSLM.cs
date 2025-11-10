using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogInControllerInSLM : MonoBehaviour
{
    public void SignOut()
    {
        FirebaseAuthController.Instance.SignOut();
    }

    public void SucceededSignOut()
    {
        SmartLearningModeController.Instance.SucceededSignOut();
    }

    public void WhenSignOutFailed()
    {
        SmartLearningModeController.Instance.WhenSignOutFailed();
    }
}
