using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleUI_EnemyHpBar : MonoBehaviour
{
    private SingleMonsterController _monster;

    public SingleMonsterController Monster { get { return _monster; } set { _monster = value; } }

    Slider bar;


    void Start()
    {
        bar = GetComponentInChildren<Slider>();
    }
    void Update()
    {
        if (_monster != null && _monster._stat != null)
        {
            bar.value = (float)_monster._stat.hp / (float)_monster._stat.maxHp;
            //bar.transform.position = _monster.transform.position + new Vector3(0.1f, 1f, 0);

            bar.transform.position = Camera.main.WorldToScreenPoint(_monster.transform.position + new Vector3(0.1f, 1f, 0));
        }
    }
}
