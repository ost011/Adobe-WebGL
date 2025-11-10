using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;

public class UGUIDragAndDrop : MonoBehaviour, IDragHandler, IBeginDragHandler, IDropHandler, IEndDragHandler
{
    public EnumSets.DragAndDrop dragAndDropStateOfIt = EnumSets.DragAndDrop.None;

    public Image icon;   // 커서는 아니지만 드래그 중일 때 켜져야하는 오브젝트
    public DragAndDropContainer cursor;

    [Space]
    public UnityEvent eventOnDropSuccess = null;

    protected bool isDragging = false;

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (!this.dragAndDropStateOfIt.Equals(EnumSets.DragAndDrop.Drag))
        {
            return;
        }

        // 드래그 Slot에서 드래그를 시작했고 아이콘과 커서 활성화

        icon.gameObject.SetActive(true);

        isDragging = true;

        cursor.ActivateCursor();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            // screen space - overlay 세팅
            // cursor.transform.position = eventData.position;

            // screen space - camera 세팅
            cursor.transform.position = Camera.main.ScreenToWorldPoint(eventData.position);

            // screen space - camera에서 eventData.position을 하면 몇만대로 튀어버림, 원인 불명 => 뭘로 해도 커서 위치 맞지 않음
            // cursor.transform.localPosition = new Vector3(eventData.position.x + refVector.x, eventData.position.y + refVector.y, 0);  // => editor o, browser x
        }
    }

    // 드래그 오브젝트에서 발생
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            icon.gameObject.SetActive(false);
        }

        isDragging = false;

        cursor.DeActivateCursor();
    }

    // 드롭 오브젝트에서 발생
    public void OnDrop(PointerEventData eventData)
    {
        if (this.dragAndDropStateOfIt.Equals(EnumSets.DragAndDrop.Drop) && cursor.IsCursorActivated)   // 커서를 들고 정확하게 드랍 오브젝트에 드랍했다
        {
            eventOnDropSuccess?.Invoke();
        }

        icon.gameObject.SetActive(false);
    }
}