using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Protocol;
using UnityEngine.UIElements;
using UnityEngine.Rendering.PostProcessing;

public class PortalController : MonoBehaviour
{
    // TODO - 읽어오기
    public GameObject portalDestination;
    private MyPlayerController _myPlayer;
    private GameObject MovingTilt;
    private float stayingTime = 0;
    private float maxStayingTime = 1f;
    private bool _inPortal = false;

    private Vector3 originalScale = new Vector3(0.4f, 0.4f);  // 원래 크기를 저장할 변수
    private GameObject effect;

    private void Start()
    {
        effect = gameObject.GetComponentInChildren<ParticleSystem>().gameObject;
    }
    private void Update()
    {
        if (_inPortal)
        {
            effect.SetActive(true);
            Teleport();
        }
        else
        {
            if (effect != null)
                effect.SetActive(false);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        // 플레이어가 포탈에 입장할 시 
        if (collision.gameObject.tag == "Player")
        {
            //StartCoroutine(CoRotateSprite());
            _inPortal = true;
            _myPlayer = collision.GetComponent<MyPlayerController>();
            _myPlayer.InPortal = true;
            if (portalDestination != null)
                _myPlayer.PortalDes.Destination = portalDestination.transform;
            else
                _myPlayer.PortalDes.Destination = null;
        }
        // 플레이어가 비활성화 된 포탈에 입장할 시 
        else if (collision.gameObject.tag == "Player" && portalDestination == null)
        {
            _inPortal = true;
            _myPlayer = collision.GetComponent<MyPlayerController>(); 
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        // 플레이어가 포탈에서 나갈 시 
        if (collision.gameObject.tag == "Player")
        {
            _inPortal = false;
            _myPlayer.InPortal = false;
            stayingTime = 0;
        }
    }
    private void Teleport()
    {
        // 반대편 포탈 비활성화 상태로인해 이동 못할 때
        if (portalDestination == null)
            return;

        stayingTime += Time.deltaTime;

        if (_myPlayer != null && stayingTime >= maxStayingTime && _myPlayer.PortalAvail)
        {
            _myPlayer.IsMoving = false;
            _myPlayer.UseTeleport = true;
            stayingTime = 0;

            // 실제 이동 일어나는 부분 
            //_myPlayer.transform.position = portalDestination.transform.position;

            //// 이동 일어난후 MyPlayer 스크립트에 위치 정보 저장 부분
            //_myPlayer.PosInfo.PosX = _myPlayer.transform.position.x;
            //_myPlayer.PosInfo.PosY = _myPlayer.transform.position.y;

            _myPlayer.InPortal = false;
            // TODO - 서버에서 변경
            //_myPlayer.State = CreatureState.Idle;

            // 순간 이동시 패킷 서버에 전송
            C_Move movePacket = new C_Move();
            movePacket.PosInfo = new PositionInfo();
            movePacket.PosInfo.PosX = portalDestination.transform.position.x;
            movePacket.PosInfo.PosY = portalDestination.transform.position.y;
            movePacket.UseTeleport = true;
            Managers.Network.Send(movePacket);
            _myPlayer.ResetFlag();
            _inPortal = false;
        }
    }
    //IEnumerator CoResetFlag()
    //{
    //    _myPlayer.PortalAvail = false;
    //    _myPlayer.PortalCool.IconVisible();
    //    yield return new WaitForSeconds(5f);
    //    _myPlayer.PortalAvail = true;
    //    _myPlayer.PortalCool.IconInvisible();
    //}
}