using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
    private GameObject loading_icon;
    void Start()
    {
        loading_icon = GameObject.Find("loading_Icon");
    }

    // Update is called once per frame
    void Update()
    {
        // 현재 회전값을 가져옵니다.
        Quaternion currentRotation = loading_icon.transform.rotation;

        // z 축을 기준으로 회전 각도를 증가시킵니다.
        float newZRotation = currentRotation.eulerAngles.z - 360 * Time.deltaTime;

        // 수정된 각도로 새로운 Quaternion을 만듭니다.
        Quaternion newRotation = Quaternion.Euler(0, 0, newZRotation);

        // 오브젝트의 회전을 새로운 회전값으로 설정합니다.
        loading_icon.transform.rotation = newRotation;
    }
}
