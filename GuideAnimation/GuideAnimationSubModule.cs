using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 가이드 애니메이션 특정 게임오브젝트에 붙이는 서브 모듈
// 해당 객체가 꺼지거나 할 때 수행할 특수기능을 부여한다
public class GuideAnimationSubModule : MonoBehaviour
{
    public UnityEvent enableEvent;
    public UnityEvent disableEvent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        enableEvent?.Invoke();
    }

    private void OnDisable()
    {
        disableEvent?.Invoke();
    }
}
