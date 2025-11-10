using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsModuleInSLMMode : SettingsModule
{
    public GameObject[] arrayOffIcons;  // 0 BGM, 1 FX

    public override void OnClickSettingBtn()
    {
        SmartLearningModeController.Instance.ActivateSpecificPopUp(EnumSets.SLMPopUpType.Settings);
    }

    protected override void ShowBGMOnState()
    {
        AllDeActivateBGMObjects();

        SetBGMObjectStateOnBGMOn();
    }

    protected override void ShowBGMOffState()
    {
        AllDeActivateBGMObjects();

        SetBGMObjectStateOnBGMOff();
    }

    protected override void ShowFXOnState()
    {
        AllDeActivateFXObjects();

        SetFXObjectStateOnFXOn();
    }

    protected override void ShowFXOffState()
    {
        AllDeActivateFXObjects();

        SetFXObjectStateOnFXOff();
    }

    private void SetBGMObjectStateOnBGMOn()
    {
        setBGM[0].SetActive(true);
        setBGM[2].SetActive(true);

        arrayOffIcons[0].SetActive(false);
    }

    private void SetBGMObjectStateOnBGMOff()
    {
        setBGM[1].SetActive(true);
        setBGM[3].SetActive(true);

        arrayOffIcons[0].SetActive(true);
    }

    private void SetFXObjectStateOnFXOn()
    {
        setFX[0].SetActive(true);
        setFX[2].SetActive(true);

        arrayOffIcons[1].SetActive(false);
    }

    private void SetFXObjectStateOnFXOff()
    {
        setFX[1].SetActive(true);
        setFX[3].SetActive(true);

        arrayOffIcons[1].SetActive(true);
    }
}
