using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PortalCool : MonoBehaviour
{
    private Image _icon;
    private MyPlayerController _myPlayer;

    public MyPlayerController MyPlayer { get { return _myPlayer; } set { _myPlayer = value; } }

    private void Start()
    {
        _icon = GetComponentInChildren<Image>();
    }

    public void IconVisible()
    {
        _icon.color = new Color32(255, 255, 255, 255);
    }

    public void IconInvisible()
    {
        _icon.color = new Color32(255, 255, 255, 0);
    }
}
