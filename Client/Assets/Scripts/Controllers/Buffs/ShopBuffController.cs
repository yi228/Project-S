using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopBuffController : MonoBehaviour
{
    public Define.ShopBuffType Type;
    void Start()
    {
        GetTypeByName();
    }
    public void AddBuff()
    {
        switch (Type)
        {
            case Define.ShopBuffType.Block:
                AddBlockBuff();
                break;
            case Define.ShopBuffType.Attack:
                AddAttackBuff();
                break;
            case Define.ShopBuffType.Speed:
                AddSpeedBuff();
                break;
            case Define.ShopBuffType.Sight:
                AddSightBuff();
                break;
        }
    }
    void AddBlockBuff()
    {
        Managers.Game.BlockBuffCount++;
    }
    void AddAttackBuff()
    {
        Managers.Game.AttackBuffCount++;
    }
    void AddSpeedBuff()
    {
        Managers.Game.SpeedBuffCount++;
    }
    void AddSightBuff()
    {
        Managers.Game.SightBuffCount++;
    }
    void GetTypeByName()
    {
        string name = gameObject.name.Substring(0, gameObject.name.Length  - 10);
        if (Enum.TryParse(typeof(Define.ShopBuffType), name, true, out object result) && result is Define.ShopBuffType matchedType)
            Type = matchedType;
        else
            Debug.Log(gameObject.name + " cant find type");
    }
}
