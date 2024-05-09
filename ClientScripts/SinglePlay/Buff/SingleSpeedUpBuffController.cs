using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SingleSpeedUpBuffController : SingleBuffController
{
    public int cost;
    private void Start()
    {
        _costText = transform.Find("CostText").GetComponent<TextMeshPro>();
        _costText.text = $"{cost} ∞ÒµÂ";
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        // «√∑π¿ÃæÓ∞° æ∆¿Ã≈€ »πµÊ Ω√ 
        if (collision.gameObject.name.Contains("Player") && _isBuffed == false && Owner == null)
        {
            //Owner = collision.GetComponent<PlayerController>();
            //if (Owner.Gold >= cost)
            //{
            collision.GetComponent<SingleMyPlayerController>().Speed++;
            _isBuffed = true;

                Debug.Log("Speed Buff!");
                base.OnTriggerEnter2D(collision);
            //}
        }
    }
    public override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
    }
}
