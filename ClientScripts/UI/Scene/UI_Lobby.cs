using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Lobby : UI_Scene
{
    // TODO - �ڵ�ȭ
    public TextMeshProUGUI lobbyPlayerCountText;
    enum Buttons
    {
        ExitButton,
    }
    enum TextsMeshPro
    {
        LobbyPlayerCountText,
    }
    float _timer = 0.1f;
    public float _delayTimer;
    bool _isGameStart = false;
    bool countFirst = true;
    void Start()
    {
        Bind<Button>(typeof(Buttons));
        //Bind<Text>(typeof(TextsMeshPro));
        GetButton((int)Buttons.ExitButton).GetComponent<Button>().onClick.AddListener(GoToSampleScene);
        // �������� ��Ŷ �ӵ� ���̷� ���� Ŭ����� ������ �ذ��� �� �ְ� �ð� ���� 3�� �α�
        GetButton((int)Buttons.ExitButton).interactable = false;
        //TODO - �ڵ�ȭ
        lobbyPlayerCountText = GameObject.Find("LobbyPlayerCountText").GetComponent<TextMeshProUGUI>(); 
        gameObject.SetActive(true);
    }
    void Update()
    {
        if (Managers.Game.MaxPlayer == Managers.Game.LobbyPlayer)
        {
            if (countFirst == true)
            {
                _delayTimer = Managers.Game.DelayBeforeStartGame;
                countFirst = false;
            }
            GetButton((int)Buttons.ExitButton).interactable = false;
            _delayTimer -= Time.deltaTime;
            lobbyPlayerCountText.text = "\n��Ƴ����ñ� �ٶ��ϴ�\n\n" + $"{(int)_delayTimer}�� �Ŀ� ���� ����";
        }
        else if (StartTrigger == false)
        {
            _timer += Time.deltaTime;
            if (_timer >= 3.0f)
            {
                GetButton((int)Buttons.ExitButton).interactable = true;
            }
            if (0 <= (int)_timer && (int)_timer <= 3)
            {
                lobbyPlayerCountText.text = "60�� ���� �������� ������ ���˴ϴ�\n���Ǳ� ���� �������!\n";

                string s = "��";
                if ((int)_timer == 1)
                    s = "��";
                else if ((int)_timer == 2)
                    s = "����";
                else if ((int)_timer == 3)
                    s = "������";
                lobbyPlayerCountText.text += s;
            }
            else if (3 < (int)_timer && (int)_timer <= 6)
            {
                lobbyPlayerCountText.text = "�ܾ�����, ������ ���� ��������\nĳ�����Դϴ�\n";

                string s = "��";
                if ((int)_timer == 4)
                    s = "��";
                else if ((int)_timer == 5)
                    s = "����";
                else if ((int)_timer == 6)
                    s = "������";
                lobbyPlayerCountText.text += s;
            }
            else if (6 < (int)_timer && (int)_timer <= 9)
            {
                lobbyPlayerCountText.text = "���ݼ����� ���� �ʰ� �� �� �־�!\n";

                string s = "��";
                if ((int)_timer == 7)
                    s = "��";
                else if ((int)_timer == 8)
                    s = "����";
                else if ((int)_timer == 9)
                    s = "������";
                lobbyPlayerCountText.text += s;
            }
            else if (9 < (int)_timer && (int)_timer <= 12)
            {
                lobbyPlayerCountText.text = "�� �� ���̸� �� - ��������\n";

                string s = "��";
                if ((int)_timer == 10)
                    s = "��";
                else if ((int)_timer == 11)
                    s = "����";
                else if ((int)_timer == 12)
                    s = "������";
                lobbyPlayerCountText.text += s;
            }
            else if (12 < (int)_timer && (int)_timer <= 15)
            {
                lobbyPlayerCountText.text = "������ 3���� ���ÿ� �����ϴ�!\n";

                string s = "��";
                if ((int)_timer == 13)
                    s = "��";
                else if ((int)_timer == 14)
                    s = "����";
                else if ((int)_timer == 15)
                    s = "������";
                lobbyPlayerCountText.text += s;
            }
            else if (15 < (int)_timer)
            {
                _timer = 0f;
            }
            lobbyPlayerCountText.text += $"\n������ �÷��̾�\n< {Managers.Game.LobbyPlayer} / {Managers.Game.MaxPlayer} >";
        }
    }

    // TODO - ���� �濡 ������ ���� �� ��ư
    private void GoToSampleScene()
    {
        Managers.Sound.Play("Effect/ClickButton");
        GetButton((int)Buttons.ExitButton).interactable = false;
        GetButton((int)Buttons.ExitButton).transform.Find("ExitButtonText").GetComponent<TextMeshProUGUI>().text = "�κ��\n�̵���";
        _timer = 0;
        C_LeaveGame leavePacket = new C_LeaveGame();
        leavePacket.LeaveGame = true;
        Managers.Network.Send(leavePacket);
        Managers.Object.Remove(Managers.Game.MyPlayerId);
        Managers.Scene.LoadScene("Lobby");
    }
    public bool StartTrigger = false;
    public void StartGame()
    {
        StartCoroutine(CoStartGame());
    }
    private IEnumerator CoStartGame()
    {
        _isGameStart = true;
        StartTrigger = true;
        //yield return new WaitForSeconds(Managers.Game.DelayBeforeStartGame);
        Managers.Game.IsStartGame = true;
        gameObject.SetActive(false);
        yield return null;
    }
    //private void GoToStartScene()
    //{
    //    Managers.Sound.Play("Effect/ClickButton");
    //    Managers.Scene.LoadScene("StartMenu");
    //}
}
