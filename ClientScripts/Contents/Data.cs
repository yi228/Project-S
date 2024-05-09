using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Data
{
    // 필요한 JsonData 클래스 만들고 Managers의 CoDataManagerInit()에 추가
    public class JsonData
    {
        public string name;
        public int coin;
        public int ruby;
    }
}
