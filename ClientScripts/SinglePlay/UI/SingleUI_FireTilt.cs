using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Google.Protobuf.Protocol;

public class SingleUI_FireTilt : UI_Scene, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    enum Images
    {
        FireTiltBackground,
        FireTiltButton,
    }
    private SingleMyPlayerController _myPlayer;
    private static Vector2 _dirVector;
    private float _radious;
    public static bool IsTouchFireTilt = false;
    private bool _isDirVector = false;
    private bool canFire = true;
    public SingleMyPlayerController MyPlayer { get { return _myPlayer; } set { _myPlayer = value; } }
    public static Vector2 DirVector { get { return _dirVector; } set { _dirVector = value; } }
    void Start()
    {
        Bind<Image>(typeof(Images));
        _radious = GetImage((int)Images.FireTiltBackground).rectTransform.rect.width * 0.5f;

        // 첫 시작은 방향 벡터가 없어서 임의 지정
        if (_dirVector == Vector2.zero)
        {
            _dirVector = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        if (IsTouchFireTilt == true)
            TurnPlayer(pointerData);

        if (canFire && IsTouchFireTilt)
        {
            //GameObject _bullet = Managers.Resource.Instantiate("SinglePlay/Creature/Bullet");
            GameObject _bullet = ObejctPoolManager.instance.playerBulletPool.Get();
            _bullet.GetComponent<SingleBulletController>().Owner = _myPlayer;
            _bullet.GetComponent<SingleBulletController>().Dir = _myPlayer.transform.up;
            _bullet.transform.position = MyPlayer.BulletPoint[0].position;
            _bullet.transform.rotation = MyPlayer.BulletPoint[0].rotation;
            _bullet.transform.localScale = new Vector3(1 + _bullet.GetComponent<SingleBulletController>().Owner.BulletScaleBuff,
                1 + _bullet.GetComponent<SingleBulletController>().Owner.BulletScaleBuff, 1);
            string _gunSoundPath;
            switch (Managers.Game.MyPlayerWeaponType)
            {
                case WeaponType.Pistol:
                    _gunSoundPath = "Effect/Weapon/Pistol/Pistol_Fire";
                    break;
                case WeaponType.Rifle:
                    _gunSoundPath = "Effect/Weapon/Rifle/Rifle_Fire";
                    break;
                case WeaponType.Sniper:
                    _gunSoundPath = "Effect/Weapon/Sniper/Sniper_Fire"; 
                    break;
                case WeaponType.Shotgun:
                    _gunSoundPath = "Effect/Weapon/Shotgun/Shotgun_Fire";
                    break;
                default:
                    _gunSoundPath = "";
                    break;
            }
            if (Managers.Sound.SoundOn == true)
                Managers.Sound.Play(_gunSoundPath);
            if (Managers.Game.MyPlayerWeaponType == WeaponType.Shotgun)
            {
                for(int i = 0; i < 2; i++)
                {
                    GameObject _exBullet = ObejctPoolManager.instance.playerBulletPool.Get();
                    _exBullet.GetComponent<SingleBulletController>().Owner = _myPlayer;
                    _exBullet.transform.position = _myPlayer.extraBulletPoints[i].position;
                    Vector3 value = (MyPlayer.extraBulletPoints[i].position - MyPlayer.transform.position).normalized;
                    _exBullet.GetComponent<SingleBulletController>().Dir = value;
                    _exBullet.transform.rotation = MyPlayer.extraBulletPoints[i].rotation;
                    _exBullet.transform.localScale = new Vector3(1 + _exBullet.GetComponent<SingleBulletController>().Owner.BulletScaleBuff,
                        1 + _exBullet.GetComponent<SingleBulletController>().Owner.BulletScaleBuff, 1);
                }
            }
            StartCoroutine(CoFireCooltime(_myPlayer.CoolTime));
        }
    }

    IEnumerator CoFireCooltime(float time)
    {
        canFire = false;
        _myPlayer.State = CreatureState.Skill;
        _myPlayer.State = CreatureState.Idle;
        yield return new WaitForSeconds(time);
        canFire = true;
    }

    PointerEventData pointerData;
    public void OnDrag(PointerEventData eventData)
    {
        pointerData = eventData;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsTouchFireTilt = true;
        _isDirVector = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        IsTouchFireTilt = false;
        GetImage((int)Images.FireTiltButton).rectTransform.localPosition = Vector2.zero;
        // TODO - 서버에서 바꾸기
        //_myPlayer.State = CreatureState.Idle;
    }
    public void TurnPlayer(PointerEventData eventData)
    {
        if (eventData == null)
            return;
        Vector2 value = eventData.position - (Vector2)GetImage((int)Images.FireTiltBackground).rectTransform.position;

        value = Vector2.ClampMagnitude(value, _radious);
        GetImage((int)Images.FireTiltButton).rectTransform.localPosition = value;
        value = value.normalized;
        if (value != Vector2.zero)
        {
            _dirVector = value;
            MyPlayer.DirVector = _dirVector;
        }
        float previousRot = MyPlayer.PosInfo.RotZ;

        MyPlayer.transform.eulerAngles = new Vector3(0, 0, -Mathf.Atan2(value.x, value.y) * Mathf.Rad2Deg);
    }
}
