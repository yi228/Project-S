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
        textSystem.text = $"�ڷΰ��⸦ �� �� �� �����ø� ����˴ϴ�";
        tmpAlpha.FadeOut();
    }
    public void PrintInGameText(SystemMSGType type, int areaId = 0)
    {
        switch (type)
        {
            case SystemMSGType.MsgtypeNone:
                break;
            case SystemMSGType.MapCloseReminder:
                textSystem.text = $"{MakeAreaNameById(areaId)} ������ 20�� �Ŀ� ���˴ϴ�";
                Managers.Sound.Play("Effect/ForewarnedCloseArea");
                break;
            case SystemMSGType.MapCloseNow:
                textSystem.text = $"{MakeAreaNameById(areaId)} ������ ���ƽ��ϴ�";
                Managers.Sound.Play("Effect/CloseArea");
                break;
            case SystemMSGType.MapMatch:
                textSystem.text = $"��� ������ ���ƽ��ϴ�\n" + "60�ʾȿ� ������ 1���� ��������";
                // ���� ü�¹� ���� ����
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
            areaName = "�ʿ�";
        else if (areaId == 2)
            areaName = "�縷";
        else if (areaId == 3)
            areaName = "����";
        else if (areaId == 4)
            areaName = "�ٴ�";
        return areaName;
    }
}