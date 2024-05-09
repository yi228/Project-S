using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectileController : MonoBehaviour
{
    [SerializeField] private int _bulletSpeed;
    public int bulletDamage;
    private Vector3 _dir;
    public Vector3 Dir { get { return _dir; } set { _dir = value; } }

    void Update()
    {
        if (_dir != null)
        {
            _dir = _dir.normalized;
            transform.position = transform.position + _dir * _bulletSpeed * Time.deltaTime;
        }
    }
}
