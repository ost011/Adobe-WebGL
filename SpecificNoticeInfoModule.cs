using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecificNoticeInfoModule : MonoBehaviour
{
    public RawImage rawNoticeImage;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InstantLoadNotice(string base64Code)
    {
        CustomDebug.Log("InstantLoadNotice ----");

        try
        {
            var bytes = Convert.FromBase64String(base64Code);

            var tex = new Texture2D(0, 0, TextureFormat.ARGB32, false);

            if (tex.LoadImage(bytes))
            {
                CustomDebug.Log("InstantLoadNotice success");

                this.rawNoticeImage.texture = tex;
            }
            else
            {
                CustomDebug.LogError("InstantLoadNotice failed");
            }
        }
        catch(Exception e)
        {
            CustomDebug.LogError($"InstantLoadNotice error : {e.Message}");
        }
        finally
        {
            LoadingManager.Instance.DeActivateLoading();
        }
    }

    public void DeActivateItem()
    {
        this.gameObject.SetActive(false);
    }

    public void ActivateItem()
    {
        this.gameObject.SetActive(true);
    }
}
