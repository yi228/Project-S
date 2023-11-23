using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using System;
using TMPro;
using UnityEngine.AddressableAssets;
public class LobbyScene : BaseScene
{
    private GameObject BattleButton;
    // 임시용 나중에 삭제 요망
    public Button testButton, rifleButton, sniperButton, shotgunButton;
    public Button buttonBuffManual, buttonPlayerManual , rankingButton, shopButton;
    public GameObject rifleLock, sniperLock, shotgunLock, loading_prefab,loading;
    public TextMeshProUGUI userName,diamond,winCount;
    private string DBurl = "https://projects-22121451-default-rtdb.firebaseio.com";
    DatabaseReference reference;
    private string Id, Nickname;
    private int Diamond, Playcount, Wincount,ProfileImage, BlockBuffCount, AttackBuffCount, SpeedBuffCount, SightBuffCount;
    private bool isFirstuser=false;
    private int quitCount = 0;
    private UI_SystemTextViewer systemTextViewer; 
    public UI_PlayerManual UI_PlayerManual;
    public UI_BuffManual UI_BuffManual;
    public UI_Ranking UI_Ranking;
    public UI_Shop UI_Shop;
    protected override void Init()
    {
        base.Init();
        Managers.Scene.CurrentScene = Define.Scene.Lobby;

        //테스트용
        //PlayerPrefs.SetString("userName", "Kevin");
        //PlayerPrefs.SetString("id", "ljykevin4");
        if (SceneManager.GetActiveScene().name == "SampleScene")
            return;
        if (PlayerPrefs.GetString("id") == "")
        {
            PlayerPrefs.SetString("userName", "tester");
            PlayerPrefs.SetString("id", "tester");
            print("테스트 계정 진입");
        }

        //초기 세팅
        loading = Instantiate(loading_prefab);
        rifleButton = GameObject.Find("RifleButton").GetComponent<Button>();
        sniperButton = GameObject.Find("SniperButton").GetComponent<Button>();
        shotgunButton = GameObject.Find("ShotgunButton").GetComponent<Button>();
        rifleLock = GameObject.Find("RifleLock");
        sniperLock = GameObject.Find("SniperLock");
        shotgunLock = GameObject.Find("ShotgunLock");
        userName = GameObject.Find("Text_Name").GetComponent<TextMeshProUGUI>();
        userName.text = PlayerPrefs.GetString("userName");
        diamond = GameObject.Find("Text_Diamond").GetComponent<TextMeshProUGUI>();
        winCount = GameObject.Find("Wincount").GetComponent<TextMeshProUGUI>();
        BattleButton = GameObject.Find("Button_Battle");
        systemTextViewer = Managers.Resource.InstantiateResources("UI_SystemTextViewer").GetComponent<UI_SystemTextViewer>();
        
        GameObject buffManual = Managers.Resource.InstantiateResources("UI_BuffManual");
        GameObject playerManual = Managers.Resource.InstantiateResources("UI_PlayerManual");
        GameObject ranking = Managers.Resource.InstantiateResources("UI_Ranking");
        // TODO - SHOP
        //GameObject shop = Managers.Resource.InstantiateResources("UI_Shop");
        GameObject shop = Managers.Resource.Instantiate("UI/Popup/UI_Shop");

        UI_BuffManual = buffManual.GetComponent<UI_BuffManual>();
        UI_PlayerManual = playerManual.GetComponent<UI_PlayerManual>();
        UI_Ranking = ranking.GetComponent<UI_Ranking>();
        UI_Shop = shop.GetComponent<UI_Shop>();

        UI_BuffManual.gameObject.SetActive(false);
        UI_PlayerManual.gameObject.SetActive(false);
        UI_Ranking.gameObject.SetActive(false);
        UI_Shop.gameObject.SetActive(false);

        buttonBuffManual = GameObject.Find("Button_BuffManual").GetComponent<Button>();
        buttonPlayerManual = GameObject.Find("Button_PlayerManual").GetComponent<Button>();
        rankingButton = GameObject.Find("Button_Ranking").GetComponent<Button>();
        shopButton = GameObject.Find("Button_Shop").GetComponent<Button>();

        buttonBuffManual.onClick.AddListener(OpenBuffManual);
        buttonPlayerManual.onClick.AddListener(OpenPlayerManual);
        rankingButton.onClick.AddListener(OpenRanking);
        shopButton.onClick.AddListener(OpenShop);

        if (BattleButton != null)
            BattleButton.GetComponent<Button>().onClick.AddListener(GoToPlayModeScene);
        if (testButton != null)
            testButton.interactable = true;

        //DB세팅
        FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);//캐시 비활성화
        reference = FirebaseDatabase.DefaultInstance.GetReference(PlayerPrefs.GetString("id"));
        ReadDB();
        Invoke("WriteDB",3f);

        //FirebaseApp.DefaultInstance.Options.DatabaseUrl = new Uri(DBurl); 
        SingleGameManager.isGameOver = false;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "SampleScene")
            return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            quitCount++;
            if (quitCount == 1)
            {
                systemTextViewer.PrintQuitText();
            }
            else if (quitCount == 2)
            {
                Application.Quit();
            }
        }
        diamond.text = Diamond.ToString();
        winCount.text = Wincount.ToString();
        if (Diamond >= 10)
        {
            rifleButton.interactable = true;
            sniperButton.interactable = true;
            shotgunButton.interactable = true;
            rifleLock.SetActive(false);
            sniperLock.SetActive(false);
            shotgunLock.SetActive(false);
        }
        else if(Diamond >= 5)
        {
            rifleButton.interactable = true;
            sniperButton.interactable = true;
            rifleLock.SetActive(false);
            sniperLock.SetActive(false);
        }
        else if (Diamond >= 1)
        {
            rifleButton.interactable = true;
            rifleLock.SetActive(false);
        }
    }
    private void GoToPlayModeScene()
    {
        BattleButton.GetComponent<Button>().interactable = false;
        Managers.Sound.Play("Effect/ClickButton");
        HideLobbyCharacter();
        Managers.Scene.LoadScene("PlayMode");
    }
    public void DiamondAdd()
    {
        Diamond++; //다이아몬드 추가
        UserData firstUser = new UserData(PlayerPrefs.GetString("id"), PlayerPrefs.GetString("userName"), Diamond, Playcount, Wincount, ProfileImage, BlockBuffCount, AttackBuffCount, SpeedBuffCount, SightBuffCount);
        string jsonFirstUser = JsonUtility.ToJson(firstUser);
        reference.SetRawJsonValueAsync(jsonFirstUser);
        print("다이아몬드 추가 완료");
    }

    public void WriteDB() 
    {

        if (isFirstuser == true)
        {
            print("초기유저 입니다.");
            UserData firstUser = new UserData(PlayerPrefs.GetString("id"), PlayerPrefs.GetString("userName"), 0, 0, 0, 0,0,0,0,0);
            string jsonFirstUser = JsonUtility.ToJson(firstUser);
            reference.SetRawJsonValueAsync(jsonFirstUser);
            print("초기데이터 업로드 완료");
            PlayerPrefs.SetInt("Diamond", Diamond);
            PlayerPrefs.SetInt("Playcount", Playcount);
            PlayerPrefs.SetInt("Wincount", Wincount);
            PlayerPrefs.SetInt("ProfileImage", ProfileImage);
            PlayerPrefs.SetInt("BlockBuffCount", BlockBuffCount);
            PlayerPrefs.SetInt("AttackBuffCount", AttackBuffCount);
            PlayerPrefs.SetInt("SpeedBuffCount", SpeedBuffCount);
            PlayerPrefs.SetInt("SightBuffCount", SightBuffCount);
        }
        else
        {
            print("초기유저가 아닙니다.");
        }
        Destroy(loading);
        print("로딩 사라짐");
    }

    public void ReadDB()
    {
        Diamond = 0;
        Playcount = 0;
        Wincount = 0;
        ProfileImage = 0;
        BlockBuffCount = 0;
        AttackBuffCount = 0;
        SpeedBuffCount = 0;
        SightBuffCount = 0;

        

        reference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (task.Result.Value != null) { 
               
                    print("유저 아이디:" + snapshot.Child("userID").Value + " 유저 닉네임:" + snapshot.Child("userNickname").Value + " 다이아몬드:" + snapshot.Child("Diamond").Value + " 플레이 횟수:" + snapshot.Child("playCount").Value + " 승리 횟수:" + snapshot.Child("winCount").Value+"프로필 이미지:"+snapshot.Child("profileImage").Value);
                    Id = snapshot.Child("userID").Value.ToString();
                    Nickname = snapshot.Child("userNickname").Value.ToString();
                    Diamond = Int32.Parse(snapshot.Child("Diamond").Value.ToString());
                    Playcount = Int32.Parse(snapshot.Child("playCount").Value.ToString());
                    Wincount = Int32.Parse(snapshot.Child("winCount").Value.ToString());
                    ProfileImage= Int32.Parse(snapshot.Child("profileImage").Value.ToString());
                    BlockBuffCount = Int32.Parse(snapshot.Child("BlockBuffCount").Value.ToString());
                    AttackBuffCount = Int32.Parse(snapshot.Child("AttackBuffCount").Value.ToString());
                    SpeedBuffCount = Int32.Parse(snapshot.Child("SpeedBuffCount").Value.ToString());
                    SightBuffCount = Int32.Parse(snapshot.Child("SightBuffCount").Value.ToString());
                    PlayerPrefs.SetInt("Diamond", Diamond);
                    PlayerPrefs.SetInt("Playcount", Playcount);
                    PlayerPrefs.SetInt("Wincount", Wincount);
                    PlayerPrefs.SetInt("ProfileImage", ProfileImage);
                    PlayerPrefs.SetInt("BlockBuffCount", BlockBuffCount);
                    PlayerPrefs.SetInt("AttackBuffCount", AttackBuffCount);
                    PlayerPrefs.SetInt("SpeedBuffCount", SpeedBuffCount);
                    PlayerPrefs.SetInt("SightBuffCount", SightBuffCount);
                    isFirstuser = false;

                }
                else
                {
                    isFirstuser = true;
                }
            }
            
        });
    }
    
    public class UserData
    {
        public string userID = "";
        public string userNickname = "";
        public int Diamond = 0;
        public int playCount = 0;
        public int winCount = 0;
        public int profileImage = 0;
        public int BlockBuffCount = 0;
        public int AttackBuffCount = 0;
        public int SpeedBuffCount = 0;
        public int SightBuffCount = 0;

        public UserData(string USERID, string USERNICKNAME, int DIAMOND, int PLAYCOUNT, int WINCOUNT, int PROFILEIMAGE, int BLOCKBUFFCOUNT, int ATTACKBUFFCOUNT, int SPEEDBUFFCOUNT, int SIGHTBUFFCOUNT)
        {
            userID = USERID;
            userNickname = USERNICKNAME;
            Diamond = DIAMOND;
            playCount = PLAYCOUNT;
            winCount = WINCOUNT;
            profileImage = PROFILEIMAGE;
            BlockBuffCount = BLOCKBUFFCOUNT;
            AttackBuffCount = ATTACKBUFFCOUNT;
            SpeedBuffCount = SPEEDBUFFCOUNT;
            SightBuffCount = SIGHTBUFFCOUNT;
        }
    }
    public void PopUpSetting()
    {
        Managers.Sound.Play("Effect/ClickButton");
        Managers.Resource.InstantiateResources("LobbySetting");
        HideLobbyCharacter();
    }
    private void OpenRanking()
    {
        Managers.Sound.Play("Effect/ClickButton");
        HideLobbyCharacter();
        UI_Ranking.gameObject.SetActive(true);
    }
    private void OpenPlayerManual()
    {
        Managers.Sound.Play("Effect/ClickButton");
        HideLobbyCharacter();
        UI_PlayerManual.gameObject.SetActive(true);
    }
    private void OpenBuffManual()
    {
        Managers.Sound.Play("Effect/ClickButton");
        HideLobbyCharacter();
        UI_BuffManual.gameObject.SetActive(true);
    }
    private void OpenShop()
    {
        Managers.Sound.Play("Effect/ClickButton");
        HideLobbyCharacter();
        UI_Shop.gameObject.SetActive(true);
    }
    private void HideLobbyCharacter()
    {
        GameObject g1 = GameObject.Find("ChemicalmanDefault(Clone)");
        GameObject g2 = GameObject.Find("TerroristDefault(Clone)");
        GameObject g3 = GameObject.Find("SniperDefault(Clone)");
        GameObject g4 = GameObject.Find("CowboyDefault(Clone)");
        if (g1 != null)
        {
            Managers.Game.ChooseCharacter = g1;
            g1.SetActive(false);
        }
        else if (g2 != null)
        {
            Managers.Game.ChooseCharacter = g2;
            g2.SetActive(false);
        }
        else if (g3 != null)
        {
            Managers.Game.ChooseCharacter = g3;
            g3.SetActive(false);
        }
        else if (g4 != null)
        {
            Managers.Game.ChooseCharacter = g4;
            g4.SetActive(false);
        }
    }
}
