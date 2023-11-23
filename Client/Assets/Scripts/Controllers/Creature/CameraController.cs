using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    void Update()
    {
        gameObject.GetComponent<GameScene>().enabled = true;
    }
    public void SetactiveDarkRoomUI()
    {

    }
}
