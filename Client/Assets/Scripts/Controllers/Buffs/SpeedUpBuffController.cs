using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Protocol;
using TMPro;

public class SpeedUpBuffController : BuffController
{
    private void Start()
    {
        _costText = transform.Find("CostText").GetComponent<TextMeshPro>();
        _costText.text = $"{Stat.Cost} °ñµå ¼Ò¸ð";
        InitTextComponent();
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        // ÇÃ·¹ÀÌ¾î°¡ ¾ÆÀÌÅÛ È¹µæ ½Ã 
        if (collision.gameObject.name.Contains("Player") && _isBuffed == false && Owner == null)
        {
            Owner = collision.GetComponent<PlayerController>();
            if (Owner.Gold >= Stat.Cost)
            {
                _isBuffed = true;
                C_ChangeSpeed speedPacket = new C_ChangeSpeed();
                speedPacket.Speed = collision.GetComponent<CreatureController>().Stat.Speed;
                speedPacket.IsBuff = true;
                speedPacket.BuffId = Id;
                Managers.Network.Send(speedPacket);
                Debug.Log("Speed Buff!");
                base.OnTriggerEnter2D(collision);
            }
        }
    }
    public override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
    }
}
