using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Protocol;

public class SingleWeaponSelect : MonoBehaviour
{
    private SingleMyPlayerController[] _players;
    void Awake()
    {
        Init();
        WeaponSelect();
    }
    private void Init()
    {
        _players = GetComponentsInChildren<SingleMyPlayerController>();
        
        
        for (int i = 0; i < _players.Length; i++) {
            _players[i].gameObject.SetActive(false);
        } }

    private void WeaponSelect()
    {
        SingleMyPlayerController player = _players[(int)Managers.Game.MyPlayerWeaponType - 1];
        player.gameObject.SetActive(true);
        
        player.InitGameUI();
        if (player != null && player.WeaponUI != null)
            player.WeaponUI.ChangeImage((int)Managers.Game.MyPlayerWeaponType);
    }
}
