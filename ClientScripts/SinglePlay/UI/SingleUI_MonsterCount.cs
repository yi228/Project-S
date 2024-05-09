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
                text.text = "��Ż�� ����ϼ���!";
            else
                text.text = "������ óġ�ϼ���!";
        }
        else
            text.text = "óġ�� ���� ��: <color=#FD0500>" + currentKillCount.ToString() + "</color> / " + maxKillCount[Stage.currentStage].ToString();
    }
}
