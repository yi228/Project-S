using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_EnemyHpBar : MonoBehaviour
{
    private MonsterController _monster;

    public MonsterController Monster { get { return _monster; } set { _monster = value; } }

    Slider bar;


    void Start()
    {
        bar = GetComponentInChildren<Slider>();
    }
    void Update()
    {
        if (_monster != null && _monster.Stat != null)
        {
            bar.value = (float)_monster.Stat.Hp / (float)_monster.Stat.MaxHp;
            bar.transform.position = _monster.transform.position + new Vector3(0.1f, 1f, 0);

            //bar.transform.position = Camera.main.WorldToScreenPoint(_monster.transform.position + new Vector3(0.1f, 1f, 0));
        }
    }
}
