using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBoss_1200_Controller : SingleMonsterBossController
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
            LookAtPlayer(false, true);
        }
        else if (boss.hp > maxHp * 0.25)
        {
            FollowPlayer();
            LookAtPlayer(false, true);
        }
        else if (boss.hp > 0)
        {
            FollowPlayer();
            FireBall(false, true, true);
        }
        else
        {
            Destroy(gameObject.transform.parent.gameObject);
            gameManager.bossKilled = true;
            if (PlayerPrefs.GetInt("StageData") <= 1)
            {
                PlayerPrefs.SetInt("StageData", 2);
            }
        }
    }
}
