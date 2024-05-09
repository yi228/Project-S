using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DataManager
{
    public Dictionary<string, Data.JsonData[]> jsonDict { get; private set; } = new Dictionary<string, Data.JsonData[]>();
    //public float Bullet1Speed = 5f;
    // AWS S3에서 다운로드를 위한 함수
    // 필요할 때마다 추가해서 만들어주기
    public IEnumerator CoDownLoadJsonData()
    {
        string URL = Managers.URL.JsonPath;
        // json 이름 = 딕셔너리 key
        string jsonName = "JsonData";
        UnityWebRequest request = UnityWebRequest.Get(URL);

        yield return request.SendWebRequest();
        // 에러 발생 시
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        // 에러 없을 시
        else
        {
            // S3의 JSON 파일 읽어와서 넣어주기
            Data.JsonData[] jsonDatas = JsonConvert.DeserializeObject<Data.JsonData[]>(request.downloadHandler.text);
            // 캐슁해서 쓰기위해 딕셔너리에 넣어두기
            jsonDict.Add(jsonName, jsonDatas);
            // 그 뒤 값 처리 해주는 부분
            foreach (Data.JsonData jsonData in jsonDatas)
            {
                Debug.Log(jsonData.name);
                Debug.Log(jsonData.coin);
                Debug.Log(jsonData.ruby);
            }
        }
    }
    public void Init()
    {

    }
}
