using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollModuleOfTableOfContents : MonoBehaviour
{
    [Header("8개보다 많으면 밑으로 내리기")]
    public RectTransform content;

    private const int YPOS_MOVE_AMOUNT = 225;
    private const int LESSON_AMOUNT_PER_PAGE = 8;

    void OnEnable()
    {
        SetLessonListYPos();
    }

    private void SetLessonListYPos()
    {
        var currentLessonValue = CurrentSmartLearningInfo.Instance.GetCurrentLessonValue();

        if (currentLessonValue > LESSON_AMOUNT_PER_PAGE)
        {
            if (currentLessonValue > 2 * LESSON_AMOUNT_PER_PAGE)
            {
                content.localPosition = new Vector3(0, 2 * YPOS_MOVE_AMOUNT, 0);
            }
            else
            {
                content.localPosition = new Vector3(0, YPOS_MOVE_AMOUNT, 0);
            }
        }
    }
}
