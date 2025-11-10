using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class LuckyDrawSubModule : MonoBehaviour
{
    public RectTransform tweenableObject;
    public Button btnTweenableObject;

    public GameObject objLocalLoading;

    private Action afterAnimPlayFinished = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartDrawAnim()
    {
        btnTweenableObject.enabled = false;
        objLocalLoading.SetActive(true);

        tweenableObject.DOShakeAnchorPos(1).SetLoops(-1, LoopType.Restart);
    }    

    public void StopDrawAnim()
    {
        tweenableObject.DOKill();

        afterAnimPlayFinished?.Invoke();
    }

    public void SetFinishCallback(Action callback)
    {
        afterAnimPlayFinished = callback;
    }

    public void ResetData()
    {
        this.tweenableObject.anchoredPosition = Vector2.zero;

        afterAnimPlayFinished = null;

        btnTweenableObject.enabled = true;
        objLocalLoading.SetActive(false);
    }
}
