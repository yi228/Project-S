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
        // ���� ȸ������ �����ɴϴ�.
        Quaternion currentRotation = loading_icon.transform.rotation;

        // z ���� �������� ȸ�� ������ ������ŵ�ϴ�.
        float newZRotation = currentRotation.eulerAngles.z - 360 * Time.deltaTime;

        // ������ ������ ���ο� Quaternion�� ����ϴ�.
        Quaternion newRotation = Quaternion.Euler(0, 0, newZRotation);

        // ������Ʈ�� ȸ���� ���ο� ȸ�������� �����մϴ�.
        loading_icon.transform.rotation = newRotation;
    }
}
