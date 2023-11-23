using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public float _scanRange;
    private RaycastHit2D[] _targets;
    public GameObject _nearestTarget;

    void FixedUpdate()
    {
        _targets = Physics2D.CircleCastAll(transform.position, _scanRange, Vector2.zero, 0, LayerMask.GetMask("Player"));
        _nearestTarget = GetNearest();
    }

    GameObject GetNearest() //가장 가까운 플레이어 찾기
    {
        GameObject result = null;
        float diff = 100;

        foreach (RaycastHit2D target in _targets)
        {
            Vector3 myPos = transform.position;
            Vector3 targetPos = target.transform.position;
            float curDiff = Vector3.Distance(myPos, targetPos);

            if (curDiff < diff)
            {
                diff = curDiff;
                result = target.collider.gameObject;
            }
        }

        return result;
    }
}