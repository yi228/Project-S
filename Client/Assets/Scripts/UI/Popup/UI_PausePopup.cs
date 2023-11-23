using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_PausePopup : UI_Popup
{
    private MyPlayerController _myPlayer;
    enum Buttons
    {
        Button_GiveUp
    }
    public Image audioButton;
    public Image shakeButton;
    public Sprite[] ImageList;
    public MyPlayerController MyPlayer { get { return _myPlayer; } set { _myPlayer = value; } }

    private void Start()
    {
        if (Managers.Sound.SoundOn)
        {
            audioButton.sprite = ImageList[0];
        }
        else
        {
            audioButton.sprite = ImageList[1];
        }
        if (Managers.Sound.ShakeOn)
        {
            shakeButton.sprite = ImageList[2];
        }
        else
        {
            shakeButton.sprite = ImageList[3];
        }
        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.Button_GiveUp).onClick.AddListener(GiveUpButton);
    }

    public void ClosePopup()
    {
        Managers.Sound.Play("Effect/ClickButton");
        gameObject.SetActive(false);
    }

    public void AudioShift()
    {
        Managers.Sound.AudioShift();
        Managers.Sound.Play("Effect/ClickButton");
        if (Managers.Sound.SoundOn)
        {
            audioButton.sprite = ImageList[0];
        }
        else
        {
            audioButton.sprite = ImageList[1];
        }
    }
    public void GiveUpButton()
    {
        GetButton((int)Buttons.Button_GiveUp).interactable = false;
        C_LeaveGame leavePacket = new C_LeaveGame();
        leavePacket.LeaveGame = true;
        Managers.Network.Send(leavePacket);
        MyPlayer.CloseGameUI();
        Managers.Object.Remove(Managers.Game.MyPlayerId);
        Managers.Sound.Play("Effect/ClickButton");
        gameObject.SetActive(false);
        Managers.Scene.LoadScene("Lobby");
    }

    public void ShakeShift()
    {
        Managers.Sound.ShakeOn = !Managers.Sound.ShakeOn;

        if (Managers.Sound.ShakeOn)
        {
            shakeButton.sprite = ImageList[2];
        }
        else
        {
            shakeButton.sprite = ImageList[3];
        }
    }
}
