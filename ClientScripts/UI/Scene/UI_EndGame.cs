using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_EndGame : UI_Scene
{
    enum Buttons
    {
        ExitButton,
    }
    void Start()
    {
        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.ExitButton).GetComponent<Button>().onClick.AddListener(GoToSampleScene);
    }

    void Update()
    {

    }
    // TODO - 게임 방에 입장후 나갈 때 버튼
    private void GoToSampleScene()
    {
        GetButton((int)Buttons.ExitButton).interactable = false;
        Managers.Sound.Play("Effect/ClickButton");
        C_LeaveGame leavePacket = new C_LeaveGame();
        leavePacket.LeaveGame = true;
        Managers.Network.Send(leavePacket);
        Managers.Object.Remove(Managers.Game.MyPlayerId);
        Managers.Scene.LoadScene("Lobby");
    }
}
