using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

/// <summary>
/// 유니티 비디오플레이어를 사용하는 스크립트
/// </summary>
public class VideoPlayManager : MonoBehaviour
{
    public RawImage screen;
    public GameObject objLoadingImg;

    private VideoPlayer videoPlayer = null;

    private bool isFirstPlay = true;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Init()
    {
        this.videoPlayer = GetComponent<VideoPlayer>();

        this.videoPlayer.url = Path.Combine(Application.streamingAssetsPath, "1.mp4");
    }

    IEnumerator CorPrePare()
    {
        ActivateLoadingImg();

        this.videoPlayer.Prepare();

        yield return new WaitWhile(() => !this.videoPlayer.isPrepared);

        DeActivateLoadingImg();

        screen.texture = this.videoPlayer.texture;

        if(isFirstPlay)
        {
            this.videoPlayer.SetDirectAudioMute(0, true);

            isFirstPlay = false;
        }
        else
        {
            this.videoPlayer.SetDirectAudioMute(0, false);
        }

        this.videoPlayer.Play();
    }

    private void ActivateLoadingImg()
    {
        objLoadingImg.SetActive(true);
    }

    private void DeActivateLoadingImg()
    {
        objLoadingImg.SetActive(false);
    }

    public void OnClickPlayBtn()
    {
        StartCoroutine(CorPrePare());

        
    }

    public void OnClickPauseBtn()
    {
        this.videoPlayer.Pause();
    }

    public void OnClickStopBtn()
    {
        this.videoPlayer.Stop();

        // this.videoPlayer.time = 0;
    }
}
