using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SingleUnityEventModule : MonoBehaviour
{
    public UnityEvent unityEvent = null;
    
    public void CallEvent()
    {
        this.unityEvent?.Invoke();
    }
}
