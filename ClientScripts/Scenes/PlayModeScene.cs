using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Google.Protobuf.Protocol;

public class PlayModeScene : BaseScene
{
    // Start is called before the first frame update
    private GameObject SinglePlayButton, MultiPlayButton, BackButton, LoginPopup;
    public int playerCount = 0;
    // ��Ŷ �� �� ������ �� ���� - key �ߺ� �̽� �Ͼ
    public bool isClick = false;
    protected override void Init()
    {
        base.Init();
        Managers.Scene.CurrentScene = Define.Scene.Lobby;
    }
    void Start()
    {
        SinglePlayButton = GameObject.Find("SinglePlay Button");
        MultiPlayButton = GameObject.Find("MultiPlay Button");
        BackButton = GameObject.Find("Back Button");
        SinglePlayButton.GetComponent<Button>().onClick.AddListener(GoToSinglePlayMode);
        MultiPlayButton.GetComponent<Button>().onClick.AddListener(GoToMultiGameScene);
        BackButton.GetComponent<Button>().onClick.AddListener(GoToLobbyScene);
        AudioSync();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Managers.Scene.LoadScene("Lobby");
        }
    }

    private void GoToLobbyScene()
    {
        BackButton.GetComponent<Button>().interactable = false;
        Managers.Sound.Play("Effect/ClickButton");
        Managers.Scene.LoadScene("Lobby");
    }

    private void GoToSinglePlayMode()
    {
        Managers.Sound.Play("Effect/ClickButton");
        Managers.Scene.LoadScene("StageSelect");
    }
    public void GoToMultiGameScene()
    {
        Managers.Sound.Play("Effect/ClickButton");
        if (isClick == false)
        {
            isClick = true;
            //Managers.Scene.LoadScene("Game");
            //UI_LoadingScene.Instance.LoadScene("Game");
            // TODO - ���⼭ ���ȭ�� ����ֱ�
            MultiPlayButton.GetComponent<Button>().interactable = false;
            C_EnterRoom enterRoomPacket = new C_EnterRoom();
            enterRoomPacket.CanEnter = false;
            enterRoomPacket.WeaponType = Managers.Game.MyPlayerWeaponType;
            Managers.Network.Send(enterRoomPacket);
            SceneManager.LoadScene("Game");
        }
        else
        {
            Debug.Log("�̹� ������");
        }
    }
}
