using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BuffManual : UI_Scene
{
    Sprite redButtonSprite;
    Sprite whiteButtonSprite;

    Color selectedColor = new Color32(255, 255, 255, 255);
    Color nonSelectedColor = new Color32(24, 74, 120, 255);
    enum Texts
    {
        TitleText,
        ManualText,
    }
    enum Images
    {
        AttackBuff_Manual,
        HpUpBuff_Manual,
        LightBuff_Manual,
        SightBuff_Manual,
        SpeedBuff_Manual,
    }
    enum Buttons
    {
        CloseButton,
        AttackBuffManualButton,
        HpUpBuffManualButton,
        LightBuffManualButton,
        SightBuffManualButton,
        SpeedBuffManualButton,
    }
    void Start()
    {
        // 선택 버튼 Sprite Load
        redButtonSprite = Resources.Load<Sprite>("Art/Icon/Btn_MainButton_Red");
        whiteButtonSprite = Resources.Load<Sprite>("Art/Icon/Btn_MainButton_White");

        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));

        // 디폴트로 권총만 보여줌
        GetImage((int)Images.AttackBuff_Manual).gameObject.SetActive(true);
        GetImage((int)Images.HpUpBuff_Manual).gameObject.SetActive(false);
        GetImage((int)Images.LightBuff_Manual).gameObject.SetActive(false);
        GetImage((int)Images.SightBuff_Manual).gameObject.SetActive(false);
        GetImage((int)Images.SpeedBuff_Manual).gameObject.SetActive(false);

        GetButton((int)Buttons.CloseButton).onClick.AddListener(ClosePopup);

        GetButton((int)Buttons.AttackBuffManualButton).onClick.AddListener(ChooseAttackBuffManual);
        GetButton((int)Buttons.HpUpBuffManualButton).onClick.AddListener(ChooseHpUpBuffManual);
        GetButton((int)Buttons.LightBuffManualButton).onClick.AddListener(ChooseLightBuffManual);
        GetButton((int)Buttons.SightBuffManualButton).onClick.AddListener(ChooseSightBuffManual);
        GetButton((int)Buttons.SpeedBuffManualButton).onClick.AddListener(ChooseSpeedBuffManual);
    }
    void ChooseAttackBuffManual()
    {
        Managers.Sound.Play("Effect/ClickButton");

        // 메뉴얼 변경
        GetImage((int)Images.AttackBuff_Manual).gameObject.SetActive(true);
        GetImage((int)Images.HpUpBuff_Manual).gameObject.SetActive(false);
        GetImage((int)Images.LightBuff_Manual).gameObject.SetActive(false);
        GetImage((int)Images.SightBuff_Manual).gameObject.SetActive(false);
        GetImage((int)Images.SpeedBuff_Manual).gameObject.SetActive(false);

        // 선택 버튼 텍스트 색상 변경
        GetButton((int)Buttons.AttackBuffManualButton).GetComponent<Image>().sprite = redButtonSprite;
        TextMeshProUGUI redText = GetButton((int)Buttons.AttackBuffManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        redText.color = selectedColor;

        // 그 외에 버튼 텍스트 색상 변경
        GetButton((int)Buttons.HpUpBuffManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text1 = GetButton((int)Buttons.HpUpBuffManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text1.color = nonSelectedColor;

        GetButton((int)Buttons.LightBuffManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text2 = GetButton((int)Buttons.LightBuffManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text2.color = nonSelectedColor;

        GetButton((int)Buttons.SightBuffManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text3 = GetButton((int)Buttons.SightBuffManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text3.color = nonSelectedColor;

        GetButton((int)Buttons.SpeedBuffManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text4 = GetButton((int)Buttons.SpeedBuffManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text4.color = nonSelectedColor;
    }

    void ChooseHpUpBuffManual()
    {
        Managers.Sound.Play("Effect/ClickButton");

        // 메뉴얼 변경
        GetImage((int)Images.AttackBuff_Manual).gameObject.SetActive(false);
        GetImage((int)Images.HpUpBuff_Manual).gameObject.SetActive(true);
        GetImage((int)Images.LightBuff_Manual).gameObject.SetActive(false);
        GetImage((int)Images.SightBuff_Manual).gameObject.SetActive(false);
        GetImage((int)Images.SpeedBuff_Manual).gameObject.SetActive(false);

        // 선택 버튼 텍스트 색상 변경
        GetButton((int)Buttons.HpUpBuffManualButton).GetComponent<Image>().sprite = redButtonSprite;
        TextMeshProUGUI redText = GetButton((int)Buttons.HpUpBuffManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        redText.color = selectedColor;

        // 그 외에 버튼 텍스트 색상 변경
        GetButton((int)Buttons.AttackBuffManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text1 = GetButton((int)Buttons.AttackBuffManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text1.color = nonSelectedColor;

        GetButton((int)Buttons.LightBuffManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text2 = GetButton((int)Buttons.LightBuffManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text2.color = nonSelectedColor;

        GetButton((int)Buttons.SightBuffManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text3 = GetButton((int)Buttons.SightBuffManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text3.color = nonSelectedColor;

        GetButton((int)Buttons.SpeedBuffManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text4 = GetButton((int)Buttons.SpeedBuffManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text4.color = nonSelectedColor;
    }
    void ChooseLightBuffManual()
    {
        Managers.Sound.Play("Effect/ClickButton");

        // 메뉴얼 변경
        GetImage((int)Images.AttackBuff_Manual).gameObject.SetActive(false);
        GetImage((int)Images.HpUpBuff_Manual).gameObject.SetActive(false);
        GetImage((int)Images.LightBuff_Manual).gameObject.SetActive(true);
        GetImage((int)Images.SightBuff_Manual).gameObject.SetActive(false);
        GetImage((int)Images.SpeedBuff_Manual).gameObject.SetActive(false);

        // 선택 버튼 텍스트 색상 변경
        GetButton((int)Buttons.LightBuffManualButton).GetComponent<Image>().sprite = redButtonSprite;
        TextMeshProUGUI redText = GetButton((int)Buttons.LightBuffManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        redText.color = selectedColor;

        // 그 외에 버튼 텍스트 색상 변경
        GetButton((int)Buttons.HpUpBuffManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text1 = GetButton((int)Buttons.HpUpBuffManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text1.color = nonSelectedColor;

        GetButton((int)Buttons.AttackBuffManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text2 = GetButton((int)Buttons.AttackBuffManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text2.color = nonSelectedColor;

        GetButton((int)Buttons.SightBuffManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text3 = GetButton((int)Buttons.SightBuffManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text3.color = nonSelectedColor;

        GetButton((int)Buttons.SpeedBuffManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text4 = GetButton((int)Buttons.SpeedBuffManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text4.color = nonSelectedColor;
    }
    void ChooseSightBuffManual()
    {
        Managers.Sound.Play("Effect/ClickButton");

        // 메뉴얼 변경
        GetImage((int)Images.AttackBuff_Manual).gameObject.SetActive(false);
        GetImage((int)Images.HpUpBuff_Manual).gameObject.SetActive(false);
        GetImage((int)Images.LightBuff_Manual).gameObject.SetActive(false);
        GetImage((int)Images.SightBuff_Manual).gameObject.SetActive(true);
        GetImage((int)Images.SpeedBuff_Manual).gameObject.SetActive(false);

        // 선택 버튼 텍스트 색상 변경
        GetButton((int)Buttons.SightBuffManualButton).GetComponent<Image>().sprite = redButtonSprite;
        TextMeshProUGUI redText = GetButton((int)Buttons.SightBuffManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        redText.color = selectedColor;

        // 그 외에 버튼 텍스트 색상 변경
        GetButton((int)Buttons.HpUpBuffManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text1 = GetButton((int)Buttons.HpUpBuffManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text1.color = nonSelectedColor;

        GetButton((int)Buttons.LightBuffManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text2 = GetButton((int)Buttons.LightBuffManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text2.color = nonSelectedColor;

        GetButton((int)Buttons.AttackBuffManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text3 = GetButton((int)Buttons.AttackBuffManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text3.color = nonSelectedColor;

        GetButton((int)Buttons.SpeedBuffManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text4 = GetButton((int)Buttons.SpeedBuffManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text4.color = nonSelectedColor;
    }
    void ChooseSpeedBuffManual()
    {
        Managers.Sound.Play("Effect/ClickButton");

        // 메뉴얼 변경
        GetImage((int)Images.AttackBuff_Manual).gameObject.SetActive(false);
        GetImage((int)Images.HpUpBuff_Manual).gameObject.SetActive(false);
        GetImage((int)Images.LightBuff_Manual).gameObject.SetActive(false);
        GetImage((int)Images.SightBuff_Manual).gameObject.SetActive(false);
        GetImage((int)Images.SpeedBuff_Manual).gameObject.SetActive(true);

        // 선택 버튼 텍스트 색상 변경
        GetButton((int)Buttons.SpeedBuffManualButton).GetComponent<Image>().sprite = redButtonSprite;
        TextMeshProUGUI redText = GetButton((int)Buttons.SpeedBuffManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        redText.color = selectedColor;

        // 그 외에 버튼 텍스트 색상 변경
        GetButton((int)Buttons.HpUpBuffManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text1 = GetButton((int)Buttons.HpUpBuffManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text1.color = nonSelectedColor;

        GetButton((int)Buttons.LightBuffManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text2 = GetButton((int)Buttons.LightBuffManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text2.color = nonSelectedColor;

        GetButton((int)Buttons.SightBuffManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text3 = GetButton((int)Buttons.SightBuffManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text3.color = nonSelectedColor;

        GetButton((int)Buttons.AttackBuffManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text4 = GetButton((int)Buttons.AttackBuffManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text4.color = nonSelectedColor;
    }
    void ClosePopup()
    {
        Managers.Sound.Play("Effect/ClickButton");
        Managers.Game.ChooseCharacter.SetActive(true);
        gameObject.SetActive(false);
    }
}
