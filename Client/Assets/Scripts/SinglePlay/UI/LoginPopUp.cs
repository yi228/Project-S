using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Google.Protobuf.Protocol;
public class LoginPopUp : MonoBehaviour
{
    public bool isClick = false;
    FirebaseAuth auth;
    string log;
    bool isLogin;
    private GameObject LoginButton, PlayButton, CloseButton, UserInfo_Text;
    void Start()
    {
        LoginButton = GameObject.Find("Login Button");
        PlayButton = GameObject.Find("Play Button");
        CloseButton = GameObject.Find("Close Button");
        UserInfo_Text = GameObject.Find("Text_UserInfo");

        //PlayButton.GetComponent<Button>().interactable = false; //로그인 전에는 플레이 불가능
        //PlayButton.GetComponent<Button>().onClick.AddListener(GoToMultiPlayMode);
        //LoginButton.GetComponent<Button>().onClick.AddListener(GoogleLogin);
        if (CloseButton != null)
            CloseButton.GetComponent<Button>().onClick.AddListener(ClosePopup);
        ////구글 로그인
        //PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        //    .RequestIdToken()
        //    .Build();
        //PlayGamesPlatform.InitializeInstance(config);
        //PlayGamesPlatform.Activate();

        ////파이어베이스
        //auth = FirebaseAuth.DefaultInstance;

    }
    private void ClosePopup()
    {
        gameObject.SetActive(false);
    }
    public void GoToMultiPlayMode()
    {
        if (isClick == false)
        {
            isClick = true;
            Managers.Scene.LoadScene("Game");
            // 여기서 대기화면 띄워주기
            C_EnterRoom enterRoomPacket = new C_EnterRoom();
            enterRoomPacket.CanEnter = false;
            Managers.Network.Send(enterRoomPacket);
        }
        else
        {
            Debug.Log("이미 눌렀어");
        }
    }
    //private void login()
    //{
    //    GPGSBinder.Inst.Login((success, localUser) =>
    //    log = $" Login State:{success}\nUserName:{localUser.userName}\nUser ID:{localUser.id}");
    //    //log = $"{success}, {localUser.userName}, {localUser.id}, {localUser.state}, {localUser.underage}");
    //    if (log.Contains("True")) //로그인 성공시
    //    {
    //        PlayButton.GetComponent<Button>().interactable = true; //멀티플레이 입장가능
    //    }
    //    UserInfo_Text.GetComponent<TextMeshProUGUI>().text = log;
    //}
    //public void GoogleLogin()
    //{
    //    Social.localUser.Authenticate(success =>
    //    {
    //        if (success)
    //        {
    //            //파이어베이스 로그인
    //            string idToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
    //            log = "d";
    //            UserInfo_Text.GetComponent<TextMeshProUGUI>().text = log;
    //            Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(idToken, null);
    //            log = idToken;
    //            UserInfo_Text.GetComponent<TextMeshProUGUI>().text = log;
    //            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
    //            {
    //                if (task.IsCanceled)
    //                {
    //                    print("cancel");
    //                    log = "cancel";
    //                    UserInfo_Text.GetComponent<TextMeshProUGUI>().text = log;
    //                    return;
    //                }
    //                if (task.IsFaulted)
    //                {
    //                    print("fault");
    //                    log = "fault";
    //                    UserInfo_Text.GetComponent<TextMeshProUGUI>().text = log;
    //                    return;
    //                }
    //                log = "success";
    //                PlayButton.GetComponent<Button>().interactable = true;
    //                UserInfo_Text.GetComponent<TextMeshProUGUI>().text = log;
    //            });

    //        }
    //        else
    //        {

    //        }
    //    }
    //    );
    //}
    //public void Firebaselogin()
    //{

    //    string idToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
    //    log = "d";
    //    UserInfo_Text.GetComponent<TextMeshProUGUI>().text = log;
    //    Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(idToken, null);
    //    log = idToken;
    //    UserInfo_Text.GetComponent<TextMeshProUGUI>().text = log;
    //    auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
    //    {
    //        if (task.IsCanceled)
    //        {
    //            print("cancel");
    //            log = "cancel";
    //            UserInfo_Text.GetComponent<TextMeshProUGUI>().text = log;
    //            return;
    //        }
    //        if (task.IsFaulted)
    //        {
    //            print("fault");
    //            log = "fault";
    //            UserInfo_Text.GetComponent<TextMeshProUGUI>().text = log;
    //            return;
    //        }
    //        log = "success";

    //        UserInfo_Text.GetComponent<TextMeshProUGUI>().text = log;
    //    });
    //}
}