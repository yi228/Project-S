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
//    // ��ǲ �ʵ� �Ҵ� ���ֱ�
//    public TMP_InputField _inputField;
//    static string couponDownloadPath = "https://evenidemonickitchen.s3.ap-northeast-2.amazonaws.com/CouponData.json";
//    static string saveLocalCouponPath = "Assets/Resources/Data/Json/CouponData.json";
//    public GameObject checkPanel;
//    public TextMeshProUGUI checkPanelText;
//    // ���� ����
//    int getCoin;

//    // AWS S3���� �ٿ�ε带 ���� �Լ� 
//    public IEnumerator CoDownLoadCouponData(string URL)
//    {
//        bool isCorrect = false;
//        bool isUsed = false;
//        UnityWebRequest request = UnityWebRequest.Get(URL);

//        yield return request.SendWebRequest();
//        // ���� �߻� ��
//        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
//        {
//            Debug.Log(request.error);
//        }
//        else
//        {
//            CouponData[] couponDatas = JsonConvert.DeserializeObject<CouponData[]>(request.downloadHandler.text);
//            foreach (CouponData couponData in couponDatas)
//            {
//                // ��ǲ �ʵ��� �ؽ�Ʈ�� ������ ���� string�� ������ �ϰ� ���� �� �ϱ�
//                if (_inputField.text == couponData.name)
//                {
//                    isCorrect = true;
//                    // ����� ���� ���ٸ� ���� ����
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
//                checkPanelText.text = $"�̹� ����� �����Դϴ�";
//            }
//            else if (isCorrect && !isUsed)
//            {
//                checkPanel.SetActive(true);
//                checkPanelText.text = $"{getCoin} ������ ������ϴ�";
//            }
//            // �ƴ� ��� �ϰ� ���� �� �ϱ�
//            else
//            {
//                checkPanel.SetActive(true);
//                checkPanelText.text = $"���� ��ȣ�� Ʋ�Ƚ��ϴ�";
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


