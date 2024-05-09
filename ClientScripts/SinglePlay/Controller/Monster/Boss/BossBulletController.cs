using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BossBulletController : BossProjectileController
{
    public IObjectPool<GameObject> Pool { get; set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            Pool.Release(gameObject);
        }
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<SingleMyPlayerController>().GapHp = bulletDamage;
            collision.GetComponent<SingleMyPlayerController>().Hp -= bulletDamage;
            collision.GetComponent<SingleMyPlayerController>().OnDamaged();
            if(isActiveAndEnabled)
                Pool.Release(gameObject);
        }
    }
}
