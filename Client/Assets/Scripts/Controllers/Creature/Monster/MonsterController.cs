using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : CreatureController
{
    private UI_EnemyHpBar _hpBar;
    private GameObject _chaseIcon;

    protected PlayerController _target;
    protected bool _canAttack = true;
    private bool _iconOn = false;
    protected int _monsterId;
    public AudioSource AudioSource;

    public int MonsterId { get { return _monsterId; } set { _monsterId = value; } }
    public PlayerController Target { get { return _target; } set { _target = value; } }
    void Start()
    {
        Init();
    }
    protected override void Init()
    {
        base.Init();
        InitStat();
        // TODO - 서버에서 변경
        //State = CreatureState.Idle;
        //TODO - 어드레서블
        _hpBar = Managers.Resource.Instantiate("UI/Scene/UI_EnemyHpBar").GetComponent<UI_EnemyHpBar>();
        _hpBar.Monster = this;
        _hpBar.transform.position = transform.position;
        AudioSource = GetComponent<AudioSource>();
    }
    protected override void UpdateAnimation()
    {
        if (_animator == null)
            return;

        switch (State)
        {
            case CreatureState.Idle:
                //_animator.Play($"PLAYER_IDLE_{(int)WeaponType}");
                if(_iconOn)
                    ChaseIconOff();
                _animator.SetBool("isMoving", false);
                break;
            case CreatureState.Moving:
                //_animator.Play($"PLAYER_MOVE_{(int)WeaponType}");
                if(!_iconOn)
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

    protected override void UpdateController()
    {
        base.UpdateController();
    }

    protected void InitStat()
    {
        // TODO - 나중에 json으로 받아오기
        //Stat.Hp = 50;
        //Stat.Attack = 7;
        //Stat.Defense = 0;
        //Stat.Speed = 3;
    }

    /*protected void Attack()
    {
        StartCoroutine(CoAttack());
    }

    protected IEnumerator CoAttack()
    {
        _canAttack = false;
        //if(_scanner._nearestTarget)
        //    _scanner._nearestTarget.GetComponent<PlayerController>().OnDamaged(Stat.Attack);
        Debug.Log(this + " hits " + _target.transform.name);

        yield return new WaitForSeconds(1f);

        _canAttack = true; 
        // TODO - 서버에서 변경
        //State = CreatureState.Moving;
    }*/

    public override void UseSkill(int skillId)
    {
        base.UseSkill(skillId);
        _animator.SetBool("isMoving", false);
        _animator.SetTrigger("doAttack");
    }
    public override void OnDead()
    {
        base.OnDead();
        if (_hpBar != null)
            Destroy(_hpBar.gameObject);
        if (_chaseIcon != null)
            Destroy(_chaseIcon);
    }
    private void ChaseIconOn()
    {
        _iconOn = true;
        if (_chaseIcon == null)
        {
            _chaseIcon = Managers.Resource.Instantiate("UI/Scene/UI_EnemyChaseIcon");
            _chaseIcon.GetComponent<UI_EnemyChaseIcon>().Monster = this;
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