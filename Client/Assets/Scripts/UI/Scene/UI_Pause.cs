using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UI_Pause : UI_Scene
{
    [SerializeField] private GameObject SettingUI;
    void Start()
    {
        SettingUI = Managers.Object.FindById(Managers.Game.MyPlayerId).GetComponent<MyPlayerController>().SettingUI;
    }
    public void OpenPausePopup()
    {
        Managers.Sound.Play("Effect/ClickButton");
        SettingUI.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        if (SettingUI == null)
            SettingUI = Managers.Object.FindById(Managers.Game.MyPlayerId).GetComponent<MyPlayerController>().SettingUI;
    }
}
