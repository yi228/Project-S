using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Timer : MonoBehaviour
{
    public TextMeshProUGUI _timeText;

    private float totalSec;

    void Update()
    {
        if (Managers.Game.IsStartGame)
        {
            totalSec = Managers.Game.ElapsedTime;
            int min = (int)totalSec / 60;
            int sec = (int)totalSec - min * 60;
            _timeText.text = string.Format("{0:00}:{1:00}", min, sec);
        }
    }
}
