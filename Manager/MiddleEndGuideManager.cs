using DanielLochner.Assets.SimpleZoom;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MiddleEndGuideManager : MonoBehaviour
{
    public SLMControllModule slmControllModule;
    
    [Space]
    public UnityEvent onSkipMiddleGuideFunc = null;
    
    public string guideLabelKey = "";

    public enum MiddleGuideXPos
    {
        MostLeft,
        Left,
        Middle,
        Right,
        MostRight,
        UltraRight,
    }

    public enum MiddleGuideYPos
    {
        Highest,
        Upper,
        Middle,
        Lower,
        Lowest
    }

    public EnumSets.MiddleGuideBubbleType middleGuideBubbleType = EnumSets.MiddleGuideBubbleType.SkippableNormal;

    // public Sprite textureBubble;

    [Space]
    public MiddleGuideXPos middleGuideXPos = MiddleGuideXPos.Middle;
    public MiddleGuideYPos middleGuideYPos = MiddleGuideYPos.Middle;


    private int xPosMiddleGuide = 0;
    private int yPosMiddleGuide = 0;

    private SimpleZoom simpleZoom;
    protected MiddleGuideBubblePropertyChanger middleGuideBubblePropertyChanger = null;

    private float delayTimeAfterCompletePanelActivate = 1f;

    private const int X_BUBBLE_POS_UNIT_VALUE = 127;
    private const int Y_BUBBLE_POS_UNIT_VALUE = 120;

    void Awake()
    {
        simpleZoom = SmartLearningModeController.Instance.GetCurrentSimpleZoom();
    }

    protected virtual void OnEnable()
    {
        DeActivateMiddleGuidePanelWithItsComponents(); // 연속해서 나올 때 출력 안되는 문제 방지용으로 컴포넌트들 껐다 켜주고 있음

        Init();
    }

    private void OnDisable()
    {
        DeActivateMiddleGuidePanelOnDisable();
    }

    protected virtual void Init()
    {
        CheckMiddleGuideXPos();
        CheckMiddleGuideYPos();

        Invoke(nameof(DynamicLoadGuidePanel), 0.02f); // 연속해서 나올 때 출력 안되는 문제 방지용

        slmControllModule.SetActionOnClickSkipBtnOnMiddleGuidePanel(() =>
        {
            onSkipMiddleGuideFunc?.Invoke();

            this.enabled = false;
        });
    }

    protected virtual void DeActivateMiddleGuidePanelOnDisable()
    {
        DeActivateMiddleGuidePanelWithItsComponents();

        this.enabled = false;

        //if (!IsBubbleSkippableType())
        //{
        //    // skippable이면 눌러서 넘겼을 거기 때문

        //    DeActivateMiddleGuidePanelWithItsComponents();

        //    this.enabled = false;
        //}
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) // 비교적 사용 빈도가 낮은 키면서 왼손/오른손 잡이 양용 가능하다고 판단/합의함
        {
            DeActivateMiddleGuidePanelOnKeyPressed();
        }
    }

    private void DeActivateMiddleGuidePanelOnKeyPressed()
    {
        if (IsBubbleSkippableWhenPressedSpecificKey())
        {
            SmartLearningModeController.Instance.SkipMiddleGuidePanel();
        }
    }

    protected void CheckMiddleGuideYPos()
    {
        switch (middleGuideYPos)
        {
            case MiddleGuideYPos.Highest:
                {
                    yPosMiddleGuide = 2 * Y_BUBBLE_POS_UNIT_VALUE;
                }
                break;
            case MiddleGuideYPos.Upper:
                {
                    yPosMiddleGuide = Y_BUBBLE_POS_UNIT_VALUE;
                }
                break;
            case MiddleGuideYPos.Middle:
                {
                    yPosMiddleGuide = 0;
                }
                break;
            case MiddleGuideYPos.Lower:
                {
                    yPosMiddleGuide = - Y_BUBBLE_POS_UNIT_VALUE;
                }
                break;
            case MiddleGuideYPos.Lowest:
                {
                    yPosMiddleGuide = -2 * Y_BUBBLE_POS_UNIT_VALUE;
                }
                break;
        }
    }

    protected void CheckMiddleGuideXPos()
    {
        switch (middleGuideXPos)
        {
            case MiddleGuideXPos.MostLeft:
                {
                    xPosMiddleGuide = -2 * X_BUBBLE_POS_UNIT_VALUE;
                }
                break;
            case MiddleGuideXPos.Left:
                {
                    xPosMiddleGuide = - X_BUBBLE_POS_UNIT_VALUE;
                }
                break;
            case MiddleGuideXPos.Middle:
                {
                    xPosMiddleGuide = 0;
                }
                break;
            case MiddleGuideXPos.Right:
                {
                    xPosMiddleGuide = X_BUBBLE_POS_UNIT_VALUE;
                }
                break;
            case MiddleGuideXPos.MostRight:
                {
                    xPosMiddleGuide = 2 * X_BUBBLE_POS_UNIT_VALUE;
                }
                break;
            case MiddleGuideXPos.UltraRight:
                {
                    // 18강 예외 대화창,임시이며,수정 예정입니다.
                    // 18강 기획서 7페이지 참고.
                    xPosMiddleGuide = 420; 
                }
                break;
        }
    }

    protected virtual void DynamicLoadGuidePanel()
    {
        if(middleGuideBubblePropertyChanger == null)
        {
            middleGuideBubblePropertyChanger = gameObject.AddComponent<MiddleGuideBubblePropertyChanger>();
        }

        middleGuideBubblePropertyChanger.SetGuideBubbleProperty(middleGuideBubbleType);

        SetSentenceInMiddleGuidePanel(guideLabelKey);

        ActivateMiddleGuidePanelWithPos(xPosMiddleGuide, yPosMiddleGuide);

        // SmartLearningModeController.Instance.SetMiddleGuideBubbleAlpha(simpleZoom.GetAlphaValueFromCurrentZoom());

        #region bool 특성이 많을 때
        //// 가이드버블 텍스쳐 바뀔 수 있고, 스킵 여부, 퀴즈/게임일 때 horizontal layout top padding이 달라지는 걸 아우를 수 있도록 리팩토링 필요해보임
        //// 가이드버블 obj를 여러개(일반, 스킵되는 퀴즈/게임, 스킵안되는 퀴즈/게임)으로 분리하려다가 public으로 등록할 것이 많아질 것 같아 한 obj의 속성을 바꾸기로 함

        //if (!isSameTextureSerial)
        //{
        //    if (!isBubbleSkippable)
        //    {
        //        SmartLearningModeController.Instance.EnableButtonInMiddleGuideBubble();
        //    }
        //    else
        //    {
        //        SmartLearningModeController.Instance.DisableButtonInMiddleGuideBubble();
        //    }

        //    SetMiddleGuideBubblePadding();

        //    SetMiddleGuideBubbleTexture();
        //}
        #endregion
    }

    #region old
    //private void OnActivateNormalMiddleGuidePanel()
    //{
    //    SetSentenceInMiddleGuidePanel(guideLabelKey);

    //    ActivateMiddleGuidePanelWithPos(xPosMiddleGuide, yPosMiddleGuide);
    //}

    //private void OnActivateGameQuizTypeMiddleGuidePanel()
    //{
    //    SetSentenceInGameQuizTypeMiddleGuidePanel(guideLabelKey);

    //    ActivateGameQuizTypeMiddleGuidePanelWithPos(xPosMiddleGuide, yPosMiddleGuide);

    //    CheckAndDisableButtonIfSkippable();

    //    ActivateProperTitleImage();

    //    SetSpecificMiddleGuideBubbleTexture();
    //}

    //private void ActivateMiddleGuidePanel()
    //{
    //    SmartLearningModeController.Instance.ActivateMiddleGuidePanel();
    //}

    //private void ActivateGameQuizTypeMiddleGuidePanel()
    //{
    //    SmartLearningModeController.Instance.ActivateGameQuizTypeMiddleGuidePanel();
    //}
    #endregion

    protected void SetSentenceInMiddleGuidePanel(string guideLabelKey)
    {
        this.slmControllModule.SetSentenceInMiddleGuidePanel(guideLabelKey);
    }

    private void ActivateMiddleGuidePanelWithPos(int middleGuideXPos, int middleGuideYPos)
    {
        SmartLearningModeController.Instance.ActivateMiddleGuidePanelWithPos(middleGuideXPos, middleGuideYPos);
    }

    public void DeActivateMiddleGuidePanel()
    {
        SmartLearningModeController.Instance.DeActivateMiddleGuidePanel();
    }

    protected void DeActivateMiddleGuidePanelWithItsComponents()
    {
        DeActivateMiddleGuidePanel();

        SmartLearningModeController.Instance.DisableImageComponentInMiddleGuidePanel();

        SmartLearningModeController.Instance.DisableMiddleGuidePanelVerticalLayoutGroup();
    }

    protected bool IsBubbleSkippableType()
    {
        if (this.middleGuideBubbleType.Equals(EnumSets.MiddleGuideBubbleType.SkippableNormal) || this.middleGuideBubbleType.Equals(EnumSets.MiddleGuideBubbleType.SkippableGame) ||
            this.middleGuideBubbleType.Equals(EnumSets.MiddleGuideBubbleType.SkippableQuiz) || this.middleGuideBubbleType.Equals(EnumSets.MiddleGuideBubbleType.SkippableTip) ||
            this.middleGuideBubbleType.Equals(EnumSets.MiddleGuideBubbleType.TopSkippableTip)|| this.middleGuideBubbleType.Equals(EnumSets.MiddleGuideBubbleType.RightSkippableTip))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsBubbleSkippableWhenPressedSpecificKey()
    {
        if (IsBubbleSkippableType() && !simpleZoom.IsZoomedInState())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}