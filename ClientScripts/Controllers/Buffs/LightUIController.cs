using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightUIController : MonoBehaviour
{
    private MyPlayerController _myPlayer;


    void Start()
    {
        _myPlayer = FindObjectOfType<MyPlayerController>();

    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = _myPlayer.transform.position;
        this.transform.rotation = _myPlayer.transform.rotation;
    }
}
