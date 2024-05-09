using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google.Protobuf.Protocol;

public class UI_SelectMode : UI_Scene
{
    Object instance1, instance2, instance3, instance4; //3D 애니메이션 캐릭터
    Image PistolButtonImage, RifleButtonImage, SniperButtonImage, ShotgunButtonImage;
    enum Buttons
    {
        PistolButton,
        RifleButton,
        SniperButton,
        ShotgunButton
    }


    void Start()
    {
        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.PistolButton).onClick.AddListener(() => ChangePlayerType((int)Buttons.PistolButton));
        GetButton((int)Buttons.RifleButton).onClick.AddListener(() => ChangePlayerType((int)Buttons.RifleButton));
        GetButton((int)Buttons.SniperButton).onClick.AddListener(() => ChangePlayerType((int)Buttons.SniperButton));
        GetButton((int)Buttons.ShotgunButton).onClick.AddListener(() => ChangePlayerType((int)Buttons.ShotgunButton));
        //instance1 = Instantiate(Resources.Load("Art/Creature/Player/Character/BattleRoyaleHeroesPBR/Prefab/Character/PBRDefault/ChemicalmanDefault"));
        PistolButtonImage = GameObject.Find("PistolButton").GetComponent<Image>();
        RifleButtonImage = GameObject.Find("RifleButton").GetComponent<Image>();
        SniperButtonImage = GameObject.Find("SniperButton").GetComponent<Image>();
        ShotgunButtonImage = GameObject.Find("ShotgunButton").GetComponent<Image>();
        ChangePlayerType(0);
    }
    private void ChangePlayerType(int type)
    {
        Managers.Sound.Play("Effect/ClickButton");
        Managers.Game.MyPlayerWeaponType = (WeaponType)(type + 1);
        Debug.Log(Managers.Game.MyPlayerWeaponType);
        ChangePlayerAnimation(type);
        ChangeButtonColor(type);
    }
    private void ChangeButtonColor(int type)
    {
        switch (type + 1)
        {
            case 1:
                PistolButtonImage.sprite = Resources.Load<Sprite>("Art/UI/GUI PRO Kit - Casual Game/ResourcesData/Sprite/Component/Button/Btn_OtherButton_Hexagon01_Red");
                RifleButtonImage.sprite = Resources.Load<Sprite>("Art/UI/GUI PRO Kit - Casual Game/ResourcesData/Sprite/Component/Button/Btn_OtherButton_Hexagon01_Blue");
                SniperButtonImage.sprite = Resources.Load<Sprite>("Art/UI/GUI PRO Kit - Casual Game/ResourcesData/Sprite/Component/Button/Btn_OtherButton_Hexagon01_Blue");
                ShotgunButtonImage.sprite = Resources.Load<Sprite>("Art/UI/GUI PRO Kit - Casual Game/ResourcesData/Sprite/Component/Button/Btn_OtherButton_Hexagon01_Blue");
                break;
            case 2:
                PistolButtonImage.sprite = Resources.Load<Sprite>("Art/UI/GUI PRO Kit - Casual Game/ResourcesData/Sprite/Component/Button/Btn_OtherButton_Hexagon01_Blue");
                RifleButtonImage.sprite = Resources.Load<Sprite>("Art/UI/GUI PRO Kit - Casual Game/ResourcesData/Sprite/Component/Button/Btn_OtherButton_Hexagon01_Red");
                SniperButtonImage.sprite = Resources.Load<Sprite>("Art/UI/GUI PRO Kit - Casual Game/ResourcesData/Sprite/Component/Button/Btn_OtherButton_Hexagon01_Blue");
                ShotgunButtonImage.sprite = Resources.Load<Sprite>("Art/UI/GUI PRO Kit - Casual Game/ResourcesData/Sprite/Component/Button/Btn_OtherButton_Hexagon01_Blue");
                break;
            case 3:
                PistolButtonImage.sprite = Resources.Load<Sprite>("Art/UI/GUI PRO Kit - Casual Game/ResourcesData/Sprite/Component/Button/Btn_OtherButton_Hexagon01_Blue");
                RifleButtonImage.sprite = Resources.Load<Sprite>("Art/UI/GUI PRO Kit - Casual Game/ResourcesData/Sprite/Component/Button/Btn_OtherButton_Hexagon01_Blue");
                SniperButtonImage.sprite = Resources.Load<Sprite>("Art/UI/GUI PRO Kit - Casual Game/ResourcesData/Sprite/Component/Button/Btn_OtherButton_Hexagon01_Red");
                ShotgunButtonImage.sprite = Resources.Load<Sprite>("Art/UI/GUI PRO Kit - Casual Game/ResourcesData/Sprite/Component/Button/Btn_OtherButton_Hexagon01_Blue");

                break;
            case 4:
                PistolButtonImage.sprite = Resources.Load<Sprite>("Art/UI/GUI PRO Kit - Casual Game/ResourcesData/Sprite/Component/Button/Btn_OtherButton_Hexagon01_Blue");
                RifleButtonImage.sprite = Resources.Load<Sprite>("Art/UI/GUI PRO Kit - Casual Game/ResourcesData/Sprite/Component/Button/Btn_OtherButton_Hexagon01_Blue");
                SniperButtonImage.sprite = Resources.Load<Sprite>("Art/UI/GUI PRO Kit - Casual Game/ResourcesData/Sprite/Component/Button/Btn_OtherButton_Hexagon01_Blue");
                ShotgunButtonImage.sprite = Resources.Load<Sprite>("Art/UI/GUI PRO Kit - Casual Game/ResourcesData/Sprite/Component/Button/Btn_OtherButton_Hexagon01_Red");
                break;
        }
    }

    private void ChangePlayerAnimation(int type)//로비 애니메이션 변경
    {
        switch (type + 1)
        {
            case 1:

                if (instance1 == null)
                {
                    instance1 = Instantiate(Resources.Load("Art/Creature/Player/Character/BattleRoyaleHeroesPBR/Prefab/Character/PBRDefault/ChemicalmanDefault"));
                    Destroy(instance2);
                    Destroy(instance3);
                    Destroy(instance4);
                }
                break;
            case 2:
                if (instance2 == null)
                {
                    instance2 = Instantiate(Resources.Load("Art/Creature/Player/Character/BattleRoyaleHeroesPBR/Prefab/Character/PBRDefault/TerroristDefault"));
                    Destroy(instance1);
                    Destroy(instance3);
                    Destroy(instance4);
                }
                break;
            case 3:

                if (instance3 == null)
                {
                    instance3 = Instantiate(Resources.Load("Art/Creature/Player/Character/BattleRoyaleHeroesPBR/Prefab/Character/PBRDefault/SniperDefault"));
                    Destroy(instance1);
                    Destroy(instance2);
                    Destroy(instance4);
                }
                break;
            case 4:

                if (instance4 == null)
                {
                    instance4 = Instantiate(Resources.Load("Art/Creature/Player/Character/BattleRoyaleHeroesPBR/Prefab/Character/PBRDefault/CowboyDefault"));
                    Destroy(instance1);
                    Destroy(instance2);
                    Destroy(instance3);
                }
                break;
        }
    }
}
