using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Google.Protobuf.Protocol;
public class LightBuffController : BuffController
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
                Owner.CanLightBuff = true;
                Owner.Gold -= Stat.Cost;
                C_ChangeGold goldPacket = new C_ChangeGold();
                goldPacket.Gold = Owner.Gold;
                base.OnTriggerEnter2D(collision);
            }
        }
    }
    public override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
    }



}
