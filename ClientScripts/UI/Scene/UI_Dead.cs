using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Google.Protobuf.Protocol;
using UnityEngine.SceneManagement;

public class UI_Dead : UI_Scene
{
    enum Buttons
    {
        Button_Dead
    }
    void Start()
    {
        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.Button_Dead).onClick.AddListener(GoToSampleScene);
    }
    public void OpenQuitPopup()
    {
        // TODO - 나중에 UI 수정하기
        Managers.Resource.InstantiateResources("Pause");
    }
    void InActiveQuitButton()
    {
        //GetButton((int)Buttons.Button_Dead).gameObject.SetActive(false);
        Application.Quit();
    }
    // TODO - 게임 방에 입장후 나갈 때 버튼
    private void GoToSampleScene()
    {
        GetButton((int)Buttons.Button_Dead).interactable = false;
        Managers.Sound.Play("Effect/ClickButton");
        C_LeaveGame leavePacket = new C_LeaveGame();
        leavePacket.LeaveGame = true;
        Managers.Network.Send(leavePacket);
        Managers.Object.Remove(Managers.Game.MyPlayerId);
        Managers.Scene.LoadScene("Lobby");
        // TODO - 테스트
        //SceneManager.LoadScene("SampleScene");
    }
}
