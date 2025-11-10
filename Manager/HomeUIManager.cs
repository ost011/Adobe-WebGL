using System;
using TMPro;
using UnityEngine;

public class HomeUIManager : MonoBehaviour
{
    [Header("Lobby - Login - Line")]
    public GameObject panelLogin; // 사용하지않음

    [Space]
    public GameObject panelLogOut; // 사용하지않음

    [Space]
    [Header("Lobby - Main - Line")]
    public GameObject[] mainPanels;

    [Space]
    [Header("Upper Line ---")]
    public TextMeshProUGUI textUserEmail;
    public TextMeshProUGUI textExtraFuncBtn;

    [Space]
    public GameObject popUpCommonError;
    public TextMeshProUGUI textCommonErrorOnPopUpCommonError;

    [Space]
    public GameObject popUpExit;
    public GameObject[] subTitlesOnExitPopUp;
    public GameObject btnExtra;

    [Space]
    public GameObject[] signInTypeBtns; // 0 signed in / 1 guest

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ForceDeActivateLoginLobbyPanel()
    {
        panelLogin.SetActive(false);
    }

    public void ActivateTargetPanel()
    {
        AllDeActivateMainLobbyPanels();

        var currentPanelType = HomeController.Instance.GetCurrentPanelType();

        var index = (int)currentPanelType;

        mainPanels[index].SetActive(true);
    }

    public void AllDeActivateMainLobbyPanels()
    {
        for (int i = 0; i < mainPanels.Length; i++)
        {
            mainPanels[i].SetActive(false);
        }
    }

    public void ActivateLogOutPanel()
    {
        panelLogOut.SetActive(true);
    }

    public void DeActivateLogOutPanel()
    {
        panelLogOut.SetActive(false);
    }

    public void SetUpperUserEmailText(string text)
    {
        this.textUserEmail.text = text;
    }

    public void SetUpperExtraFuncBtnType(EnumSets.SignInType btnType)
    {
        var index = Convert.ToInt32(btnType);

        signInTypeBtns[index].SetActive(true);
    }

    public void ActivateCommonErrorPopUp(EnumSets.ErrorType errorType)
    {
        var errorMsg = GetProperMessage(errorType);

        this.textCommonErrorOnPopUpCommonError.text = errorMsg;

        this.popUpCommonError.SetActive(true);
    }

    private string GetProperMessage(EnumSets.ErrorType errorType)
    {
        var errorMsg = "";

        switch (errorType)
        {
            case EnumSets.ErrorType.InValidEmailAddress:
                {
                    errorMsg = "이메일 형식이 부정확합니다.";
                }
                break;
            case EnumSets.ErrorType.GetRewardBodyTextError:
                {
                    errorMsg = "보상 이메일을 보내기위한 정보 초기화에 실패했습니다.";
                }
                break;
            case EnumSets.ErrorType.SendRewardEmailFailed:
                {
                    errorMsg = "보상 이메일 전송을 실패했습니다.";
                }
                break;
            case EnumSets.ErrorType.ReSendRewardEmailFailed:
                {
                    errorMsg = "이메일 재전송, 실패했습니다.";
                }
                break;
            case EnumSets.ErrorType.UpdatePrizeDataAfterSendRewardEmailFailed:
                {
                    errorMsg = "보상 이메일 전송 후 데이터 초기화를\n실패했습니다.";
                }
                break;
            case EnumSets.ErrorType.DrawError:
                {
                    errorMsg = "럭키 드로우 실패";
                }
                break;
            case EnumSets.ErrorType.AuthError:
                {
                    errorMsg = "사용자 인증에 실패했습니다. 재로그인 부탁드립니다.";
                }
                break;
            case EnumSets.ErrorType.DBUpdateError:
                {
                    errorMsg = "사용자 정보, 업데이트 실패";
                }
                break;
            case EnumSets.ErrorType.UnavailableError:
                {
                    errorMsg = "수행 중 오류가 발생했습니다.\n문의사항은 adobe@ethan-alice.com\n으로 연락주세요.";
                }
                break;
            case EnumSets.ErrorType.NotYetReadyToGetAGift:
                {
                    errorMsg = "아직 학습을 완료하지 않아서 선물을\n받을 수 없습니다.";
                }
                break;
            case EnumSets.ErrorType.GuestSignInType:
                {
                    errorMsg = "손님으로 입장시 선물을 받을 수 없습니다.\n회원가입 / 계정연동을 해주세요";
                }
                break;
        }

        return errorMsg;
    }

    public void DeActivateCommonErrorPopUp()
    {
        this.popUpCommonError.SetActive(false);
    }

    public void UpdateExitContentsOnSignInType()
    {
        AllDeActivateSubTitlesOnExitPopUp();

        subTitlesOnExitPopUp[0].SetActive(true);

        btnExtra.SetActive(false);
    }

    public void UpdateExitContentsOnGuestType()
    {
        AllDeActivateSubTitlesOnExitPopUp();

        subTitlesOnExitPopUp[1].SetActive(true);

        btnExtra.SetActive(true);
    }


    private void AllDeActivateSubTitlesOnExitPopUp()
    {
        for (int i = 0; i < subTitlesOnExitPopUp.Length; i++)
        {
            subTitlesOnExitPopUp[i].SetActive(false);
        }
    }
}
