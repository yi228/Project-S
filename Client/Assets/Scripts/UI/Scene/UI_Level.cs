using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Level : MonoBehaviour
{
    private MyPlayerController _myPlayer;
    public MyPlayerController MyPlayer { get { return _myPlayer; } set { _myPlayer = value; } }

    public TextMeshProUGUI _levelText;

    private int level;

    void Update()
    {
        level = _myPlayer.Level + 1;
        _levelText.text = level.ToString();
    }
}
