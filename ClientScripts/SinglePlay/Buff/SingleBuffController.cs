using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SingleBuffController : MonoBehaviour
{
    // 인원수만큼 버프가 적용돼서 한명만 먹어도 전부 버프가 되는 것을 방지
    // 총알도 이런식으로 막음
    protected bool _isBuffed = false;
    protected PlayerController Owner;
    protected TextMeshPro _costText;
    protected int _cost;
    public int Cost { get { return _cost; } set { _cost = value; } }
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어가 아이템 획득 시 없어짐
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
