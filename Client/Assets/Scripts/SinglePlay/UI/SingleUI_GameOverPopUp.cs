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
