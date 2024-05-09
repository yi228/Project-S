using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Latency : UI_Popup
{
    TextMeshProUGUI LatencyText;
    enum Texts
    {
        LatencyText
    }
    private void Start()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        LatencyText = GetTextMeshProUGUI((int)Texts.LatencyText);
        gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Managers.Game.PrevLatency != DateTime.MinValue)
        {
            LatencyText.text = $"Latency: {(int)Managers.Game.NowLatency.TotalMilliseconds} ms";
        }
    }
}