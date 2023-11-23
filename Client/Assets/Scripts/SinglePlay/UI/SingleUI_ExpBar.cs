using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleUI_ExpBar : MonoBehaviour
{
    private SingleMyPlayerController _myPlayer;
    public SingleMyPlayerController MyPlayer { get { return _myPlayer; } set { _myPlayer = value; } }

    private int[] reqExp = { 40, 100, 250, 385, 600, 1000 };

    Slider bar;

    void Start()
    {
        bar = GetComponentInChildren<Slider>();
        GetComponent<Canvas>().sortingOrder = 4;
    }

    void Update()
    {
        if (MyPlayer != null)
            if (MyPlayer.Level > 0)
                bar.value = (float)(MyPlayer.TotalExp - reqExp[MyPlayer.Level - 1]) / (float)(reqExp[MyPlayer.Level] - reqExp[MyPlayer.Level - 1]);
            else
                bar.value = (float)MyPlayer.TotalExp / (float)reqExp[MyPlayer.Level];
    }
}
