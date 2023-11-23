using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SingleUI_Pause : UI_Scene
{
    private GameObject settingUI;
    public GameObject SettingUI { get { return settingUI; } set { settingUI = value; } }

    public void OpenPausePopup()
    {
        Managers.Sound.Play("Effect/ClickButton");
        settingUI.SetActive(true);
    }
}
