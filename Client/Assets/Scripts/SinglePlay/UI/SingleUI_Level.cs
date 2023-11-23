using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SingleUI_Level : MonoBehaviour
{
    private SingleMyPlayerController _myPlayer;
    public SingleMyPlayerController MyPlayer { get { return _myPlayer; } set { _myPlayer = value; } }

    public TextMeshProUGUI _levelText;

    private int level;

    void Update()
    {
        level = _myPlayer.Level + 1;
        _levelText.text = level.ToString();
    }
}
