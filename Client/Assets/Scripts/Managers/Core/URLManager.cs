using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class URLManager
{
    // 파싱할 JSON 파일의 서버 주소
    public string JsonPath = "https://evenidemonickitchen.s3.ap-northeast-2.amazonaws.com/CouponData.json";

    public string Ec2Url = "https://evenidemonickitchen.s3.ap-northeast-2.amazonaws.com/EC2Url.json";
}