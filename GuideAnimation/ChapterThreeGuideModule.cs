using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterThreeGuideModule : GuideTemplate
{
    public GameObject bgCharacters;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    public override void LoadFirstStep()
    {
        ActivateBgCharacters();

        base.LoadFirstStep();
    }

    public void ActivateBgCharacters()
    {
        bgCharacters.SetActive(true);
    }

    public void DeActivateBgChracters()
    {
        bgCharacters.SetActive(false);
    }
}
