using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    private static LoadingManager instance = null;
    public static LoadingManager Instance
    { 
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<LoadingManager>();
            }

            return instance;
        }
    }

    public GameObject panelLoading;

    private Queue<int> queueRequest = new Queue<int>();

    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void ActivateLoading()
    {
        queueRequest.Enqueue(1);

        this.panelLoading.SetActive(true);
    }

    public void DeActivateLoading()
    {
        if(queueRequest.Count > 0)
        {
            queueRequest.Dequeue();
        }

        CustomDebug.Log($"DeActivate Loading : {queueRequest.Count}");

        if (queueRequest.Count == 0)
        {
            this.panelLoading.SetActive(false);
        }
    }

    public void ForceDeActivateLoading()
    {
        queueRequest.Clear();

        this.panelLoading.SetActive(false);
    }
}
