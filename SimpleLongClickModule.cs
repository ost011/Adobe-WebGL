using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SimpleLongClickModule : MonoBehaviour,IPointerDownHandler, IPointerUpHandler
{
    public float holdingTime = 0f;

    public UnityEvent unityEvent;

    [SerializeField]
    protected bool isPressed = false;

    protected IEnumerator enumerator = null;

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;

        this.enumerator = CorHoldPressing();

        StartCoroutine(this.enumerator);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;

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

            yield return null;
        }

        unityEvent?.Invoke();

        this.enumerator = null;
    }
}
