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
        // slider �ʱⰪ ����
        prefabLoadSlider.value = 0;

        startButton = GetButton((int)Buttons.StartButton);
        startText = GetTextMeshProUGUI((int)Texts.StartText);
        startText.text = "���� ���ҽ��� �ٿ�ε� �� �Դϴ�";
        // TODO - ����� 3��
        //���� �α���

#if UNITY_ANDROID
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .RequestIdToken()
            .Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
#endif
        //���̾�̽�
        auth = FirebaseAuth.DefaultInstance;
        // ���ӿ��� �� ���ҽ� Addressable�� �ҷ�����
        LoadAllAsync();
    }

    void Update()
    {

    }

    private void LoadAllAsync()
    {
        Managers.Resource.LoadAllAsync<GameObject>("Prefab", (key, count, totalCount) =>
        {
            // �ε��� ��� ����
            Debug.Log($"{key} {count}/{totalCount}");
            float progressRate = (float)count / (float)totalCount;
            prefabLoadSlider.value = progressRate;
            if (count == totalCount)
            {
                // �� �Ϸ�ǰ� ���� ������ �Լ�
                Debug.Log("Load All Prefabs");
                prefabLoadSlider.value = 1f;
                prefabLoadSlider.gameObject.SetActive(false);
                startText.text = "��ġ�ϸ� ������ �����ؿ�!";
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
        // TODO - ����� 3��
#if UNITY_ANDROID
        Social.localUser.Authenticate(success =>
        {
            if (success)
            {
                //���̾�̽� �α���
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
