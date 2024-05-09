using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SingleUI_Warning : MonoBehaviour
{
    private TextMeshProUGUI warningText;

    void Start()
    {
        warningText = GetComponentInChildren<TextMeshProUGUI>();
    }
    void Update()
    {
        SetWarningText();
    }
    private void SetWarningText()
    {
        string bossName;
        switch (Stage.currentStage)
        {
            case 0:
                bossName = "���� �ź���";
                break;
            case 1:
                bossName = "���̾� �巡��";
                break;
            case 2:
                bossName = "ŷ������";
                break;
            case 3:
                bossName = "��";
                break;
            default:
                bossName = "";
                break;
        }
        warningText.text = "<color=#FF0702>���� ����</color>\r\n<size=120><color=#FF4700>" + bossName + "</color></size>\r\n";
    }
}
