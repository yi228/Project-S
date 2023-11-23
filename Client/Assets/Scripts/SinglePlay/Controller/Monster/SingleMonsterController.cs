using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Status
{
    public int hp;
    public int maxHp;
    public int attack;
    public int defense;
    public int speed;
    public int checkRange;
    public int attackRange;
    public int missDist;
    public float attackCool;
    public int exp;
}

public class SingleMonsterController : CreatureController
{
    private SingleUI_EnemyHpBar _hpBar;
    private GameObject _chaseIcon;
    public new Status _stat = new Status();

    protected SingleMyPlayerController _target;
    public bool _canChase = false;
    public bool _canAttack = true;
    private bool _iconOn = false;
    protected int _singleMonsterId;
    public AudioSource AudioSource;
    private NavMeshAgent navMesh;

    public int SingleMonsterId { get { return _singleMonsterId; } set { _singleMonsterId = value; } }
    public SingleMyPlayerController Target { get { return _target; } set { _target = value; } }

    private bool isDead = false;

    void Start()
    {
        Init();
        prevPos = transform.position;
    }
    void Update()
    {
        UpdateAnimation();
        CheckTarget();
        if (_target != null && State == CreatureState.Moving)
            Rotate();
    }
    protected override void Init()
    {
        base.Init();
        _hpBar = Managers.Resource.Instantiate("SinglePlay/UI/UI_EnemyHpBar").GetComponent<SingleUI_EnemyHpBar>();
        _hpBar.transform.SetParent(SingleGameManager.instance.monHpBarParent.transform);
        _hpBar.Monster = this;
        _hpBar.transform.position = transform.position;
        //InitStat();
        State = CreatureState.Idle;
        //_hpBar = Managers.Resource.Instantiate("UI/Scene/UI_EnemyHpBar").GetComponent<UI_EnemyHpBar>();
        //_hpBar.Monster = this;
        //_hpBar.transform.position = transform.position;
        AudioSource = GetComponent<AudioSource>();
        navMesh = GetComponent<NavMeshAgent>();
        navMesh.enabled = true;
        navMesh.speed = _stat.speed;
        navMesh.updateRotation = false;
        navMesh.updateUpAxis = false;
        SetTarget();
    }
    protected override void UpdateAnimation()
    {
        if (_animator == null)
            return;

        switch (State)
        {
            case CreatureState.Idle:
                //_animator.Play($"PLAYER_IDLE_{(int)WeaponType}");
                if (_iconOn)
                    ChaseIconOff();
                _animator.SetBool("isMoving", false);
                break;
            case CreatureState.Moving:
                //_animator.Play($"PLAYER_MOVE_{(int)WeaponType}");
                if (!_iconOn)
                    ChaseIconOn();
                _animator.SetBool("isMoving", true);
                break;
            case CreatureState.Skill:
                // 여기는 attack 한번만 하는 걸로 하는게 좋을듯
                //UseSkill에서 실행
                break;
            case CreatureState.Dead:
                if (_iconOn)
                    ChaseIconOff();
                _animator.SetBool("isMoving", false);
                break;
        }
    }
    protected void Rotate()
    {
        Vector2 value = prevPos - transform.position;
        prevPos = transform.position;
        value.Normalize();
        float dist = Vector2.Distance(_target.transform.position, transform.position);
        if (dist <= _stat.attackRange)
            value = transform.position - _target.transform.position;
        transform.eulerAngles = new Vector3(0, 0, -Mathf.Atan2(value.x, value.y) * Mathf.Rad2Deg);
    }
    protected void SetTarget()
    {
        GameObject go = FindObjectOfType<SingleWeaponSelect>().gameObject;
        foreach(SingleMyPlayerController tg in go.GetComponentsInChildren<SingleMyPlayerController>())
            if (tg.gameObject.activeSelf)
                _target = tg;
    }
    protected void CheckTarget()
    {
        float dist = Vector2.Distance(_target.transform.position, transform.position);
        navMesh.SetDestination(_target.transform.position);
        navMesh.isStopped = true;
        State = CreatureState.Idle;
        if (dist <= _stat.checkRange && navMesh.remainingDistance <= _stat.checkRange)
            FollowTarget();
    }
    protected virtual void FollowTarget()
    {
        State = CreatureState.Moving;
        //navMesh.SetDestination(_target.transform.position);
        float dist = Vector2.Distance(_target.transform.position, transform.position);
        if (dist <= _stat.attackRange && _canAttack)
        {
            navMesh.isStopped = true;
            StartCoroutine(CoAttack());
        }
        else if(dist > _stat.attackRange)
            navMesh.isStopped = false;
    }
    public override void OnDamaged()
    {
        base.OnDamaged();
        if (Managers.Sound.SoundOn == true)
            Managers.Sound.Play("Effect/Player/HitSound");
        FollowTarget();
    }
    protected override void UpdateController()
    {
        base.UpdateController();
    }
    private IEnumerator CoAttack()
    {
        _canAttack = false;
        _animator.SetBool("isMoving", false);
        _animator.SetTrigger("doAttack");
        _target.Hp -= _stat.attack;
        _target.GetComponent<SingleMyPlayerController>().GapHp = _stat.attack;
        _target.OnDamaged();
        yield return new WaitForSeconds(_stat.attackCool);
        _canAttack = true;
    }
    public override void OnDead()
    {
        if (!isDead)
        {
            isDead = true;
            base.OnDead();
            _target.KillCount++;
            _target.TotalExp += _stat.exp;
            Destroy(gameObject);
            if (_hpBar != null)
                Destroy(_hpBar.gameObject);
            if (_chaseIcon != null)
                Destroy(_chaseIcon);
        }
    }
    private void ChaseIconOn()
    {
        _iconOn = true;
        if (_chaseIcon == null)
        {
            _chaseIcon = Managers.Resource.Instantiate("UI/Scene/UI_EnemyChaseIcon");
            _chaseIcon.GetComponent<UI_EnemyChaseIcon>().SingleMonster = this;
        }
        else
            _chaseIcon.GetComponent<UI_EnemyChaseIcon>().IconOn();
    }
    private void ChaseIconOff()
    {
        _iconOn = false;
        if (_chaseIcon != null)
            _chaseIcon.GetComponent<UI_EnemyChaseIcon>().IconOff();
    }
}
