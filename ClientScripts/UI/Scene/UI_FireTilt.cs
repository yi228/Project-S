using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Google.Protobuf.Protocol;

public class UI_FireTilt : UI_Scene, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    enum Images
    {
        FireTiltBackground,
        FireTiltButton,
    }
    private MyPlayerController _myPlayer;
    private static Vector2 _dirVector;
    private float _radious;
    public static bool IsTouchFireTilt = false;
    private bool _isDirVector = false;
    public MyPlayerController MyPlayer { get { return _myPlayer; } set { _myPlayer = value; } }
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

        if (_coSkillCooltime == null && _dirVector != Vector2.zero && IsTouchFireTilt && _isDirVector)
        {
            C_Move movePacket = new C_Move();
            movePacket.PosInfo = MyPlayer.PosInfo;
            //movePacket.PosInfo.RotZ = MyPlayer.transform.eulerAngles.z;
            Managers.Network.Send(movePacket);

            //Debug.Log($"Bullet Dir({DirVector.x}, {DirVector.y})");
            C_Skill skill = new C_Skill() { Info = new SkillInfo() };
            skill.Stat = new Stat();
            skill.Info.SkillId = 1;
            // 방향 벡터
            DirVector dir = new DirVector();
            dir.PosX = DirVector.x;
            dir.PosY = DirVector.y;
            skill.DirVector.Add(dir);
            //skill.PlayerRotz = MyPlayer.transform.eulerAngles.z;
            // 총알 위치
            PositionInfo bulletPos = new PositionInfo();
            if (MyPlayer.BulletPoint[0] != null)
            {
                bulletPos.PosX = MyPlayer.BulletPoint[0].position.x;
                bulletPos.PosY = MyPlayer.BulletPoint[0].position.y;
                skill.PosInfo.Add(bulletPos);
                // 버프 여부 - 현재는 샷건에만 적용해줌 (2023.06.01)
                skill.IsBulletBuff = MyPlayer.IsBulletBuff;
                if (skill.IsBulletBuff)
                {
                    for (int i = 1; i < MyPlayer.BulletPoint.Count; i++)
                    {
                        PositionInfo bp = new PositionInfo();
                        bp.PosX = MyPlayer.BulletPoint[i].position.x;
                        bp.PosY = MyPlayer.BulletPoint[i].position.y;
                        skill.PosInfo.Add(bp);
                        // 산탄총의 방향 벡터 구하기
                        Vector3 value = (MyPlayer.BulletPoint[i].position - MyPlayer.transform.position).normalized;
                        DirVector otherDir = new DirVector();
                        otherDir.PosX = value.x;
                        otherDir.PosY = value.y;
                        skill.DirVector.Add(otherDir);
                    }
                }
                skill.WeaponType = MyPlayer.WeaponType;
                Managers.Network.Send(skill);
                //Managers.Sound.PlayFireSound();
                _coSkillCooltime = StartCoroutine(CoFireCooltime(MyPlayer.CoolTime));
            }
        }

    }

    Coroutine _coSkillCooltime;
    IEnumerator CoFireCooltime(float time)
    {
        yield return new WaitForSeconds(time);
        _coSkillCooltime = null;
    }
    public void StartFireBullet()
    {
        StartCoroutine(CoStartFireBullet());
    }
    public IEnumerator CoStartFireBullet()
    {
        yield return new WaitForSeconds(MyPlayer.CoolTime);
        C_ChangeState statePacket = new C_ChangeState();
        //statePacket.State = CreatureState.Idle;
        Managers.Network.Send(statePacket);
        MyPlayer.CoSkill = null;
    }
    PointerEventData pointerData;
    public void OnDrag(PointerEventData eventData)
    {
        pointerData = eventData;
        //// 플레이어 회전 코드
        //if (UI_MovingTilt.IsTouchMovingTilt == false && UI_FireTilt.IsTouchFireTilt == true)
        //{
        //    TurnPlayer(eventData);
        //}

        //// 플레이어 회전 코드
        //if (UI_MovingTilt.IsTouchMovingTilt == true && UI_FireTilt.IsTouchFireTilt == true)
        //{
        //    TurnPlayer(eventData);
        //}
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

        //MyPlayer.transform.eulerAngles = new Vector3(0, 0, -Mathf.Atan2(value.x, value.y) * Mathf.Rad2Deg);
        MyPlayer.PosInfo.RotZ = -Mathf.Atan2(value.x, value.y) * Mathf.Rad2Deg;

        if (previousRot != MyPlayer.PosInfo.RotZ)
        {
            // 각도 변환 패킷 서버에 전송
            C_ChangeRotz rotzPacket = new C_ChangeRotz();
            rotzPacket.RotZ = MyPlayer.PosInfo.RotZ;
            Managers.Network.Send(rotzPacket);

        }
        //if (previousRot != _myPlayer.transform.eulerAngles.z)
        //{
        //    // 각도 변환 패킷 서버에 전송
        //    C_ChangeRotz rotzPacket = new C_ChangeRotz();
        //    rotzPacket.RotZ = MyPlayer.PosInfo.RotZ;
        //    Managers.Network.Send(rotzPacket);
        //}
    }
}