using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragObjToExplainedPositionController : MonoBehaviour
{
    public Image objCursor;
    public int countTotalSteps = 0;
    public int countNeedToActivateEventOnFailedSeveralTimes = 0;

    [Space]
    public GameObject[] guideAnimations;

    [Space]
    public UnityEvent onFinalSuccessFunc = null;

    [Space]
    public UnityEvent onDropAnotherDropSlotFunc = null;

    [Space]
    public UnityEvent onFailedSeveralTimes = null;

    [Space]
    public Sprite[] spritesOfCursor;

    [Header("DropSuccessImage-----------------------")]
    [Space]
    public GameObject[] objsOnDropSuccess;

    private Camera mainCamera;
    private int failCount = 0;
    private Action onCurrentDropSuccessCallback = null;
    private int indexCurrentChosenDragObj = -1;
    private List<int> listSuccessIndex = new List<int>();

    void Awake()
    {
        mainCamera = Camera.main;
    }

    public void OnBeginDragging(int index, Action onCurrentDropSuccess = null)
    {
        SetCurrentChosenIndex(index);

        onCurrentDropSuccessCallback = onCurrentDropSuccess;

        ActivateCursor();

        UpdateCursorSprite(indexCurrentChosenDragObj);
    }

    public void OnEndDrag()
    {
        DeActivateCursor();

        // OnDrop -> OnEndDrag순으로 실행되기 때문에 Reset되기 전에 제대로 놓았다면 CheckDropSuccess가 실행됨
        ResetData();
    }

    public void OnDrag(PointerEventData eventData)
    {
        objCursor.transform.position = mainCamera.ScreenToWorldPoint(eventData.position);
    }

    public void CheckDropSuccess(int specificDropAreaIndex)
    {
        if (specificDropAreaIndex == indexCurrentChosenDragObj)
        {
            // 정답 지점에 드랍 성공

            OnSuccessDropProperPosition();
        }
        else
        {
            failCount++;

            if (failCount == countNeedToActivateEventOnFailedSeveralTimes)
            {
                onFailedSeveralTimes?.Invoke();
            }

            onDropAnotherDropSlotFunc?.Invoke();

            CustomDebug.Log("onDropAnotherDropSlot");
        }
    }

    public void OnSuccessDropProperPosition()
    {
        CustomDebug.Log($"==============={indexCurrentChosenDragObj}th DragObjToExplainedPosition Success");

        ActivateCurrentDropSucceedImage();

        listSuccessIndex.Add(indexCurrentChosenDragObj);

        onCurrentDropSuccessCallback?.Invoke();

        if (listSuccessIndex.Count == countTotalSteps)
        {
            CustomDebug.Log($"Final Drop Success");

            onFinalSuccessFunc?.Invoke();
        }
        else
        {
            CheckWhichGuideShouldBeActivated();
        }
    }

    private void CheckWhichGuideShouldBeActivated()
    {
        if (guideAnimations[indexCurrentChosenDragObj].activeSelf)
        {
            // 현재 활성화된 커서 순서의 오브젝트를 드랍 성공했음
            // 다음 커서 키워줄 타이밍

            guideAnimations[indexCurrentChosenDragObj].SetActive(false);

            var nextCursorIndex = GetNextCursorIndex();

            guideAnimations[nextCursorIndex].SetActive(true);
        }
    }

    private int GetNextCursorIndex()
    {
        var index = -1;

        for (int i = 0; i < countTotalSteps; i++)
        {
            if (! listSuccessIndex.Contains(i))
            {
                index = i;

                break;
            }
        }

        return index;
    }

    private void ActivateCurrentDropSucceedImage()
    {
        objsOnDropSuccess[indexCurrentChosenDragObj].SetActive(true);
    }

    public void SetCurrentChosenIndex(int index)
    {
        this.indexCurrentChosenDragObj = index;
    }

    // cursor===============================
    private void ActivateCursor()
    {
        objCursor.gameObject.SetActive(true);
    }

    private void DeActivateCursor()
    {
        objCursor.gameObject.SetActive(false);
    }

    private void UpdateCursorSprite(int index)
    {
        objCursor.sprite = spritesOfCursor[index];

        objCursor.SetNativeSize();
    }

    private void ResetData()
    {
        indexCurrentChosenDragObj = -1;
        onCurrentDropSuccessCallback = null;
    }
}
