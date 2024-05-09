using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Gold : MonoBehaviour
{
    public TextMeshProUGUI _goldText;
    MyPlayerController player;
    void Start()
    {
        player = Managers.Object.FindById(Managers.Game.MyPlayerId).GetComponent<MyPlayerController>();
    }
    void Update()
    {
        _goldText.text = $"{player.Gold}";
    }
}
