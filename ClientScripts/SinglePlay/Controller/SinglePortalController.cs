using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePortalController : MonoBehaviour
{
    private float stayingTime = 0;
    private float maxStayingTime = 1f;
    private bool _inPortal = false;

    private Vector3 originalScale = new Vector3(0.4f, 0.4f);  // ���� ũ�⸦ ������ ����
    private GameObject effect;

    private SingleMyPlayerController _myPlayer;

    private void Start()
    {
        effect = gameObject.GetComponentInChildren<ParticleSystem>().gameObject;
    }
    private void Update()
    {
        if (_inPortal)
        {
            effect.SetActive(true);
            CountTime();
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
            _myPlayer = collision.gameObject.GetComponent<SingleMyPlayerController>();
            _inPortal = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        // �÷��̾ ��Ż���� ���� �� 
        if (collision.gameObject.tag == "Player")
        {
            _inPortal = false;
            stayingTime = 0;
        }
    }
    private void CountTime()
    {
        stayingTime += Time.deltaTime;

        if (SingleGameManager.instance.bossKilled && stayingTime >= maxStayingTime)
        {
            stayingTime = 0;
            _myPlayer.IsMoving = false;
            _myPlayer.UseTeleport = true;
            SingleGameManager.instance.StageClear();
        }
    }
}
