using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBoss_1100_Controller : SingleMonsterBossController
{
    void Update()
    {
        applyHpSlider();
        if (boss.hp > maxHp * 0.75)
        {
            Spin();
        }
        else if (boss.hp > maxHp * 0.5)
        {
            LookAtPlayer(true, true);
        }
        else if (boss.hp > maxHp * 0.25)
        {
            FollowPlayer();
            Spin();
        }
        else if (boss.hp > 0)
        {
            FollowPlayer();
            LookAtPlayer(true, true);
        }
        else
        {
            Destroy(gameObject.transform.parent.gameObject);
            gameManager.bossKilled = true;
            if(PlayerPrefs.GetInt("StageData") == 0)
            {
                PlayerPrefs.SetInt("StageData", 1);
            }
        }
    }
}
