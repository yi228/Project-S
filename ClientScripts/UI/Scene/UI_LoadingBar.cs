using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_LoadingBar : UI_Scene
{
    GameObject fillArea;
    Slider LoadSlider;
    enum Buttons
    {
        LoadButton,
        SceneLoadButton,
    }
    enum Sliders
    {
        LoadSlider,
    }

    void Start()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Slider>(typeof(Sliders));

        GetButton((int)Buttons.LoadButton).onClick.AddListener(LoadAllAsync);
        LoadSlider = GetSlider((int)Sliders.LoadSlider);
        fillArea = LoadSlider.transform.Find("Fill Area").gameObject;

        // �����̴� �� 0�� �� �Ⱥ��̰� ��Ȱ��ȭ
        if (LoadSlider.value == 0f)
            fillArea.SetActive(false);
    }
    private void Update()
    {
        // �����̴� �� 0�� �ƴ� �� �Ⱥ��̰� ��Ȱ��ȭ
        if (LoadSlider.value != 0f)
            fillArea.SetActive(true);
    }
    void LoadAllAsync()
    {
        GetButton((int)Buttons.LoadButton).interactable = false;
        Managers.Resource.LoadAllAsync<GameObject>("Prefab", (key, count, totalCount) =>
        {
            // �ε��� ��� ����
            Debug.Log($"{key} {count}/{totalCount}");
            GetSlider((int)Sliders.LoadSlider).value = (float)count / (float)totalCount;
            if (count == totalCount)
            {
                // �� �Ϸ�ǰ� ���� ������ �Լ�
                Debug.Log("Load All Prefabs");
                // TODO
                Object loadingScene = null;
                if (Managers.Resource.Res.TryGetValue("LoadingScene", out loadingScene) == false)
                {
                    loadingScene = Managers.Resource.Load<GameObject>("UI/Scene/LoadingScene");
                    Managers.Resource.Res.Add("LoadingScene", loadingScene);
                    Debug.Log("Temp");
                }
                Managers.Scene.LoadScene("Lobby");
            }
        });
    }
    public void GoToLobbyScene()
    {
        Managers.Scene.LoadScene("Lobby");
    }
}
