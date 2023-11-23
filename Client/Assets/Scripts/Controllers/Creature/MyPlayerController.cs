using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Protocol;
using Unity.VisualScripting;
using UnityEngine.Rendering.PostProcessing;

public class MyPlayerController : PlayerController
{
    Camera minimapCam;

    private bool _portalAvail;
    private float _coolTime;
    private bool _inPortal;
    // PP 용도
    PostProcessProfile _ppProfile;
    ChromaticAberration _ca;
    Grain _grain;
    Define.PPType ppType = Define.PPType.Idle;
    float onLerpTime = 1.0f;
    float offLerpTime = 2.0f;
    float currentTime = 0f;
    float CAValue = 0f;
    float GrainValue = 0f;
    float origin_CAValue = 0f;
    float origin_GrainValue = 0f;

    public float CoolTime { get { return _coolTime; } set { _coolTime = value; } }
    public bool PortalAvail { get { return _portalAvail; } set { _portalAvail = value; } }
    public bool InPortal { get { return _inPortal; } set { _inPortal = value; } }
    public bool IsWinner = false;
    //public int Level { get { return _level; } set { _level = value; } }
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
        Init();
        //_coolTime = _coolTimeList[_level - 1];
        switch (Managers.Game.MyPlayerWeaponType)
        {
            case WeaponType.Pistol:
                //CoolTime = 0.5f;
                _weaponUI.ChangeImage(1);
                break;
            case WeaponType.Rifle:
                //CoolTime = 0.2f;
                _weaponUI.ChangeImage(2);
                break;
            case WeaponType.Sniper:
                //CoolTime = 1.2f;
                _weaponUI.ChangeImage(3);
                break;
            case WeaponType.Shotgun:
                //CoolTime = 1.0f;
                _weaponUI.ChangeImage(4);
                break;
        }
        minimapCam = Managers.Resource.InstantiateResources("MinimapCamera").GetComponent<Camera>();
        _portalAvail = true;
    }
    void UpdatePP()
    {
        switch (ppType)
        {
            case Define.PPType.Update:
                OnPP();
                break;
            case Define.PPType.Stop:
                OffPP();
                break;
        }
    }
    void OnPP()
    {
        if (currentTime >= onLerpTime)
        {
            ppType = Define.PPType.Stop;
            currentTime = 0;
            origin_CAValue = CAValue;
            origin_GrainValue = GrainValue;
            return;
        }
        currentTime += Time.deltaTime;
        CAValue = Mathf.Lerp(0, 1, currentTime / onLerpTime);
        GrainValue = Mathf.Lerp(0, 1, currentTime / onLerpTime);
        _ca.intensity.value = CAValue;
        _grain.intensity.value = GrainValue;
    }
    void OffPP()
    {
        if (currentTime >= offLerpTime)
        {
            ppType = Define.PPType.Idle;
            currentTime = 0;
            return;
        }
        currentTime += Time.deltaTime;
        CAValue = Mathf.Lerp(origin_CAValue, 0, currentTime / offLerpTime);
        GrainValue = Mathf.Lerp(origin_GrainValue, 0, currentTime / offLerpTime);
        _ca.intensity.value = CAValue;
        _grain.intensity.value = GrainValue;
    }
    void Update()
    {
        SoundSourceFollowPlayer();
        base.UpdateAnimation();
        UpdateMinimapCam();
        UpdatePP();
    }
    void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }
    protected override void Init()
    {
        base.Init();
        InitGameUI();
        InitPP();
    }
    protected void InitGameUI()
    {
        _movingTilt = Managers.Resource.InstantiateResources("UI_MovingTilt").GetComponent<UI_MovingTilt>();
        _movingTilt.MyPlayer = this;
        _fireTilt = Managers.Resource.InstantiateResources("UI_FireTilt").GetComponent<UI_FireTilt>();
        _fireTilt.MyPlayer = this;
        _weaponUI = Managers.Resource.InstantiateResources("UI_Weapon").GetComponent<UI_Weapon>();
        _weaponUI.MyPlayer = this;
        _timerUI = Managers.Resource.InstantiateResources("UI_Timer").GetComponent<UI_Timer>();
        _pauseUI = Managers.Resource.InstantiateResources("UI_Pause").GetComponent<UI_Pause>();
        _hpBarUI = Managers.Resource.InstantiateResources("UI_HpBar").GetComponent<UI_HpBar>();
        _hpBarUI.MyPlayer = this;
        _minimapUI = Managers.Resource.InstantiateResources("UI_Minimap").GetComponent<UI_Minimap>();
        _goldUI = Managers.Resource.InstantiateResources("UI_Gold").GetComponent<UI_Gold>();
        //TODO - 어드레서블
        _levelUI = Managers.Resource.Instantiate("UI/Scene/UI_Level").GetComponent<UI_Level>();
        _levelUI.MyPlayer = this;
        _portalCoolUI = Managers.Resource.InstantiateResources("UI_PortalCool").GetComponent<UI_PortalCool>();
        _portalCoolUI.MyPlayer = this;
        _systemTextViewerUI = Managers.Resource.InstantiateResources("UI_SystemTextViewer").GetComponent<UI_SystemTextViewer>();
        _portalDesUI = Managers.Resource.InstantiateResources("UI_PortalDes").GetComponent<UI_PortalDes>();
        _portalDesUI.MyPlayer = this;
        //TODO - 어드레서블
        _expBarUI = Managers.Resource.Instantiate("UI/Scene/UI_ExpBar").GetComponent<UI_ExpBar>();
        _expBarUI.MyPlayer = this;
        //TODO - 어드레서블
        _latencyUI = Managers.Resource.Instantiate("UI/Popup/UI_Latency").GetComponent<UI_Latency>();
        SettingUI = Managers.Resource.InstantiateResources("Pause");
        SettingUI.GetComponent<UI_PausePopup>().MyPlayer = this;
        SettingUI.SetActive(false);
        //_weaponUI.MyPlayer = this;
    }
    void InitPP()
    {
        _ppProfile = GameObject.Find("PP Volume_Ingame").GetComponent<PostProcessVolume>().profile;
        _ppProfile.TryGetSettings<ChromaticAberration>(out _ca);
        _ppProfile.TryGetSettings<Grain>(out _grain);
    }
    public void CloseGameUI()
    {
        _portalDesUI.gameObject.SetActive(false);
        _systemTextViewerUI.gameObject.SetActive(false);
        _levelUI.gameObject.SetActive(false);
        _goldUI.gameObject.SetActive(false);
        _minimapUI.gameObject.SetActive(false);
        _hpBarUI.gameObject.SetActive(false);
        _pauseUI.gameObject.SetActive(false);
        _timerUI.gameObject.SetActive(false);
        _weaponUI.gameObject.SetActive(false);
        _fireTilt.gameObject.SetActive(false);
        _movingTilt.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Portal")
        {
            PortalController pc = collision.gameObject.GetComponent<PortalController>();
            if (PortalAvail == true && pc.portalDestination != null)
            {
                ppType = Define.PPType.Update;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Portal" && ppType == Define.PPType.Update)
        {
            ppType = Define.PPType.Stop;
            origin_CAValue = CAValue;
            origin_GrainValue = GrainValue;
        }
    }
    public override void LevelUp()
    {
        if (_levelChanged)
        {
            _levelChanged = false;
            GameObject effect = Managers.Resource.InstantiateResources("LevelUpEffect");
            effect.GetComponent<EffectController>().Creature = this;
            effect.transform.position = transform.position;
            if (Managers.Sound.SoundOn == true)
                Managers.Sound.Play("Effect/Player/LevelUpSound");
            GameObject.Destroy(effect, 5f);
        }
    }

    private void UpdateMinimapCam()
    {
        if (transform.position.x <= -50 && transform.position.x >= -90)
        {
            if (transform.position.y <= 90 && transform.position.y >= 50)
            {
                minimapCam.transform.position = new Vector3(-70, 70, -10);
            }
            else if (transform.position.y <= -50 && transform.position.y >= -90)
            {
                minimapCam.transform.position = new Vector3(-70, -70, -10);
            }
        }
        else if (transform.position.x <= 90 && transform.position.x >= 50)
        {
            if (transform.position.y <= 90 && transform.position.y >= 50)
            {
                minimapCam.transform.position = new Vector3(70, 70, -10);
            }
            else if (transform.position.y <= -50 && transform.position.y >= -90)
            {
                minimapCam.transform.position = new Vector3(70, -70, -10);
            }
        }
        else if (transform.position.x <= 15 && transform.position.x >= -15)
        {
            if (transform.position.y <= 15 && transform.position.y >= -15)
            {
                minimapCam.transform.position = new Vector3(0, 0, -10);
            }
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
    }
}