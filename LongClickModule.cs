using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LongClickModule : SimpleLongClickModule
{
    public Image spriteFillType;

    public RectTransform baseRt;
    public RectTransform targetRt;

    // Start is called before the first frame update
    void Start()
    {

    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;

        SetSpriteFillTypePos();

        this.enumerator = CorHoldPressing();

        StartCoroutine(this.enumerator);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;

        ResetData();

        if (this.enumerator != null)
        {
            StopCoroutine(this.enumerator);

            this.enumerator = null;
        }
    }

     IEnumerator CorHoldPressing()
    {
        var current = 0f;
        var percent = 0f;

        while (percent < 1)
        {
            current += Time.deltaTime;

            percent = current / holdingTime;

            spriteFillType.fillAmount = percent;

            yield return null;
        }

        ResetData();

        unityEvent?.Invoke();

        this.enumerator = null;
    }

   

    private void SetSpriteFillTypePos()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRt, Input.mousePosition, Camera.main, out var screenPoint);

        targetRt.localPosition = screenPoint;
    }

    private void ResetData()
    {
        spriteFillType.fillAmount = 0;
    }
}
