using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance = null;
    public static SoundManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<SoundManager>();
            }

            return instance;
        }
    }

    public AudioSource bgmSource;
    public AudioSource fxSource;
    public AudioSource fxMouseClickSource;

    [Space]
    public AudioClip[] clipBGMs; // 0 tutorial, 1 lobby, 2 slm

    [Space]
    public AudioClip clipMouseClick;
    public AudioClip[] clipsFx; // 0 TableOfContentsOpen, 1 SLMQuizSuccess, 2 SLMQuizFail, 3 LessonComplete, 4 OpenPrizeBox

    private bool isBGMOnState = true;
    public bool IsBGMOnState => this.isBGMOnState;

    private bool isFXOnState = true;
    public bool IsFXOnState => this.isFXOnState;

    private EnumSets.BGMType currentBGMType = EnumSets.BGMType.Lobby;

    private IEnumerator mouseClickCheckingHandler = null;

    private const string PLAY_BGM_STATE = "playBGMState";
    private const string PLAY_FX_STATE = "playFXState";

    private void OnDestroy()
    {
        DeInitMouseClickHandler();    
    }

    private void Awake()
    {
        Init();
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    private void Init()
    {
        this.isBGMOnState = Convert.ToBoolean(PlayerPrefs.GetInt(PLAY_BGM_STATE, 1));

        this.isFXOnState = Convert.ToBoolean(PlayerPrefs.GetInt(PLAY_FX_STATE, 1));

        if(this.isFXOnState)
        {
            InitMouseClickHandler();
        }
    }

    private void InitMouseClickHandler()
    {
        mouseClickCheckingHandler = CorMouseClickChecking();

        StartCoroutine(mouseClickCheckingHandler);
    }

    private void DeInitMouseClickHandler()
    {
        if(mouseClickCheckingHandler != null)
        {
            StopCoroutine(mouseClickCheckingHandler);

            mouseClickCheckingHandler = null;
        }
    }

    public void PlayBGM(EnumSets.BGMType bgmType)
    {
        // 설정 bgm off -> on 상황일 때를 대비하기위해서 전역변수에 저장
        this.currentBGMType = bgmType;

        if (!this.isBGMOnState)
        {
            return;
        }

        StopBGM();

        var clip = this.clipBGMs[(int)this.currentBGMType];

        bgmSource.clip = clip;

        bgmSource.Play();
    }

    // 설정창에서 bgm off -> on 할 때 사용
    // 이전에 기록해뒀던 bgm type 을 재생한다
    private void PlayBGM()
    {
        StopBGM();

        var clip = this.clipBGMs[(int)this.currentBGMType];

        bgmSource.clip = clip;

        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();

            bgmSource.clip = null;
        }
    }

    public void PlayMouseClickFX()
    {
        if (!this.isFXOnState)
        {
            return;
        }

        StartCoroutine(CorPlayMouseClickFX());
    }

    IEnumerator CorPlayMouseClickFX()
    {
        StopMouseClickFX();

        yield return null;

        fxMouseClickSource.PlayOneShot(clipMouseClick);

        CustomDebug.Log($"Play one shot, mouse Click Fx ");
    }

    private void StopMouseClickFX()
    {
        if (fxMouseClickSource.isPlaying)
        {
            fxMouseClickSource.Stop();
        }
    }

    public void PlayFX(EnumSets.FxType fxType)
    {
        if (!this.isFXOnState)
        {
            return;
        }

        StopFX();

        var clipIndex = (int)fxType;

        var clip = clipsFx[clipIndex];

        fxSource.PlayOneShot(clip);
    }

    private void StopFX()
    {
        if (fxSource.isPlaying)
        {
            fxSource.Stop();
        }
    }

    // ------------------------------------
    // bgm, fx 상태 - on off 부분
    
    public void TurnBGMOn()
    {
        this.isBGMOnState = true;

        PlayerPrefs.SetInt(PLAY_BGM_STATE, 1);
        PlayerPrefs.Save();

        PlayBGM();
    }

    public void TurnBGMOff()
    {
        this.isBGMOnState = false;

        PlayerPrefs.SetInt(PLAY_BGM_STATE, 0);
        PlayerPrefs.Save();

        StopBGM();
    }

    public void TurnFxOn()
    {
        InitMouseClickHandler();

        this.isFXOnState = true;

        PlayerPrefs.SetInt(PLAY_FX_STATE, 1);
        PlayerPrefs.Save();
    }

    public void TurnFxOff()
    {
        DeInitMouseClickHandler();

        this.isFXOnState = false;

        PlayerPrefs.SetInt(PLAY_FX_STATE, 0);
        PlayerPrefs.Save();

        StopFX();
        StopMouseClickFX();
    }

    //----------------------------------
    // 마우스 클릭 체크부
    IEnumerator CorMouseClickChecking()
    {
        while(true)
        {
            if(Input.GetMouseButtonUp(0))
            {
               PlayMouseClickFX();
            }

            yield return null;
        }
    }

    //private void Update()
    //{
    //    if(Input.GetMouseButtonUp(0))
    //    {
    //        fxMouseClickSource.PlayOneShot(clipMouseClick);

    //        CustomDebug.Log($"mouse click fx, in update ");
    //    }
    //}
}
