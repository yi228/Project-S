using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HpBar : UI_Scene
{
    private MyPlayerController _myPlayer;
    public MyPlayerController MyPlayer { get { return _myPlayer; } set { _myPlayer = value; } }

    Slider bar;

    void Start()
    {
        bar = GetComponentInChildren<Slider>();
        GetComponent<Canvas>().sortingOrder = 4;
    }
    void Update()
    {
        if (MyPlayer != null && MyPlayer.Stat != null)
            bar.value = (float)MyPlayer.Stat.Hp / (float)MyPlayer.Stat.MaxHp;
    }
}
