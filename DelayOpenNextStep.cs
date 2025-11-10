using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayOpenNextStep : MonoBehaviour
{
    public float delayTime;
    public UnityEvent onDelayFunc = null;

    void OnEnable()
    {
        StartCoroutine(CorActivateNextStep());
    }

    IEnumerator CorActivateNextStep()
    {
        yield return new WaitForSeconds(delayTime);
        
        onDelayFunc?.Invoke();
    }
}
