using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Protocol;
using UnityEngine.UIElements;
using UnityEngine.Rendering.PostProcessing;

public class PortalController : MonoBehaviour
{
    // TODO - �о����
    public GameObject portalDestination;
    private MyPlayerController _myPlayer;
    private GameObject MovingTilt;
    private float stayingTime = 0;
    private float maxStayingTime = 1f;
    private bool _inPortal = false;

    private Vector3 originalScale = new Vector3(0.4f, 0.4f);  // ���� ũ�⸦ ������ ����
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
        // �÷��̾ ��Ż�� ������ �� 
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
        // �÷��̾ ��Ȱ��ȭ �� ��Ż�� ������ �� 
        else if (collision.gameObject.tag == "Player" && portalDestination == null)
        {
            _inPortal = true;
            _myPlayer = collision.GetComponent<MyPlayerController>(); 
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        // �÷��̾ ��Ż���� ���� �� 
        if (collision.gameObject.tag == "Player")
        {
            _inPortal = false;
            _myPlayer.InPortal = false;
            stayingTime = 0;
        }
    }
    private void Teleport()
    {
        // �ݴ��� ��Ż ��Ȱ��ȭ ���·����� �̵� ���� ��
        if (portalDestination == null)
            return;

        stayingTime += Time.deltaTime;

        if (_myPlayer != null && stayingTime >= maxStayingTime && _myPlayer.PortalAvail)
        {
            _myPlayer.IsMoving = false;
            _myPlayer.UseTeleport = true;
            stayingTime = 0;

            // ���� �̵� �Ͼ�� �κ� 
            //_myPlayer.transform.position = portalDestination.transform.position;

            //// �̵� �Ͼ�� MyPlayer ��ũ��Ʈ�� ��ġ ���� ���� �κ�
            //_myPlayer.PosInfo.PosX = _myPlayer.transform.position.x;
            //_myPlayer.PosInfo.PosY = _myPlayer.transform.position.y;

            _myPlayer.InPortal = false;
            // TODO - �������� ����
            //_myPlayer.State = CreatureState.Idle;

            // ���� �̵��� ��Ŷ ������ ����
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