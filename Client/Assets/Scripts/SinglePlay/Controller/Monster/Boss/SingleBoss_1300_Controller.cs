using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBoss_1300_Controller : SingleMonsterBossController
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
            Spin();
        }
        else if (boss.hp > maxHp * 0.25)
        {
            StopFollow();
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
            if (PlayerPrefs.GetInt("StageData") <= 2)
            {
                PlayerPrefs.SetInt("StageData", 3);
            }
        }
    }
}
