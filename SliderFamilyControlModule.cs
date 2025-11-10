using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderFamilyControlModule : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent unityEvent;

    private Slider baseSlider = null;

    private RectTransform rtBaseSlider = null;

    private float widthSlider = 0;

    private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        baseSlider = GetComponentInParent<Slider>();

        rtBaseSlider = baseSlider.GetComponent<RectTransform>();

        widthSlider = rtBaseSlider.sizeDelta.x;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"Pointer Down : {transform.name}");

        unityEvent?.Invoke();

        StartCoroutine(CorSetSliderValue(eventData.position.x));
    }

    IEnumerator CorSetSliderValue(float pointXPos)
    {
        yield return waitForEndOfFrame;
        
        var modifiedSliderValue = pointXPos / widthSlider;

        this.baseSlider.value = modifiedSliderValue;
    }
}
