using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class LZItemSlot : MonoBehaviour
{
	public Image icon;
	private Vector3 initPosIcon = Vector3.zero;

	LZItemData mItem; //current item

	static LZItemData mDraggedItem; // now dragged item

	public Action dropSuccess = null;

	/// <summary>
	/// Set slot icon
	/// </summary>
	public void SetSlot(LZItemData itemData = null)
	{
		mItem = itemData;

		if (initPosIcon.Equals(Vector3.zero))
		{
			// icon = transform.GetChild(0).GetComponent<Image>();
			initPosIcon = icon.transform.localPosition;
		}

		if (itemData == null)
		{
			icon.enabled = true;
		}
		else
		{
			icon.enabled = false;
			// icon.spriteName = mItem.spriteName;
		}
	}

	public void SetSlot(bool isDrop, LZItemData itemData = null)
	{
		mItem = itemData;

		if (initPosIcon.Equals(Vector3.zero))
		{
			// icon = transform.GetChild(0).GetComponent<Image>();
			initPosIcon = icon.transform.localPosition;
		}

		if (itemData == null)
		{
			icon.enabled = true;
		}
		else
		{
			icon.enabled = false;
			// icon.spriteName = mItem.spriteName;

			icon.transform.localPosition = initPosIcon;

			if (isDrop)
			{
				dropSuccess?.Invoke();
			}
		}
	}

	public void OnClick()
	{
#if UNITY_EDITOR || UNITY_WEBPLAYER
		Calculate();
#endif
	}

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
	void OnPress(bool isDown)
	{
		if(isDown)
        {
			Calculate();
            // CameraManager.ins.SetScreenMovePause();
        }
	}
	
	void OnDrop (GameObject go)
	{
		Calculate();
        // CameraManager.ins.SetScreenMovable();
    }
#endif

	void Calculate()
	{
		if (mDraggedItem != null)
		{
			// cursor - exist / slot - none		==> drop(equipt)
			if (mItem == null)
			{
				mItem = mDraggedItem;
				mDraggedItem = null;

				SetSlot(true, mItem);
				UpdateCursor();
			}
			// cursor - exist / slot - exist	==> replace
			else
			{
				LZItemData tempItem = mDraggedItem;
				mDraggedItem = mItem;
				mItem = tempItem;

				SetSlot(false, mItem);
				UpdateCursor();
			}
		}
		// cursor - none / slot - exist		==> pickup
		else if (mItem != null)
		{
			mDraggedItem = mItem;
			mItem = null;

			SetSlot();
			UpdateCursor();
		}
	}

	void UpdateCursor()
	{
		if (mDraggedItem != null)
		{
			LZCursor.SetSprite(mDraggedItem.sprite);
		}
		else
		{
			LZCursor.Clear();
		}
	}

	/// <summary>
	/// 드래그 중에 스크린에서 손가락을 떼는 경우 다시 처음으로 되돌린다
	/// </summary>
	public void JustTakeOffFromSceen()
	{
		if (mDraggedItem != null)
		{
			LZItemData tempItem = mDraggedItem;
			mItem = tempItem;

			SetSlot(false, mItem);

			mDraggedItem = null;

			UpdateCursor();

			// CameraManager.ins.SetScreenMovable();
		}
	}
}
