using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuScene : MonoBehaviour
{
    void Start()
    {
        Managers.Scene.CurrentScene = Define.Scene.StartMenu;
    }
}
