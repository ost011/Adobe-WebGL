using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterFiveGuideModule : GuideTemplate
{
    public RectTransform bgCharacter;
    
    public Vector2[] posesCharacter;
    public Vector2[] scalesCharacter;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    public override void LoadFirstStep()
    {
        ActivateBgCharacter();

        base.LoadFirstStep();
    }

    public void ActivateBgCharacter()
    {
        bgCharacter.gameObject.SetActive(true);
    }

    public void DeActivateBgCharacter()
    {
        bgCharacter.gameObject.SetActive(false);
    }

    public void SetCharacterState(int index)
    {
        CustomDebug.LogWithColor($"SetCharacterState, index = {index} / {posesCharacter[index]} / {scalesCharacter[index]}", CustomDebug.ColorSet.Magenta);

        bgCharacter.anchoredPosition = posesCharacter[index];

        bgCharacter.transform.localScale = scalesCharacter[index];
    }
}
