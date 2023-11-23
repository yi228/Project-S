using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private Transform[] _spawnPoints;
    private GameObject _monster;
    [SerializeField]
    private int _sectionId;

    void Awake()
    {
        _spawnPoints = GetComponentsInChildren<Transform>();

        //int monSelect = Random.Range(1, 3);
        //switch (_sectionId)
        //{
        //    case 1:
        //        if(monSelect == 1)
        //            _monster = Managers.Resource.Load<GameObject>("Creature/Bat");
        //        else
        //            _monster = Managers.Resource.Load<GameObject>("Creature/Dragon");
        //        break;
        //    case 2:
        //        if(monSelect == 1)
        //            _monster = Managers.Resource.Load<GameObject>("Creature/EvilMage");
        //        else
        //            _monster = Managers.Resource.Load<GameObject>("Creature/Spector");
        //        break;
        //    case 3:
        //        if(monSelect == 1)
        //            _monster = Managers.Resource.Load<GameObject>("Creature/Lizard");
        //        else
        //            _monster = Managers.Resource.Load<GameObject>("Creature/MonsterPlant");
        //        break;
        //    case 4:
        //            _monster = Managers.Resource.Load<GameObject>("Creature/Golem");
        //        break;
        //}
    }

    void Start()
    {
        //Spawn();
    }

    private void Spawn() //몬스터 위치 랜덤 스폰
    {
        Transform point = _spawnPoints[Random.Range(1, _spawnPoints.Length)];
        Instantiate(_monster, point);
    }
}
