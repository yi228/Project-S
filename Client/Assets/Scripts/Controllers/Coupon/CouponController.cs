//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Networking;
//using UnityEditor;
//using TMPro;
//using UnityEngine.SceneManagement;
//using Newtonsoft.Json;
//using Unity.VisualScripting.Antlr3.Runtime;

//public class CouponController : MonoBehaviour
//{
//    // 인풋 필드 할당 해주기
//    public TMP_InputField _inputField;
//    static string couponDownloadPath = "https://evenidemonickitchen.s3.ap-northeast-2.amazonaws.com/CouponData.json";
//    static string saveLocalCouponPath = "Assets/Resources/Data/Json/CouponData.json";
//    public GameObject checkPanel;
//    public TextMeshProUGUI checkPanelText;
//    // 얻을 코인
//    int getCoin;

//    // AWS S3에서 다운로드를 위한 함수 
//    public IEnumerator CoDownLoadCouponData(string URL)
//    {
//        bool isCorrect = false;
//        bool isUsed = false;
//        UnityWebRequest request = UnityWebRequest.Get(URL);

//        yield return request.SendWebRequest();
//        // 에러 발생 시
//        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
//        {
//            Debug.Log(request.error);
//        }
//        else
//        {
//            CouponData[] couponDatas = JsonConvert.DeserializeObject<CouponData[]>(request.downloadHandler.text);
//            foreach (CouponData couponData in couponDatas)
//            {
//                // 인풋 필드의 텍스트랑 서버의 쿠폰 string이 같으면 하고 싶은 일 하기
//                if (_inputField.text == couponData.name)
//                {
//                    isCorrect = true;
//                    // 사용한 적이 없다면 보상 지급
//                    if (PlayerPrefs.GetString($"{couponData.name}") != "used")
//                    {
//                        FinanceManager.coins += couponData.coin;
//                        //FinanceManager.ruby += couponData.ruby;
//                        PlayerPrefs.SetString($"{couponData.name}", "used");
//                        PlayerPrefs.Save();
//                        PlayerDataSaver.SaveAll();
//                        getCoin = couponData.coin;
//                        //checkPanel.SetActive(true);
//                    }
//                    else
//                        isUsed = true;
//                }
//            }
//            if (isCorrect && isUsed)
//            {
//                checkPanel.SetActive(true);
//                checkPanelText.text = $"이미 사용한 쿠폰입니다";
//            }
//            else if (isCorrect && !isUsed)
//            {
//                checkPanel.SetActive(true);
//                checkPanelText.text = $"{getCoin} 코인을 얻었습니다";
//            }
//            // 아닐 경우 하고 싶은 일 하기
//            else
//            {
//                checkPanel.SetActive(true);
//                checkPanelText.text = $"쿠폰 번호가 틀렸습니다";
//            }
//        }
//    }
//    public void DownLoadCouponData()
//    {

//        StartCoroutine(CoDownLoadCouponData(couponDownloadPath));
//    }
//    public void CheckButton()
//    {
//        _inputField.text = "";
//        checkPanel.SetActive(false);
//    }
//}
//public class CouponData
//{
//    public string name;
//    public int coin;
//    public int ruby;
//}


