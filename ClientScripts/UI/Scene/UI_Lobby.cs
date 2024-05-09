using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Lobby : UI_Scene
{
    // TODO - 자동화
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
        // 서버에서 패킷 속도 차이로 인한 클라와의 갭차이 해결할 수 있게 시간 차이 3초 두기
        GetButton((int)Buttons.ExitButton).interactable = false;
        //TODO - 자동화
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
            lobbyPlayerCountText.text = "\n살아남으시길 바랍니다\n\n" + $"{(int)_delayTimer}초 후에 게임 시작";
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
                lobbyPlayerCountText.text = "60초 마다 랜덤으로 지역이 폐쇄됩니다\n폐쇄되기 전에 벗어나세요!\n";

                string s = "ㆍ";
                if ((int)_timer == 1)
                    s = "ㆍ";
                else if ((int)_timer == 2)
                    s = "ㆍㆍ";
                else if ((int)_timer == 3)
                    s = "ㆍㆍㆍ";
                lobbyPlayerCountText.text += s;
            }
            else if (3 < (int)_timer && (int)_timer <= 6)
            {
                lobbyPlayerCountText.text = "단언컨대, 권총은 가장 균형잡힌\n캐릭터입니다\n";

                string s = "ㆍ";
                if ((int)_timer == 4)
                    s = "ㆍ";
                else if ((int)_timer == 5)
                    s = "ㆍㆍ";
                else if ((int)_timer == 6)
                    s = "ㆍㆍㆍ";
                lobbyPlayerCountText.text += s;
            }
            else if (6 < (int)_timer && (int)_timer <= 9)
            {
                lobbyPlayerCountText.text = "돌격소총은 쉬지 않고 쏠 수 있어!\n";

                string s = "ㆍ";
                if ((int)_timer == 7)
                    s = "ㆍ";
                else if ((int)_timer == 8)
                    s = "ㆍㆍ";
                else if ((int)_timer == 9)
                    s = "ㆍㆍㆍ";
                lobbyPlayerCountText.text += s;
            }
            else if (9 < (int)_timer && (int)_timer <= 12)
            {
                lobbyPlayerCountText.text = "난 한 발이면 돼 - 스나이퍼\n";

                string s = "ㆍ";
                if ((int)_timer == 10)
                    s = "ㆍ";
                else if ((int)_timer == 11)
                    s = "ㆍㆍ";
                else if ((int)_timer == 12)
                    s = "ㆍㆍㆍ";
                lobbyPlayerCountText.text += s;
            }
            else if (12 < (int)_timer && (int)_timer <= 15)
            {
                lobbyPlayerCountText.text = "샷건은 3발이 동시에 나갑니다!\n";

                string s = "ㆍ";
                if ((int)_timer == 13)
                    s = "ㆍ";
                else if ((int)_timer == 14)
                    s = "ㆍㆍ";
                else if ((int)_timer == 15)
                    s = "ㆍㆍㆍ";
                lobbyPlayerCountText.text += s;
            }
            else if (15 < (int)_timer)
            {
                _timer = 0f;
            }
            lobbyPlayerCountText.text += $"\n접속한 플레이어\n< {Managers.Game.LobbyPlayer} / {Managers.Game.MaxPlayer} >";
        }
    }

    // TODO - 게임 방에 입장후 나갈 때 버튼
    private void GoToSampleScene()
    {
        Managers.Sound.Play("Effect/ClickButton");
        GetButton((int)Buttons.ExitButton).interactable = false;
        GetButton((int)Buttons.ExitButton).transform.Find("ExitButtonText").GetComponent<TextMeshProUGUI>().text = "로비로\n이동중";
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
