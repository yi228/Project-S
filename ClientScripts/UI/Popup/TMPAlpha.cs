using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Google.Protobuf.Protocol;

public class TMPAlpha : MonoBehaviour
{
    // 러프 계열 함수 이용해서 시스템 메세지 없어지는 시간 제어 
    [SerializeField]
    private float lerpTime;
    private TextMeshProUGUI text;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    public void FadeOut()
    {
        StartCoroutine(AlphaLerp(1, 0));
    }
    private IEnumerator AlphaLerp(float start, float end)
    {
        float currentTime = 0.0f;
        float percent = 0.0f;

        while (percent < 1.0f) 
        {
            currentTime += Time.deltaTime;
            percent = currentTime / lerpTime;

            Color color = text.color;
            color.a = Mathf.Lerp(start, end, percent);
            text.color = color;

            yield return null;
        }
    }
}
