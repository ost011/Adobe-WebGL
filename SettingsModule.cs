using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class SettingsModule : MonoBehaviour
{
    public GameObject[] setBGM; // 0 on, 1 off
    public GameObject[] setFX;

    [Space]
    public Button[] btnsBGM;
    public Button[] btnsFX;

    private const int DELAY_DURATION = 500;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        var isBGMOnState = SoundManager.Instance.IsBGMOnState;

        if(isBGMOnState)
        {
            ShowBGMOnState();
        }
        else
        {
            ShowBGMOffState();
        }

        var isFXOnState = SoundManager.Instance.IsFXOnState;

        if(isFXOnState)
        {
            ShowFXOnState();
        }
        else
        {
            ShowFXOffState();
        }
    }

    public virtual void OnClickSettingBtn()
    {
        HomeController.Instance.ActivatePopUp(EnumSets.PopUpType.Settings);
    }

    // 리펙토링 필요
    public void OnChangeSoundOnOff(SoundSettingModule module)
    {
        // soundManager func 호출 필요

        CustomDebug.Log($"On Change Next Sound OnOff ->  {module.nextSoundSettingType}");

        switch (module.nextSoundSettingType)
        {
            case EnumSets.NextSoundSettingType.BGMOn:
                {
                    DelayActivateBGMBtns().Forget();

                    ShowBGMOnState();

                    SoundManager.Instance.TurnBGMOn();
                }
                break;
            case EnumSets.NextSoundSettingType.BGMOff:
                {
                    DelayActivateBGMBtns().Forget();

                    ShowBGMOffState();

                    SoundManager.Instance.TurnBGMOff();
                }
                break;
            case EnumSets.NextSoundSettingType.FxOn:
                {
                    DelayActivateFXBtns().Forget();

                    ShowFXOnState();

                    SoundManager.Instance.TurnFxOn();
                }
                break;
            case EnumSets.NextSoundSettingType.FxOff:
                {
                    DelayActivateFXBtns().Forget();

                    ShowFXOffState();

                    SoundManager.Instance.TurnFxOff();
                }
                break;
        }

        //if (module.GetToggleState())
        //{
            

        //    // module.ActivateSubText();
        //    module.UpdateSoundSettingBySoundSettingType();
        //}
        //else
        //{
        //    CustomDebug.Log($"On Change Sound, just text Deactivate ->  {module.nextSoundSettingType}");

        //    // module.DeActivateSubText();
        //}
    }

    /// <summary>
    /// 마구잡이로 버튼을 누르는 행위를 막기위해서 일정시간 버튼 변경 딜레이를 부여함
    /// / mouse click fx 탐지를 코루틴에서 수행하는데 중간에 꼬이는 것을 막기위함
    /// </summary>
    private async UniTask DelayActivateBGMBtns()
    {
        for (int i = 0; i < btnsBGM.Length; i++)
        {
            btnsBGM[i].interactable = false;
        }

        await UniTask.Delay(DELAY_DURATION);

        for (int i = 0; i < btnsBGM.Length; i++)
        {
            btnsBGM[i].interactable = true;
        }
      
    }

    /// <summary>
    /// 마구잡이로 버튼을 누르는 행위를 막기위해서 일정시간 버튼 변경 딜레이를 부여함
    /// / mouse click fx 탐지를 코루틴에서 수행하는데 중간에 꼬이는 것을 막기위함
    /// </summary>
    private async UniTask DelayActivateFXBtns()
    {
        for (int i = 0; i < btnsFX.Length; i++)
        {
            btnsFX[i].interactable = false;
        }

        await UniTask.Delay(DELAY_DURATION);

        for (int i = 0; i < btnsFX.Length; i++)
        {
            btnsFX[i].interactable = true;
        }
    }

    protected void AllDeActivateBGMObjects()
    {
        for (int i = 0; i < setBGM.Length; i++)
        {
            setBGM[i].SetActive(false);
        }
    }

    protected void AllDeActivateFXObjects()
    {
        for (int i = 0; i < setFX.Length; i++)
        {
            setFX[i].SetActive(false);
        }
    }

    private void ActivateBGMObject(int index)
    {
        setBGM[index].SetActive(true);
    }

    private void ActivateFXObject(int index)
    {
        setFX[index].SetActive(true);
    }

    protected virtual void ShowBGMOnState()
    {
        AllDeActivateBGMObjects();

        ActivateBGMObject(0);
    }

    protected virtual void ShowFXOnState()
    {
        AllDeActivateFXObjects();

        ActivateFXObject(0);
    }

    protected virtual void ShowBGMOffState()
    {
        AllDeActivateBGMObjects();

        ActivateBGMObject(1);
    }

    protected virtual void ShowFXOffState()
    {
        AllDeActivateFXObjects();

        ActivateFXObject(1);
    }
}
