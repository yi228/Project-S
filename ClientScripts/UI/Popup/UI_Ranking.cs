using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using Firebase;
using Firebase.Database;
using System;
using TMPro;
//using UnityEngine.AddressableAssets;
public class UI_Ranking : UI_Scene
{
    public Button Button_Back;
    DatabaseReference reference;
    public TextMeshProUGUI[] Nickname, Score;
    public Image[] characters,characterImages;
    int rankingCount = 10;
    public int myDiamond, myWincount, myplaycount,myBlockBuffCount,myAttackBuffCount,mySpeedBuffCount,mySightBuffCount;
    public int character;
    public struct UserData
    {
        public string userID;
        public string userNickname;
        public int Diamond;
        public int playCount;
        public int winCount;
        public int profileImage;
        public int BlockBuffCount;
        public int AttackBuffCount;
        public int SpeedBuffCount;
        public int SightBuffCount;

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
    public List<UserData> rankList;
    // Start is called before the first frame update
    void Start()
    {
        FirebaseDatabase.DefaultInstance.GoOffline();
        FirebaseDatabase.DefaultInstance.PurgeOutstandingWrites();
        FirebaseDatabase.DefaultInstance.GoOnline();
        Button_Back = GameObject.Find("Button_Back").GetComponent<Button>();
        Button_Back.onClick.AddListener(ClosePopup);
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        rankList = new List<UserData>();
        ReadDB();
        //SortScores();
        //PrintRanking();

    }
    private void Update()
    {
        sortRanking();
        
    }
    private void sortRanking()
    {
        for (int i = 0; i < rankList.Count; i++)
        {
            if (i < 10)
            {
                Nickname[i + 1].text = rankList[i].userNickname;
                Score[i + 1].text = rankList[i].winCount.ToString();
                characters[i + 1].sprite = characterImages[rankList[i].profileImage].sprite;
            }
            if (rankList[i].userNickname == PlayerPrefs.GetString("userName"))
            {
                myDiamond = rankList[i].Diamond;
                myWincount = rankList[i].winCount;
                myplaycount = rankList[i].playCount;
                Nickname[0].text = PlayerPrefs.GetString("userName");
                Score[0].text = rankList[i].winCount.ToString();
                characters[0].sprite = characterImages[rankList[i].profileImage].sprite;
            }
        }
        print("랭킹 업데이트중");
    }
    private void RenderRankingName()
    {
    }
    private void ClosePopup()
    {
        Managers.Sound.Play("Effect/ClickButton");
        Managers.Game.ChooseCharacter.SetActive(true);
        gameObject.SetActive(false);
    }
    public void selectCharacter(int num)
    {
        character = num;
    }
    public void UploadProfileImage()
    {
        PlayerPrefs.SetInt("ProfileImage", character);
        UserData data = new UserData(PlayerPrefs.GetString("id"), PlayerPrefs.GetString("userName"), myDiamond, myplaycount, myWincount, character,myBlockBuffCount,myAttackBuffCount,mySpeedBuffCount,mySightBuffCount);
        string jsonData = JsonUtility.ToJson(data);
        FirebaseDatabase.DefaultInstance.GetReference(PlayerPrefs.GetString("id")).SetRawJsonValueAsync(jsonData);
        print("프로필 변경 완료"+character);
        ReadDB();
        //sortRanking();
    }
    public void ReadDB()
    {
        print("디비 읽기시작");
        reference.GetValueAsync().ContinueWith(task =>
        {
            if (true)//task.IsCompleted
            {
                rankList.Clear();
                DataSnapshot snapshot = task.Result;

                foreach (var data in snapshot.Children)
                {
                   
                    if (data.Child("userID").Value != null)
                    {
                        string Id = data.Child("userID").Value.ToString();
                        string Nickname = data.Child("userNickname").Value.ToString();
                        int Diamond = Int32.Parse(data.Child("Diamond").Value.ToString());
                        int Playcount = Int32.Parse(data.Child("playCount").Value.ToString());
                        int Wincount = Int32.Parse(data.Child("winCount").Value.ToString());
                        int ProfileImage = Int32.Parse(data.Child("profileImage").Value.ToString());
                        int BlockBuffCount = Int32.Parse(data.Child("BlockBuffCount").Value.ToString());
                        int AttackBuffCount = Int32.Parse(data.Child("AttackBuffCount").Value.ToString());
                        int SpeedBuffCount = Int32.Parse(data.Child("SpeedBuffCount").Value.ToString());
                        int SightBuffCount = Int32.Parse(data.Child("SightBuffCount").Value.ToString());
                        UserData user = new UserData();
                        user.userID = Id;
                        user.userNickname = Nickname;
                        user.Diamond = Diamond;
                        user.playCount = Playcount;
                        user.winCount = Wincount;
                        user.profileImage = ProfileImage;
                        user.BlockBuffCount = BlockBuffCount;
                        user.AttackBuffCount = AttackBuffCount;
                        user.SpeedBuffCount = SpeedBuffCount;
                        user.SightBuffCount = SightBuffCount;
                        rankList.Add(user);
                        print("유저 아이디:" + user.userID+ " 유저 닉네임:" + user.userNickname + " 다이아몬드:" + data.Child("Diamond").Value + " 플레이 횟수:" + data.Child("playCount").Value + " 승리 횟수:" + data.Child("winCount").Value);
                    }
                }               
            }
            rankList.Sort((x, y) => y.winCount.CompareTo(x.winCount)); //정렬하기 
            
            for (int i = 0; i < rankList.Count; i++) 
            {
                print(": 이름 - " + rankList[i].userNickname + ", winCount -" + rankList[i].winCount); //리스트에는 잘 정렬되어서 들어감
            }
            

        });//여기 밖으로 나가면 rankList가 초기화 되는 문제때문에 안에서실행
        

    }
    //public void SortScores() //오름차순 정리
    //{
    //    rankList.Sort((x, y) => y.Diamond.CompareTo(x.Diamond));
    //}
    //public void PrintRanking() //랭킹 표시
    //{
    //    Debug.Log("Current Ranking Count:"+rankList.Count);

    //    for (int i = 0; i < rankList.Count; i++)
    //    {
    //        print("Rank " + (i + 1) + ": 이름 - " + rankList[i].userNickname);
    //    }
    //}
}
