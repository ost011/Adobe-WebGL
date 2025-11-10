using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterFourGuideModule : GuideTemplate
{
    public GameObject[] bgCharacter;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    public override void LoadFirstStep()
    {
        ActivateBgCharacter(0);
     
        base.LoadFirstStep();
    }

    public void ActivateBgCharacter(int index)
    {
        bgCharacter[index].SetActive(true);
    }

    public void DeActivateAllBgCharacter()
    {
        for (int i = 0; i < bgCharacter.Length; i++)
        {
            bgCharacter[i].SetActive(false);

        }
    }
}

