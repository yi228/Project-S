using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public MyPlayerController MyPlayer { get; set; }
    Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();
    //public Dictionary<int, GameObject> Objects { get { return _objects; } set { _objects = value; } }

    public GameObjectType GetObjectTypeById(int id)
    {
        int type = (id >> 24) & 0x7F;
        return (GameObjectType)type;
    }

    public void Add(ObjectInfo info, bool isMyPlayer = false)
    {
        GameObjectType objectType = GetObjectTypeById(info.ObjectId);
        if (objectType == GameObjectType.Player)
        {
            if (isMyPlayer)
            {
                Debug.Log($"플레이어 Id: {info.ObjectId}의 타입은 {(int)info.WeaponType}, {info.WeaponType}");
                //GameObject go = Managers.Resource.Instantiate($"Creature/MyPlayer");
                GameObject go = Managers.Resource.InstantiateResources($"MyPlayer_{(int)info.WeaponType}");
                go.name = info.Name;
                _objects.Add(info.ObjectId, go);

                MyPlayer = go.GetComponent<MyPlayerController>();
                MyPlayer.Id = info.ObjectId;
                MyPlayer.PosInfo = info.PosInfo;
                MyPlayer.Stat = info.Stat;
                MyPlayer.Gold = info.Gold;
                switch (Managers.Game.MyPlayerWeaponType)
                {
                    case WeaponType.Pistol:
                        MyPlayer.CoolTime = 0.5f;
                        break;
                    case WeaponType.Rifle:
                        MyPlayer.CoolTime = 0.2f;
                        break;
                    case WeaponType.Sniper:
                        MyPlayer.CoolTime = 1.2f;
                        break;
                    case WeaponType.Shotgun:
                        MyPlayer.CoolTime = 1.0f;
                        break;
                }
                Camera.main.orthographicSize = MyPlayer.Stat.CameraSize;
                Managers.Game.MyPlayerId = MyPlayer.Id;
                // 캐릭터 스텟 Init 후 상점 버프 적용
                Managers.Game.UseBuff();
                if (Managers.Game.MyPlayerWeaponType == WeaponType.Shotgun)
                    MyPlayer.IsBulletBuff = true;
                C_ChangeWeaponType weaponPacket = new C_ChangeWeaponType();
                weaponPacket.WeaponType = Managers.Game.MyPlayerWeaponType;
                Managers.Network.Send(weaponPacket);
            }
            else
            {
                Debug.Log($"플레이어 Id: {info.ObjectId}의 타입은 {(int)info.WeaponType}, {info.WeaponType}");
                //GameObject go = Managers.Resource.Instantiate($"Creature/Player");
                GameObject go = Managers.Resource.InstantiateResources($"Player_{(int)info.WeaponType}");
                go.name = info.Name;
                _objects.Add(info.ObjectId, go);

                PlayerController pc = go.GetComponent<PlayerController>();
                pc.Id = info.ObjectId;
                pc.PosInfo = info.PosInfo;
                pc.Stat = info.Stat;
                pc.Gold = info.Gold;
            }
        }
        else if (objectType == GameObjectType.Monster)
        {
            GameObject go = Managers.Resource.InstantiateResources($"{info.Name}");
            go.name = info.Name;
            _objects.Add(info.ObjectId, go);

            MonsterController mc = go.GetComponent<MonsterController>();
            mc.Id = info.ObjectId;
            mc.PosInfo = info.PosInfo;
            mc.Stat = info.Stat;
            mc.Gold = info.Gold;
        }
        else if (objectType == GameObjectType.Projectile)
        {
            // 얘랑 BulletController에서 설정한 애랑 다른 애임
            GameObject go = Managers.Resource.InstantiateResources("Bullet");
            // 나타나자마자 위에 바라 보는 거 방지
            go.SetActive(false);
            go.name = $"Bullet_{info.ObjectId}";
            _objects.Add(info.ObjectId, go);

            BulletController bc = go.GetComponent<BulletController>();
            bc.Id = info.ObjectId;
            bc.PosInfo.PosX = info.PosInfo.PosX;
            bc.PosInfo.PosY = info.PosInfo.PosY;
            bc.Stat = info.Stat;
            bc.transform.position = new Vector3(bc.PosInfo.PosX, bc.PosInfo.PosY, 0);
            //bc.PosInfo.RotZ = MyPlayer.PosInfo.RotZ;
            bc.PosInfo.RotZ = info.PosInfo.RotZ;
            bc.transform.eulerAngles = new Vector3(0, 0, bc.PosInfo.RotZ);
            //Debug.Log(info.PosInfo.RotZ);
            bc.Owner = MyPlayer;
            bc.Dir = MyPlayer.DirVector;
            go.SetActive(true);
            MyPlayer.FireTilt.StartFireBullet();
        }
        else if (objectType == GameObjectType.Buff)
        {
            GameObject go = Managers.Resource.InstantiateResources($"{info.Name}");
            go.name = info.Name;
            _objects.Add(info.ObjectId, go);

            BuffController bc = go.GetComponent<BuffController>();
            bc.Id = info.ObjectId;
            bc.PosInfo = info.PosInfo;
            bc.Stat = new Stat();
            bc.Stat.Cost = info.Stat.Cost;
        }
    }

    public void Remove(int id)
    {
        GameObject go = FindById(id);
        if (go == null)
            return;

        _objects.Remove(id);
        Managers.Resource.Destroy(go);
    }

    public void RemoveAll()
    {
        Clear();
        MyPlayer = null;
    }

    public GameObject FindById(int id)
    {
        GameObject go = null;
        _objects.TryGetValue(id, out go);
        return go;
    }

    public GameObject Find(Func<GameObject, bool> condition)
    {
        foreach (GameObject obj in _objects.Values)
        {
            if (condition.Invoke(obj))
                return obj;
        }

        return null;
    }

    public void Clear()
    {
        foreach (GameObject obj in _objects.Values)
        {
            if (obj != null)
                Managers.Resource.Destroy(obj);
        }
        _objects.Clear();
    }
}