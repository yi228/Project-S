using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;

public class UI_Shop : UI_Popup
{
    DatabaseReference reference;

    enum Texts
    {
        // ���� ����
        BlockBuffCostText,
        AttackBuffCostText,
        SpeedBuffCostText,
        SightBuffCostText,
        // ���� ����
        BlockBuffCountText,
        AttackBuffCountText,
        SpeedBuffCountText,
        SightBuffCountText,
    }
    enum Images
    {
        ApplyBlockBuffImage,
        ApplyAttackBuffImage,
        ApplySpeedBuffImage,
        ApplySightBuffImage,
    }
    enum Buttons
    {
        // ������ ��ư
        QuitButton,
        // ���� ���� ��ư
        BlockBuffButton,
        AttackBuffButton,
        SpeedBuffButton,
        SightBuffButton,
        // ���� ���� ��ư
        ApplyBlockBuffButton,
        ApplyAttackBuffButton,
        ApplySpeedBuffButton,
        ApplySightBuffButton,
    }
    public TextMeshProUGUI BlockBuffCostText, AttackBuffCostText, SpeedBuffCostText, SightBuffCostText, Text_Diamond;
    public TextMeshProUGUI BlockBuffCountText, AttackBuffCountText, SpeedBuffCountText, SightBuffCountText;
    public Image ApplyBlockBuffImage, ApplyAttackBuffImage, ApplySpeedBuffImage, ApplySightBuffImage;
    public int diamondCount, BlockBuffCount, AttackBuffCount, SpeedBuffCount, SightBuffCount;
    void Start()
    {
        FirebaseDatabase.DefaultInstance.GoOffline();
        FirebaseDatabase.DefaultInstance.PurgeOutstandingWrites();
        FirebaseDatabase.DefaultInstance.GoOnline();
        reference = FirebaseDatabase.DefaultInstance.GetReference(PlayerPrefs.GetString("id"));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
        ReadDB();




    }
    public override void Init()
    {
        
        base.Init();
        Managers.UI.UI_Shop = this;
        InitTexts();
        //InitImages();
        InitButtons();
    }
    private void Update()
    {
        Text_Diamond.text = diamondCount.ToString(); //���� ��� ���̾� ���� ����

        BlockBuffCountText.text = BlockBuffCount.ToString() + "��";
        AttackBuffCountText.text = AttackBuffCount.ToString() + "��";
        SpeedBuffCountText.text = SpeedBuffCount.ToString() + "��";
        SightBuffCountText.text = SightBuffCount.ToString() + "��";
    }
    private void InitTexts()
    {
        
        BlockBuffCostText = GetTextMeshProUGUI((int)Texts.BlockBuffCostText);
        AttackBuffCostText = GetTextMeshProUGUI((int)Texts.AttackBuffCostText);
        SpeedBuffCostText = GetTextMeshProUGUI((int)Texts.SpeedBuffCostText);
        SightBuffCostText = GetTextMeshProUGUI((int)Texts.SightBuffCostText);
       
        //���Ⱑ �����ε�
        //BlockBuffCountText = GetTextMeshProUGUI((int)Texts.BlockBuffCountText);
        //BlockBuffCountText.text = Managers.Game.BlockBuffCount.ToString() + "��";

        //AttackBuffCountText = GetTextMeshProUGUI((int)Texts.AttackBuffCountText);
        //AttackBuffCountText.text = Managers.Game.AttackBuffCount.ToString() + "��";

        //SpeedBuffCountText = GetTextMeshProUGUI((int)Texts.SpeedBuffCountText);
        //SpeedBuffCountText.text = Managers.Game.SpeedBuffCount.ToString() + "��";

        //SightBuffCountText = GetTextMeshProUGUI((int)Texts.SightBuffCountText);
        //SightBuffCountText.text = Managers.Game.SightBuffCount.ToString() + "��";
    }


    private void InitImages()
    {
        ApplyBlockBuffImage = GetImage((int)Images.ApplyBlockBuffImage);
        ApplyBlockBuffImage.gameObject.SetActive(Managers.Game.IsApplyBlockBuff);

        ApplyAttackBuffImage = GetImage((int)Images.ApplyAttackBuffImage);
        ApplyAttackBuffImage.gameObject.SetActive(Managers.Game.IsApplyAttackBuff);

        ApplySpeedBuffImage = GetImage((int)Images.ApplySpeedBuffImage);
        ApplySpeedBuffImage.gameObject.SetActive(Managers.Game.IsApplySpeedBuff);

        ApplySightBuffImage = GetImage((int)Images.ApplySightBuffImage);
        ApplySightBuffImage.gameObject.SetActive(Managers.Game.IsApplySightBuff);
    }
    private void InitButtons()
    {
        GetButton((int)Buttons.QuitButton).onClick.AddListener(ClosePopupUI);

        GetButton((int)Buttons.BlockBuffButton).onClick.AddListener
            (() => BuyBuff(GetButton((int)Buttons.BlockBuffButton)));
        GetButton((int)Buttons.AttackBuffButton).onClick.AddListener
            (() => BuyBuff(GetButton((int)Buttons.AttackBuffButton)));
        GetButton((int)Buttons.SpeedBuffButton).onClick.AddListener
            (() => BuyBuff(GetButton((int)Buttons.SpeedBuffButton)));
        GetButton((int)Buttons.SightBuffButton).onClick.AddListener
            (() => BuyBuff(GetButton((int)Buttons.SightBuffButton)));

        GetButton((int)Buttons.ApplyBlockBuffButton).onClick.AddListener
            (() => ApplyBuff(Define.ShopBuffType.Block));
        GetButton((int)Buttons.ApplyAttackBuffButton).onClick.AddListener
            (() => ApplyBuff(Define.ShopBuffType.Attack));
        GetButton((int)Buttons.ApplySpeedBuffButton).onClick.AddListener
            (() => ApplyBuff(Define.ShopBuffType.Speed));
        GetButton((int)Buttons.ApplySightBuffButton).onClick.AddListener
            (() => ApplyBuff(Define.ShopBuffType.Sight));

    }
    private void ApplyBuff(Define.ShopBuffType type)
    {
        switch (type)
        {
            case Define.ShopBuffType.Block:
                Managers.Game.IsApplyBlockBuff = !Managers.Game.IsApplyBlockBuff;
                Debug.Log($"Managers.Game.IsApplyBlockBuff: {Managers.Game.IsApplyBlockBuff}");
                ApplyBlockBuffImage.gameObject.SetActive(Managers.Game.IsApplyBlockBuff);
                break;
            case Define.ShopBuffType.Attack:
                Managers.Game.IsApplyAttackBuff = !Managers.Game.IsApplyAttackBuff;
                Debug.Log($"Managers.Game.IsApplyAttackBuff: {Managers.Game.IsApplyAttackBuff}");
                ApplyAttackBuffImage.gameObject.SetActive(Managers.Game.IsApplyAttackBuff);
                break;
            case Define.ShopBuffType.Speed:
                Managers.Game.IsApplySpeedBuff = !Managers.Game.IsApplySpeedBuff;
                Debug.Log($"Managers.Game.IsApplyfSpeedBuff: {Managers.Game.IsApplySpeedBuff}");
                ApplySpeedBuffImage.gameObject.SetActive(Managers.Game.IsApplySpeedBuff);
                break;
            case Define.ShopBuffType.Sight:
                Managers.Game.IsApplySightBuff = !Managers.Game.IsApplySightBuff;
                Debug.Log($"Managers.Game.IsApplySightBuff: {Managers.Game.IsApplySightBuff}");
                ApplySightBuffImage.gameObject.SetActive(Managers.Game.IsApplySightBuff);
                break;
        }
    }
    private void BuyBuff(Button button)
    {
        button.GetComponent<ShopBuffController>().AddBuff();
        // ��ȭ ���� - TODO
    }
    public void ReadDB()
    {

        reference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result.Value != null)
                {
                    DataSnapshot snapshot = task.Result;
                    diamondCount = Int32.Parse(snapshot.Child("Diamond").Value.ToString()); //���̾Ƹ�� 
                    print(Int32.Parse(snapshot.Child("BlockBuffCount").Value.ToString()));
                    print(Int32.Parse(snapshot.Child("AttackBuffCount").Value.ToString()));
                    print(Int32.Parse(snapshot.Child("SpeedBuffCount").Value.ToString()));
                    print(Int32.Parse(snapshot.Child("SightBuffCount").Value.ToString()));
                    //Todo- �Ʒ�ó�� �ϴϱ� ���� �ȵǴ°Ű����� ������ �� ��� ������ �ʰ� ���ϸ� �ɵ�
                    BlockBuffCount = Int32.Parse(snapshot.Child("BlockBuffCount").Value.ToString());
                    AttackBuffCount = Int32.Parse(snapshot.Child("AttackBuffCount").Value.ToString());
                    SpeedBuffCount = Int32.Parse(snapshot.Child("SpeedBuffCount").Value.ToString());
                    SightBuffCount = Int32.Parse(snapshot.Child("SightBuffCount").Value.ToString());
                    Init();
                    Managers.Game.BlockBuffCount = BlockBuffCount;
                    Managers.Game.AttackBuffCount = AttackBuffCount;
                    Managers.Game.SpeedBuffCount = SpeedBuffCount;
                    Managers.Game.SightBuffCount = SightBuffCount;
                    print("saddsadas");
                    Init();
                }
            }
            

        });


    }
}
