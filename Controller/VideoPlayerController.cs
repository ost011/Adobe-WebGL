using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using System.Text;
using UnityEngine.UI;

public class VideoPlayerController : MonoBehaviour
{
    public MediaPlayer avProManager;

    [Space]
    public Slider progressSlider;
    private SliderHandleControlModule handleControlModule = null;

    [Space]
    public PlayerUIControllerModule playerUIControllerModule;

    private bool wasPlaying = false;

    private IEnumerator progressCheckerEnumerator = null;

    private StringBuilder sb = new StringBuilder();

    private const float MOVE_DURATION_VALUE = 500f; // ms

    private void OnDisable()
    {
        if (CanPlayVideo())
        {
            if (IsPlayingVideo())
            {
                StopVideo();
            }

            avProManager.CloseVideo(); // Closes the video and any resources allocated
           
            progressCheckerEnumerator = null;

            ResetData();
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        handleControlModule = this.progressSlider.handleRect.GetComponent<SliderHandleControlModule>();

        handleControlModule.SetBeginDragCallback(PauseVideo);
        handleControlModule.SetEndDragCallback(ResumeVideo);
        handleControlModule.SetDragCallback(HandlingVideoDuration);

        avProManager.Events.AddListener(WhenEventOccurred);
    }

    private void WhenEventOccurred(MediaPlayer arg0, MediaPlayerEvent.EventType eventType, ErrorCode errorCode)
    {
        // Debug.Log($"WhenEventOccurred -> eventType : {eventType} / errorCode : {errorCode}");
    }

    public void OpenVideoFile(string videoFileName)
    {
        var videoFullPath = GetVideoFullPath(videoFileName);

        avProManager.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, videoFullPath, false);
         
        StartCoroutine(CorWaitUntilVideoReady());
    }

    IEnumerator CorWaitUntilVideoReady()
    {
        Debug.Log("----- video 로딩 중 ----");
        LoadingManager.Instance.ActivateLoading();
        
        yield return new WaitUntil(() => CanPlayVideo());

        Debug.Log($"----- video 로딩 완료 >>>>>> ");
        LoadingManager.Instance.DeActivateLoading();

        // avProManager.Control.MuteAudio(true);

        progressCheckerEnumerator = CorCheckProgress();

        WhenHoveredOnPlayerScreen();

        this.playerUIControllerModule.ActivatePlayBtn();

        // 자동재생을 안하는 이유 -> 웹페이지는 자동재생을 막기때문
        //  ForcePlayVideo();
    }

    IEnumerator CorCheckProgress()
    {
        var fullDuration = GetFullDuration();

        var percent = 0f;

        yield return null;

        // Debug.Log($"fullDuration : {fullDuration}");

        // while(IsPlayingVideo()) // video 가 재생완료되더라도 IsPlayingVideo 은 true 를 반환함 
        while (percent < 1) 
        {
            var currentDuration = avProManager.Control.GetCurrentTimeMs();

            // Debug.Log($"currentDuration : {currentDuration}");

            percent = currentDuration / fullDuration;

            SetSliderValue(percent);

            yield return null;
        }

        // 재생완료, 재생 재준비
        ResetToInit();
    }

    private void ResetToInit()
    {
        StopVideo();

        Seek(0);

        SetSliderValue(0);

        this.playerUIControllerModule.ActivatePlayBtn();

        WhenHoveredOnPlayerScreen();

        progressCheckerEnumerator = null;
        progressCheckerEnumerator = CorCheckProgress();
    }

    private void ResetData()
    {
        SetSliderValue(0);

        this.playerUIControllerModule.ResetData();
    }

    public void PauseVideo()
    {
        // isHandlingProgressWithHandlePressing = true;

        this.wasPlaying = IsPlayingVideo();

        if (this.wasPlaying)
        {
            Pause();
        }

        StopCoroutine(progressCheckerEnumerator);

        this.playerUIControllerModule.ActivatePlayBtn();

        // HandlingVideoDuration();
    }

    public void ResumeVideo()
    {
        // isHandlingProgressWithHandlePressing = false;
        if (this.wasPlaying)
        {
            Play();

            this.wasPlaying = false; //reset
        }

        StartCoroutine(progressCheckerEnumerator);

        this.playerUIControllerModule.ActivatePauseBtn();
    }

    public void ForcePlayVideo()
    {
        Play();

        StartCoroutine(progressCheckerEnumerator);

        this.playerUIControllerModule.ActivatePauseBtn();
    }

    // slider 의 handle 을 드래그할 때 수행
    public void HandlingVideoDuration()
    {
        var fullDuration = GetFullDuration();

        var targetProgressDuration = progressSlider.value * fullDuration;

        Seek(targetProgressDuration);

       // Debug.Log($"progressSlider.value : {progressSlider.value}");
    }

    public void LimitHandlingVideoDuration()
    {
        if(!IsPlayingVideo())
        {
            HandlingVideoDuration();
        }
    }

    public void MoveForward()
    {
        Debug.Log($"try MoveForward >>>");

        var modifiedTime = GetCurrentTime() + MOVE_DURATION_VALUE;

        Seek(modifiedTime);

        var fullDuration = GetFullDuration();

        var modifiedSliderValue = modifiedTime / fullDuration;

        SetSliderValue(modifiedSliderValue);
    }

    public void MoveBackward()
    {
        Debug.Log($"<<< try MoveBackward ");

        var modifiedTime = GetCurrentTime() - MOVE_DURATION_VALUE;

        Seek(modifiedTime);

        var fullDuration = GetFullDuration();

        var modifiedSliderValue = modifiedTime / fullDuration;

        SetSliderValue(modifiedSliderValue);
    }

    public void WhenHoveredOnPlayerScreen()
    {
        // Debug.Log(">>>>> When Hovered OnPlayerScreen");

        this.playerUIControllerModule.ActivateBgPanel();
    }

    public void WhenExitOnPlayerScreen()
    {
        // Debug.Log(">>>>> When Exit OnPlayerScreen");

        // 버튼 등에 마우스 포인터가 올라가더라도 플레이어 ui 패널을 비활성화 처리를 피하기위함
        if(this.playerUIControllerModule.IsMouseOverUIWithIgnores())
        {
            return;
        }

        // 영상 일시정지 상태일 때는 플레이어 ui 패널을 끄지않음
        // 아래는 영상이 재생 중일 때 플레이어 ui 패널을 비활성화함
        if (IsPlayingVideo())
        {
            this.playerUIControllerModule.DeActivateBGPanel();
        }
    }

    //-----------------------------------------------------
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

    private void StopVideo()
    {
        avProManager.Stop();
    }

    private void Pause()
    {
        avProManager.Pause();
    }

    private void Play()
    {
        avProManager.Play();
    }

    private string GetVideoFullPath(string videoFileName)
    {
        sb.Clear();

        sb.Append("Video/");
        //sb.Append("AVProVideoSamples/");
        sb.Append(videoFileName);
        sb.Append(".mp4");

        return sb.ToString();
    }

    private void Seek(float time)
    {
        avProManager.Control.SeekFast(time);
    }

    private void SetSliderValue(float sliderValue)
    {
        progressSlider.value = sliderValue;
    }
}
