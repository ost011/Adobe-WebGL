using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using Cysharp.Threading.Tasks;

public class GuideStageTwoTextBoxModule : MonoBehaviour
{
    public GameObject textBox;
    public TextMeshProUGUI textSentence;

    [Space]
    public GameObject[] objsFemale; // 0 idle, 1 nice
    public GameObject[] objsMale; // 0 idle, 1 lordly

    [Space]
    public GameObject youtubeArea;

    private GameObject currentCharacter = null;

    private int currentPressedNextBtnCount = 0;

    private GuideTextBoxStatus currentTextBoxStatus;

    private Func<int, GuideTextBoxStatus> checkNextStepStatus = null; // 현재 텍스트 상자의 next 버튼을 누른 횟수, 진행 가능 여부 반환

    private Action readyToLoadNextSequence = null; // 아래 대화상자 출력이 끝났을 때 시퀀스 이어하기 

    private const string HUB_LINK = "https://www.adobe.com/kr/lead/creativecloud/quick-start.html?promoid=D8F91LNN&mv=other";

    private void OnDestroy()
    {
        checkNextStepStatus = null;
        readyToLoadNextSequence = null;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    IEnumerator CorTestCode()
    {
        yield return new WaitForSeconds(2f);

        ForceLoadStep();
    }

    public void LoadFirstStep()
    {
        ForceLoadStep();
    }

    // 필요 뷰에서 텍스트박스를 띄워야할 때 호출하기
    // 필요한 텍스트, 캐릭터 등은 자동으로 맞춰짐
    public void ForceLoadStep()
    {
        DeActivateAllCharacters();

        var status = this.checkNextStepStatus?.Invoke(currentPressedNextBtnCount);

        CustomDebug.Log($"ForceLoadStep : {status}");

        if (status.HasValue)
        {
            this.currentTextBoxStatus = status.Value;
            
            CustomDebug.Log($"currentTextBoxStatus : {currentTextBoxStatus}");

            if (!this.currentTextBoxStatus.IsNeedToShow)
            {
                HideTextBox();
            }
            else
            {
                ShowDelayedTextBox().Forget();
                // ShowTextBox();
            }
        }
    }

    public void NextStep()
    {
        DeActivateAllCharacters();

        ++currentPressedNextBtnCount;
        CustomDebug.Log($"--> currentPressedNextBtnCount : {currentPressedNextBtnCount}");

        var status = this.checkNextStepStatus?.Invoke(currentPressedNextBtnCount);

        if (status.HasValue)
        {
            this.currentTextBoxStatus = status.Value;

            if(!this.currentTextBoxStatus.IsNeedToShow)
            {
                HideTextBox();

                readyToLoadNextSequence?.Invoke();
            }
            else
            {
                ShowTextBox();
            }
        }
        else
        {
            HideTextBox();
        }
    }

    public void SetCheckNextStepStatusCallback(Func<int, GuideTextBoxStatus> callback)
    {
        CustomDebug.Log("SetCheckNextStepStatusCallback done");

        this.checkNextStepStatus = callback;
    }

    private async UniTask ShowDelayedTextBox()
    {
        CustomDebug.Log($"ShowDelayedTextBox");

        await UniTask.Delay(500);

        ShowTextBox();
    }

    private void ShowTextBox()
    {
        SetSentence(this.currentTextBoxStatus.Sentence);

        CheckWhatCharacterShowing();

        ActivateTextBox();

        if(this.currentTextBoxStatus.IsNeedToShowYoutubeArea)
        {
            youtubeArea.SetActive(true);
        }
    }

    private void HideTextBox()
    {
        CustomDebug.Log($"HideTextBox");

        SetForceReInitCurrentPressedNextBtnCount();

        DeActivateTextBox();

        SetSentence(string.Empty);

        // DeActivateAllCharacters();
    }

    private void DeActivateTextBox()
    {
        this.textBox.SetActive(false);
    }

    private void ActivateTextBox()
    {
        this.textBox.SetActive(true);
    }

    private void SetSentence(string sentence)
    {
        this.textSentence.text = sentence;
    }

    private void DeActivateAllCharacters()
    {
        for (int i = 0; i < objsFemale.Length; i++)
        {
            objsFemale[i].SetActive(false);
        }

        for (int i = 0; i < objsMale.Length; i++)
        {
            objsMale[i].SetActive(false);
        }
    }

    private void CheckWhatCharacterShowing()
    {
        var whatCharacter = currentTextBoxStatus.CharacterType;
        var whatEmotion = currentTextBoxStatus.Emotion;

        CustomDebug.Log($"CheckWhatCharacterShowing => whatCharacter : {whatCharacter} / whatEmotion : {whatEmotion}");

        if (whatCharacter.Equals((int)EnumSets.CharacterType.Female))
        {
            switch(whatEmotion)
            {
                case (int)EnumSets.CharacterSpineSpecialEmotion.Idle:
                    {
                        currentCharacter = objsFemale[0];
                    }
                    break;
                case (int)EnumSets.CharacterSpineSpecialEmotion.Nice:
                    {
                        currentCharacter = objsFemale[1];
                    }
                    break;
            }
        }
        else
        {
            if (whatCharacter.Equals((int)EnumSets.CharacterType.Male))
            {
                switch (whatEmotion)
                {
                    case (int)EnumSets.CharacterSpineSpecialEmotion.Idle:
                        {
                            currentCharacter = objsMale[0];
                        }
                        break;
                    case (int)EnumSets.CharacterSpineSpecialEmotion.Lordly:
                        {
                            currentCharacter = objsMale[1];
                        }
                        break;
                }
            }
        }

        currentCharacter.SetActive(true);
    }

    public void SetForceReInitCurrentPressedNextBtnCount()
    {
        this.currentPressedNextBtnCount = 0;
    }

    public void SetShowNextSequence(Action isReady)
    {
        this.readyToLoadNextSequence = isReady;
    }


    //----------------------------------------------------------
    // 유튜브 배너 관련
    public void OnClickBanner()
    {
        WebWindowController.OpenTargetWindow(HUB_LINK);
    }
}
