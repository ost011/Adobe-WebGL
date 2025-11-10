using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SLMPopUpManager : MonoBehaviour
{
    public SmartLearningModeUIManager smartLearningModeUIManager;

    [SerializeField]
    private EnumSets.SLMPopUpType currentActivatedPopUpType = EnumSets.SLMPopUpType.None;

    private string commonErrorMsg = "";

    public void ActivatePopUp(EnumSets.SLMPopUpType popUpType)
    {
        switch (popUpType)
        {
            case EnumSets.SLMPopUpType.LoadPrefabError:
                {
                    this.smartLearningModeUIManager.ActivateLoadingErrorPopUp();

                    currentActivatedPopUpType = EnumSets.SLMPopUpType.LoadPrefabError;
                }
                break;
            case EnumSets.SLMPopUpType.UpdatingDBError:
                {
                    this.smartLearningModeUIManager.ActivateUpdateDBErrorPopUp();

                    currentActivatedPopUpType = EnumSets.SLMPopUpType.UpdatingDBError;
                }
                break;
            case EnumSets.SLMPopUpType.SkipCurrentLecture:
                {
                    this.smartLearningModeUIManager.ActivateSkipCurrentLecturePopUp();

                    currentActivatedPopUpType = EnumSets.SLMPopUpType.SkipCurrentLecture;
                }
                break;
            case EnumSets.SLMPopUpType.AskingGoBackToHomeScene:
                {
                    this.smartLearningModeUIManager.ActivateAskingGoBackToHomeScenePopUp();

                    currentActivatedPopUpType = EnumSets.SLMPopUpType.AskingGoBackToHomeScene;
                }
                break;
            // ExitWebGL은 따로 관리
            //case EnumSets.SLMPopUpType.ExitWebGL:
            //    {
            //        this.smartLearningModeUIManager.ActivateExitWebGLPopUp();

            //        currentActivatedPopUpType = EnumSets.SLMPopUpType.ExitWebGL;
            //    }
            //    break;
            case EnumSets.SLMPopUpType.Settings:
                {
                    this.smartLearningModeUIManager.ActivateSettingsPopUp();

                    currentActivatedPopUpType = EnumSets.SLMPopUpType.Settings;

                    return;  // DarkBG 띄우지 않게끔
                }
            case EnumSets.SLMPopUpType.CommonError:
                {
                    this.smartLearningModeUIManager.ActivateCommonErrorPopUp(commonErrorMsg);

                    currentActivatedPopUpType = EnumSets.SLMPopUpType.CommonError;
                }
                break;
            case EnumSets.SLMPopUpType.Conversion:
                {
                    this.smartLearningModeUIManager.ActivateConversionFromGuestToLoginPopUp();

                    currentActivatedPopUpType = EnumSets.SLMPopUpType.Conversion;

                    return;
                }
                break;
        }

        this.smartLearningModeUIManager.ActivateCommonDarkBG();
    }

    public void DeActivatePopUp(EnumSets.SLMPopUpType popUpType)
    {
        switch (popUpType)
        {
            case EnumSets.SLMPopUpType.LoadPrefabError:
                {
                    this.smartLearningModeUIManager.DeActivateLoadingErrorPopUp();
                }
                break;
            case EnumSets.SLMPopUpType.UpdatingDBError:
                {
                    this.smartLearningModeUIManager.DeActivateUpdateDBErrorPopUp();
                }
                break;
            case EnumSets.SLMPopUpType.SkipCurrentLecture:
                {
                    this.smartLearningModeUIManager.DeActivateSkipCurrentLecturePopUp();
                }
                break;
            case EnumSets.SLMPopUpType.AskingGoBackToHomeScene:
                {
                    this.smartLearningModeUIManager.DeActivateAskingGoBackToHomeScenePopUp();
                }
                break;
            //case EnumSets.SLMPopUpType.ExitWebGL:
            //    {
            //        this.smartLearningModeUIManager.DeActivateExitWebGLPopUp();
            //    }
            //    break;
            case EnumSets.SLMPopUpType.Settings:
                {
                    this.smartLearningModeUIManager.DeActivateSettingsPopUp();
                }
                break;
            case EnumSets.SLMPopUpType.CommonError:
                {
                    this.smartLearningModeUIManager.DeActivateCommonErrorPopUp();

                    commonErrorMsg = "";
                }
                break;
            case EnumSets.SLMPopUpType.Conversion:
                {
                    this.smartLearningModeUIManager.DeActivateConversionFromGuestToLoginPopUp();

                    return;
                }
                break;
        }

        currentActivatedPopUpType = EnumSets.SLMPopUpType.None;

        this.smartLearningModeUIManager.DeActivateCommonDarkBG();
    }

    public void ActivateCommonErrorPopUp(EnumSets.ErrorType errorType)
    {
        commonErrorMsg = GetProperErrorMessage(errorType);

        ActivatePopUp(EnumSets.SLMPopUpType.CommonError);
    }

    public void DeActivateCurrentActivatedPopUp()
    {
        DeActivatePopUp(currentActivatedPopUpType);
    }

    private string GetProperErrorMessage(EnumSets.ErrorType errorType)
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
                    errorMsg = "보상 이메일 전송 후 데이터 초기화를 실패했습니다.";
                }
                break;
            case EnumSets.ErrorType.DrawError:
                {
                    errorMsg = "럭키 드로우 실패";
                }
                break;
            case EnumSets.ErrorType.AuthError:
                {
                    errorMsg = "사용자 인증 실패";
                }
                break;
            case EnumSets.ErrorType.DBUpdateError:
                {
                    errorMsg = "사용자 정보, 업데이트 실패";
                }
                break;
            case EnumSets.ErrorType.UnavailableError:
                {
                    errorMsg = "수행 중 오류가 발생했습니다.\n문의사항은 adobe@ethan-alice.com으로 연락주세요.";
                }
                break;
            case EnumSets.ErrorType.NotYetReadyToGetAGift:
                {
                    errorMsg = "아직 레슨을 완료하지않아서 선물을 받을 수 없습니다.";
                }
                break;
            case EnumSets.ErrorType.GuestSignInType:
                {
                    errorMsg = "손님으로 입장시 선물을 받을 수 없습니다. 회원가입 / 계정연동을 해주세요 ";
                }
                break;
        }

        return errorMsg;
    }
}
