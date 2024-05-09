using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Google.Protobuf.Protocol;

public class SingleUI_MovingTilt : UI_Scene, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    enum Images
    {
        MovingTiltBackground,
        MovingTiltButton,
    }
    private SingleMyPlayerController _myPlayer;
    private static Vector2 _dirVector;
    private float _radious;
    public bool _closeToWall;
    public static bool IsTouchMovingTilt = false;
    public Vector3 _movePosition;
    private Vector2 value;
    public SingleMyPlayerController MyPlayer { get { return _myPlayer; } set { _myPlayer = value; } }
    public static Vector2 DirVector { get { return _dirVector; } set { _dirVector = value; } }

    [SerializeField]
    private GameObject _spawnPoint0, _spawnPoint1, _spawnPoint2, _spawnPoint3, parentPlayer;

    void Start()
    {
        Bind<Image>(typeof(Images));

        _radious = GetImage((int)Images.MovingTiltBackground).rectTransform.rect.width * 0.5f;
        if (_dirVector == Vector2.zero)
        {
            _dirVector = Vector2.zero;
        }
        _spawnPoint0 = GameObject.Find("SpawnPoint0");
        _spawnPoint1 = GameObject.Find("SpawnPoint1");
        _spawnPoint2 = GameObject.Find("SpawnPoint2");
        _spawnPoint3 = GameObject.Find("SpawnPoint3");
        parentPlayer = GameObject.Find("Player");
    }
    void Update()
    {
        if (MyPlayer == null)
            return;

        _closeToWall = Physics2D.Raycast(MyPlayer.transform.position, _movePosition.normalized, 1, LayerMask.GetMask("Wall"));

        if (IsTouchMovingTilt == true && MyPlayer.IsMoving == false)
        {
            //CreatureState prevState = MyPlayer.State;
            //Vector3 prevPos = _myPlayer.transform.position;

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
                (value.x * _myPlayer.Speed * Time.deltaTime,
                value.y * _myPlayer.Speed * Time.deltaTime,
                0f);

            if (IsTouchMovingTilt == true && SingleUI_FireTilt.IsTouchFireTilt == false)
            {
                if (value != Vector2.zero)
                {
                    UI_FireTilt.DirVector = value;
                    MyPlayer.DirVector = UI_FireTilt.DirVector;
                }
                //float previousRot = _myPlayer.transform.eulerAngles.z;
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
        Vector3 destPos = new Vector3(MyPlayer.PosInfo.PosX + _movePosition.x, MyPlayer.PosInfo.PosY + _movePosition.y, 0);

        MyPlayer.PosInfo.PosX += _movePosition.x;
        MyPlayer.PosInfo.PosY += _movePosition.y;
        MyPlayer.PosInfo.RotZ = MyPlayer.transform.eulerAngles.z;
        if (MyPlayer.PosInfo.PosX == 0f && MyPlayer.PosInfo.PosY == 0f)
        {
            SetPlayerStagePos();
        }
        else if(!MyPlayer.UseTeleport)
        {
            MyPlayer.transform.position = new Vector3(MyPlayer.PosInfo.PosX, MyPlayer.PosInfo.PosY, 0f);
        }
    }
    public void SetPlayerStagePos()
    {
        switch (Stage.currentStage)
        {
            case 0:
                Debug.Log("Stage 0");
                MyPlayer.transform.position = _spawnPoint0.transform.position;
                MyPlayer.PosInfo.PosX = _spawnPoint0.transform.position.x;
                MyPlayer.PosInfo.PosY = _spawnPoint0.transform.position.y;
                break;
            case 1:
                Debug.Log("Stage 1");
                MyPlayer.transform.position = _spawnPoint1.transform.position;
                MyPlayer.PosInfo.PosX = _spawnPoint1.transform.position.x;
                MyPlayer.PosInfo.PosY = _spawnPoint1.transform.position.y;
                break;
            case 2:
                Debug.Log("Stage 2");
                MyPlayer.transform.position = _spawnPoint2.transform.position;
                MyPlayer.PosInfo.PosX = _spawnPoint2.transform.position.x;
                MyPlayer.PosInfo.PosY = _spawnPoint2.transform.position.y;
                break;
            case 3:
                Debug.Log("Stage 3");
                MyPlayer.transform.position = _spawnPoint3.transform.position;
                MyPlayer.PosInfo.PosX = _spawnPoint3.transform.position.x;
                MyPlayer.PosInfo.PosY = _spawnPoint3.transform.position.y;
                break;
        }
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
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        IsTouchMovingTilt = false;
        GetImage((int)Images.MovingTiltButton).rectTransform.localPosition = Vector2.zero;
        _movePosition = Vector3.zero;
    }
}
