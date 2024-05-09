using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SingleUI_GameOverPopUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private Button restartButton;
    void Start()
    {
        switch (Stage.currentStage)
        {
            case 0:
                stageText.text = "�ʿ�";
                break;
            case 1:
                stageText.text = "�縷";
                break;
            case 2:
                stageText.text = "����";
                break;
            case 3:
                stageText.text = "�ٴ�";
                break;
        }
    }
    public void RestartButtonPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ExitButtonPressed()
    {
        
        Managers.Scene.LoadScene("Lobby");
        Destroy(gameObject);
        
    }
}
