using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CreatureController : MonoBehaviour
{
    protected Stat _stat;
    protected BoxCollider2D _bc;
    protected Rigidbody2D _rb;
    protected SpriteRenderer _sr;
    PositionInfo _positionInfo = new PositionInfo();
    protected bool _isDead = false;
    protected int _id;
    protected Vector3 _dirVector;
    protected Animator _animator;
    // 기본 레벨 1로 시작 - 서버는 0인듯
    protected int _level = 0;
    protected bool _levelChanged = false;
    // 나를 잡은 오브젝트가 받는 경험치
    protected int _exp;
    // 내가 갖고 있는 총 경험치
    protected int _totalExp;
    protected bool _isMoving = false;
    protected int _gold;
    public Stat Stat { get { return _stat; } set { _stat = value; } }
    public Animator Animator { get { return _animator; } set { _animator = value; } }
    private float gapHp = 0;
    public float GapHp { get { return gapHp; } set { gapHp = value; } }
    public float Hp 
    { 
        get { return Stat.Hp; }
        set 
        {
            gapHp = Stat.Hp - value;
            Stat.Hp = Math.Clamp(value, 0, Stat.MaxHp); 
        } 
    }
    public int Level { get { return _level; } 
        set {
            if(_level !=value)
                _levelChanged = true;
            _level = value; 
        } 
    }
    public int Exp { get { return _exp; } set { _exp = value; } }

    public int TotalExp
    {
        get { return _totalExp; }
        set
        {
            _totalExp = value;
            LevelUp();
        }
    }
    public float CameraSize
    {
        get { return Stat.CameraSize; }
        set
        {
            if(Stat != null)
                Stat.CameraSize = value;
            Camera.main.orthographicSize = value;
        }
    }
    public bool IsMoving { get { return _isMoving; } set { _isMoving = value; } }

    public Vector3 DirVector { get { return _dirVector; } set { _dirVector = value; } }

    public CreatureState State
    {
        get { return PosInfo.State; }
        set
        {
            if (PosInfo.State == value)
                return;

            PosInfo.State = value;
            UpdateAnimation();
        }
    }
    public PositionInfo PosInfo
    {
        get { return _positionInfo; }
        set
        {
            _positionInfo = value;
            transform.position = new Vector3(_positionInfo.PosX, _positionInfo.PosY, 0);
            transform.eulerAngles = new Vector3(0, 0, _positionInfo.RotZ);
            // TODO - 움직인 것에 따른 애니메이션 재생
        }
    }
    public bool IsDead { get { return _isDead; } set { _isDead = value; } }
    public int Id { get { return _id; } set { _id = value; } }
    public int Gold { get { return _gold; } set { _gold = value; } }
    public bool CanRespawn = false;
    protected virtual void Init()
    {
        //_stat = GetComponent<Status>();
        _rb = GetComponent<Rigidbody2D>();
        _bc = GetComponent<BoxCollider2D>();
        _sr = GetComponent<SpriteRenderer>();

        _animator = GetComponent<Animator>();
    }
    // 데미지를 입었을 때 실행하는 함수 - ex) 스프라이트 교체라든가 ,hp bar 업데이트 등등
    public virtual void OnDamaged()
    {
        GameObject effect = Managers.Resource.InstantiateResources("HitEffect");
        //TODO - 어드레서블
        //GameObject text = Managers.Resource.InstantiateResources("UI_DamageText");
        GameObject text = Managers.Resource.Instantiate("UI/Scene/UI_DamageText");
        effect.GetComponent<EffectController>().Creature = this;
        text.GetComponent<UI_DamageText>().Id = Id;
        text.GetComponent<UI_DamageText>().SetText(gapHp);
        effect.transform.position = transform.position;
        text.GetComponent<UI_DamageText>().Pos = transform;
        GameObject.Destroy(effect, 1f);
        Shake();
        StartCoroutine(CoGetHit());
        Debug.Log($"크리쳐 {Id}가 데미지를 입음");
        //Managers.Sound.PlayHitSound(Id);
    }
    IEnumerator CoGetHit()
    {
        if (!gameObject.CompareTag("BossMonster"))
        {
            _sr.color = new Color32(255, 0, 0, 255);
            yield return new WaitForSeconds(0.3f);
            _sr.color = new Color32(255, 255, 255, 255);
            yield return null;
        }        
    }
    public virtual void OnDead()
    {
        // TODO - 서버에서 STATE 변경
        //State = CreatureState.Dead;
        // 죽었을 때 처리 부분
        GameObject effect = Managers.Resource.InstantiateResources("DieEffect");
        //effect.GetComponent<SpriteRenderer>().sortingOrder = 20;
        effect.transform.position = transform.position;
        effect.GetComponent<Animator>().Play("START");
        GameObject.Destroy(effect, 5f);
    }
    protected virtual void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Chasing:
                UpdateChasing();
                break;
            case CreatureState.Skill:
                UpdateSkill();
                break;
            case CreatureState.Dead:
                UpdateDead();
                break;
        }
    }
    protected virtual void UpdateIdle()
    {

    }
    protected virtual void UpdateMoving()
    {

    }
    protected virtual void UpdateChasing()
    {

    }
    protected virtual void UpdateSkill()
    {

    }
    protected virtual void UpdateDead()
    {

    }
    protected virtual void UpdateAnimation()
    {

    }
    // 스르륵 움직이는 부분, 로컬에서 좌표 옮겨주기
    protected bool isPosUpdating = false;
    public Vector3 prevPos = new Vector3();
    public float prevRotz;
    //public Vector3 destPos;
    // 스르륵 움직이는 것 처리
    protected Vector3 originDestPos = new Vector3();
    public Vector3 DestPos = Vector3.zero;
    float originalRotZ;
    public float DestRotZ;
    public bool IsTurning = false;
    public bool UseTeleport = false;
    public virtual void UpdatePosition()
    {
        if (UseTeleport == true)
            return;

        if (IsMoving == false)
            return;

        // 해당 함수를 destPos에 대해 처음 실행할 때
        if (isPosUpdating == false)
        {
            prevPos = transform.position;
            isPosUpdating = true;
            originDestPos = DestPos;
        }

        Vector3 moveDir = originDestPos - transform.position;
        float dist = moveDir.magnitude;
        // 순간이동 할 때는 바로 처리
        if (dist >= 10f)
        {
            PosInfo.PosX = originDestPos.x;
            PosInfo.PosY = originDestPos.y;
            IsMoving = false;
            isPosUpdating = false;
            originDestPos = Vector3.zero;
        }
        else if (dist <= Stat.Speed * Time.deltaTime)
        {
            PosInfo.PosX = originDestPos.x;
            PosInfo.PosY = originDestPos.y;
            IsMoving = false;
            isPosUpdating = false;
            originDestPos = Vector3.zero;
            //gameObject.GetComponent<Animator>().SetBool("isMoving", false);
        }
        else
        {
            Vector3 movePos = moveDir.normalized * Stat.Speed * Time.deltaTime;
            PosInfo.PosX += movePos.x;
            PosInfo.PosY += movePos.y;
            Vector3 v = new Vector3(movePos.x, movePos.y, 0f);
            transform.position += v;
            IsMoving = true;
            //gameObject.GetComponent<Animator>().SetBool("isMoving", true);
        }
    }


    public virtual void UseSkill(int skillId)
    {

    }
    // 레벨업 이벤트 보여주기 
    public virtual void LevelUp()
    {
        Debug.Log("레벨업!");
        Debug.Log("현재 레벨: " + Level);
    }
    protected virtual void Shake()
    {

    }
    protected virtual void StartShake()
    {

    }
    protected virtual void StopShake()
    {

    }
}
