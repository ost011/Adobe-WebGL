using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchScalingImg : MonoBehaviour//, IPointerClickHandler//, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public float xAxisSpeed = 200f;
    public float yAxisSpeed = 200f;

    // [SerializeField]
    // private UISprite spriteTargetScalableObject = null;

    [SerializeField]
    private Image scalableTargetImage = null;

    [SerializeField]
    private GameObject cursorOrigin;
    [SerializeField]
    private GameObject cursorFollowing;
    private Transform cursorPosition;

    [SerializeField]
    private bool isMousePressed = false;

    [SerializeField]
    private bool isIn = false;

    private Vector2 nowPos = Vector2.zero;
    private Vector3 worldNowPos = Vector3.zero;

    private Vector2 prePos = Vector2.zero;
    private Vector3 worldPrePos = Vector3.zero;

    private Vector3 movePos = Vector3.zero;

    private void Awake()
    {
        // this.spriteTargetScalableObject = this.gameObject.GetComponent<UISprite>();
        this.scalableTargetImage = this.gameObject.GetComponent<Image>();
        //cursorPosition = cursorFollowing.GetComponent<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // CheckTouchSequence();

#if UNITY_EDITOR
        CheckTouchSequenceWithMouse();
#else
                CheckTouchSequence();
#endif
    }

    private void CursorOriginActiveFalse()
    {
        cursorOrigin.gameObject.SetActive(false);
        cursorFollowing.gameObject.SetActive(true);
    }
    private void CursorOriginActiveTrue()
    {
        cursorOrigin.gameObject.SetActive(true);
        cursorFollowing.gameObject.SetActive(false);
    }
    public void CheckTouchSequence()
    {
        if (isMousePressed && Input.touchCount == 1 && Input.GetTouch(0).phase.Equals(TouchPhase.Began))
        {
            //Debug.Log("muyahoBegan");

            var touch = Input.GetTouch(0);
            prePos = touch.position - touch.deltaPosition;
            worldPrePos = Camera.main.ScreenToWorldPoint(this.prePos);

            isIn = true;

            // CameraManager.ins.SetScreenMovePause();

            CursorOriginActiveFalse();
            //cursorFollowing.transform.position = touch.deltaPosition;    //added temporarily
        }
        else
        {
            if (isIn) // 746.69 , 430.4 / (-3.2, 1.7, 0.0)
            {
                if (Input.GetTouch(0).phase.Equals(TouchPhase.Moved))
                {
                    // Debug.Log("muyahoisIn");

                    var touch = Input.GetTouch(0);

                    nowPos = touch.position - touch.deltaPosition;
                    worldNowPos = Camera.main.ScreenToWorldPoint(this.nowPos);


                    //cursorFollowing.transform.position = touch.deltaPosition; //added temporarily

                    this.movePos = worldPrePos - worldNowPos;
                    // Debug.Log($"--- movePos : {movePos}");

                    if (movePos.y < 0)
                    {
                        movePos.y = 0;
                    }

                    if (movePos.x > 0)
                    {
                        movePos.x = 0;
                    }

                    // 210 / 50
                    var tmpX = Mathf.Abs(movePos.x) * xAxisSpeed;
                    var tmpY = Mathf.Abs(movePos.y) * yAxisSpeed;

                    int intX = (int)tmpX;
                    int intY = (int)tmpY;

                    //this.spriteTargetScalableObject.height = intY;
                    //this.spriteTargetScalableObject.width = intX;

                    scalableTargetImage.rectTransform.sizeDelta = new Vector2(intX, intY);

                    //if (this.spriteTargetScalableObject.height > validAreaValue.y + heightValidRange)
                    //{
                    //    RestrictScalableTargetHeightValue();
                    //}
                    //if (this.spriteTargetScalableObject.width > validAreaValue.x + widthValidRange)
                    //{
                    //    RestrictScalableTargetWidthValue();
                    //}

                    if (this.scalableTargetImage.sprite.rect.height > validAreaValue.y + heightValidRange)
                    {
                        RestrictScalableTargetHeightValue();

                        Debug.Log("this.scalableTargetImage.sprite.rect.height > validAreaValue.y + heightValidRange");
                    }
                    if (this.scalableTargetImage.sprite.rect.width > validAreaValue.x + widthValidRange)
                    {
                        RestrictScalableTargetWidthValue();

                        Debug.Log("this.scalableTargetImage.sprite.rect.width > validAreaValue.x + widthValidRange");
                    }
                }
                else
                {
                    if (Input.GetTouch(0).phase.Equals(TouchPhase.Ended) || Input.GetTouch(0).phase.Equals(TouchPhase.Canceled))
                    {

                        CursorOriginActiveTrue();

                        CheckIsScalingValidArea();

                        isIn = false;

                        // CameraManager.ins.SetScreenMovable();
                    }
                }
            }
        }
    }

    public void CheckTouchSequenceWithMouse()
    {
        if (isMousePressed && !isIn)
        {
            Debug.Log("isMousePressed && !isIn");

            var touch = Input.mousePosition;

            worldPrePos = Camera.main.ScreenToWorldPoint(touch);

            isIn = true;

            // CameraManager.ins.SetScreenMovePause();

            CursorOriginActiveFalse();
            //cursorFollowing.transform.position = Input.mousePosition;
        }
        else
        {
            if (isIn) // 746.69 , 430.4 / (-3.2, 1.7, 0.0)
            {
                if (Input.GetMouseButton(0))
                {

                    // Debug.Log("muyahoisIn");

                    var touch = Input.mousePosition;

                    worldNowPos = Camera.main.ScreenToWorldPoint(touch);

                    this.movePos = worldPrePos - worldNowPos;
                    // Debug.Log($"--- movePos : {movePos}");


                    //cursorFollowing.transform.position = Input.mousePosition;

                    if (movePos.y < 0)
                    {
                        movePos.y = 0;
                    }

                    if (movePos.x > 0)
                    {
                        movePos.x = 0;
                    }

                    // 210 / 50
                    var tmpX = Mathf.Abs(movePos.x) * xAxisSpeed;
                    var tmpY = Mathf.Abs(movePos.y) * yAxisSpeed;

                    int intX = (int)tmpX;
                    int intY = (int)tmpY;

                    //this.spriteTargetScalableObject.height = intY;
                    //this.spriteTargetScalableObject.width = intX;

                    this.scalableTargetImage.rectTransform.sizeDelta = new Vector2(intX, intY);

                    //added temporarily

                    //if (this.spriteTargetScalableObject.height > validAreaValue.y + heightValidRange)
                    //{
                    //    RestrictScalableTargetHeightValue();
                    //}
                    //if (this.spriteTargetScalableObject.width > validAreaValue.x + widthValidRange)
                    //{
                    //    RestrictScalableTargetWidthValue();
                    //}

                    if (this.scalableTargetImage.rectTransform.rect.height > validAreaValue.y + heightValidRange)
                    {
                        RestrictScalableTargetHeightValue();
                    }
                    if (this.scalableTargetImage.rectTransform.rect.width > validAreaValue.x + widthValidRange)
                    {
                        RestrictScalableTargetWidthValue();
                    }
                }
                else
                {
                    if (Input.GetMouseButtonUp(0))
                    {

                        CursorOriginActiveTrue();


                        CheckIsScalingValidArea();

                        isIn = false;

                        // CameraManager.ins.SetScreenMovable();
                    }
                }
            }
        }
    }
    public void OnClickStartO()
    {
        isMousePressed = true;
    }

    public void OnDragEndO()
    {
        isMousePressed = false;
    }

    public GameObject nextStepCodeObj;

    public Vector2 validAreaValue = Vector2.zero;
    public int heightValidRange = 20;
    public int widthValidRange = 20;

    private void CheckIsScalingValidArea()
    {
        // 210 / 50
        //var targetHeight = this.spriteTargetScalableObject.height;
        //var targetWidth = this.spriteTargetScalableObject.width;

        var targetHeight = this.scalableTargetImage.rectTransform.rect.height;
        var targetWidth = this.scalableTargetImage.rectTransform.rect.width;

        if (targetHeight >= validAreaValue.y - heightValidRange && targetHeight <= validAreaValue.y + heightValidRange)
        {
            if (targetWidth >= validAreaValue.x - widthValidRange && targetWidth <= validAreaValue.x + widthValidRange)
            {
                cursorOrigin.SetActive(false);
                cursorFollowing.SetActive(false);
                nextStepCodeObj.SetActive(true);
                CustomDebug.Log("CheckIsScalingValidArea Success!!!!");
            }
            else
            {
                ResetScalableTargetValue();
            }
        }
        else
        {
            ResetScalableTargetValue();
        }
    }
    private void RestrictScalableTargetHeightValue()
    {
        // this.spriteTargetScalableObject.height = (int)validAreaValue.y + heightValidRange;
        this.scalableTargetImage.rectTransform.sizeDelta = new Vector2(this.scalableTargetImage.rectTransform.rect.width, (int)validAreaValue.y + heightValidRange);
    }
    private void RestrictScalableTargetWidthValue()
    {
        // this.spriteTargetScalableObject.width = (int)validAreaValue.x + widthValidRange;
        this.scalableTargetImage.rectTransform.sizeDelta = new Vector2((int)validAreaValue.x + widthValidRange, this.scalableTargetImage.rectTransform.rect.height);
    }
    private void ResetScalableTargetValue()
    {
        //this.spriteTargetScalableObject.height = 0;
        //this.spriteTargetScalableObject.width = 0;

        this.scalableTargetImage.rectTransform.sizeDelta = new Vector2(0, 0);
    }
}