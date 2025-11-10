using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoLectureModule : MonoBehaviour
{
    public string targetVideoFileName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetTargetVideoFileNameToVideoPlayer()
    {
        HomeController.Instance.SetWhatVideoPlaying(this.targetVideoFileName);
    }
}
