using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BulletController : CreatureController
{
    private MyPlayerController _owner;
    private Vector2 _dir;
    public MyPlayerController Owner { get { return _owner; } set { _owner = value; } }
    public Vector2 Dir { get { return _dir; } set { _dir = value; } }
    // 서버에서 여러번 연산 하는 거 막기
    public bool IsTouch = false;
    protected override void Init()
    {
        base.Init();
    }
    private void Start()
    {
        Init();
        //transform.eulerAngles = new Vector3(0, 0, MyPlayer.PosInfo.RotZ);
    }

    private void Update()
    {
        //transform.Translate(Dir * 5f * Time.fixedDeltaTime);
        if (IsMoving == true)
        {
            //Debug.Log(Dir);
            PosInfo.PosX += (Dir.x * Time.deltaTime * Stat.Speed);
            PosInfo.PosY += (Dir.y * Time.deltaTime * Stat.Speed);
            transform.position += new Vector3((Dir.x * Time.deltaTime * Stat.Speed), (Dir.y * Time.deltaTime * Stat.Speed), 0);
        }
        //if (IsMoving == true)
        //    base.UpdatePosition();
        //Debug.Log(Stat.Speed);
        //if (Inclination != Vector3.zero)
        //{
        //    transform.Translate(Inclination * Time.deltaTime);
        //    Debug.Log("Moving: " + Inclination * Time.deltaTime);
        //}
    }
    public override void UpdatePosition()
    {

    }
    private void OnDamagingPlayer(PlayerController player)
    {
        // 플레이어가 맞았을 때
        if (player.tag == "NotPlayer")
        {
            PlayerController enemy = player;
            if (enemy == null)
            {
                Debug.Log("No Owner");
                return;
            }
            // hit 요청 서버에 하기
            // 총알을 서버에서 Leave 시켜야 에러 안날듯
            C_HitPlayer hitPacket = new C_HitPlayer();
            //  때린 Player 정보 전달
            hitPacket.HitterObjectInfo = new ObjectInfo();
            hitPacket.HitterObjectInfo.ObjectId = Owner.Id;
            hitPacket.HitterObjectInfo.Stat = Owner.Stat;

            //  맞은 Player 또는 Monster 정보 전달
            hitPacket.EnemyObjectInfo = new ObjectInfo();
            hitPacket.EnemyObjectInfo.ObjectId = enemy.Id;
            hitPacket.EnemyObjectInfo.Stat = enemy.Stat;
            hitPacket.BulletId = this.Id;

            Managers.Network.Send(hitPacket);
        }
    }
    private void OnDamagingMonster(MonsterController monster)
    {
        // 몬스터가 맞았을 때
        if (monster.tag == "Monster")
        {
            MonsterController enemy = monster;
            if (enemy == null)
            {
                Debug.Log("No Enemy");
                return;
            }
            else
            {
                // hit 요청 서버에 하기
                // 총알을 서버에서 Leave 시켜야 에러 안날듯
                C_HitMonster hitPacket = new C_HitMonster();
                //  때린 Player 정보 전달
                hitPacket.HitterObjectInfo = new ObjectInfo();
                hitPacket.HitterObjectInfo.ObjectId = Owner.Id;
                hitPacket.HitterObjectInfo.Stat = Owner.Stat;

                //  맞은 Player 또는 Monster 정보 전달
                hitPacket.EnemyObjectInfo = new ObjectInfo();
                hitPacket.EnemyObjectInfo.ObjectId = enemy.Id;
                hitPacket.EnemyObjectInfo.Stat = enemy.Stat;
                hitPacket.BulletId = this.Id;

                Managers.Network.Send(hitPacket);
            }
        }
    }
    public Vector3 Inclination = Vector3.zero;
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            Managers.Object.Remove(Id);
        }
        if (IsTouch == false)
        {
            CreatureController cc = collision.transform.GetComponent<CreatureController>();
            if (cc == null)
                return;
            GameObjectType type = Managers.Object.GetObjectTypeById(cc.Id);
            if (!(type == GameObjectType.Player || type == GameObjectType.Monster))
                return;
            if (cc.Id == Owner.Id)
                return;
            IsTouch = true;
            switch (type)
            {
                case GameObjectType.Player:
                    OnDamagingPlayer(collision.transform.GetComponent<PlayerController>());
                    break;
                case GameObjectType.Monster:
                    OnDamagingMonster(collision.transform.GetComponent<MonsterController>());
                    break;
            }
        }
    }
}