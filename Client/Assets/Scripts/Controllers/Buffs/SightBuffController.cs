using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Protocol;
using TMPro;

public class SightBuffController : BuffController
{
    private void Start()
    {
        _costText = transform.Find("CostText").GetComponent<TextMeshPro>();
        _costText.text = $"{Stat.Cost} 골드 소모";
        InitTextComponent();
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어가 아이템 획득 시 
        if (collision.gameObject.name.Contains("Player") && _isBuffed == false && Owner == null)
        {
            Owner = collision.GetComponent<PlayerController>();
            if (Owner.Gold >= Stat.Cost)
            {
                _isBuffed = true;
                C_ChangeSight sightPacket = new C_ChangeSight();
                sightPacket.Sight = collision.GetComponent<CreatureController>().CameraSize;
                sightPacket.IsBuff = true;
                sightPacket.BuffId = Id;
                Managers.Network.Send(sightPacket);
                Debug.Log("Sight Buff! " + collision.transform.GetComponent<CreatureController>().Id);
                base.OnTriggerEnter2D(collision);
            }
        }
    }
    public override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
    }
}
