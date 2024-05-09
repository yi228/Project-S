using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuffController : CreatureController
{
    // �ο�����ŭ ������ ����ż� �Ѹ� �Ծ ���� ������ �Ǵ� ���� ����
    // �Ѿ˵� �̷������� ����
    protected bool _isBuffed = false;
    protected PlayerController Owner;
    protected TextMeshPro _costText;
    protected int _cost;
    public int Cost { get { return _cost; } set { _cost = value; } }
    protected void InitTextComponent()
    {
        _costText.enableWordWrapping = false;
    }
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾ ������ ȹ�� �� ������
        if (collision.gameObject.name.Contains("Player"))
        {
            Managers.Sound.Play("Effect/GetBuff");
            gameObject.SetActive(false);
        }
    }
    public virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() == Owner)
        {
            Owner = null;
        }
    }
}
