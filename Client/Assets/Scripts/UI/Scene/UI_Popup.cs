using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup : UI_Base
{
    protected CanvasGroup canvasGroup;
    protected Canvas canvas;
    protected Vector3 endPosition = new Vector3(0, 0, 0);
    protected Vector3 startPosition;
    public Define.UIAnimation Type = Define.UIAnimation.Idle;
    protected float currentTime = 0f;
    public float lerpTime = 0.5f;
    protected bool showAnimation = false;
    protected virtual void UpdateAnimation()
    {
        switch (Type)
        {
            case Define.UIAnimation.Up:
                ShowPopup();
                break;
            case Define.UIAnimation.Down:
                ClosePopup();
                break;
        }
    }
    public override void Init()
    {
        if (showAnimation)
        {
            canvas = GetComponent<Canvas>();
            canvasGroup = Util.GetOrAddComponent<CanvasGroup>(gameObject);
            canvasGroup.alpha = 0;
            // 밑으로 위치 초기화
            transform.position = new Vector3(0, -13, 0);
            startPosition = transform.position;
        }
    }
    // 애니메이션 효과 원하면 함수에 type만 바꿔주면 됨 추가 해주면 됨
    public virtual void ShowPopup()
    {
        if (showAnimation)
        {
            canvasGroup.interactable = false;
            currentTime += Time.deltaTime;
            if (currentTime >= lerpTime)
                currentTime = lerpTime;
            transform.position = Vector3.Lerp(startPosition, endPosition, currentTime / lerpTime);
            canvasGroup.alpha = Mathf.Lerp(0, 1, currentTime / lerpTime);
            // 도착 시 정지
            if (transform.position == endPosition)
            {
                canvasGroup.interactable = true;
                Type = Define.UIAnimation.Idle;
                currentTime = 0f;
            }
        }
        else
        {
            transform.position = endPosition;
            canvasGroup.interactable = true;
        }
    }
    public virtual void ClosePopup()
    {
        if (showAnimation)
        {
            canvasGroup.interactable = false;
            currentTime += Time.deltaTime;
            if (currentTime >= lerpTime)
                currentTime = lerpTime;
            transform.position = Vector3.Lerp(endPosition, startPosition, currentTime / lerpTime);
            canvasGroup.alpha = Mathf.Lerp(1, 0, currentTime / lerpTime);
            // 도착 시 정지
            if (transform.position == startPosition)
            {
                canvasGroup.interactable = true;
                Type = Define.UIAnimation.Idle;
                currentTime = 0f;
                Managers.Game.ChooseCharacter.SetActive(true);
            }
        }
        else
        {
            transform.position = endPosition;
            canvasGroup.interactable = true;
            Managers.Game.ChooseCharacter.SetActive(true);
        }
    }
    public virtual void ClosePopupUI()
    {
        Managers.Sound.Play("Effect/ClickButton");
        if (showAnimation)
            Type = Define.UIAnimation.Down;
        Managers.Game.ChooseCharacter.SetActive(true);
        gameObject.SetActive(false);
    }
}
