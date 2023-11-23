using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class StageScene : MonoBehaviour
{
    private Button Stage0_Button, Stage1_Button, Stage2_Button, Stage3_Button, Back_Button,Home_Button ;
    private GameObject Stage1_Lock, Stage2_Lock, Stage3_Lock;
    void Start()
    {
        Stage0_Button = GameObject.Find("Stage0").GetComponent<Button>();
        Stage0_Button.onClick.AddListener(Stage0);
        Stage1_Button = GameObject.Find("Stage1").GetComponent<Button>();
        Stage1_Button.onClick.AddListener(Stage1);
        Stage2_Button = GameObject.Find("Stage2").GetComponent<Button>();
        Stage2_Button.onClick.AddListener(Stage2);
        Stage3_Button = GameObject.Find("Stage3").GetComponent<Button>();
        Stage3_Button.onClick.AddListener(Stage3);
        Back_Button = GameObject.Find("Button_Back").GetComponent<Button>();
        Back_Button.onClick.AddListener(GoToPlayModeScene);
        Stage1_Lock = GameObject.Find("Stage1_Lock");
        Stage2_Lock = GameObject.Find("Stage2_Lock");
        Stage3_Lock = GameObject.Find("Stage3_Lock");
        //Home_Button = GameObject.Find("Button_Home").GetComponent<Button>();
        //Home_Button.onClick.AddListener(GoToLobbyScene);
        loadData();
    }
    private void loadData()
    {
        switch (PlayerPrefs.GetInt("StageData")) {
            case 0:
                break;
            case 1:
                Stage1_Button.interactable = true;
                Stage2_Button.interactable = false;
                Stage3_Button.interactable = false;
                Stage1_Lock.SetActive(false);
                break;
            case 2:
                Stage1_Button.interactable = true;
                Stage2_Button.interactable = true;
                Stage3_Button.interactable = false;
                Stage1_Lock.SetActive(false);
                Stage2_Lock.SetActive(false);
                break;
            case 3:
                Stage1_Button.interactable = true;
                Stage2_Button.interactable = true;
                Stage3_Button.interactable = true;
                Stage1_Lock.SetActive(false);
                Stage2_Lock.SetActive(false);
                Stage3_Lock.SetActive(false);
                break;

        }

    }
    private void Stage0()
    {
        GoToStageScene(0);
    }
    private void Stage1()
    {
        GoToStageScene(1);
    }
    private void Stage2()
    {
        GoToStageScene(2);
    }
    private void Stage3()
    {
        GoToStageScene(3);
    }
    private void GoToPlayModeScene()
    {
        Managers.Scene.LoadScene("PlayMode");
    }
    private void GoToLobbyScene()
    {
        Managers.Scene.LoadScene("Lobby");
    }
    private void GoToStageScene(int _stageNum)
    {
        
        Managers.Scene.CurrentScene = Define.Scene.SingleGame;
        Stage.currentStage = _stageNum;
        Managers.Scene.LoadScene("SingleGame");
    }
}
public static class Stage
{
    public static int currentStage = 0;
    public static int maxStage = 0;
}