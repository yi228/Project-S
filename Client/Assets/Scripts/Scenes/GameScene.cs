using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    GameObject SettingUI;
    protected override void Init()
    {
        base.Init();
        Managers.Scene.CurrentScene = Define.Scene.Game;

        Managers.Map.LoadMap(1);
    }
    private void Start()
    {
        AudioSync();
    }
    void Update()
    {
        if (SettingUI == null && Managers.Game.MyPlayerId != 0)
        {
            SettingUI = Managers.Object.FindById(Managers.Game.MyPlayerId).GetComponent<MyPlayerController>().SettingUI;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SettingUI.SetActive(true);
        }
    }
}
