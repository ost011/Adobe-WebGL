using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using System.Text;
using UnityEngine.Events;
using System;

public class VideoPlayerSmartLearningContents : MonoBehaviour
{
    [Space]
    public MediaPlayer avProManager;

    private StringBuilder sb = new StringBuilder();
    
    [Space]
    [Header("ActionOnFirstFrameReady------------------------------")]
    public UnityEvent actionOnFirstFrameReady = null;

    [Space]
    [Header("ActionOnFinishedPlaying------------------------------")]
    public UnityEvent actionOnFinishedPlaying = null;


    private void Awake()
    {
        avProManager.Events.AddListener(OnVideoEvent);
    }

    private void InitSetMediaPlayer()
    {
        avProManager.m_Muted = true;

        avProManager.m_AutoStart = false;

        avProManager.m_AutoOpen = false;
    }

    public void OpenVideoFile(string videoFileName)
    {
        InitSetMediaPlayer();

        avProManager.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, videoFileName, false);

        StartCoroutine(CorWaitUntilVideoReady());
    }

    IEnumerator CorWaitUntilVideoReady()
    {
        CustomDebug.Log("----- video loading start -----");
        LoadingManager.Instance.ActivateLoading();

        yield return new WaitUntil(() => CanPlayVideo());

        CustomDebug.Log("----- video loading success -----");
        LoadingManager.Instance.DeActivateLoading();

        var fullDuration = GetFullDuration() / 1000; // ms / 1000
        CustomDebug.Log($"fullDuration : {fullDuration}");

        ForcePlayVideo();

        EventOnFirstFrameReady();

        yield return new WaitForSeconds(fullDuration);
    }


    public void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
    {
        switch (et)
        {
            case MediaPlayerEvent.EventType.Error:
                {
                    CustomDebug.Log($"Error occured >DFDOF , {errorCode}");
                }
                break;
           case MediaPlayerEvent.EventType.FirstFrameReady:
                {
                    CustomDebug.Log("video start first frame Ready");
                }
                break;
            case MediaPlayerEvent.EventType.FinishedPlaying:
                {
                    CustomDebug.Log("video FinishedPlayeing");
                    CustomDebug.Log("video Pause");
                    EventOnFinishedPlaying();
                }
                break;
            case MediaPlayerEvent.EventType.Stalled:
                {
                    CustomDebug.Log("video Stalled");
                }
                break;
        }

        CustomDebug.Log("Event: " + et.ToString());
    }


    private void EventOnFirstFrameReady()
    {
        actionOnFirstFrameReady?.Invoke();
    }

    private void EventOnFinishedPlaying()
    {
        actionOnFinishedPlaying?.Invoke();
    }

    private void DeInit()
    {
        if (CanPlayVideo())
        {
            if (IsPlayingVideo())
            {
                StopVideo();
            }

            CloseVideo(); // Closes the video and any resources allocated
        }
    }

   
    private void ForcePlayVideo()
    {
        Play();
    }

    private void Play()
    {
        avProManager.Play();
    }

    public void StopVideo()
    {
        avProManager.Stop();
    }

    public void CloseVideo()
    {
        avProManager.CloseVideo(); // Closes the video and any resources allocated
    }

    public void Pause()
    {
        avProManager.Pause();
    }

    private bool CanPlayVideo()
    {
        return avProManager.Control.CanPlay();
    }

    private bool IsPlayingVideo()
    {
        return avProManager.Control.IsPlaying();
    }

    private float GetFullDuration()
    {
        var fullDuration = avProManager.Info.GetDurationMs();

        return fullDuration;
    }

    private float GetCurrentTime()
    {
        var currentTime = avProManager.Control.GetCurrentTimeMs();

        return currentTime;
    }

    private void OnDisable()
    {
        DeInit();

        avProManager.Events.RemoveListener(OnVideoEvent);
    }
}
