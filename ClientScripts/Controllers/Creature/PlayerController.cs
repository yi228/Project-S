using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CreatureController
{
    protected List<Transform> _bulletPoint = new List<Transform>();
    protected Coroutine _coSkill;
    protected UI_MovingTilt _movingTilt;
    protected UI_FireTilt _fireTilt;
    protected UI_Weapon _weaponUI;
    protected UI_Timer _timerUI;
    protected UI_Pause _pauseUI;
    protected UI_HpBar _hpBarUI;
    protected UI_Minimap _minimapUI;
    protected UI_Gold _goldUI;
    protected UI_Level _levelUI;
    protected UI_PortalCool _portalCoolUI;
    protected UI_SystemTextViewer _systemTextViewerUI;
    protected UI_PortalDes _portalDesUI;
    protected UI_ExpBar _expBarUI;
    protected UI_Latency _latencyUI;
    // 방어막
    protected GameObject _block;
    protected int _bulletNum = 1;
    protected WeaponType _weaponType;
    protected bool _isUsePortal = false;

    public int BulletNum { get { return _bulletNum; } set { _bulletNum = value; } }
    public bool IsBulletBuff = false;
    public List<Transform> BulletPoint { get { return _bulletPoint; } set { _bulletPoint = value; } }
    public Coroutine CoSkill { get { return _coSkill; } set { _coSkill = value; } }
    public UI_MovingTilt MovingTilt { get { return _movingTilt; } set { _movingTilt = value; } }
    public UI_FireTilt FireTilt { get { return _fireTilt; } set { _fireTilt = value; } }
    public UI_Weapon WeaponUI { get { return _weaponUI; } set { _weaponUI = value; } }
    public UI_Timer TimerUI { get { return _timerUI; } set { _timerUI = value; } }
    public UI_Pause PauseUI { get { return _pauseUI; } set { _pauseUI = value; } }
    public UI_HpBar HpBarUI { get { return _hpBarUI; } set { _hpBarUI = value; } }
    public UI_Minimap MinimapUI { get { return _minimapUI; } set { _minimapUI = value; } }
    public UI_Gold GoldUI { get { return _goldUI; } set { _goldUI = value; } }
    public UI_Level LevelUI { get { return _levelUI; } set { _levelUI = value; } }
    public UI_PortalCool PortalCool { get { return _portalCoolUI; } set { _portalCoolUI = value; } }
    public UI_SystemTextViewer SystemTextViewerUI { get { return _systemTextViewerUI; } set { _systemTextViewerUI = value; } }
    public UI_PortalDes PortalDes { get { return _portalDesUI; } set { _portalDesUI = value; } }
    public UI_ExpBar ExpBarUI { get { return _expBarUI; } set { _expBarUI = value; } }
    public UI_Latency LatencyUI { get { return _latencyUI; } set { _latencyUI = value; } }
    public GameObject SettingUI;
    public WeaponType WeaponType { get { return _weaponType; } set { _weaponType = value; } }
    public bool IsUsePortal { get { return _isUsePortal; } set { _isUsePortal = value; } }
    public bool CanLightBuff = false;
    protected bool isSingle = false;

    private void Start()
    {
        Init();
    }
    public void UpdatePositionPlayer(Vector3 destPos, float rotZ)
    {
        if (UseTeleport == true)
            return;

        if (IsMoving == false)
            return;
        // 각도 수정
        PosInfo.RotZ = rotZ;
        transform.eulerAngles = new Vector3(0, 0, rotZ);

        while (true)
        {
            // 해당 함수를 destPos에 대해 처음 실행할 때
            if (isPosUpdating == false)
            {
                prevPos = transform.position;
                isPosUpdating = true;
                originDestPos = destPos;
            }

            Vector3 moveDir = originDestPos - transform.position;
            float dist = moveDir.magnitude;
            if (dist <= Stat.Speed * Time.deltaTime)
            {
                PosInfo.PosX = originDestPos.x;
                PosInfo.PosY = originDestPos.y;
                IsMoving = false;
                isPosUpdating = false;
                originDestPos = Vector3.zero;
                break;
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
    }
    private void Update()
    {
        if (!isSingle)
        {
            //if (IsMoving == true)
            //    base.UpdatePosition();
            if (UseTeleport == true && originDestPos != Vector3.zero)
            {
                PosInfo.PosX = originDestPos.x;
                PosInfo.PosY = originDestPos.y;
                IsMoving = false;
                isPosUpdating = false;
                originDestPos = Vector3.zero;
            }
            else if (IsMoving == true)
                base.UpdatePosition();
            //Debug.Log($"다른 플레이어 ID: {Id}: {State}");
        }
    }
    public override void UseSkill(int skillId)
    {
        // TODO - 1은 총 쏘기로 임시 설정
        if (skillId == 1)
        {
            // ObjectManager에서 총알 스폰되고 바로 실행하게 함
            //_coSkill = StartCoroutine(_fireTilt.CoStartFireBullet());
        }
    }
    protected override void UpdateSkill() // 공격 함수
    {

    }
    protected override void UpdateAnimation()
    {
        switch (State)
        {
            case CreatureState.Idle:
                _animator.SetBool("isMoving", false);
                break;
            case CreatureState.Moving:
                _animator.SetBool("isMoving", true);
                break;
            case CreatureState.Skill:
                _animator.SetBool("isMoving", false);
                _animator.SetTrigger("doAttack");
                break;
            case CreatureState.Dead:
                _animator.SetBool("isMoving", false);
                break;
        }
    }
    protected void InitBulletPoint()
    {
        Transform[] children = GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.name == "BulletPoint")
                BulletPoint.Add(child);
        }
    }

    public void AddBlock()
    {
        _block = Managers.Resource.Instantiate("Buff/Block");
        _block.transform.position = transform.position;
        _block.transform.SetParent(transform);
    }
    public void BreakBlock()
    {
        Debug.Log("부서짐");
        _block.SetActive(false);
    }
    protected override void Init()
    {
        base.Init();
        InitBulletPoint();
    }
}