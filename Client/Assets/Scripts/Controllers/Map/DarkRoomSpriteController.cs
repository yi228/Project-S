using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DarkRoomSpriteController : MonoBehaviour
{
    public Image image;
    void Start()
    {
        image = GetComponent<Image>();
        float spriteX = image.sprite.bounds.size.x;
        float spriteY = image.sprite.bounds.size.y;

        float screenY = Camera.main.orthographicSize * 2;
        float screenX = screenY / Screen.height * Screen.width;

        transform.localScale = new Vector2(Mathf.Ceil(screenX / spriteX), Mathf.Ceil(screenY / spriteY));
    }
}
