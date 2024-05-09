using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Google.Protobuf.Protocol;
using System;

public class UI_MovingTilt : UI_Scene, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    enum Images
    {
        MovingTiltBackground,
        MovingTiltButton,
    }
    private MyPlayerController _myPlayer;
    private static Vector2 _dirVector;
    private float _radious;
    public bool _closeToWall;
    public static bool IsTouchMovingTilt = false;
    public Vector3 _movePosition;
    private Vector2 value;
    float _packetTick;
    public MyPlayerController MyPlayer { get { return _myPlayer; } set { _myPlayer = value; } }
    public static Vector2 DirVector { get { return _dirVector; } set { _dirVector = value; } }
    void Start()
    {
        Bind<Image>(typeof(Images));

        _radious = GetImage((int)Images.MovingTiltBackground).rectTransform.rect.width * 0.5f;
        if (_dirVector == Vector2.zero)
        {
            _dirVector = Vector2.zero;
        }
        _packetTick = Managers.Game.PacketTick;
    }
    float _packetTimer = 0f;
    //float _packetTick = 0.05f;
   
    void Update()
    {
        //Debug.Log($"({(int)Math.Round(MyPlayer.transform.position.x)}, {(int)Math.Round(MyPlayer.transform.position.y)}) 갈 수 있음? " + Managers.Map.CanGo(MyPlayer.transform.position));
        if (MyPlayer == null)
            return;
        _packetTimer += Time.deltaTime;

        _closeToWall = Physics2D.Raycast(MyPlayer.transform.position, _movePosition.normalized, 1, LayerMask.GetMask("Wall"));

        if (MyPlayer.IsMoving == false)
        {
            //MyPlayer.IsMoving = true;
            //MyPlayer.transform.position += _movePosition;
        }

        if (IsTouchMovingTilt == true && MyPlayer.IsMoving == false)
        {
            CreatureState prevState = MyPlayer.State;
            Vector3 prevPos = _myPlayer.transform.position;

            if (!_closeToWall)
            {
                // TODO - 이동
                MyPlayer.gameObject.GetComponent<Animator>().SetBool("isMoving", true);
            }

            MyPlayer.PosInfo.PosX = _myPlayer.transform.position.x;
            MyPlayer.PosInfo.PosY = _myPlayer.transform.position.y;

            value = value.normalized;

            if (value != Vector2.zero)
            {
                _dirVector = value;
                MyPlayer.DirVector = _dirVector;
            }
            _movePosition = new Vector3
                (value.x * _myPlayer.Stat.Speed * Time.deltaTime,
                value.y * _myPlayer.Stat.Speed * Time.deltaTime,
                0f);
            //MyPlayer.UpdatePosition(_movePosition + MyPlayer.transform.position, MyPlayer.Stat.Speed);
            //Debug.Log($"movePos: {_movePosition}");

            if (UI_MovingTilt.IsTouchMovingTilt == true && UI_FireTilt.IsTouchFireTilt == false)
            {
                if (value != Vector2.zero)
                {
                    UI_FireTilt.DirVector = value;
                    MyPlayer.DirVector = UI_FireTilt.DirVector;
                }
                float previousRot = _myPlayer.transform.eulerAngles.z;
                _myPlayer.PosInfo.RotZ = _myPlayer.transform.eulerAngles.z;
                _myPlayer.transform.eulerAngles = new Vector3(0, 0, -Mathf.Atan2(value.x, value.y) * Mathf.Rad2Deg);

            }
            else
            {
                // TODO - 이동
                _myPlayer.gameObject.GetComponent<Animator>().SetBool("isMoving", false);
            }
        }
        else
        {
            // TODO - 이동
            _myPlayer.gameObject.GetComponent<Animator>().SetBool("isMoving", false);
        }
        //// 이동 안 할 땐 굳이 패킷 보내주지 않기
        //if (_movePosition.x == 0f && _movePosition.y == 0f)
        //    return;
        //if (_closeToWall == false)
        {
            Vector3 destPos = new Vector3(MyPlayer.PosInfo.PosX + _movePosition.x, MyPlayer.PosInfo.PosY + _movePosition.y, 0);
            if (Managers.Map.CanGo(destPos))
            {
                MyPlayer.PosInfo.PosX += _movePosition.x;
                MyPlayer.PosInfo.PosY += _movePosition.y;
                MyPlayer.PosInfo.RotZ = MyPlayer.transform.eulerAngles.z;
                MyPlayer.transform.position = new Vector3(MyPlayer.PosInfo.PosX, MyPlayer.PosInfo.PosY, 0f);
            }
        }
        //MyPlayer.transform.position = _movePosition;
        if (_packetTimer >= _packetTick && MyPlayer.IsUsePortal == false)
        {
            C_Move movePacket = new C_Move();
            Managers.Game.PrevLatency = DateTime.Now;
            movePacket.PosInfo = MyPlayer.PosInfo;
            //movePacket.PosInfo.RotZ = MyPlayer.transform.eulerAngles.z;
            Managers.Network.Send(movePacket);
            //_myPlayer.gameObject.GetComponent<Animator>().SetBool("isMoving", true);
            _packetTimer = 0f;
        }
        //MyPlayer.UpdatePosition();
    }
    public void OnDrag(PointerEventData eventData)
    {
        value = eventData.position - (Vector2)GetImage((int)Images.MovingTiltBackground).rectTransform.position;
        // 조이스틱 원안에 가둬두기
        value = Vector2.ClampMagnitude(value, _radious);
        // 조이스틱 원안에서 터치된 곳으로 이동시키기
        GetImage((int)Images.MovingTiltButton).rectTransform.localPosition = value;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        IsTouchMovingTilt = true;
        //_myPlayer.State = CreatureState.Moving;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        IsTouchMovingTilt = false;
        GetImage((int)Images.MovingTiltButton).rectTransform.localPosition = Vector2.zero;
        _movePosition = Vector3.zero;
        //_myPlayer.State = CreatureState.Idle;
    }
}