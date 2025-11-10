using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Selectable sprite that follows the mouse.
/// </summary>

[RequireComponent(typeof(Image))]
public class LZCursor : MonoBehaviour
{
	static public LZCursor instance;

	// Camera used to draw this cursor
	public Camera uiCamera;

	[Space]
	public Transform dummyObj;

	Transform mTrans;
	// UISprite mSprite;
	Image image;

	// [SerializeField]
	// private SetDynamicSpriteAtlas setDynamicSpriteAtlas = null;

	public bool isTutorial = false;

	void Awake() { instance = this; }
	void OnDestroy() { instance = null; }

	/// <summary>
	/// Cache the expected components and starting values.
	/// </summary>

	void Start()
	{
		mTrans = transform;
		// mSprite = GetComponent<UISprite>();
		image = GetComponent<Image>();

		if (uiCamera == null)
		{
			// uiCamera = NGUITools.FindCameraForLayer(gameObject.layer);
			uiCamera = Camera.main;
		}
		//if (!isTutorial)
		//{
		//	setDynamicSpriteAtlas = GetComponent<SetDynamicSpriteAtlas>();

		//	if (setDynamicSpriteAtlas == null)
		//	{
		//		setDynamicSpriteAtlas = this.gameObject.AddComponent<SetDynamicSpriteAtlas>();
		//	}
		//}
	}

	/// <summary>
	/// Reposition the widget.
	/// </summary>

	void Update()
	{
		Vector3 pos = Vector3.zero;

#if UNITY_EDITOR
		pos = Input.mousePosition;
#else
        if(Input.touchCount > 0)
		{
			pos = Input.GetTouch(0).position;
		}
		else
		{
            LZInventoryManager.Instance.TakeOffFromSceen();
			// pos = Vector3.zero;
		}
#endif
		if (uiCamera != null)
		{
			// Since the screen can be of different than expected size, we want to convert
			// mouse coordinates to view space, then convert that to world position.
			pos.x = Mathf.Clamp01(pos.x / Screen.width);
			pos.y = Mathf.Clamp01(pos.y / Screen.height);
			
			// var worldPoint = uiCamera.ViewportToWorldPoint(pos);
			var worldPoint = uiCamera.ViewportToScreenPoint(pos);
			
			mTrans.position = new Vector3(worldPoint.x, worldPoint.y, 0);    // -8.89 ~ 8.89, -4 ~ 6
			
			// For pixel-perfect results
			if (uiCamera.orthographic)
			{
				Vector3 lp = mTrans.localPosition;
				lp.x = Mathf.Round(lp.x);
				lp.y = Mathf.Round(lp.y);
				
				mTrans.localPosition = lp;

				if (dummyObj != null)
				{
					dummyObj.localPosition = lp;
				}
			}
		}
		else
		{
			// Simple calculation that assumes that the camera is of fixed size
			pos.x -= Screen.width * 0.5f;
			pos.y -= Screen.height * 0.5f;
			pos.x = Mathf.Round(pos.x);
			pos.y = Mathf.Round(pos.y);

			mTrans.localPosition = pos;
		}
	}

	[Obsolete]
    private void ObsoleteUpdate()
    {
		Vector3 pos = Vector3.zero;

#if UNITY_EDITOR
		pos = Input.mousePosition;
#else
        if(Input.touchCount > 0)
		{
			pos = Input.GetTouch(0).position;
		}
		else
		{
            LZInventoryManager.Instance.TakeOffFromSceen();
			// pos = Vector3.zero;
		}
#endif
		if (uiCamera != null)
		{
			// Since the screen can be of different than expected size, we want to convert
			// mouse coordinates to view space, then convert that to world position.
			pos.x = Mathf.Clamp01(pos.x / Screen.width);
			pos.y = Mathf.Clamp01(pos.y / Screen.height);

			var worldPoint = uiCamera.ViewportToWorldPoint(pos);
			//mTrans.position = uiCamera.ViewportToWorldPoint(pos);
			mTrans.position = new Vector3(worldPoint.x, worldPoint.y, 0);

			// For pixel-perfect results
			if (uiCamera.orthographic)
			{
				Vector3 lp = mTrans.localPosition;
				lp.x = Mathf.Round(lp.x);
				lp.y = Mathf.Round(lp.y);

				mTrans.localPosition = lp;

				if (dummyObj != null)
				{
					dummyObj.localPosition = lp;
				}
			}
		}
		else
		{
			// Simple calculation that assumes that the camera is of fixed size
			pos.x -= Screen.width * 0.5f;
			pos.y -= Screen.height * 0.5f;
			pos.x = Mathf.Round(pos.x);
			pos.y = Mathf.Round(pos.y);

			mTrans.localPosition = pos;
		}
	}

    /// <summary>
    /// Clear the cursor back to its original value.
    /// </summary>

    static public void Clear()
	{
		if (instance != null)
			SetSprite();
	}

	/// <summary>
	/// Override the cursor with the specified sprite.
	/// </summary>
	//static public void Set(string spriteName)
	//{
	//	if (instance != null && instance.mSprite)
	//	{
	//		if (spriteName == "")
	//		{
	//			instance.mSprite.enabled = false;
	//			return;
	//		}

	//		CustomDebug.Log($"spriteName : {spriteName}");

	//		if (instance.setDynamicSpriteAtlas != null)
	//		{
	//			instance.setDynamicSpriteAtlas.SetSpecificSprite(spriteName);
	//		}

	//		instance.mSprite.enabled = true;
	//		instance.mSprite.spriteName = spriteName;
	//		instance.mSprite.MakePixelPerfect();
	//		instance.Update();
	//	}
	//}

	public static void SetSprite(Sprite sprite = null)
    {
		if (instance != null && instance.image)
		{
			if (sprite == null)
			{
				instance.image.enabled = false;
				return;
			}

			//if (instance.setDynamicSpriteAtlas != null)
			//{
			//	instance.setDynamicSpriteAtlas.SetSpecificSprite(spriteName);
			//}

			instance.image.enabled = true;
			instance.image.sprite = sprite;
			instance.image.SetNativeSize();
			// instance.mSprite.MakePixelPerfect();
			instance.Update();
		}
	}
}
