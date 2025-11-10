using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
// using System.Drawing.Imaging;
using System.IO;
// using System.Windows.Forms;
using UnityEngine;

public class SaveImageModule : MonoBehaviour
{
    private const string UPPER_TITLE = "포토샵 @@@ 과정 수료증";
    private const string LOWER_TITLE = "붱철님은 포토샵 @@@ 과정을 모두 완료하였기에 이 수료증을 드립니다";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SaveImage()
    {
        /*
        try
        {
            var baseImagePath = Path.Combine(UnityEngine.Application.streamingAssetsPath, "certificate.jpg");

            var baseImage = Bitmap.FromFile(baseImagePath);

            var graphics = System.Drawing.Graphics.FromImage(baseImage);

            graphics.DrawString(UPPER_TITLE, new System.Drawing.Font(new FontFamily("Arial"), 50, System.Drawing.FontStyle.Bold, GraphicsUnit.Pixel),
               Brushes.Black, new Point(baseImage.Width / 10, 50));

            graphics.DrawString(LOWER_TITLE, new System.Drawing.Font(new FontFamily("Arial"), 20, System.Drawing.FontStyle.Italic, GraphicsUnit.Pixel),
                Brushes.Black, new Point(baseImage.Width / 15, baseImage.Height - 30));

            graphics.Save();

            var savePath = Path.Combine(UnityEngine.Application.persistentDataPath, "CImage.jpg");

            Debug.Log($"save path : {savePath}");

            baseImage.Save(savePath);

            Debug.Log($"save complete");
        }
        catch(Exception e)
        {
            Debug.LogError($"save failed , {e.Message}");
        }
        */
    }

    public void SaveImageWithFolderDialog()
    {
        /*
        try
        {
            using(FolderBrowserDialog folderDialog = GetSaveFolderPath())
            {
                if(folderDialog.ShowDialog().Equals(DialogResult.OK))
                {
                    var dir = folderDialog.SelectedPath;

                    var baseImagePath = Path.Combine(UnityEngine.Application.streamingAssetsPath, "certificate.jpg");

                    var baseImage = Bitmap.FromFile(baseImagePath);

                    var graphics = System.Drawing.Graphics.FromImage(baseImage);

                    graphics.DrawString(UPPER_TITLE, new System.Drawing.Font(new FontFamily("Arial"), 50, System.Drawing.FontStyle.Bold, GraphicsUnit.Pixel),
                       Brushes.Black, new Point(baseImage.Width / 10, 50));

                    graphics.DrawString(LOWER_TITLE, new System.Drawing.Font(new FontFamily("Arial"), 20, System.Drawing.FontStyle.Italic, GraphicsUnit.Pixel),
                        Brushes.Black, new Point(baseImage.Width / 15, baseImage.Height - 30));

                    graphics.Save();

                    var savePath = Path.Combine(dir, "CImage.jpg");

                    Debug.Log($"save path : {savePath}");

                    baseImage.Save(savePath);

                    Debug.Log($"save complete");
                }
            }
            
        }
        catch (Exception e)
        {
            Debug.LogError($"save failed , {e.Message}");
        }

        */
    }

    /*
    private FolderBrowserDialog GetSaveFolderPath()
    {
        var saveFolderPath = new FolderBrowserDialog();

        saveFolderPath.RootFolder = Environment.SpecialFolder.Desktop;

        return saveFolderPath;
    }
    */

    public void SaveScreenShot()
    {
        try
        {
            var savePath = Path.Combine(UnityEngine.Application.persistentDataPath, "CImage_CaptureScreenshot.jpg");

            ScreenCapture.CaptureScreenshot(savePath);
           
        }
        catch(Exception e)
        {
            Debug.LogError($"SaveScreenShot Error : {e.Message}");
        }
        
    }
}
