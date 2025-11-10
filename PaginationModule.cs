using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI.Extensions;
using System;

public class PaginationModule : MonoBehaviour
{
    [SerializeField]
    private PagingDot[] modules = null;

    [SerializeField]
    private ScrollSnapBase scrollSnap = null;

    private int currentPageIndex = 0;

    public int CurrentPage
    {
        get
        {
            return this.scrollSnap.CurrentPage;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        if(scrollSnap == null)
        {
            Debug.LogError("A ScrollSnap script must be attached");

            return;
        }

        InitPageDotModule();

        InitScrollSnap();
    }

    private void InitPageDotModule()
    {
        this.modules = this.GetComponentsInChildren<PagingDot>();

        for (int i = 0; i < modules.Length; i++)
        {
            modules[i].SetIndex(i);

            modules[i].SetOnClickEvent(OnClickUnderDot);
        }
    }

    private void InitScrollSnap()
    {
        if (scrollSnap.Pagination)
        {
            scrollSnap.Pagination = null;
        }

        scrollSnap.OnSelectionPageChangedEvent.AddListener(CheckPagingDot);
    }

    private void CheckPagingDot(int pageIndex)
    {
        modules[currentPageIndex].DeActivateMark();

        modules[pageIndex].ActivateMark();

        this.currentPageIndex = pageIndex;
    }

    private void OnClickUnderDot(int dotIndex)
    {
        scrollSnap.ChangePage(dotIndex);
    }

}
