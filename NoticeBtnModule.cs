using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;

public class NoticeBtnModule : MonoBehaviour
{
    public TextMeshProUGUI title;

    public Image imageBtn;

    [Space]
    public Sprite spriteIdle;
    public Sprite spriteSelected;

    private string path = "";
    public string Path => this.path;

    private string titleStr = "";
    public string TitleStr => this.titleStr;

    private Action<NoticeBtnModule> onClickCallback = null;

    private Color idleColor = Color.white;
    private Color selectedColor = Color.blue;

    private Color idleBtnColor = Color.white;
    private Color selectedBtnColor = Color.black;

    private StringBuilder sb = new StringBuilder();

    private const string PREFIX_NOTICE_SHOW_URL = "https://www.beginner-game.com/notices/";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetTitle(string title)
    {
        this.titleStr = title;

        this.title.text = this.titleStr;
    }

    public void SetPath(string target)
    {
        this.path = GetFinalPath(target);
    }

    public void SetOnClickCallback(Action<NoticeBtnModule> action)
    {
        this.onClickCallback = action;
    }

    public void UpdateBtnImageColorToSelectedColor()
    {
        this.imageBtn.sprite = spriteSelected;

        // this.title.fontStyle = FontStyles.Bold;

        // this.imageBtn.color = selectedColor;

        this.title.color = selectedBtnColor;
    }

    public void UpdateBtnImageColorToIdleColor()
    {
        this.imageBtn.sprite = spriteIdle;

        // this.title.fontStyle = FontStyles.Normal;

        //this.imageBtn.color = idleColor;

        this.title.color = idleBtnColor;
    }

    public void OnClickShowNoticeBtn()
    {
        this.onClickCallback?.Invoke(this);
    }

    private string GetFinalPath(string target)
    {
        sb.Clear();

        sb.Append(PREFIX_NOTICE_SHOW_URL);
        sb.Append(target);

        return sb.ToString();
    }
}
