using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterTwoGuideModule : GuideTemplate
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    public override void LoadFirstStep()
    {
        // 카메라 이동이 필요할 듯
        CustomDebug.Log("May be Camera Moving~");

        base.LoadFirstStep();
    }
}
