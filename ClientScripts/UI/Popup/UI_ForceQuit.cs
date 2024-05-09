using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ForceQuit : UI_Popup
{
    enum Buttons
    {
        ExitButton
    }
    private void Start()
    {
        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.ExitButton).onClick.AddListener(ForceQuit);
    }
    void ForceQuit()
    {
        Managers.Sound.Play("Effect/ClickButton");
        Application.Quit();
    }
}
