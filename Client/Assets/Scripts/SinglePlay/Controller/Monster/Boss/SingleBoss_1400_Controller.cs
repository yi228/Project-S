using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBoss_1400_Controller : SingleMonsterBossController
{
    void Update()
    {
        applyHpSlider();
        if (boss.hp > maxHp * 0.75)
        {
            FireBall(true, false, true);
        }
        else if (boss.hp > maxHp * 0.5)
        {
            FollowPlayer();
            Rotate();
            anim.SetBool("isAttack", true);
        }
        else if (boss.hp > maxHp * 0.25)
        {
            StopFollow();
            anim.SetBool("isAttack", false);
            FireBall(true, true, true);
        }
        else if (boss.hp > 0)
        {
            FollowPlayer();
            Rotate();
            anim.SetBool("isAttack", true);
            FireBall(false, true, true);
        }
        else
        {
            Destroy(gameObject.transform.parent.gameObject);
            gameManager.bossKilled = true;
        }
    }
    private void Rotate()
    {
        Vector2 value = transform.position - _target.transform.position;
        transform.eulerAngles = new Vector3(0, 0, -Mathf.Atan2(value.x, value.y) * Mathf.Rad2Deg);
    }
}
