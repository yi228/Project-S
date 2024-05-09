using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using UnityEngine;
using Firebase;
using Firebase.Database;
using System;
using System.IO;

class PacketHandler
{
    // 내가 게임에 입장할 때 패킷
    // 만들고 보니까 클라 쪽에서 세션은 굳이 필요 없을듯 함
    public static void S_EnterGameHandler(PacketSession session, IMessage packet)
    {
        S_EnterGame enterGamePacket = packet as S_EnterGame;
        Managers.Object.Add(enterGamePacket.Object, isMyPlayer: true);
    }
    // 게임에서 죽었을 때
    public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        S_LeaveGame leaveGamePacket = packet as S_LeaveGame;
        if (leaveGamePacket.IsAttacked && Managers.Object.MyPlayer.IsWinner == false)
            Managers.Resource.InstantiateResources("UI_Dead");
        Managers.Object.RemoveAll();
        Managers.Game.IsStartGame = false;
    }
    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        S_Spawn spawnPacket = packet as S_Spawn;

        foreach (ObjectInfo player in spawnPacket.Objects)
        {
            Managers.Object.Add(player, isMyPlayer: false);
        }
    }
    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        S_Despawn despawnPacket = packet as S_Despawn;
        //if (despawnPacket.CanRespawn == true)
        //    return;
        foreach (int id in despawnPacket.ObjectIds)
        {
            Managers.Object.Remove(id);
        }
        Managers.Game.LobbyPlayer = despawnPacket.PlayerCount;
    }
    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        S_Move movePacket = packet as S_Move;
        TimeSpan latency = DateTime.Now - Managers.Game.PrevLatency;

        Debug.Log($"Latency: {latency.TotalMilliseconds} ms");
        Managers.Game.NowLatency = latency;

        GameObject go = Managers.Object.FindById(movePacket.ObjectId);
        if (go == null)
        {
            //Debug.Log($"Cant find GameObject {movePacket.ObjectId}");
            return;
        }

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc == null)
        {
            Debug.Log("Cant find CreatureController");
            return;
        }
        cc.State = movePacket.PosInfo.State;
        // 서버에서 이동하라고 한 부분
        // 클라에서 움직였지만 서버 요청을 준 후 다시 움직이는 코드임
        GameObjectType type = Managers.Object.GetObjectTypeById(cc.Id);
        if (type == GameObjectType.Player)
        {
            PlayerController pc = go.GetComponent<PlayerController>();
            // 움직인게 내 플레이어가 아니고 다른 플레이어일 때는 스르르 움직이게 보여주기
            // 내껏도 스르르 움직이게 서버에서 조종하면 트레이서 일어남
            MyPlayerController mc = go.GetComponent<MyPlayerController>();
            if (mc == null && movePacket.UseTeleport == false)
            {
                //Vector3 destPos = new Vector3(movePacket.PosInfo.PosX, movePacket.PosInfo.PosY, 0f);
                //pc.IsMoving = true;
                //pc.UpdatePositionPlayer(destPos, movePacket.PosInfo.RotZ);

                Vector3 destPos = new Vector3(movePacket.PosInfo.PosX, movePacket.PosInfo.PosY, 0f);
                cc.IsMoving = true;
                cc.DestPos = destPos;
                cc.PosInfo.RotZ = movePacket.PosInfo.RotZ;
                cc.transform.eulerAngles = new Vector3(0, 0, movePacket.PosInfo.RotZ);
            }
            //if (pc != null && movePacket.UseTeleport == false)
            //{
            //    //Vector3 destPos = new Vector3(movePacket.PosInfo.PosX, movePacket.PosInfo.PosY, 0f);
            //    //pc.IsMoving = true;
            //    //pc.UpdatePositionPlayer(destPos, movePacket.PosInfo.RotZ);

            //    Vector3 destPos = new Vector3(movePacket.PosInfo.PosX, movePacket.PosInfo.PosY, 0f);
            //    cc.IsMoving = true;
            //    cc.DestPos = destPos;
            //    cc.PosInfo.RotZ = movePacket.PosInfo.RotZ;
            //    cc.transform.eulerAngles = new Vector3(0, 0, movePacket.PosInfo.RotZ);
            //}
            // 텔레 포트중
            else if (movePacket.UseTeleport == true)
            {
                pc.PosInfo = movePacket.PosInfo;
                pc.PosInfo.RotZ = movePacket.PosInfo.RotZ;
                pc.transform.eulerAngles = new Vector3(0, 0, movePacket.PosInfo.RotZ);
                cc.UseTeleport = false;
                Debug.Log($"{cc.Id}가 텔레포트중");
            }
        }
        else if (type == GameObjectType.Monster)
        {
            //cc.PosInfo.PosX = movePacket.PosInfo.PosX;
            //cc.PosInfo.PosY = movePacket.PosInfo.PosY;
            //cc.PosInfo.RotZ = movePacket.PosInfo.RotZ;
            //cc.transform.position = new Vector3(cc.PosInfo.PosX, cc.PosInfo.PosY, 0);
            //cc.transform.eulerAngles = new Vector3(0, 0, cc.PosInfo.RotZ);
            MonsterController mc = cc.GetComponent<MonsterController>();
            Vector3 destPos = new Vector3(movePacket.PosInfo.PosX, movePacket.PosInfo.PosY, 0f);

            cc.IsMoving = true;
            cc.DestPos = destPos;

            // 각도 수정
            cc.transform.eulerAngles = new Vector3(0, 0, movePacket.PosInfo.RotZ);
            cc.PosInfo.RotZ = movePacket.PosInfo.RotZ;
        }
        else if (type == GameObjectType.Projectile)
        {
            //Vector3 destPos = new Vector3(movePacket.PosInfo.PosX, movePacket.PosInfo.PosY, 0f);
            //cc.IsMoving = true;
            //cc.DestPos = destPos;
            //cc.PosInfo.RotZ = movePacket.PosInfo.RotZ;
            //cc.transform.eulerAngles = new Vector3(0, 0, cc.PosInfo.RotZ);
            BulletController bc = cc.GetComponent<BulletController>();
            // 각도 수정
            bc.PosInfo.RotZ = movePacket.PosInfo.RotZ;
            bc.transform.eulerAngles = new Vector3(0, 0, cc.PosInfo.RotZ);
            // 크기 수정
            bc.transform.localScale += new Vector3(movePacket.BulletScaleBuff, movePacket.BulletScaleBuff, 0);
            // 방향 벡터 수정
            //Debug.Log(movePacket.PosInfo);
            bc.Dir = new Vector3(movePacket.PosInfo.PosX, movePacket.PosInfo.PosY, 0f);
            bc.IsMoving = true;
        }
    }
    public static void S_SkillHandler(PacketSession session, IMessage packet)
    {
        S_Skill skillPacket = packet as S_Skill;

        GameObject go = Managers.Object.FindById(skillPacket.ObjectId);
        if (go == null)
        {
            Debug.Log("Cant find GameObject");
            return;
        }

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc != null)
        {
            GameObjectType type = Managers.Object.GetObjectTypeById(cc.Id);
            if (type == GameObjectType.Player)
            {
                cc.State = skillPacket.State;
                cc.UseSkill(skillPacket.Info.SkillId);

                MyPlayerController mc = cc.GetComponent<MyPlayerController>();
                // 총을 쏜게 나일 때
                if (mc != null)
                {
                    Managers.Sound.PlayFireSound();
                }
            }
            else if (type == GameObjectType.Monster)
            {
                cc.State = skillPacket.State;
                cc.UseSkill(skillPacket.Info.SkillId);
            }
        }
    }
    // 플레이어가 뭔가를 때릴때
    public static void S_HitPlayerHandler(PacketSession session, IMessage packet)
    {
        //S_HitPlayer hitPacket = packet as S_HitPlayer;
        //GameObject enemy = Managers.Object.FindById(hitPacket.EnemyObjectInfo.ObjectId);
        //if (enemy == null)
        //{
        //    Debug.Log($"Cant find GameObject {hitPacket.EnemyObjectInfo.ObjectId}");
        //    return;
        //}

        //enemy.GetComponent<CreatureController>().Stat.Hp = hitPacket.EnemyObjectInfo.Stat.Hp;
    }
    // 몬스터가 뭔가를 때릴때
    public static void S_HitMonsterHandler(PacketSession session, IMessage packet)
    {
        //S_HitMonster hitPacket = packet as S_HitMonster;

        //GameObject hitter = Managers.Object.FindById(hitPacket.HitterObjectInfo.ObjectId);
        //if (hitter == null)
        //{
        //    Debug.Log($"Cant find GameObject {hitPacket.HitterObjectInfo.ObjectId}");
        //    return;
        //}

        //GameObject enemy = Managers.Object.FindById(hitPacket.EnemyObjectInfo.ObjectId);
        //if (enemy == null)
        //{
        //    Debug.Log($"Cant find GameObject {hitPacket.EnemyObjectInfo.ObjectId}");
        //    return;
        //}
        //hitter.GetComponent<MonsterController>().Attack();
        //enemy.GetComponent<CreatureController>().Stat.Hp = hitPacket.EnemyObjectInfo.Stat.Hp;
        //enemy.GetComponent<CreatureController>().OnDamaged();
    }
    public static void S_FindPlayerHandler(PacketSession session, IMessage packet)
    {
        S_FindPlayer findPacket = packet as S_FindPlayer;

        MonsterController monster = Managers.Object.FindById(findPacket.MonsterId).GetComponent<MonsterController>();
        PlayerController player = Managers.Object.FindById(findPacket.PlayerId).GetComponent<PlayerController>();
        monster.Target = player;
    }

    public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
    {
        S_ChangeHp changePacket = packet as S_ChangeHp;

        GameObject go = Managers.Object.FindById(changePacket.ObjectId);
        if (go == null)
            return;

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc != null)
        {
            cc.Hp = changePacket.Hp;
            // 버프로 인한 체력 증가는 데미지 입는 표시 안 함
            if (changePacket.IsBuff == false)
            {
                MyPlayerController mc = cc.GetComponent<MyPlayerController>();
                if (mc != null)
                {
                    mc.OnDamaged();
                    Managers.Sound.PlayHitSound(mc.Id);
                    Debug.Log($"playerId: {mc.Id}가 맞았습니다");
                }
                else
                {
                    cc.OnDamaged();
                    Managers.Sound.PlayHitSound(cc.Id);
                    Debug.Log($"playerId: {cc.Id}가 맞았습니다");
                }
            }
        }
    }

    public static void S_DieHandler(PacketSession session, IMessage packet)
    {
        S_Die diePacket = packet as S_Die;

        GameObject go = Managers.Object.FindById(diePacket.ObjectId);
        if (go == null)
            return;

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc != null)
        {
            cc.Hp = 0;
            cc.OnDead();
        }
    }
    // 플레이어가 방에 들어 왔을때 인원수 늘려주기
    public static void S_EnterRoomHandler(PacketSession session, IMessage packet)
    {
        S_EnterRoom S_EnterRoomPacket = packet as S_EnterRoom;
        Managers.Game.MaxPlayer = S_EnterRoomPacket.MaxPlayer;
        Managers.Game.LobbyPlayer = S_EnterRoomPacket.PlayerCount;
        Managers.Game.DelayBeforeStartGame = S_EnterRoomPacket.DelayBeforeStartGame;
        Managers.Game.PacketTick = S_EnterRoomPacket.PacketTick;
    }
    // 플레이어가 방에 들어 왔다가 다시 나갈 때
    public static void S_LeaveRoomHandler(PacketSession session, IMessage packet)
    {
        S_LeaveRoom S_LeaveRoomPacket = packet as S_LeaveRoom;
        Managers.Game.LobbyPlayer = S_LeaveRoomPacket.PlayerCount;
    }

    public static void S_StartGameHandler(PacketSession session, IMessage packet)
    {
        S_StartGame startPacket = packet as S_StartGame;
        UI_Lobby lobby = GameObject.Find("Lobby").GetComponent<UI_Lobby>();
        lobby.StartGame();
    }
    public static void S_EndGameHandler(PacketSession session, IMessage packet)
    {
        S_EndGame endPacket = packet as S_EndGame;
        Debug.Log("End Game");
        Managers.Object.MyPlayer.IsWinner = true;
        GameObject go = GameObject.Find("UI_Dead");
        if (go != null)
            Managers.Resource.Destroy(go);
        Managers.Resource.InstantiateResources("UI_EndGame");
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference(PlayerPrefs.GetString("id"));

        int Diamond = PlayerPrefs.GetInt("Diamond");
        int Playcount = PlayerPrefs.GetInt("Playcount");
        int Wincount = PlayerPrefs.GetInt("Wincount");
        int ProfileImage = PlayerPrefs.GetInt("ProfileImage");
        int BlockBuffCount = PlayerPrefs.GetInt("BlockBuffCount");
        int AttackBuffCount = PlayerPrefs.GetInt("AttackBuffCount");
        int SpeedBuffCount = PlayerPrefs.GetInt("SpeedBuffCount");
        int SightBuffCount = PlayerPrefs.GetInt("SightBuffCount");
        Wincount++;
        Diamond++;
        //데이터 쓰기
        UserData firstUser = new UserData(PlayerPrefs.GetString("id"), PlayerPrefs.GetString("userName"), Diamond, Playcount, Wincount, ProfileImage, BlockBuffCount, AttackBuffCount, SpeedBuffCount, SightBuffCount);
        string jsonFirstUser = JsonUtility.ToJson(firstUser);
        reference.SetRawJsonValueAsync(jsonFirstUser);

    }
    public static void S_LevelUpHandler(PacketSession session, IMessage packet)
    {
        S_LevelUp levelUpPacket = packet as S_LevelUp;
    }
    public static void S_GetExpHandler(PacketSession session, IMessage packet)
    {
        S_GetExp expPacket = packet as S_GetExp;
        PlayerController player = Managers.Object.FindById(expPacket.ObjectId).GetComponent<PlayerController>();
        // 무결성 검사
        if (player.TotalExp + expPacket.Exp == expPacket.TotalExp)
        {
            // 클라에서 경험치 관리 부분
            player.TotalExp = expPacket.TotalExp;
            // 서버랑 플레이어 패킷의 레벨이 다르면 레벨업 했다는 거
            if (player.Level != expPacket.Level)
            {
                // 서버와 정보 맞춰주고 레벨업 이벤트 틀어주기
                Debug.Log("이전 레벨: " + player.Level);
                player.Level = expPacket.Level;
                player.LevelUp();
            }
        }
    }
    // 스탯 바뀔 때 사용(아마 상점용)
    public static void S_ChangeStatHandler(PacketSession session, IMessage packet)
    {
        S_ChangeStat statPacket = packet as S_ChangeStat;
        PlayerController player = Managers.Object.FindById(statPacket.ObjectId).GetComponent<PlayerController>();
        // 서버에서 확인을 거친 스탯 증가를 클라에도 적용
        player.Stat = statPacket.Stat;
    }
    // 상태 바뀔 때(State)
    public static void S_ChangeStateHandler(PacketSession session, IMessage packet)
    {
        S_ChangeState statePacket = packet as S_ChangeState;
        PlayerController player = Managers.Object.FindById(statePacket.ObjectId).GetComponent<PlayerController>();
        // 서버에서 확인을 거친 State 변화를 클라에도 적용
        player.State = statePacket.State;
    }
    public static void S_ChangeRotzHandler(PacketSession session, IMessage packet)
    {
        S_ChangeRotz rotZPacket = packet as S_ChangeRotz;

        GameObject go = Managers.Object.FindById(rotZPacket.ObjectId);
        if (go == null)
        {
            Debug.Log($"Cant find GameObject {rotZPacket.ObjectId}");
            return;
        }

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc == null)
        {
            Debug.Log("Cant find CreatureController");
            return;
        }
        cc.PosInfo.RotZ = rotZPacket.RotZ;
        cc.transform.eulerAngles = new Vector3(0, 0, rotZPacket.RotZ);
        //C_CheckInfo checkPacket = new C_CheckInfo();
        //checkPacket.RotZ = rotZPacket.RotZ;
        //checkPacket.Id = cc.Id;
        //Managers.Network.Send(checkPacket);
    }
    public static void S_ChangeWeaponTypeHandler(PacketSession session, IMessage packet)
    {
        S_ChangeWeaponType weaponPacket = packet as S_ChangeWeaponType;

        GameObject go = Managers.Object.FindById(weaponPacket.ObjectId);
        if (go == null)
        {
            Debug.Log($"Cant find GameObject {weaponPacket.ObjectId}");
            return;
        }

        PlayerController pc = go.GetComponent<PlayerController>();
        if (pc == null)
        {
            Debug.Log($"Cant find PlayerController {weaponPacket.ObjectId}");
            return;
        }
        // 서버 정보 클라에 적용시키는 부분
        pc.WeaponType = weaponPacket.WeaponType;
    }
    public static void S_PlaySoundHandler(PacketSession session, IMessage packet)
    {
        S_PlaySound soundPacket = packet as S_PlaySound;

        GameObject go = Managers.Object.FindById(soundPacket.ObjectId);
        if (go == null)
        {
            Debug.Log($"Cant find GameObject {soundPacket.ObjectId}");
            return;
        }

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc == null)
        {
            Debug.Log($"Cant find CreatureController {soundPacket.ObjectId}");
            return;
        }
        // 내가 촌 쏭이면 내 AudioSource에서 소리 나게 하기
        MyPlayerController mc = cc.GetComponent<MyPlayerController>();
        MonsterController monster = cc.GetComponent<MonsterController>();
        if (mc != null)
        {
            Managers.Sound.Play(soundPacket.Path);
        }
        // 몬스터한테 맞는 소리 재생
        else if (monster != null)
        {
            AudioClip clip = Managers.Sound.GetOrAddAudioClip(soundPacket.Path);
            monster.AudioSource.clip = clip;
            if (Managers.Sound.SoundOn == true)
                monster.AudioSource.mute = false;
            else
                monster.AudioSource.mute = true;
            monster.AudioSource.Play();
        }
        // 다른 플레이어가 쏜 총이면 다른 플레이어의 AudioSource에서 소리 나게 하기
        else
        {
            PlayerController pc = cc.GetComponent<PlayerController>();
            AudioSource audio = pc.GetComponent<AudioSource>();
            AudioClip clip = Managers.Sound.GetOrAddAudioClip(soundPacket.Path);
            if (Managers.Sound.SoundOn == true)
                audio.mute = false;
            else
                audio.mute = true;
            audio.PlayOneShot(clip);
            Debug.Log("other player shot");
        }
    }
    public static void S_CountTimeHandler(PacketSession session, IMessage packet)
    {
        S_CountTime timePacket = packet as S_CountTime;
        Managers.Game.ElapsedTime = timePacket.Time;
    }
    public static void S_ChangeGoldHandler(PacketSession session, IMessage packet)
    {
        S_ChangeGold goldPacket = packet as S_ChangeGold;
        PlayerController player = Managers.Object.FindById(goldPacket.ObjectId).GetComponent<PlayerController>();
        // 서버에서 확인을 거친 Gold 변화를 클라에도 적용
        if (player != null)
            player.Gold = goldPacket.Gold;
    }
    public static void S_UseTeleportHandler(PacketSession session, IMessage packet)
    {
        S_UseTeleport teleportPacket = packet as S_UseTeleport;
        PlayerController player = Managers.Object.FindById(teleportPacket.ObjectId).GetComponent<PlayerController>();
        // 서버에서 확인을 거친 Posinfo 변화를 클라에도 적용
        player.PosInfo = teleportPacket.PosInfo;
        Debug.Log(player.State);
    }
    public static void S_ViewTextHandler(PacketSession session, IMessage packet)
    {
        S_ViewText textPacket = packet as S_ViewText;
        UI_SystemTextViewer textViewer = Managers.Object.MyPlayer.SystemTextViewerUI.GetComponent<UI_SystemTextViewer>();
        textViewer.PrintInGameText(textPacket.MsgType, textPacket.AreaId);
    }
    public static void S_DestroyPortalHandler(PacketSession session, IMessage packet)
    {
        S_DestroyPortal destroyPacket = packet as S_DestroyPortal;
        Managers.Game.HandleDestroyPortal(destroyPacket.AreaId);
    }
    public static void S_ChangeSpeedHandler(PacketSession session, IMessage packet)
    {
        S_ChangeSpeed speedPacket = packet as S_ChangeSpeed;
        GameObject go = Managers.Object.FindById(speedPacket.ObjectId);
        CreatureController cc = go.GetComponent<CreatureController>();
        cc.Stat.Speed = speedPacket.Speed;
    }
    public static void S_ChangeAttackHandler(PacketSession session, IMessage packet)
    {
        S_ChangeAttack attackPacket = packet as S_ChangeAttack;
        GameObject go = Managers.Object.FindById(attackPacket.ObjectId);
        CreatureController cc = go.GetComponent<CreatureController>();
        cc.Stat.Attack = attackPacket.Attack;
    }
    public static void S_ChangeSightHandler(PacketSession session, IMessage packet)
    {
        S_ChangeSight sightPacket = packet as S_ChangeSight;
        GameObject go = Managers.Object.FindById(sightPacket.ObjectId);
        if (go.GetComponent<MyPlayerController>() != null)
        {
            MyPlayerController mc = go.GetComponent<MyPlayerController>();
            mc.CameraSize = sightPacket.Sight;
        }
    }
    public static void S_AddBlockHandler(PacketSession session, IMessage packet)
    {
        S_AddBlock blockPacket = packet as S_AddBlock;
        GameObject go = Managers.Object.FindById(blockPacket.ObjectId);
        if (go.GetComponent<PlayerController>() != null && blockPacket.IsBuff == true)
        {
            PlayerController pc = go.GetComponent<PlayerController>();
            pc.AddBlock();
        }
    }
    public static void S_BreakBlockHandler(PacketSession session, IMessage packet)
    {
        S_BreakBlock breakPacket = packet as S_BreakBlock;
        GameObject go = Managers.Object.FindById(breakPacket.ObjectId);
        if (go.GetComponent<PlayerController>() != null)
        {
            PlayerController pc = go.GetComponent<PlayerController>();
            pc.BreakBlock();
        }
    }
    public static void S_MapManipulationHandler(PacketSession session, IMessage packet)
    {
        C_MapManipulation mapPacket = new C_MapManipulation();
        string mapData = "";
        string directory = "Assets/Resources/Prefabs/Data/Map/";
        string filePath = Path.Combine(directory, "Map_1.txt");

        if (File.Exists(filePath))
        {
            mapData = File.ReadAllText(filePath);
        }
        else
        {
            Debug.Log("no exist MapData");
        }
        mapPacket.MapData = mapData;
        Managers.Network.Send(mapPacket);
    }
    public static void S_ForceQuitHandler(PacketSession session, IMessage packet)
    {
        Managers.UI.ShowPopupUI<UI_ForceQuit>();
    }


    public class UserData
    {
        public string userID = "";
        public string userNickname = "";
        public int Diamond = 0;
        public int playCount = 0;
        public int winCount = 0;
        public int profileImage = 0;
        public int BlockBuffCount = 0;
        public int AttackBuffCount = 0;
        public int SpeedBuffCount = 0;
        public int SightBuffCount = 0;

        public UserData(string USERID, string USERNICKNAME, int DIAMOND, int PLAYCOUNT, int WINCOUNT, int PROFILEIMAGE,int BLOCKBUFFCOUNT, int ATTACKBUFFCOUNT, int SPEEDBUFFCOUNT, int SIGHTBUFFCOUNT)
        {
            userID = USERID;
            userNickname = USERNICKNAME;
            Diamond = DIAMOND;
            playCount = PLAYCOUNT;
            winCount = WINCOUNT;
            profileImage = PROFILEIMAGE;
            BlockBuffCount = BLOCKBUFFCOUNT;
            AttackBuffCount = ATTACKBUFFCOUNT;
            SpeedBuffCount = SPEEDBUFFCOUNT;
            SightBuffCount = SIGHTBUFFCOUNT;
        }
    }
}