using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SingleMyPlayerController : PlayerController
{
    //[SerializeField] Camera minimapCam;

    //private float[] _coolTimeList = { 0.7f, 0.5f, 0.4f, 0.3f, 0.2f };

    //private int _level = 1;
    private SingleUI_MovingTilt movingTiltUI;
    private SingleUI_FireTilt fireTiltUI;
    private SingleUI_Pause pauseUI;
    private SingleUI_PausePopUp pausePopUpUI;
    private SingleUI_HpBar hpBarUI;
    private SingleUI_Level levelUI;
    private SingleUI_Weapon weaponUI;
    private SingleUI_ExpBar expBarUI;
    private SingleUI_MonsterCount countUI;
    private SpriteRenderer spriteRenderer;
    public SingleUI_MovingTilt MovingTiltUI { get { return movingTiltUI; } set { movingTiltUI = value; } }
    public Transform[] extraBulletPoints;

    private int _hp;
    private int _maxHp;
    private int _attack;
    private float _speed;
    private int _killCount;

    public float BulletScaleBuff = 0f;
    public float BulletSpeedBuff = 0f;

    public new int Hp { get { return _hp; } set { _hp = value; } }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public int Attack { get { return _attack; } set { _attack = value; } }
    public float Speed { get { return _speed; } set { _speed = value; } }
    public int KillCount { get { return _killCount; } set { _killCount = value; } }

    private bool _portalAvail;
    private float _coolTime;
    private bool _inPortal;

    public float CoolTime { get { return _coolTime; } set { _coolTime = value; } }
    public bool PortalAvail { get { return _portalAvail; } set { _portalAvail = value; } }
    public bool InPortal { get { return _inPortal; } set { _inPortal = value; } }
    //public int Level { get { return _level; } set { _level = value; } }

    private int[] reqExp = { 40, 100, 250, 385, 600, 1000 };
    private int levelUpCount = 0;

    public GameObject @Sound;
    void Awake()
    {
        @Sound = GameObject.Find("@Sound");
    }
    void SoundSourceFollowPlayer()
    {
        @Sound.transform.position = transform.position;
    }
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Init();
        SingleGameManager.instance.Player = this;
        //_coolTime = _coolTimeList[_level - 1];
        switch (Managers.Game.MyPlayerWeaponType)
        {
            case WeaponType.Pistol:
                //CoolTime = 0.5f;
                weaponUI.ChangeImage(1);
                break;
            case WeaponType.Rifle:
                //CoolTime = 0.2f;
                weaponUI.ChangeImage(2);
                break;
            case WeaponType.Sniper:
                //CoolTime = 1.2f;
                weaponUI.ChangeImage(3);
                break;
            case WeaponType.Shotgun:
                //CoolTime = 1.0f;
                weaponUI.ChangeImage(4);
                break;
        }
        _portalAvail = true;
        
    }
    void Update()
    {
        SoundSourceFollowPlayer();
        base.UpdateAnimation();
        //Debug.Log($"내 플레이어 ID: {Id}: {State}");
        CheckLevelChanged();
    }
    void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }
    protected override void Init()
    {
        base.Init();
        SingleGameManager.instance.Player = this;
        base.isSingle = true;
        //InitGameUI();
        InitStat();
    }
    public void InitGameUI()
    {
        movingTiltUI = Managers.Resource.Instantiate("SinglePlay/UI/UI_MovingTilt").GetComponent<SingleUI_MovingTilt>();
        movingTiltUI.MyPlayer = this;
        fireTiltUI = Managers.Resource.Instantiate("SinglePlay/UI/UI_FireTilt").GetComponent<SingleUI_FireTilt>();
        fireTiltUI.MyPlayer = this;
        pauseUI = Managers.Resource.Instantiate("SinglePlay/UI/UI_Pause").GetComponent<SingleUI_Pause>();
        pausePopUpUI = Managers.Resource.Instantiate("SinglePlay/UI/UI_PausePopUp").GetComponent<SingleUI_PausePopUp>();
        pausePopUpUI.MyPlayer = this;
        pauseUI.SettingUI = pausePopUpUI.gameObject;
        hpBarUI = Managers.Resource.Instantiate("SinglePlay/UI/UI_HpBar").GetComponent<SingleUI_HpBar>();
        hpBarUI.MyPlayer = this;
        levelUI = Managers.Resource.Instantiate("SinglePlay/UI/UI_Level").GetComponent<SingleUI_Level>();
        levelUI.MyPlayer = this;
        weaponUI = Managers.Resource.Instantiate("SinglePlay/UI/UI_Weapon").GetComponent<SingleUI_Weapon>();
        weaponUI.MyPlayer = this;
        expBarUI = Managers.Resource.Instantiate("SinglePlay/UI/UI_ExpBar").GetComponent<SingleUI_ExpBar>();
        expBarUI.MyPlayer = this;
        countUI = Managers.Resource.Instantiate("SinglePlay/UI/UI_MonsterCount").GetComponent<SingleUI_MonsterCount>();
    }
    private void InitStat()
    {
        switch (Managers.Game.MyPlayerWeaponType)
        {
            case WeaponType.Pistol:
                _hp = 200;
                _maxHp = 200;
                _attack = 30;
                _speed = 5;
                _level = 0;
                _coolTime = 0.5f;
                CameraSize = 5;
                break;
            case WeaponType.Rifle:
                _hp = 200;
                _maxHp = 200;
                _attack = 12;
                _speed = 4;
                _level = 0;
                _coolTime = 0.2f;
                CameraSize = 4.5f;
                break;
            case WeaponType.Sniper:
                _hp = 200;
                _maxHp = 200;
                _attack = 100;
                _speed = 3;
                _level = 0;
                _coolTime = 1.2f;
                CameraSize = 6;
                break;
            case WeaponType.Shotgun:
                _hp = 200;
                _maxHp = 200;
                _attack = 30;
                _speed = 4.2f;
                _level = 0;
                _coolTime = 1f;
                CameraSize = 4.3f;
                break;
        }
        _totalExp = 0;
    }
    public void CloseGameUI()
    {
        movingTiltUI.gameObject.SetActive(false);
        fireTiltUI.gameObject.SetActive(false);
        pauseUI.gameObject.SetActive(false);
        hpBarUI.gameObject.SetActive(false);
        levelUI.gameObject.SetActive(false);
        weaponUI.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어가 아이템 획득 시 
        if (collision.gameObject.tag == "DarkRoom")
        {
            //GameObject go = Managers.Resource.InstantiateResources("DarkroomUI");
            //go.GetComponent<DarkRoomUIController>().MyPlayer = this;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 플레이어가 아이템 획득 시 
        if (collision.gameObject.tag == "DarkRoom")
        {
            //go.GetComponent<DarkRoomUIController>().MyPlayer = this;
        }
    }
    public override void LevelUp()
    {
        if (_levelChanged)
        {
            _levelChanged = false;
            levelUpCount++;
            GameObject effect = Managers.Resource.InstantiateResources("LevelUpEffect");
            effect.GetComponent<EffectController>().Creature = this;
            effect.transform.position = transform.position;
            if (Managers.Sound.SoundOn == true)
                Managers.Sound.Play("Effect/Player/LevelUpSound");
            Destroy(effect, 5f);
            _hp += 15;
            if (_hp > _maxHp)
                _hp = _maxHp;  
            switch (Managers.Game.MyPlayerWeaponType)
            {
                case WeaponType.Pistol:
                    _speed += 0.5f;
                    break;
                case WeaponType.Rifle:
                    BulletScaleBuff += 0.1f;
                    break;
                case WeaponType.Sniper:
                    BulletSpeedBuff += 2f;
                    break;
                case WeaponType.Shotgun:
                    BulletScaleBuff += 0.2f;
                    break;
            }
        }
    }
    public void InitLevel()
    {
        _level = 0;
        _totalExp = 0;
        switch (Managers.Game.MyPlayerWeaponType)
        {
            case WeaponType.Pistol:
                _speed -= 0.5f * levelUpCount;
                break;
            case WeaponType.Rifle:
                BulletScaleBuff -= 0.1f * levelUpCount;
                break;
            case WeaponType.Sniper:
                BulletSpeedBuff -= 2f * levelUpCount;
                break;
            case WeaponType.Shotgun:
                BulletScaleBuff -= 0.2f * levelUpCount;
                break;
        }
    }
    private void CheckLevelChanged()
    {
        if(_totalExp >= reqExp[_level])
        {
            _level++;
            _levelChanged = true;
            LevelUp();
        }
    }
    public Camera mainCamera;
    Vector3 cameraPos;
    float shakeRange = 0.05f;
    float duration = 0.2f;
    protected override void Shake()
    {
        if (Managers.Sound.ShakeOn)
        {
            mainCamera = Camera.main;
            cameraPos = mainCamera.transform.position;
            InvokeRepeating("StartShake", 0f, 0.005f);
            Invoke("StopShake", duration);
        }
    }
    protected override void StartShake()
    {
        float cameraPosX = Random.value * shakeRange * 2 - shakeRange;
        float cameraPosY = Random.value * shakeRange * 2 - shakeRange;
        Vector3 cameraPos = mainCamera.transform.position;
        cameraPos.x += cameraPosX;
        cameraPos.y += cameraPosY;
        mainCamera.transform.position = cameraPos;
    }

    protected override void StopShake()
    {
        CancelInvoke("StartShake");
        mainCamera.transform.position = cameraPos;
    }
    public void ResetFlag()
    {
        StartCoroutine(CoResetFlag());
    }
    IEnumerator CoResetFlag()
    {
        PortalAvail = false;
        PortalCool.IconVisible();
        yield return new WaitForSeconds(5f);
        PortalAvail = true;
        PortalCool.IconInvisible();
    }
    public override void OnDamaged()
    {
        base.OnDamaged();
        //테스트용 풀피
        //_hp = 200;
        if (_hp <= 0)
            SingleGameManager.instance.GameOver();
        if (Managers.Sound.SoundOn == true)
            Managers.Sound.Play("Effect/Player/HitSound");
    }
    public void OnBossDamaged(int damage)
    {
        OnDamaged();
        //색변경
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.4f);
        gameObject.tag = "NotPlayer";
        Invoke("OffDamaged", 2);
    }
    void OffDamaged()
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
        gameObject.tag = "Player";
    }
}
