using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBuffController : BuffController
{
    private int _bulletNumIncrease = 3;
    public int BulletNumIncrease { get { return _bulletNumIncrease; } set { _bulletNumIncrease = value; } }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어가 아이템 획득 시 
        if (collision.gameObject.name.Contains("Player"))
        {
            collision.transform.GetComponent<PlayerController>().IsBulletBuff = true;
            base.OnTriggerEnter2D(collision);
        }
    }
}