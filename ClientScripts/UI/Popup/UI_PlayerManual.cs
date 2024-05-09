using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerManual : UI_Scene
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
        Pistol_Manual,
        Rifle_Manual,
        Sniper_Manual,
        Shotgun_Manual,
    }
    enum Buttons
    {
        CloseButton,
        PistolManualButton,
        RifleManualButton,
        SniperManualButton,
        ShotgunManualButton,
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
        GetImage((int)Images.Pistol_Manual).gameObject.SetActive(true);
        GetImage((int)Images.Rifle_Manual).gameObject.SetActive(false);
        GetImage((int)Images.Sniper_Manual).gameObject.SetActive(false);
        GetImage((int)Images.Shotgun_Manual).gameObject.SetActive(false);

        GetButton((int)Buttons.CloseButton).onClick.AddListener(ClosePopup);

        GetButton((int)Buttons.PistolManualButton).onClick.AddListener(ChoosePistolManual);
        GetButton((int)Buttons.RifleManualButton).onClick.AddListener(ChooseRifleManual);
        GetButton((int)Buttons.SniperManualButton).onClick.AddListener(ChooseSniperManual);
        GetButton((int)Buttons.ShotgunManualButton).onClick.AddListener(ChooseShotgunManual);
    }
    void ChoosePistolManual()
    {
        Managers.Sound.Play("Effect/ClickButton");

        // 메뉴얼 변경
        GetImage((int)Images.Pistol_Manual).gameObject.SetActive(true);
        GetImage((int)Images.Rifle_Manual).gameObject.SetActive(false);
        GetImage((int)Images.Sniper_Manual).gameObject.SetActive(false);
        GetImage((int)Images.Shotgun_Manual).gameObject.SetActive(false);

        // 선택 버튼 텍스트 색상 변경
        GetButton((int)Buttons.PistolManualButton).GetComponent<Image>().sprite = redButtonSprite;
        TextMeshProUGUI redText = GetButton((int)Buttons.PistolManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        redText.color = selectedColor;

        // 그 외에 버튼 텍스트 색상 변경
        GetButton((int)Buttons.RifleManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text1 = GetButton((int)Buttons.RifleManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text1.color = nonSelectedColor;

        GetButton((int)Buttons.SniperManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text2 = GetButton((int)Buttons.SniperManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text2.color = nonSelectedColor;

        GetButton((int)Buttons.ShotgunManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text3 = GetButton((int)Buttons.ShotgunManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text3.color = nonSelectedColor;
    }
    void ChooseRifleManual()
    {
        Managers.Sound.Play("Effect/ClickButton");
        GetImage((int)Images.Pistol_Manual).gameObject.SetActive(false);
        GetImage((int)Images.Rifle_Manual).gameObject.SetActive(true);
        GetImage((int)Images.Sniper_Manual).gameObject.SetActive(false);
        GetImage((int)Images.Shotgun_Manual).gameObject.SetActive(false);

        // 선택 버튼 텍스트 색상 변경
        GetButton((int)Buttons.RifleManualButton).GetComponent<Image>().sprite = redButtonSprite;
        TextMeshProUGUI redText = GetButton((int)Buttons.RifleManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        redText.color = selectedColor;

        // 그 외에 버튼 텍스트 색상 변경
        GetButton((int)Buttons.PistolManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text1 = GetButton((int)Buttons.PistolManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text1.color = nonSelectedColor;

        GetButton((int)Buttons.SniperManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text2 = GetButton((int)Buttons.SniperManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text2.color = nonSelectedColor;

        GetButton((int)Buttons.ShotgunManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text3 = GetButton((int)Buttons.ShotgunManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text3.color = nonSelectedColor;
    }
    void ChooseSniperManual()
    {
        Managers.Sound.Play("Effect/ClickButton");
        GetImage((int)Images.Pistol_Manual).gameObject.SetActive(false);
        GetImage((int)Images.Rifle_Manual).gameObject.SetActive(false);
        GetImage((int)Images.Sniper_Manual).gameObject.SetActive(true);
        GetImage((int)Images.Shotgun_Manual).gameObject.SetActive(false);

        // 선택 버튼 텍스트 색상 변경
        GetButton((int)Buttons.SniperManualButton).GetComponent<Image>().sprite = redButtonSprite;
        TextMeshProUGUI redText = GetButton((int)Buttons.SniperManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        redText.color = selectedColor;

        // 그 외에 버튼 텍스트 색상 변경
        GetButton((int)Buttons.RifleManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text1 = GetButton((int)Buttons.RifleManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text1.color = nonSelectedColor;

        GetButton((int)Buttons.PistolManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text2 = GetButton((int)Buttons.PistolManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text2.color = nonSelectedColor;

        GetButton((int)Buttons.ShotgunManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text3 = GetButton((int)Buttons.ShotgunManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text3.color = nonSelectedColor;
    }
    void ChooseShotgunManual()
    {
        Managers.Sound.Play("Effect/ClickButton");
        GetImage((int)Images.Pistol_Manual).gameObject.SetActive(false);
        GetImage((int)Images.Rifle_Manual).gameObject.SetActive(false);
        GetImage((int)Images.Sniper_Manual).gameObject.SetActive(false);
        GetImage((int)Images.Shotgun_Manual).gameObject.SetActive(true);

        // 선택 버튼 텍스트 색상 변경
        GetButton((int)Buttons.ShotgunManualButton).GetComponent<Image>().sprite = redButtonSprite;
        TextMeshProUGUI redText = GetButton((int)Buttons.ShotgunManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        redText.color = selectedColor;

        // 그 외에 버튼 텍스트 색상 변경
        GetButton((int)Buttons.RifleManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text1 = GetButton((int)Buttons.RifleManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text1.color = nonSelectedColor;

        GetButton((int)Buttons.SniperManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text2 = GetButton((int)Buttons.SniperManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text2.color = nonSelectedColor;

        GetButton((int)Buttons.PistolManualButton).GetComponent<Image>().sprite = whiteButtonSprite;
        TextMeshProUGUI text3 = GetButton((int)Buttons.PistolManualButton).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text3.color = nonSelectedColor;
    }
    void ClosePopup()
    {
        Managers.Sound.Play("Effect/ClickButton");
        Managers.Game.ChooseCharacter.SetActive(true);
        gameObject.SetActive(false);
    }
}
