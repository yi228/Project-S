using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    // 레이턴시 측정용
    public DateTime PrevLatency;
    public TimeSpan NowLatency;
    public float DelayBeforeStartGame;
    // 로비에 모인 플레이어의 수
    public int LobbyPlayer = 1;
    // 게임에 입장하면 부여 받는 플레이어 ID
    public int MyPlayerId = 0;
    // 한 방에 최대 인원수
    public int MaxPlayer;
    public float PacketTick;
    // 게임 시작 여부(timer 등 맞추기 위함)
    private bool _isStartGame = false;
    public bool IsStartGame
    {
        get { return _isStartGame; }
        set
        {
            _isStartGame = value;
            // 게임 입장하고 나서 IsGameStart가 True일 때 게임 Bgm 실행
            Managers.Sound.ChangeBgmWhenSceneLoaded();
        }
    }
    public WeaponType MyPlayerWeaponType = WeaponType.Pistol;
    public int ElapsedTime = 0;
    // 포탈 파괴하기 위해 포탈 폴더 가져오기
    GameObject portals;
    public GameObject ChooseCharacter;
    public void HandleDestroyPortal(int areaId)
    {
        if (portals == null)
            portals = GameObject.Find("Portals");

        GameObject areaPortals = portals.transform.Find($"Section {areaId}").gameObject;
        Managers.Resource.Destroy(areaPortals);
    }
    #region 버프 상점용 변수와 함수
    // 보유 버프 개수
    public int _blockBuffCount = 0;
    public int _attackBuffCount = 0;
    public int _speedBuffCount = 0;
    public int _sightBuffCount = 0;

    // 적용할지 여부
    public bool IsApplyBlockBuff = false;
    public bool IsApplyAttackBuff = false;
    public bool IsApplySpeedBuff = false;
    public bool IsApplySightBuff = false;

    public int BlockBuffCount
    {
        get { return _blockBuffCount; }
        set
        {
            _blockBuffCount = value;
            Managers.UI.UpdateShopUI();
        }
    }
    public int AttackBuffCount
    {
        get { return _attackBuffCount; }
        set
        {
            _attackBuffCount = value;
            Managers.UI.UpdateShopUI();
        }
    }
    public int SpeedBuffCount
    {
        get { return _speedBuffCount; }
        set
        {
            _speedBuffCount = value;
            Managers.UI.UpdateShopUI();
        }
    }
    public int SightBuffCount
    {
        get { return _sightBuffCount; }
        set
        {
            _sightBuffCount = value;
            Managers.UI.UpdateShopUI();
        }
    }
    public void UseBuff()
    {
        if (BlockBuffCount > 0 && IsApplyBlockBuff)
        {
            C_ShopBuff shopBuff = new C_ShopBuff();
            BlockBuffCount--;
            // 패킷 전달
            shopBuff.IsBuff = IsApplyBlockBuff;
            shopBuff.Type = ShopBuffType.ShopBlock;
            Managers.Network.Send(shopBuff);
        }
        if (AttackBuffCount > 0 && IsApplyAttackBuff)
        {
            C_ShopBuff shopBuff = new C_ShopBuff();
            AttackBuffCount--;
            // 패킷 전달
            shopBuff.IsBuff = IsApplyAttackBuff;
            shopBuff.Type = ShopBuffType.ShopAttack;
            Managers.Network.Send(shopBuff);
        }
        if (SpeedBuffCount > 0 && IsApplySpeedBuff)
        {
            C_ShopBuff shopBuff = new C_ShopBuff();
            SpeedBuffCount--;
            // 패킷 전달
            shopBuff.IsBuff = IsApplySpeedBuff;
            shopBuff.Type = ShopBuffType.ShopSpeed;
            Managers.Network.Send(shopBuff);
        }
        if (SightBuffCount > 0 && IsApplySightBuff)
        {
            C_ShopBuff shopBuff = new C_ShopBuff();
            SightBuffCount--;
            // 패킷 전달
            shopBuff.IsBuff = IsApplySightBuff;
            shopBuff.Type = ShopBuffType.ShopSight;
            Managers.Network.Send(shopBuff);
        }
    }
    #endregion
}