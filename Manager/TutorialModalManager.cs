using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class TutorialModalManager : MonoBehaviour
{
    public GameObject modalTutorial;

    public int version; // 차후 사용법이 바뀔 것을 감안해서 버전정보를 playerprefs 접미로 붙이기

    [SerializeField]
    private bool isNoMoreSeeState = false;

    private StringBuilder sb = new StringBuilder();

    private const string PREFIX_STR = "tutorialModalNoMoreSee_";

    private void Awake()
    {
        Init();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Init()
    {
        CheckNoMoreState();
    }

    private void CheckNoMoreState()
    {
        var uid = UserManager.Instance.GetUID();

        var tmpIsNoMoreSeeState = PlayerPrefs.GetInt(GetPrefsKey(uid), 0);

        if (tmpIsNoMoreSeeState == 1)
        {
            this.isNoMoreSeeState = true;
        }
    }

    private string GetPrefsKey(string uid)
    {
        sb.Clear();

        sb.Append(PREFIX_STR);
        sb.Append(uid);
        sb.Append(version);

        return sb.ToString();
    }

    public void ActivateModal()
    {
        if(this.isNoMoreSeeState)
        {
            return;
        }

        modalTutorial.SetActive(true);
    }

    public void DeActivateModal()
    {
        modalTutorial.SetActive(false);

        if(this.isNoMoreSeeState)
        {
            var uid = UserManager.Instance.GetUID();

            PlayerPrefs.SetInt(GetPrefsKey(uid), 1);
            PlayerPrefs.Save();
        }
    }

    // 토글에서 참조 중
    public void SetNoMoreSeeState(bool noMoreSee)
    {
        this.isNoMoreSeeState = noMoreSee;
    }
}
