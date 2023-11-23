using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObejctPoolManager : MonoBehaviour
{
    public static ObejctPoolManager instance;

    public int defaultCapacity = 10;
    public int maxPoolSize = 20;
    [SerializeField] private GameObject bossBulletPrefab;
    [SerializeField] private GameObject playerBulletPrefab;

    public IObjectPool<GameObject> bossBulletPool { get; private set; }
    public IObjectPool<GameObject> playerBulletPool { get; private set; }

    void Start()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        Init();
    }
    private void Init()
    {
        bossBulletPool = new ObjectPool<GameObject>(CreateBossBullet, GetItem, ReleaseItem, DeleteItem, true, defaultCapacity, maxPoolSize);
        playerBulletPool = new ObjectPool<GameObject>(CreatePlayerBullet, GetItem, ReleaseItem, DeleteItem, true, defaultCapacity, maxPoolSize);

        for (int i = 0; i < defaultCapacity; i++)
        {
            BossBulletController bossBullet = CreateBossBullet().GetComponent<BossBulletController>();
            bossBullet.Pool.Release(bossBullet.gameObject);
            SingleBulletController playerBullet = CreatePlayerBullet().GetComponent<SingleBulletController>();
            playerBullet.Pool.Release(playerBullet.gameObject);
        }
    }
    private GameObject CreateBossBullet()
    {
        GameObject go = Instantiate(bossBulletPrefab, gameObject.transform);
        go.GetComponent<BossBulletController>().Pool = bossBulletPool;
        return go;
    }
    private GameObject CreatePlayerBullet()
    {
        GameObject go = Instantiate(playerBulletPrefab, gameObject.transform);
        go.GetComponent<SingleBulletController>().Pool = playerBulletPool;
        return go;
    }
    private void GetItem(GameObject _poolItem)
    {
        _poolItem.SetActive(true);
        if (_poolItem.GetComponent<SingleBulletController>() != null)
            _poolItem.GetComponent<SingleBulletController>().IsTouch = false;
    }
    private void ReleaseItem(GameObject _poolItem)
    {
        _poolItem.SetActive(false);
    }
    private void DeleteItem(GameObject _poolItem)
    {
        Destroy(_poolItem);
    }
}
