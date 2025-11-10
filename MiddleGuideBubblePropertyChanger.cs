using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddleGuideBubblePropertyChanger : MonoBehaviour
{
    // public Sprite[] guideBubbleSprites = null; // 리소스 순서가 EnumSets.MiddleGuideBubbleType 과 순서가 같아야함

    private EnumSets.MiddleGuideBubbleType guideBubbleType = EnumSets.MiddleGuideBubbleType.SkippableNormal;

    private int[] arrayNormalPaddingValue = new int[] { LEFT_PADDING_VALUE_NORMAL_GAMEQUIZ, RIGHT_PADDING_VALUE_NORMAL_GAMEQUIZ, TOP_PADDING_VALUE_NORMAL, BOTTOM_PADDING_VALUE_NORMAL, SPACING_VALUE_NORMAL_GAMEQUIZ };
    private int[] arrayGameQuizPaddingValue = new int[] { LEFT_PADDING_VALUE_NORMAL_GAMEQUIZ, RIGHT_PADDING_VALUE_NORMAL_GAMEQUIZ, TOP_PADDING_VALUE_GAMEQUIZ, BOTTOM_PADDING_VALUE_GAMEQUIZ, SPACING_VALUE_NORMAL_GAMEQUIZ };
    private int[] arrayTipPaddingValue = new int[] { LEFT_PADDING_VALUE_TIP, RIGHT_PADDING_VALUE_TIP, TOP_PADDING_VALUE_TIP, BOTTOM_PADDING_VALUE_TIP, SPACING_VALUE_TIP };
    private int[] arrayTopRightTipPaddingValue = new int[] { LEFT_PADDING_VALUE_TOPLEFT_TIP, RIGHT_PADDING_VALUE_TOPLEFT_TIP, TOP_PADDING_VALUE_TOPLEFT_TIP, BOTTOM_PADDING_VALUE_TOPLEFT_TIP, SPACING_VALUE_TOPLEFT_TIP };
    private int[] arrayDownRightTipPaddingValue = new int[] { LEFT_PADDING_VALUE_DOWNRight_TIP, RIGHT_PADDING_VALUE_DOWNRight_TIP, TOP_PADDING_VALUE_DOWNRight_TIP, BOTTOM_PADDING_VALUE_DOWNRight_TIP, SPACING_VALUE_DOWNRight_TIP };

    private const int LEFT_PADDING_VALUE_NORMAL_GAMEQUIZ = 70;
    private const int RIGHT_PADDING_VALUE_NORMAL_GAMEQUIZ = 70;
    private const int TOP_PADDING_VALUE_NORMAL = 52;
    private const int BOTTOM_PADDING_VALUE_NORMAL = 62;
    private const int SPACING_VALUE_NORMAL_GAMEQUIZ = 15;

    private const int TOP_PADDING_VALUE_GAMEQUIZ = 42;
    private const int BOTTOM_PADDING_VALUE_GAMEQUIZ = 52;

    private const int LEFT_PADDING_VALUE_TIP = 22;
    private const int RIGHT_PADDING_VALUE_TIP = 22;
    private const int TOP_PADDING_VALUE_TIP = 17;
    private const int BOTTOM_PADDING_VALUE_TIP = 65;
    private const int SPACING_VALUE_TIP = 0;

    private const int LEFT_PADDING_VALUE_TOPLEFT_TIP = 32;
    private const int RIGHT_PADDING_VALUE_TOPLEFT_TIP = 32;
    private const int TOP_PADDING_VALUE_TOPLEFT_TIP = 25;
    private const int BOTTOM_PADDING_VALUE_TOPLEFT_TIP = 45;
    private const int SPACING_VALUE_TOPLEFT_TIP = 0;


    private const int LEFT_PADDING_VALUE_DOWNRight_TIP = 32;
    private const int RIGHT_PADDING_VALUE_DOWNRight_TIP = 32;
    private const int TOP_PADDING_VALUE_DOWNRight_TIP = 17;
    private const int BOTTOM_PADDING_VALUE_DOWNRight_TIP = 56;
    private const int SPACING_VALUE_DOWNRight_TIP = 0;

    //private const int TOP_PADDING_VALUE_NORMAL = 52;
    //private const int BOTTOM_PADDING_VALUE_NORMAL = 62;
    //private const int TOP_PADDING_VALUE_GAMEQUIZ = 42;
    //private const int BOTTOM_PADDING_VALUE_GAMEQUIZ = 52;
    //private const int TOP_PADDING_VALUE_TIP = 17;
    //private const int BOTTOM_PADDING_VALUE_TIP = 72;56


    public void SetGuideBubbleProperty(EnumSets.MiddleGuideBubbleType currentGuideBubbleType)
    {
        this.guideBubbleType = currentGuideBubbleType;

        SetGuideBubbleActivatingUpperTitleProperty();

        SetGuideBubbleTexture();

        SetGuideBubbleSkippableProperty();

        SetGuideBubblePaddingProperty();

        SetGuideBubblePivot();
    }

    private void SetGuideBubbleSkippableProperty()
    {
        if(IsGuideBubbleSkippable())
        {
            SmartLearningModeController.Instance.EnableButtonInMiddleGuideBubble();
        }
        else
        {
            SmartLearningModeController.Instance.DisableButtonInMiddleGuideBubble();
        }
    }

    private void SetGuideBubblePaddingProperty()
    {
        int[] arrayPaddingValues;

        if (IsGuideBubbleNormal())
        {
            arrayPaddingValues = GetPaddingArrayValue(arrayNormalPaddingValue);
        }
        else
        {
            if (IsGuideBubbleGameType() || IsGuideBubbleQuizType())
            {
                arrayPaddingValues = GetPaddingArrayValue(arrayGameQuizPaddingValue);
            }
            else if(IsGuideBubbleTipType())
            {
                arrayPaddingValues = GetPaddingArrayValue(arrayTipPaddingValue);
            }
            else if(IsGuideBubbleTopTipType())
            {
                arrayPaddingValues = GetPaddingArrayValue(arrayTopRightTipPaddingValue);
            }
            else
            {
                arrayPaddingValues = GetPaddingArrayValue(arrayDownRightTipPaddingValue);
            }
        }

        SmartLearningModeController.Instance.SetMiddleGuideBubblePadding(arrayPaddingValues);
    }

    private int[] GetPaddingArrayValue(int[] arrayPaddingValues)
    {
        var array = new int[5];

        for (int i = 0; i < arrayPaddingValues.Length; i++)
        {
            array[i] = arrayPaddingValues[i];
        }

        return array;
    }

    private void SetGuideBubbleActivatingUpperTitleProperty()
    {
        if (IsGuideBubbleNormal())
        {
            // deactivate upper title

            SmartLearningModeController.Instance.DeActivateMiddleGuideUpperTitleImages();
        }
        else
        {
            if(IsGuideBubbleGameType())
            {
                // activate game title

                SmartLearningModeController.Instance.ActivateUpperGameTitleImage();
            }
            else
            {
                if (IsGuideBubbleQuizType())
                {
                    // activate quiz title

                    SmartLearningModeController.Instance.ActivateUpperQuizTitleImage();
                }
                else
                {
                    // tip Type

                    SmartLearningModeController.Instance.ActivateUpperTipTitleImage();
                }
            }
        }
    }

    private void SetGuideBubblePivot()
    {
        var xValue = 0.5f;
        var yValue = 0.5f;

        if (IsGuideBubbleTipType())
        {
            xValue = 0f;
            yValue = 0f;
        }

        SmartLearningModeController.Instance.SetMiddleGuideBubblePivot(xValue, yValue);
    }

    private void SetGuideBubbleTexture()
    {
        int index = (int) guideBubbleType;

        SmartLearningModeController.Instance.SetMiddleGuideBubbleTexture(index);
    }
    private bool IsGuideBubbleSkippable()
    {
        if(guideBubbleType.Equals(EnumSets.MiddleGuideBubbleType.SkippableNormal) || guideBubbleType.Equals(EnumSets.MiddleGuideBubbleType.SkippableGame) || guideBubbleType.Equals(EnumSets.MiddleGuideBubbleType.SkippableQuiz)
            || guideBubbleType.Equals(EnumSets.MiddleGuideBubbleType.SkippableTip) || guideBubbleType.Equals(EnumSets.MiddleGuideBubbleType.RightSkippableTip) || guideBubbleType.Equals(EnumSets.MiddleGuideBubbleType.TopSkippableTip))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsGuideBubbleNormal()
    {
        if(guideBubbleType.Equals(EnumSets.MiddleGuideBubbleType.SkippableNormal) || guideBubbleType.Equals(EnumSets.MiddleGuideBubbleType.UnSkippableNormal))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsGuideBubbleGameType()
    {
        if (guideBubbleType.Equals(EnumSets.MiddleGuideBubbleType.SkippableGame) || guideBubbleType.Equals(EnumSets.MiddleGuideBubbleType.UnSkippableGame))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsGuideBubbleQuizType()
    {
        if (guideBubbleType.Equals(EnumSets.MiddleGuideBubbleType.SkippableQuiz) || guideBubbleType.Equals(EnumSets.MiddleGuideBubbleType.UnSkippableQuiz))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsGuideBubbleTipType()
    {
        if (guideBubbleType.Equals(EnumSets.MiddleGuideBubbleType.SkippableTip) || guideBubbleType.Equals(EnumSets.MiddleGuideBubbleType.UnSkippableTip))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsGuideBubbleTopTipType()
    {
        if (guideBubbleType.Equals(EnumSets.MiddleGuideBubbleType.TopSkippableTip))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool IsGuideBubbleDownTipType()
    {
        if (guideBubbleType.Equals(EnumSets.MiddleGuideBubbleType.RightSkippableTip))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
