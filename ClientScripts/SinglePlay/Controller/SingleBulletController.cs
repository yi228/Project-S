using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SingleBulletController : CreatureController
{
    public IObjectPool<GameObject> Pool { get; set; }

    public float _bulletSpeed;
    private SingleMyPlayerController _owner;
    private Vector3 _dir;
    public SingleMyPlayerController Owner { get { return _owner; } set { _owner = value; } }
    public Vector3 Dir { get { return _dir; } set { _dir = value; } }
    // 서버에서 여러번 연산 하는 거 막기
    public bool IsTouch = false;
    protected override void Init()
    {
        base.Init();
    }
    private void Start()
    {
        Init();
        //_dir = _owner.transform.up;
    }

    private void Update()
    {
        if (_dir != null)
            transform.position = transform.position + _dir * (_bulletSpeed + _owner.BulletSpeedBuff) * Time.deltaTime;
    }

    private void OnDamagingMonster(SingleMonsterController monster)
    {
        // 몬스터가 맞았을 때
        if (monster.tag == "Monster" && !IsTouch)
        {
            IsTouch = true;
            SingleMonsterController enemy = monster;
            if (enemy == null)
            {
                Debug.Log("No Enemy");
                return;
            }
            else
            {
                //몬스터 데미지 입히기
                enemy._stat.hp -= _owner.Attack;
                enemy.GapHp = _owner.Attack;
                enemy.OnDamaged();
                //임시 사망 처리
                if (enemy._stat.hp < 0)
                {
                    enemy.OnDead();
                    //_owner.KillCount++;
                }
                else
                    Debug.Log(enemy._stat.hp);
            }
        }
    }
    public Vector3 Inclination = Vector3.zero;
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") && isActiveAndEnabled)
        {
            Pool.Release(gameObject);
        }
        //if (IsTouch == false)
        //{
            if (collision.CompareTag("Monster"))
            {
                //IsTouch = true;
                OnDamagingMonster(collision.transform.GetComponent<SingleMonsterController>());
                if (isActiveAndEnabled)
                {
                    Pool.Release(gameObject);
                    transform.localScale = Vector3.one;
                }
            }
            else if (collision.CompareTag("BossMonster"))
            {
                //IsTouch = true;
                collision.transform.GetComponent<BossMonster>().hp -= _owner.Attack;
                collision.transform.GetComponent<SingleMonsterBossController>().GapHp = _owner.Attack;
                collision.transform.GetComponent<SingleMonsterBossController>().OnDamaged();
                if (isActiveAndEnabled)
                {
                    Pool.Release(gameObject);
                    transform.localScale = Vector3.one;
                }
            }
        //}
    }
}
