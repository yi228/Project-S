using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UI_Weapon : UI_Scene
{
    private MyPlayerController _myPlayer;
    public MyPlayerController MyPlayer { get { return _myPlayer; } set { _myPlayer = value; } }

    public Image _weaponImage;
    public Sprite[] _imageList;

    public void ChangeImage(int weaponType)
    {
        _weaponImage.sprite = _imageList[weaponType - 1];
        float width = _weaponImage.sprite.bounds.size.x;
        float height = _weaponImage.sprite.bounds.size.y;
        float ratio = width / height;
        _weaponImage.rectTransform.sizeDelta = new Vector2(_weaponImage.rectTransform.sizeDelta.y * ratio, _weaponImage.rectTransform.sizeDelta.y);
    }
    private void Start()
    {
        GetComponent<Canvas>().sortingOrder = 4;
    }
}
