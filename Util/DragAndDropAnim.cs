using Lofle.Tween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDropAnim : MonoBehaviour
{
    public Transform startPos;
    public Transform endPos;

    private TweenScale tweenScale = null;
    private TweenPosition tweenPosition = null;

    private float touchSomethingTime = 0.3f;
    private float draggingTime = 1f;

    private void Awake()
    {
        Init();
    }

    void OnEnable()
    {
        // Invoke(nameof(StartDragAnim), 0.5f);
    }

    private void Init()
    {
        this.tweenScale = GetComponent<TweenScale>();
        this.tweenPosition = GetComponent<TweenPosition>();

        transform.localPosition = startPos.localPosition;
    }

    private void ResetAllTween()
    {
        

        transform.localScale = Vector3.one;
    }

    private void TouchSomething()
    {
        this.tweenScale.From = Vector3.one;  // alternative : 0(not one) -> 0.8
        this.tweenScale.To = new Vector3(0.8f, 0.8f, 0.8f);

        this.tweenScale.TotalTime = touchSomethingTime;

        this.tweenScale.PlayForward();
    }

    private void DragSomethingToEndPosition()
    {
        this.tweenPosition.From = new Vector3(startPos.localPosition.x, startPos.localPosition.y, transform.localPosition.z);
        this.tweenPosition.To = new Vector3(endPos.localPosition.x, endPos.localPosition.y, transform.localPosition.z);
        this.tweenPosition.TotalTime = draggingTime;

        this.tweenPosition.PlayForward();
    }

    private void TouchReverse()
    {
        this.tweenScale.From = new Vector3(0.8f, 0.8f, 0.8f);
        this.tweenScale.To = Vector3.one;

        this.tweenScale.TotalTime = touchSomethingTime;

        this.tweenScale.PlayForward();
    }

    private void StartDragAnim()
    {
        if (this.gameObject.activeSelf)
        {
            StartCoroutine(CorStartDragAnim());
        }
    }

    IEnumerator CorStartDragAnim()
    {
        yield return new WaitForEndOfFrame();

        TouchSomething();
        yield return new WaitForSeconds(touchSomethingTime);

        DragSomethingToEndPosition();
        yield return new WaitForSeconds(draggingTime);

        TouchReverse();
        yield return new WaitForSeconds(touchSomethingTime);

        ResetAllTween();

        yield return new WaitForSeconds(.5f);
        StartDragAnim();
    }
}
