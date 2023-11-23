using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class SingleUI_MonsterCount : MonoBehaviour
{
    private int[] maxKillCount = { 10, 10, 15, 15 };
    private int currentKillCount = 0;

    private TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        currentKillCount = SingleGameManager.instance.Player.KillCount;
        if (SingleGameManager.instance.bossSpawned)
        {
            if (SingleGameManager.instance.bossKilled)
                text.text = "포탈을 사용하세요!";
            else
                text.text = "보스를 처치하세요!";
        }
        else
            text.text = "처치한 몬스터 수: <color=#FD0500>" + currentKillCount.ToString() + "</color> / " + maxKillCount[Stage.currentStage].ToString();
    }
}
