using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

public class GuideSLMBottomRoutine : MonoBehaviour
{
    public GameObject[] objsBatteryImg;
    public GameObject[] objsPillarLight;
    public GameObject[] objsBlockingGuage;

    [Space]
    public GameObject sentenceBox;
    public RectTransform rectTargetBox;

    public TextMeshProUGUI textOnSententceBox;

    [Space]
    public RectTransform targetBox;

    [Space]
    public UnityEvent completeEvent;

    private string[] progressSentenceArry = {"스테이지 선택방법 익히기", "연구 선택방법 익히기", "플레이 진행도 둘러보기" };

    private int currentSequence = -1;

    private int movingXPos = 0;

    private const float PROGRESS_INTERVAL_TIME = 1f;
    private const float PROGRESS_STAY_DURATION = 1f;

    private const int INIT_X_POS = -394;
    private const int MOVING_DISTANCE = 168;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartBottomRoutine()
    {
        targetBox.gameObject.SetActive(true);

        movingXPos = INIT_X_POS;

        MovingBox().Forget();
    }

    private async UniTaskVoid MovingBox()
    {
        HideSentenceBox();

        movingXPos = movingXPos + MOVING_DISTANCE;

        await this.targetBox.DOAnchorPosX(movingXPos, PROGRESS_INTERVAL_TIME, true).SetEase(Ease.Linear);

        ++currentSequence;

        ShowNextStep();

        await UniTask.Delay((int)(PROGRESS_STAY_DURATION * 1000));

        if(currentSequence == 2)
        {
            // 마지막에 도달한 것임
            // 다음 시퀀스 실행
            HideSentenceBox();

            completeEvent?.Invoke();
        }
        else
        {
            MovingBox().Forget();
        }
    }

    private void ShowNextStep()
    {
        ShowNextBatteryImg();
        ShowNextPillarLight();
        HidePreBlockingGuage();

        ActivateSentenceBox();
    }

    private void ActivateSentenceBox()
    {
        rectTargetBox.anchoredPosition = new Vector2(targetBox.anchoredPosition.x, rectTargetBox.anchoredPosition.y);

        textOnSententceBox.text = progressSentenceArry[currentSequence];

        sentenceBox.SetActive(true);
    }

    private void HideSentenceBox()
    {
        sentenceBox.SetActive(false);
    }

    private void ShowNextBatteryImg()
    {
        objsBatteryImg[currentSequence].SetActive(true);
    }

    private void ShowNextPillarLight()
    {
        objsPillarLight[currentSequence].SetActive(true);
    }

    private void HidePreBlockingGuage()
    {
        objsBlockingGuage[currentSequence].SetActive(false);
    }

}
