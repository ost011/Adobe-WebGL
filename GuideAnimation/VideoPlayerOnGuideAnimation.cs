using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using System.Text;

public class VideoPlayerOnGuideAnimation : MonoBehaviour
{
    public MediaPlayer avProManager;

    [Space]
    public GameObject panelLoading;

    [Space]
    public GameObject objNextStep;

    private StringBuilder sb = new StringBuilder();

    private void OnDisable()
    {
        DeInit();    
    }

    // Start is called before the first frame update
    void Start()
    {
        OpenVideoFile();
    }

    private void DeInit()
    {
        if (CanPlayVideo())
        {
            if (IsPlayingVideo())
            {
                StopVideo();
            }

            avProManager.CloseVideo(); // Closes the video and any resources allocated
        }
    }

    public void OpenVideoFile()
    {
        var videoFileName = "adobeHateArt";

        var videoFullPath = GetVideoFullPath(videoFileName);

        avProManager.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, videoFullPath, false);

        StartCoroutine(CorWaitUntilVideoReady());
    }

    IEnumerator CorWaitUntilVideoReady()
    {
        Debug.Log("----- video 로딩 중 ----");

        yield return new WaitUntil(() => CanPlayVideo());

        Debug.Log($"----- video 로딩 완료 >>>>>> ");

        var fullDuration = GetFullDuration() / 1000; // ms / 1000
        CustomDebug.Log($"fullDuration : {fullDuration}");

        ForcePlayVideo();
        
        DeActivateLoadingPanel();

        yield return new WaitForSeconds(fullDuration);

        objNextStep.SetActive(true);
    }

    private void ForcePlayVideo()
    {
        Play();

        // StartCoroutine(progressCheckerEnumerator);

        // this.playerUIControllerModule.ActivatePauseBtn();
    }

    private void Play()
    {
        avProManager.Play();
    }

    private void StopVideo()
    {
        avProManager.Stop();
    }

    private bool CanPlayVideo()
    {
        return avProManager.Control.CanPlay();
    }

    private bool IsPlayingVideo()
    {
        return avProManager.Control.IsPlaying();
    }

    private string GetVideoFullPath(string videoFileName)
    {
        sb.Clear();

        sb.Append("Video/");
        sb.Append(videoFileName);
        sb.Append(".mp4");

        return sb.ToString();
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

    private void DeActivateLoadingPanel()
    {
        panelLoading.SetActive(false);
    }
}
