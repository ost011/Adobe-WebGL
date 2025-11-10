using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class CaptureManager : MonoBehaviour
{
    private static CaptureManager instance = null;
    public static CaptureManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<CaptureManager>();
            }

            return instance;
        }
    }

    public GameObject captureSystem;

    public Camera captureCamera;

    [DllImport("__Internal")]
    private static extern void DownloadSomething(byte[] array, int byteLength, string fileName);

    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void DoCapture()
    {
        try
        {
            var activeRenderTex = RenderTexture.active;

            RenderTexture.active = this.captureCamera.targetTexture;

            this.captureCamera.Render();

            var cameraTex = this.captureCamera.targetTexture;

            var image = new Texture2D(cameraTex.width, cameraTex.height);

            image.ReadPixels(new Rect(0, 0, cameraTex.width, cameraTex.height), 0, 0);
            image.Apply();

            RenderTexture.active = activeRenderTex;

            var bytes = image.EncodeToJPG();

            Destroy(image);

            var savePath = Path.Combine(Application.persistentDataPath, "CImage3.jpg");

            Debug.Log($"savePath : {savePath}");

            File.WriteAllBytes(savePath, bytes);
        }
        catch(Exception e)
        {
            Debug.LogError($"DoCapture Error : {e.Message}");
        }
        

    }

    public void DoCapture2()
    {
        StartCoroutine(CorDoCapture2());
    }

    IEnumerator CorDoCapture2()
    {
        captureSystem.SetActive(true);

        yield return new WaitForEndOfFrame();

        try
        {
            var activeRenderTex = RenderTexture.active;

            RenderTexture.active = this.captureCamera.targetTexture;

            this.captureCamera.Render();

            var cameraTex = this.captureCamera.targetTexture;

            var image = new Texture2D(cameraTex.width, cameraTex.height);

            image.ReadPixels(new Rect(0, 0, cameraTex.width, cameraTex.height), 0, 0);
            image.Apply();

            RenderTexture.active = activeRenderTex;

            var bytes = image.EncodeToJPG();

            DownloadSomething(bytes, bytes.Length, "CImage4.jpg");

            Destroy(image);

        }
        catch (Exception e)
        {
            Debug.LogError($"DoCapture2 Error : {e.Message}");
        }
        finally
        {
            captureSystem.SetActive(false);
        }
    }
}
