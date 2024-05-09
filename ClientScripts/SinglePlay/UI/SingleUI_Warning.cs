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
                bossName = "∫∏Ω∫ ∞≈∫œ¿Ã";
                break;
            case 1:
                bossName = "∆ƒ¿ÃæÓ µÂ∑°∞Ô";
                break;
            case 2:
                bossName = "≈∑Ω∫∆Â≈Õ";
                break;
            case 3:
                bossName = "∞Ò∑Ω";
                break;
            default:
                bossName = "";
                break;
        }
        warningText.text = "<color=#FF0702>∫∏Ω∫ µÓ¿Â</color>\r\n<size=120><color=#FF4700>" + bossName + "</color></size>\r\n";
    }
}
