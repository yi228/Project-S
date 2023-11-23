using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFireballController : BossProjectileController
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<SingleMyPlayerController>().GapHp = bulletDamage;
            collision.GetComponent<SingleMyPlayerController>().Hp -= bulletDamage;
            collision.GetComponent<SingleMyPlayerController>().OnDamaged();
            if (isActiveAndEnabled)
                Destroy(gameObject);
        }
    }
}
