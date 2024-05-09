using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Protocol;
using TMPro;

public class AttackBuffController : BuffController
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
                C_ChangeAttack attackPacket = new C_ChangeAttack();
                attackPacket.Attack = collision.GetComponent<CreatureController>().Stat.Attack;
                attackPacket.IsBuff = true;
                attackPacket.BuffId = Id;
                Managers.Network.Send(attackPacket);
                Debug.Log("Attack Buff!");
                base.OnTriggerEnter2D(collision);
            }
        }
    }
    public override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
    }
}
