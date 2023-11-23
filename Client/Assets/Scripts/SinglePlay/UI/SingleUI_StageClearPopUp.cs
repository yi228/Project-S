using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleUI_StageClearPopUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private Button nextStageButton;
    void Start()
    {
        switch (Stage.currentStage)
        {
            case 0:
                stageText.text = "초원";
                break;
            case 1:
                stageText.text = "사막";
                break;
            case 2:
                stageText.text = "설원";
                break;
            case 3:
                stageText.text = "바다";
                nextStageButton.interactable = false;
                break;
        }
    }
    public void NextStageButtonPressed()
    {
        if(Stage.currentStage < 3)
        {
            Stage.currentStage++;
            SingleGameManager.instance.Player.MovingTiltUI.GetComponent<SingleUI_MovingTilt>().SetPlayerStagePos();
            SingleGameManager.instance.Player.GetComponent<SingleMyPlayerController>().Hp = SingleGameManager.instance.Player.GetComponent<SingleMyPlayerController>().MaxHp;
            SingleGameManager.instance.Player.GetComponent<SingleMyPlayerController>().InitLevel();
            SingleGameManager.instance.InitGame();
        }
            
        Destroy(gameObject);
    }
    public void ExitButtonPressed()
    {
        Managers.Scene.LoadScene("Lobby");
    }
}
