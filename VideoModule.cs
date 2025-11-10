using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using System;
using UnityEngine.Events;

public class VideoModule : MonoBehaviour
{
    public MediaPlayer mp;
    // public MeshRenderer bgMeshRenderer;

    [Space]
    [Header("ActionOnFirstFrameReady------------------------------")]
    public UnityEvent actionOnFirstFrameReady = null;

    [Space]
    [Header("ActionOnFinishedPlaying------------------------------")]
    public UnityEvent actionOnFinishedPlaying = null;

    private float popupAniDuration = 0.3f;

    private void Awake()
    {
        mp.Events.AddListener(OnVideoEvent);
    }

    public void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
    {
        switch (et)
        {
            case MediaPlayerEvent.EventType.Error:
                {
                    Debug.Log($"Error occured >DFDOF , {errorCode}");

                    Invoke(nameof(ModifyTimeScaleToZero), popupAniDuration);

                    //CameraManager.ins.SetScreenMovePause();

                    //LearningModeController.Instance.ActivateGotErrorWhenLoadingSmartlearningModeAssetsPopUp(() =>
                    //{
                    //    ReturnBackToOriginalTimeScale();

                    //    ExitSmartLearningMode();
                    //});
                }
                break;
            case MediaPlayerEvent.EventType.FirstFrameReady:
                {
                    EventOnFirstFrameReady();
                }
                break;
            case MediaPlayerEvent.EventType.FinishedPlaying:
                {
                    EventOnFinishedPlaying();
                }
                break;
        }

        Debug.Log("Event: " + et.ToString());
    }

    private void EventOnFirstFrameReady()
    {
        actionOnFirstFrameReady?.Invoke();
    }

    private void EventOnFinishedPlaying()
    {
        actionOnFinishedPlaying?.Invoke();
    }

    public void ActivateVideoPlaying(string videoPath)
    {
        if (!mp.enabled)
        {
            mp.enabled = true;
        }

        LoadVideo(videoPath);

        Debug.Log($"videopath : {mp.m_VideoPath}");
    }

    private void LoadVideo(string filePath)
    {
        mp.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, filePath, true);
    }

    private void ModifyTimeScaleToZero()
    {
        Time.timeScale = 0f;
    }

    private void ReturnBackToOriginalTimeScale()
    {
        Time.timeScale = 1f;
    }

    private void ExitSmartLearningMode()
    {
        //LearningModeController.Instance.ExitGameScene();

        //LearningModeController.Instance.DeActivatePopUp();

        //LearningModeController.Instance.afterSpecificLessonCompleteHandlingModuleInSmartLearningMode.OnClickCloseBtn();
    }
}