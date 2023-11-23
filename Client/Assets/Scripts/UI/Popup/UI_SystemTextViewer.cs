using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Google.Protobuf.Protocol;

public class UI_SystemTextViewer : MonoBehaviour
{
    private TextMeshProUGUI textSystem;
    private TMPAlpha tmpAlpha;

    void Awake()
    {
        textSystem = GameObject.Find("TextViewer").GetComponent<TextMeshProUGUI>();
        tmpAlpha = textSystem.GetComponent<TMPAlpha>();
    }

    public void PrintQuitText()
    {
        textSystem.text = $"뒤로가기를 한 번 더 누르시면 종료됩니다";
        tmpAlpha.FadeOut();
    }
    public void PrintInGameText(SystemMSGType type, int areaId = 0)
    {
        switch (type)
        {
            case SystemMSGType.MsgtypeNone:
                break;
            case SystemMSGType.MapCloseReminder:
                textSystem.text = $"{MakeAreaNameById(areaId)} 지역이 20초 후에 폐쇄됩니다";
                Managers.Sound.Play("Effect/ForewarnedCloseArea");
                break;
            case SystemMSGType.MapCloseNow:
                textSystem.text = $"{MakeAreaNameById(areaId)} 지역이 폐쇄됐습니다";
                Managers.Sound.Play("Effect/CloseArea");
                break;
            case SystemMSGType.MapMatch:
                textSystem.text = $"모든 구역이 폐쇄됐습니다\n" + "60초안에 최후의 1인을 가리세요";
                // 몬스터 체력바 전부 삭제
                DestroyAllHPBar();
                Managers.Sound.Play("Effect/FinalStage");
                break;
        }
        tmpAlpha.FadeOut();
    }
    void DestroyAllHPBar()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("MonsterHPBar");
        foreach (GameObject go in gos)
        {
            Destroy(go);
        }
    }
    string MakeAreaNameById(int areaId)
    {
        string areaName = "";
        if (areaId == 1)
            areaName = "초원";
        else if (areaId == 2)
            areaName = "사막";
        else if (areaId == 3)
            areaName = "설원";
        else if (areaId == 4)
            areaName = "바다";
        return areaName;
    }
}