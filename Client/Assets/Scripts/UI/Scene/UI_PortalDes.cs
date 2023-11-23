using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PortalDes : MonoBehaviour
{
    private MyPlayerController _myPlayer;
    private Transform _destination;
    private TextMeshProUGUI destText;

    public MyPlayerController MyPlayer { get { return _myPlayer; } set { _myPlayer = value; } }
    public Transform Destination { get { return _destination; } set { _destination = value; } }
    public TextMeshProUGUI DestText { get { return destText; } set { destText = value; } }
    public Color OriginalColor;
    public bool IsDestroyDestination = false;
    void Start()
    {
        destText = GetComponentInChildren<TextMeshProUGUI>();
        OriginalColor = destText.color;
    }

    void Update()
    {
        if (_myPlayer.InPortal && destText != null && Destination != null && MyPlayer != null)
        {
            destText.text = $"{MakeDestString(_destination)}";
            destText.color = OriginalColor;
        }
        else if (_myPlayer.InPortal && destText != null && Destination == null && MyPlayer != null)
        {
            destText.text = $"반대편 포탈이 비활성화 상태입니다";
            destText.color = OriginalColor;
        }
        else
        {
            destText.color = new Color(255, 255, 255, 0);
        }
    }

    string MakeDestString(Transform Dest)
    {
        Vector3 destVec = Dest.position;
        string ret = "";

        if (destVec.x <= -50 && destVec.x >= -90)
        {
            if (destVec.y <= 90 && destVec.y >= 50)
            {
                ret = "초원 지역 포탈";
            }
            else if (destVec.y <= -50 && destVec.y >= -90)
            {
                ret = "설원 지역 포탈";
            }
        }
        else if (destVec.x <= 90 && destVec.x >= 50)
        {
            if (destVec.y <= 90 && destVec.y >= 50)
            {
                ret = "사막 지역 포탈";
            }
            else if (destVec.y <= -50 && destVec.y >= -90)
            {
                ret = "바다 지역 포탈";
            }
        }
        else if (destVec.x <= 15 && destVec.x >= -15)
        {
            if (destVec.y <= 15 && destVec.y >= -15)
            {
                ret = "어둠 지역 포탈";
            }
        }

        return ret;
    }
}
