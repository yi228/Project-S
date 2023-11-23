using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public Define.Scene _currentScene;
    public Define.Scene CurrentScene
    {
        get { return _currentScene; }
        set
        {
            if (_currentScene == value)
                return;
            _currentScene = value;
            //if (CurrentScene == Define.Scene.Game)
            //    Managers.Network.Init();
            Managers.Sound.ChangeBgmWhenSceneLoaded();
        }
    }
    public void LoadScene(Define.Scene type)
    {
        Managers.Clear();

        SceneManager.LoadScene(GetSceneName(type));
    }
    public void LoadScene(string sceneName)
    {
        //Managers.Clear();
#if UNITY_STANDALONE
        SceneManager.LoadScene(sceneName);
#elif UNITY_ANDROID
        UI_LoadingScene.Instance.LoadScene(sceneName);
#endif
    }

    string GetSceneName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        return name;
    }
}
