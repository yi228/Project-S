using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SingleMonster_1100_Controller : SingleMonsterController
{
    void Start()
    {
        Init();
    }
    protected override void Init()
    {
        InitStat();
        base.Init();
        SingleMonsterId = 1100;
    }
    private void InitStat()
    {
        //TODO - 몬스터별 스탯 적용
        _stat.hp = 50;
        _stat.maxHp = 50;
        _stat.attack = 20;
        _stat.defense = 0;
        _stat.speed = 3;
        _stat.checkRange = 7;
        _stat.attackRange = 2;
        _stat.missDist = 20;
        _stat.attackCool = 1f;
        _stat.exp = 50;
    }
}
