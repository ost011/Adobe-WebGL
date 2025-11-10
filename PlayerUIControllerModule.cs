using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

// 플레이어 재생, 일시정지 등의 ui들을 관리하는 모듈
public class PlayerUIControllerModule : MonoBehaviour
{
    public GameObject objPlay;
    public GameObject objPause;

    [Space]
    public CanvasGroup canvasGroupBG;
    public float bgControlDelayTime = 0f;

    [SerializeField]
    private bool bgActivating = false;
   
    private IEnumerator bgActivateEnumerator = null;
    private IEnumerator bgDeActivateEnumerator = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ActivateBgPanel()
    {
        if(this.bgActivating)
        {
            return;
        }

        if(this.bgActivateEnumerator == null)
        {
            this.bgActivateEnumerator = CorActivateBgPanel();

            StartCoroutine(this.bgActivateEnumerator);

            bgActivating = true;
        }
    }

    IEnumerator CorActivateBgPanel()
    {
        // this.canvasGroupBG.blocksRaycasts = true;

        var current = 0f;
        var percent = 0f;

        while(percent < 1f)
        {
            current += Time.deltaTime;

            percent = current / this.bgControlDelayTime;

            this.canvasGroupBG.alpha = percent;

            yield return null;
        }

        this.bgActivateEnumerator = null;
    }

    public void DeActivateBGPanel()
    {
        if(bgDeActivateEnumerator == null)
        {
            bgDeActivateEnumerator = CorDeActivateBGPanel();

            StartCoroutine(bgDeActivateEnumerator);

            this.bgActivating = false;

        }
    }

    IEnumerator CorDeActivateBGPanel()
    {
        var current = 0f;
        var percent = 0f;

        while (percent < 1f)
        {
            current += Time.deltaTime;

            percent = current / this.bgControlDelayTime;

            this.canvasGroupBG.alpha = 1 - percent;

            yield return null;
        }

        this.canvasGroupBG.alpha = 0f;

        bgDeActivateEnumerator = null;

        // this.canvasGroupBG.blocksRaycasts = false;
    }

    
    public void ActivatePlayBtn()
    {
        AllDeActivateBtns();

        objPlay.SetActive(true);
    }

    public void ActivatePauseBtn()
    {
        AllDeActivateBtns();

        objPause.SetActive(true);
    }

    private void AllDeActivateBtns()
    {
        objPlay.SetActive(false);
        objPause.SetActive(false);
    }

    public void ResetData()
    {
        AllDeActivateBtns();

        this.canvasGroupBG.alpha = 0f;

        bgDeActivateEnumerator = null;

        this.bgActivating = false;

        bgActivateEnumerator = null;
    }
    //-------------------------------

    /// <summary>
    /// 마우스 포인터 호버를 무시하는 오브젝트인지 확인
    /// </summary>
    public bool IsMouseOverUIWithIgnores()
    {
        var pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        var raycastResultList = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

        var datas = from data in raycastResultList
                    where (data.gameObject.GetComponent<MouseUIClickThrough>() != null)
                    select data;

        //if(datas.Count() > 0)
        //{
        //    Debug.Log($"shotshot : {datas.ElementAt(0).gameObject.name}");
        //}

        return datas.Count() > 0;
    }
}
