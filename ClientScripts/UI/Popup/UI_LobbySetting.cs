using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_LobbySetting : MonoBehaviour
{
    public Image audioButton;
    public Image shakeButton;
    public Sprite[] ImageList;

    private void Start()
    {
        if (Managers.Sound.SoundOn)
        {
            audioButton.sprite = ImageList[0];
        }
        else
        {
            audioButton.sprite = ImageList[1];
        }
        if (Managers.Sound.ShakeOn)
        {
            shakeButton.sprite = ImageList[2];
        }
        else
        {
            shakeButton.sprite = ImageList[3];
        }
    }

    public void ClosePopup()
    {
        Managers.Sound.Play("Effect/ClickButton");
        Managers.Game.ChooseCharacter.SetActive(true);

        Destroy(gameObject);
    }

    public void AudioShift()
    {
        Managers.Sound.AudioShift();
        Managers.Sound.Play("Effect/ClickButton");
        if (Managers.Sound.SoundOn)
        {
            audioButton.sprite = ImageList[0];
        }
        else
        {
            audioButton.sprite = ImageList[1];
        }
    }

    public void ShakeShift()
    {
        Managers.Sound.ShakeOn = !Managers.Sound.ShakeOn;

        if (Managers.Sound.ShakeOn)
        {
            shakeButton.sprite = ImageList[2];
        }
        else
        {
            shakeButton.sprite = ImageList[3];
        }
    }
}
