using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_DamageText : MonoBehaviour
{
    private Transform _pos;
    public TextMeshProUGUI damageText;
    private TMPAlpha tmpAlpha;
    public int Id;

    public Transform Pos {  get { return _pos; } set { _pos = value; } }

    private void Start()
    {
        tmpAlpha = GetComponentInChildren<TMPAlpha>();
        tmpAlpha.FadeOut();
        Destroy(gameObject, 2f);
        //Debug.Log($"My Owner Id: {Id}");
    }

    void Update()
    {
        if (_pos != null)
            damageText.transform.position = Camera.main.WorldToScreenPoint(_pos.position + new Vector3(0.1f, 0.6f, 0));
        else
            damageText.transform.position = Camera.main.WorldToScreenPoint(transform.position);
    }

    public void SetText(float damage)
    {
        if (damageText != null)
            damageText.text = ((int)damage).ToString();
    }

    //IEnumerator CoDestroy()
    //{
    //    yield return new WaitForSeconds(2f);
    //    Destroy(gameObject);
    //}
}
