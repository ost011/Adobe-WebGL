using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpineHandlingModule : MonoBehaviour
{
    public GameObject[] characterMaleJinos; // 0 idle, 1 upset, 2 angry, 3 happy, 4 lordly
    private RectTransform[] rectTransformsMale = null;

    private RectTransform currentMaleCharacter = null;
    private Vector2 malePos = Vector2.zero;
    private Vector2 maleSide = Vector2.zero;

    public GameObject[] characterFemaleJinis; // 0 idle, 1 suprise, 2 happy, 3 nice
    private RectTransform[] rectTransformsFemale = null;

    private RectTransform currentFemaleCharacter = null;
    private Vector2 femalePos = Vector2.zero;
    private Vector2 femaleSide = Vector2.zero;

    [Space]
    public RectTransform posRight;
    public RectTransform posLeft;
    public RectTransform posMoreRight;

    //[Space]
    //[Header("PosForSpecificLecture")]
    //public RectTransform posForS1L2;

    private Vector2 rightSide = Vector2.one * 30;
    private Vector2 leftSide = new Vector2(-30, 30);

    // private Vector2 sideForS1L2 = new Vector2(20, 20);


    private EnumSets.CharacterSpineSpecialEmotion currentMaleEmotion = EnumSets.CharacterSpineSpecialEmotion.None;
    private EnumSets.CharacterSpineSpecialEmotion currentFemaleEmotion = EnumSets.CharacterSpineSpecialEmotion.None;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        rectTransformsMale = new RectTransform[characterMaleJinos.Length];

        for (int i = 0; i < rectTransformsMale.Length; i++)
        {
            rectTransformsMale[i] = characterMaleJinos[i].GetComponent<RectTransform>();
        }

        rectTransformsFemale = new RectTransform[characterFemaleJinis.Length];

        for (int i = 0; i < rectTransformsFemale.Length; i++)
        {
            rectTransformsFemale[i] = characterFemaleJinis[i].GetComponent<RectTransform>();
        }

    }

    /// <summary>
    /// 남자 캐릭터는 idle, Happy, Upset, angry, Lordly 만 사용
    /// </summary>
    public void ActivateMaleCharacter(EnumSets.CharacterSpineSpecialEmotion emotion, EnumSets.CharacterPos pos)
    {
        if(this.currentMaleEmotion.Equals(emotion))
        {
            // 이미 같은 감정타입이 들어오면 아무것도 안함
            return;
        }
        else
        {
            this.currentMaleEmotion = emotion;

            this.malePos = GetProperPos(pos);
            this.maleSide = GetProperSide(pos);

            CheckWhatCharacterActivating(EnumSets.CharacterType.Male, emotion);
        }
    }

    private void ShowMaleCharacter(EnumSets.CharacterSpineSpecialEmotion emotion)
    {
        switch(emotion)
        {
            case EnumSets.CharacterSpineSpecialEmotion.Idle:
                {
                    currentMaleCharacter = rectTransformsMale[0];
                }
                break;
            case EnumSets.CharacterSpineSpecialEmotion.Upset:
                {
                    currentMaleCharacter = rectTransformsMale[1];
                }
                break;
            case EnumSets.CharacterSpineSpecialEmotion.Angry:
                {
                    currentMaleCharacter = rectTransformsMale[2];
                }
                break;
            case EnumSets.CharacterSpineSpecialEmotion.Happy:
                {
                    currentMaleCharacter = rectTransformsMale[3];
                }
                break;
            case EnumSets.CharacterSpineSpecialEmotion.Lordly:
                {
                    currentMaleCharacter = rectTransformsMale[4];
                }
                break;
        }

        if(currentMaleCharacter != null)
        {
            currentMaleCharacter.anchoredPosition = this.malePos;
            currentMaleCharacter.transform.localScale = this.maleSide;

            currentMaleCharacter.gameObject.SetActive(true);
        }
    }

    public void ActivateMaleCharacter(EnumSets.CharacterSpineSpecialEmotion emotion, Vector2[] posInfo)
    {
        if (this.currentMaleEmotion.Equals(emotion))
        {
            // 이미 같은 감정타입이 들어오면 아무것도 안함
            return;
        }
        else
        {
            this.currentMaleEmotion = emotion;

            this.malePos = posInfo[0];
            this.maleSide = posInfo[1];

            CheckWhatCharacterActivating(EnumSets.CharacterType.Male, emotion);
        }
    }

    /// <summary>
    /// 여자 캐릭터는 idle, Surprise, Happy, Nice 만 사용
    /// </summary>
    public void ActivateFemaleCharacter(EnumSets.CharacterSpineSpecialEmotion emotion, EnumSets.CharacterPos pos)
    {
        if (this.currentFemaleEmotion.Equals(emotion))
        {
            // 이미 같은 감정타입이 들어오면 아무것도 안함
            return;
        }
        else
        {
            this.currentFemaleEmotion = emotion;

            this.femalePos = GetProperPos(pos);
            this.femaleSide = GetProperSide(pos);

            CheckWhatCharacterActivating(EnumSets.CharacterType.Female, emotion);
        }
    }

    public void ActivateFemaleCharacter(EnumSets.CharacterSpineSpecialEmotion emotion, Vector2[] posInfo)
    {
        if (this.currentFemaleEmotion.Equals(emotion))
        {
            // 이미 같은 감정타입이 들어오면 아무것도 안함
            return;
        }
        else
        {
            this.currentFemaleEmotion = emotion;

            this.femalePos = posInfo[0];
            this.femaleSide = posInfo[1];

            CheckWhatCharacterActivating(EnumSets.CharacterType.Female, emotion);
        }
    }

    private void CheckWhatCharacterActivating(EnumSets.CharacterType characterType, EnumSets.CharacterSpineSpecialEmotion emotion)
    {
        switch (characterType)
        {
            case EnumSets.CharacterType.Male:
                {
                    ShowMaleCharacter(emotion);
                }
                break;
            case EnumSets.CharacterType.Female:
                {
                    ShowFemaleCharacter(emotion);
                }
                break;
        }
    }

    private Vector2 GetProperPos(EnumSets.CharacterPos pos)
    {
        var targetPos = Vector2.zero;

        switch (pos)
        {
            case EnumSets.CharacterPos.Right:
                {
                    targetPos = posRight.anchoredPosition;
                }
                break;
            case EnumSets.CharacterPos.Left:
                {
                    targetPos = posLeft.anchoredPosition;
                }
                break;
            case EnumSets.CharacterPos.MoreRight:
                {
                    targetPos = posMoreRight.anchoredPosition;
                }
                break;
        }

        return targetPos;
    }

    private Vector2 GetProperSide(EnumSets.CharacterPos pos)
    {
        var targetSide = Vector2.zero;

        switch (pos)
        {
            case EnumSets.CharacterPos.Right:
                {
                    targetSide = rightSide;
                }
                break;
            case EnumSets.CharacterPos.Left:
                {
                    targetSide = leftSide;
                }
                break;
            case EnumSets.CharacterPos.MoreRight:
                {
                    targetSide = rightSide;
                }
                break;
        }

        return targetSide;
    }

    private void ShowFemaleCharacter(EnumSets.CharacterSpineSpecialEmotion emotion)
    {
        switch (emotion)
        {
            case EnumSets.CharacterSpineSpecialEmotion.Idle:
                {
                    currentFemaleCharacter = rectTransformsFemale[0];
                }
                break;
            case EnumSets.CharacterSpineSpecialEmotion.Surprise:
                {
                    currentFemaleCharacter = rectTransformsFemale[1];
                }
                break;
            case EnumSets.CharacterSpineSpecialEmotion.Happy:
                {
                    currentFemaleCharacter = rectTransformsFemale[2];
                }
                break;
            case EnumSets.CharacterSpineSpecialEmotion.Nice:
                {
                    currentFemaleCharacter = rectTransformsFemale[3];
                }
                break;
        }

        if (currentFemaleCharacter != null)
        {
            currentFemaleCharacter.anchoredPosition = this.femalePos;
            currentFemaleCharacter.transform.localScale = this.femaleSide;

            currentFemaleCharacter.gameObject.SetActive(true);
        }
    }

    public void DeActivateCharacter(EnumSets.CharacterType characterType)
    {
        ResetData();

        switch (characterType)
        {
            case EnumSets.CharacterType.Male:
                {
                    DeActivateMaleCharacter();
                }
                break;
            case EnumSets.CharacterType.Female:
                {
                    DeActivateFemaleCharacter();
                }
                break;
        }
    }

    private void DeActivateMaleCharacter()
    {
        currentMaleCharacter = null;

        for (int i = 0; i < characterMaleJinos.Length; i++)
        {
            characterMaleJinos[i].SetActive(false);
        }
    }

    private void DeActivateFemaleCharacter()
    {
        for (int i = 0; i < characterFemaleJinis.Length; i++)
        {
            characterFemaleJinis[i].SetActive(false);
        }
    }

    private void ResetData()
    {
        this.currentMaleEmotion = EnumSets.CharacterSpineSpecialEmotion.None;
        this.currentFemaleEmotion = EnumSets.CharacterSpineSpecialEmotion.None;

        this.currentMaleCharacter = null;
        this.currentFemaleCharacter = null;
    }

    public void ForceAllDeActivateCharacters()
    {
        DeActivateMaleCharacter();

        DeActivateFemaleCharacter();

        ResetData();
    }
}
