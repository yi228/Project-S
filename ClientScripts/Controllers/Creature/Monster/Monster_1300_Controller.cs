using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_1300_Controller : MonsterController
{
    void Start()
    {
        Init();
    }

    void Update()
    {
        base.UpdateAnimation();
        if (IsMoving == true)
            base.UpdatePosition();
    }
    protected override void Init()
    {
        base.Init();
        MonsterId = 1300;
    }
}
