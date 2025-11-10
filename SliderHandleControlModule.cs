using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class SliderHandleControlModule : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public RectTransform rtBaseSlider;
    private Slider baseSlider = null;

    private float widthSlider = 0;

    private Action pointerDownAction = null;

    private Action beginDragAction = null;
    private Action dragAction = null;
    private Action endDragAction = null;

    private bool isDragging = false;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        this.baseSlider = rtBaseSlider.GetComponent<Slider>();

        widthSlider = rtBaseSlider.sizeDelta.x;
    }

    public void SetPointerDownCallback(Action callback)
    {
        this.pointerDownAction = callback;
    }

    public void SetBeginDragCallback(Action callback)
    {
        this.beginDragAction = callback;
    }

    public void SetDragCallback(Action callback)
    {
        this.dragAction = callback;
    }

    public void SetEndDragCallback(Action callback)
    {
        this.endDragAction = callback;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.pointerDownAction?.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");

        this.isDragging = false;

        this.endDragAction?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Debug.Log($"drag -- position : {eventData.position} / ");

        var adjustXPos = Mathf.Clamp(eventData.position.x, 0, widthSlider);

        var modifiedSliderValue = adjustXPos / widthSlider;

        this.baseSlider.value = modifiedSliderValue;
        
        this.dragAction?.Invoke();
    }

    // 스크롤하지않고, 바로 슬라이더 단추를 눌렀다가 뗐을 때의 처리를 위함
    public void OnPointerUp(PointerEventData eventData)
    {
        if(!this.isDragging)
        {
            this.endDragAction?.Invoke();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("On Begin Drag");

        this.isDragging = true;

        beginDragAction?.Invoke();
    }
}
