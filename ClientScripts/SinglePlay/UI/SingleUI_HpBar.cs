using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleUI_HpBar : UI_Scene
{
    private SingleMyPlayerController _myPlayer;
    public SingleMyPlayerController MyPlayer { get { return _myPlayer; } set { _myPlayer = value; } }

    Slider bar;

    void Start()
    {
        bar = GetComponentInChildren<Slider>();
        GetComponent<Canvas>().sortingOrder = 4;
    }
    void Update()
    {
        bar.value = (float)MyPlayer.Hp / (float)MyPlayer.MaxHp;
    }
}
