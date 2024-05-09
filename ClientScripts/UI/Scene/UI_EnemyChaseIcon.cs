using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using Image = UnityEngine.UI.Image;

public class UI_EnemyChaseIcon : MonoBehaviour
{
    private MonsterController _monster;
    private SingleMonsterController _singleMonster;

    public MonsterController Monster { get { return _monster; } set { _monster = value; } }
    public SingleMonsterController SingleMonster { get { return _singleMonster; } set { _singleMonster = value; } }

    private Image icon;

    private float lerpTime = 0.8f;

    void Start()
    {
        icon = GetComponentInChildren<Image>();
        IconOn();
    }
    void Update()
    {
        if (_monster != null)
            icon.transform.position = _monster.transform.position + new Vector3(0.1f, 1.5f, 0);
        else if(_singleMonster != null)
            icon.transform.position = _singleMonster.transform.position + new Vector3(0.1f, 1.5f, 0);
    }
    public void IconOn()
    {
        StartCoroutine(AlphaLerp(0, 1));
    }
    public void IconOff()
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

            Color color = icon.color;
            color.a = Mathf.Lerp(start, end, percent);
            icon.color = color;

            yield return null;
        }
    }
}
