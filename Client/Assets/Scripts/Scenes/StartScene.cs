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

public class StartScene : UI_Scene
{
    enum Sliders 
    {
        PrefabLoadSlider,
    }
    enum Buttons
    {
        StartButton,
    }
    enum Texts
    {
        StartText,
    }
    public bool isClick = false;
    FirebaseAuth auth;
    string log;
    bool isLogin;
    private Slider prefabLoadSlider;
    private Button startButton;
    private TextMeshProUGUI startText;

    void Start()
    {
        Bind<Slider>(typeof(Sliders));
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        prefabLoadSlider = GetSlider((int)Sliders.PrefabLoadSlider);
        // slider 초기값 세팅
        prefabLoadSlider.value = 0;

        startButton = GetButton((int)Buttons.StartButton);
        startText = GetTextMeshProUGUI((int)Texts.StartText);
        startText.text = "게임 리소스를 다운로드 중 입니다";
        // TODO - 모바일 3개
        //구글 로그인

#if UNITY_ANDROID
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .RequestIdToken()
            .Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
#endif
        //파이어베이스
        auth = FirebaseAuth.DefaultInstance;
        // 게임에서 쓸 리소스 Addressable로 불러오기
        LoadAllAsync();
    }

    void Update()
    {

    }

    private void LoadAllAsync()
    {
        Managers.Resource.LoadAllAsync<GameObject>("Prefab", (key, count, totalCount) =>
        {
            // 로딩바 사용 가능
            Debug.Log($"{key} {count}/{totalCount}");
            float progressRate = (float)count / (float)totalCount;
            prefabLoadSlider.value = progressRate;
            if (count == totalCount)
            {
                // 다 완료되고 나서 실행할 함수
                Debug.Log("Load All Prefabs");
                prefabLoadSlider.value = 1f;
                prefabLoadSlider.gameObject.SetActive(false);
                startText.text = "터치하면 게임을 시작해요!";
                // TODO
                Object loadingScene = null;
                if (Managers.Resource.Res.TryGetValue("LoadingScene", out loadingScene) == false)
                {
                    loadingScene = Managers.Resource.Load<GameObject>("UI/Scene/LoadingScene");
                    Managers.Resource.Res.Add("LoadingScene", loadingScene);
                    Debug.Log("Temp");
                }
                startButton.GetComponent<Button>().onClick.AddListener(GoogleLogin);
            }
        });
    }
    private void GoToLobbyScene()
    {
        Managers.Scene.LoadScene("Lobby");
    }
    public void GoogleLogin()
    {
        startButton.interactable = false;
        Managers.Sound.Play("Effect/ClickButton");
        // TODO - 모바일 3개
#if UNITY_ANDROID
        Social.localUser.Authenticate(success =>
        {
            if (success)
            {
                //파이어베이스 로그인
                string idToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken(); ;
                Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(idToken, null);
                log = idToken;
                PlayerPrefs.SetString("userName", ((PlayGamesLocalUser)Social.localUser).userName);
                PlayerPrefs.SetString("id", ((PlayGamesLocalUser)Social.localUser).userName);
                auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled)
                    {
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        return;
                    }
                    log = "success";
                    GoToLobbyScene();
                });
            }
            else
            {

            }
        }
        );
#endif
    }
}
