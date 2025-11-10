using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IntroManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnClickSetDataBtn()
    {
        string uid = "Sirius888";

        var userData = new Dictionary<string, object>
        {
            {"name", "Alan" },
            {"point", 0 },
            {"createTimeWithDateTime", DateTime.Now },
        };

        FirebaseFirestoreController.Instance.SetUserData(uid, userData, (isReady) => { 
        
            if(isReady)
            {

            }
        });
    }

    public void OnClickInitBtn()
    {
        FirebaseFirestoreController.Instance.Init();
    }

    public void OnClickGetDataBtn()
    {
        FirebaseFirestoreController.Instance.GetUserData("AAA", this.gameObject.name, "OnRequestSuccess", "OnRequestFailed");
    }

    private void OnRequestSuccess(string data)
    {
        Debug.Log($"OnRequestSuccess : {data}");
    }

    private void OnRequestFailed(string error)
    {
        Debug.LogError($"OnRequestFailed : {error}");
    }
}
