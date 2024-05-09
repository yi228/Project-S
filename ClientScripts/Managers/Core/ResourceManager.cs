using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using Object = UnityEngine.Object;

public class ResourceManager
{
    Dictionary<string, UnityEngine.Object> _resources = new Dictionary<string, UnityEngine.Object>();
    public Dictionary<string, UnityEngine.Object> Res { get { return _resources; }  set { _resources = value; } }
    public void Init()
    {
        //LoadAllAsync<GameObject>("Prefab", (key, count, totalCount) =>
        //{
        //    // 로딩바 사용 가능
        //    Debug.Log($"{key} {count}/{totalCount}");

        //    if (count == totalCount)
        //    {
        //        // 다 완료되고 나서 실행할 함수
        //        foreach (GameObject go in _resources.Values)
        //            Debug.Log(go.name + " 소환 완료");
        //    }
        //});
    }
    #region 어드레서블
    //// 사용 예제 코드
    //Managers.Resource.LoadAllAsync<GameObject>("Prefabs",(key, count, totalCount) =>
    //    {
    //        // 로딩바 사용 가능
    //        Debug.Log($"{key} {count}/{totalCount}");

    //        if (count == totalCount)
    //        {
    //            // 다 완료되고 나서 실행할 함수
    //        }
    //    });
    ////
    public void LoadAsync<T>(string key, Action<T> callback = null) where T : UnityEngine.Object
    {
        // 캐시 확인
        if (_resources.TryGetValue(key, out Object resource))
        {
            callback?.Invoke(resource as T);
            return;
        }
        // 리소스 비동기 로딩 시작
        var asyncOperation = Addressables.LoadAssetAsync<T>(key);
        // 로딩 완료 후에 실행
        asyncOperation.Completed += (op) =>
        {
            _resources.Add(key, op.Result);
            callback?.Invoke(op.Result);
        };
    }
    // 같은 Label인 애들 전부 Load
    public void LoadAllAsync<T>(string label, Action<string, int, int> callback) where T : UnityEngine.Object
    {
        var opHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));
        opHandle.Completed += (op) =>
        {
            int loadCount = 0;
            int totalCount = op.Result.Count;

            foreach (var result in op.Result)
            {
                LoadAsync<T>(result.PrimaryKey, (obj) =>
                {
                    loadCount++;
                    callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
                });
            }
        };
    }
    public T LoadResources<T>(string key) where T : Object
    {
        if (_resources.TryGetValue(key, out Object resource))
            return resource as T;

        return null;
    }
    public GameObject InstantiateResources(string key, Transform parent = null, bool pooling = false)
    {
        GameObject prefab = LoadResources<GameObject>($"{key}");
        if (prefab == null)
        {
            Debug.Log($"Failed to load prefab");
            //Debug.Log($"Failed to load prefab : {key}, Check Local Prefabs");
            //Instantiate($"../../@Resources/{key}");
        }

        GameObject go = Object.Instantiate(prefab, parent);
        go.name = prefab.name;
        return go;
    }
    #endregion
    public T Load<T>(string path) where T : Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if (index >= 0)
                name = name.Substring(index + 1);
        }

        return Resources.Load<T>($"Prefabs/{path}");
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject original = Load<GameObject>(path);
        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;
        return go;
    }

    public void Destroy(GameObject go, float time = 0.0f)
    {
        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc == null)
        {
            if (go == null)
                return;

            Object.Destroy(go, time);
        }
        // 게임안에 ID를 부여 받은 것들은 서버에서도 Leave 처리 해주기
        else
        {
            C_OutGame outPacket = new C_OutGame();
            outPacket.ObjectId = cc.Id;
            Managers.Network.Send(outPacket);
            Object.Destroy(go, time);
        }
    }
}
