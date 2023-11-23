using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Game.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Server.Game
{
    public class GameRoom : JobSerializer
    {
        public int RoomId { get; set; }
        Map _map = new Map();
        Dictionary<int, Player> _players = new Dictionary<int, Player>();
        Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
        Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();
        Dictionary<int, Buff> _buffs = new Dictionary<int, Buff>();
        // 몬스터들의 리스폰 타이머를 관리하는 곳
        Dictionary<int, Timer> _respawnTimers = new Dictionary<int, Timer>();
        int _respawnTick = 10000;
        // TODO - 최소 데미지
        int _minDamage = 0;
        // Room이 게임을 시작했는가
        public bool IsGameStart = false;
        // Room이 가득 찼는가
        public bool IsFull = false;
        // Room에서의 게임이 종료 됐는지
        public bool IsGameOver = false;
        // 플레이어가 한 번이라도 들어왔던 방인가
        public bool IsPlayerIn = false;
        public Player Winner = null;
        public Map Map { get { return _map; } set { _map = value; } }

        // 게임 시작전 딜레이 타이머
        Timer delayTimer = new System.Timers.Timer();
        int _delayTimerTick = 3000;

        // 흐른 시간 전용 타이머
        Timer timer = new System.Timers.Timer();
        int _timerTick = 1000; 
        int _elapsedTime = 0;

        // 폐쇄 시간 알림 타이머
        Timer closedTextTimer = new System.Timers.Timer();
        int _closeTextTick = 40000;
        // for test
        //int _closeTextTick = 10000;

        // 실제 폐쇄 시간 타이머
        Timer closedTimer = new System.Timers.Timer();
        int _closeTick = 60000;
        // for test
        //int _closeTick = 20000;

        // 최후의 시간 타이머, 매치 라운드 시작 후 _lastTick후에 로직 실행
        Timer lastTimer = new System.Timers.Timer();
        int _lastTick = 60000;
        // for test
        //int _lastTick = 10000;

        // 매치 라운드 시간 타이머
        Timer matchTimer = new System.Timers.Timer();
        int _matchTick;

        List<int> _closedAreaIds = new List<int>();
        List<Area> _closedAreas = new List<Area>();
        void InitSpawnMonster()
        {
            SpawnMonster(MonsterType.Bat, -71f, 68f);
            SpawnMonster(MonsterType.MonsterPlant, -83.8f, 77);

            SpawnMonster(MonsterType.Dragon, 66.74f, 72.67f);
            SpawnMonster(MonsterType.EvilMage, 80.22f, 65.37f);

            SpawnMonster(MonsterType.Golem, 63.49f, -67.3f);
            SpawnMonster(MonsterType.Lizard, 77.43f, -71.24f);

            SpawnMonster(MonsterType.Spector, -63.2f, -64.9f);
            SpawnMonster(MonsterType.Spector, -78.6f, -78.8f);

            //어둠 구역
            SpawnMonster(MonsterType.Spector, 0f, -12.6f);
            SpawnMonster(MonsterType.Spector, -4.8f, -0.5f);

            SpawnMonster(MonsterType.EvilMage, 4.5f, -2.3f);

            SpawnMonster(MonsterType.Lizard, -1.5f, 10f);

            SpawnMonster(MonsterType.Bat, -5.74f, 6.7f);

            SpawnMonster(MonsterType.Dragon, 5.35f, -8.8f);

            SpawnMonster(MonsterType.Golem, 2.9f, 4.9f);

            SpawnMonster(MonsterType.MonsterPlant, -13.75f, -2.8f);
        }
        // 버프 랜더 스폰 로직
        void InitSpawnBuff()
        {
            Random random = new Random();
            List<BuffType> buffTypes = new List<BuffType> { BuffType.Hp, BuffType.Speed, BuffType.Attack, BuffType.Sight };

            for (int i = 0; i < 6; i++)
            {
                BuffType buff;
                if (0 <= i && i <= 3)
                {
                    int index = random.Next(buffTypes.Count);
                    buff = buffTypes[index];
                    buffTypes.RemoveAt(index);
                }
                else
                {
                    buff = (BuffType)random.Next(1, 5);
                }
                if (i == 0)
                    SpawnBuff(buff, -88, 87);
                else if (i == 1)
                    SpawnBuff(buff, 51, 69);
                else if (i == 2)
                    SpawnBuff(buff, -88, -52);
                else if (i == 3)
                    SpawnBuff(buff, 51.7f, -52.5f);
                else if (i == 4)
                    SpawnBuff(buff, -13.5f, -0.1f);
                else if (i == 5)
                    SpawnBuff(buff, 12.3f, -0.5f);
            }
            int randIndex = random.Next(0, 4);

            // 어둠구역 손전등(Light) 버프
            if (randIndex == 0)
                SpawnBuff(BuffType.Light, -87f, 62f);
            else if (randIndex == 1)
                SpawnBuff(BuffType.Light, 86f, 87f);
            else if (randIndex == 2)
                SpawnBuff(BuffType.Light, 51f, -89f);
            else if (randIndex == 3)
                SpawnBuff(BuffType.Light, -88.5f, -54f);
        }
        public Monster SpawnMonster(MonsterType monsterType, float x, float y)
        {
            Monster monster = ObjectManager.Instance.Add<Monster>();
            monster.MonsterType = monsterType;
            monster.RespawnPos = new Vector2Float(x, y);
            Random rand = new Random();
            monster.Info.Name = monsterType.ToString();
            monster.Info.PosInfo.PosX = x;
            monster.Info.PosInfo.PosY = y;
            monster.Info.PosInfo.RotZ = 0;

            Timer respawnTimer = new System.Timers.Timer();
            _respawnTimers.Add(monster.Id, respawnTimer);
            Push(EnterGame, monster);
            return monster;
        }
        public Monster RespawnMonster(MonsterType monsterType, Vector2Float respawnPos, int monsterId)
        {
            // 죽은 몬스터의 리스폰 타이머 Stop
            _respawnTimers[monsterId].Stop();
           Monster monster = ObjectManager.Instance.Add<Monster>();
            monster.MonsterType = monsterType;
            monster.RespawnPos = respawnPos;
            Random rand = new Random();
            monster.Info.Name = monsterType.ToString();
            monster.Info.PosInfo.PosX = respawnPos.x;
            monster.Info.PosInfo.PosY = respawnPos.y;
            monster.Info.PosInfo.RotZ = 0;

            Timer respawnTimer = new System.Timers.Timer();
            _respawnTimers.Add(monster.Id, respawnTimer);
            Push(EnterGame, monster);
            return monster;
        }

        void SpawnBuff(BuffType type, float x, float y)
        {
            Buff buff = ObjectManager.Instance.Add<Buff>();
            Random rand = new Random();
            string buffName = "";
            if (type == BuffType.Hp)
                buffName = "HpUpBuff";
            else if (type == BuffType.Speed)
                buffName = "SpeedBuff";
            else if (type == BuffType.Sight)
                buffName = "SightBuff";
            else if (type == BuffType.Attack)
                buffName = "AttackBuff";
            else if (type == BuffType.Light)
                buffName = "LigthBuff";

            buff.Info.Name = buffName;
            buff.Info.PosInfo.PosX = x;
            buff.Info.PosInfo.PosY = y;
            buff.Info.PosInfo.RotZ = 0;
            buff.Type = type;
            Push(EnterGame, buff);
        }
        void StartAllTimer()
        {
            StartCountTimeTimer();
            StartCloseAreaTextTimer();
            StartCloseAreaTimer();
            StartMatchAreaTextTimer();
        }
        void StartDelayTimer()
        {
            delayTimer.Interval = _delayTimerTick;
            delayTimer.Elapsed += ((s, e) => { StartGame(); });
            delayTimer.AutoReset = true;
            delayTimer.Enabled = true;
        }
        void StartCountTimeTimer()
        {
            timer.Interval = _timerTick;
            timer.Elapsed += ((s, e) => { HandleChangeTime(); });
            timer.AutoReset = true;
            timer.Enabled = true;
        }
        // 구역 폐쇄 안내 메시지
        void StartCloseAreaTextTimer()
        {
            closedTextTimer.Interval = _closeTextTick;
            closedTextTimer.Elapsed += ((s, e) => { HandleCloseAreaText(); });
            closedTextTimer.AutoReset = true;
            closedTextTimer.Enabled = true;
        }
        // 시간이 다 돼, 실제로 구역 폐쇄
        void StartCloseAreaTimer()
        {
            closedTimer.Interval = _closeTick;
            closedTimer.Elapsed += ((s, e) => { HandleCloseArea(); });
            closedTimer.AutoReset = true;
            closedTimer.Enabled = true;
        }
        // 매치 라운드에 보내주는 텍스트
        void StartMatchAreaTextTimer()
        {
            // 마지막 지역 폐쇄 문구 나오고 난 후에 MatchRound 문구 보여줄 때까지의 시간 간격
            // 인게임 TMPAlpha가 3초까지 지속돼서 3으로 해줌
            int interval = 3000;
            _matchTick = _closeTick * 4 + interval;
            matchTimer.Interval = _matchTick;
            matchTimer.Elapsed += ((s, e) => { HandleMatchAreaText(); });
            matchTimer.AutoReset = true;
            matchTimer.Enabled = true;

            // 최후의 타이머 세팅
            lastTimer.Interval = _matchTick + _lastTick;
            lastTimer.Elapsed += ((s, e) => { HandleEndGame(); });
            lastTimer.AutoReset = true;
            lastTimer.Enabled = true;
        }
        public void Init()
        {
            InitSpawnMonster();
            InitSpawnBuff();
            Map.LoadMap(1);
        }
        public void Update()
        {
            if (IsGameOver)
                return;
            foreach (Projectile projectile in _projectiles.Values)
            {
                projectile.Update();
            }
            foreach (Monster monster in _monsters.Values)
            {
 
                monster.Update();
            }
        }
        public void EnterGame(GameObject gameObject)
        {
            if (gameObject == null)
                return;

            GameObjectType type = ObjectManager.Instance.GetObjectTypeById(gameObject.Id);

            gameObject.IsInGame = true;
            if (type == GameObjectType.Player)
            {
                IsPlayerIn = true;
                Player player = gameObject as Player;
                // 서버를 처음 동작하고 첫방의 첫 플레이어가 입장시
                // 중복키 들어가는 경우 해결하기 위함
                if (_players.ContainsKey(gameObject.Id))
                {
                    Player newPlayer = ObjectManager.Instance.Add<Player>();
                    _players.Add(newPlayer.Id, newPlayer);
                }
                else
                {
                    _players.Add(gameObject.Id, player);
                }
                player.Room = this;
                player.BackUpRoom = player.Room;
                // TODO - 플레이어 스탯 재설정 JSON으로 하기
                Random rand = new Random();

                player.Info.Name = $"Player_{player.Info.ObjectId}";
                player.Info.PosInfo.State = CreatureState.Idle;

                //// for test
                //player.Info.PosInfo.PosX = -71 + _players.Count;
                //player.Info.PosInfo.PosY = 77 + _players.Count;
                //player.Info.PosInfo.RotZ = 0;

                // for ec2
                if (_players.Count == 1)
                {
                    player.Info.PosInfo.PosX = -71;
                    player.Info.PosInfo.PosY = 77;
                    player.Info.PosInfo.RotZ = 0;
                }
                else if (_players.Count == 2)
                {
                    player.Info.PosInfo.PosX = 56;
                    player.Info.PosInfo.PosY = 77;
                    player.Info.PosInfo.RotZ = 0;
                }
                else if (_players.Count == 3)
                {
                    player.Info.PosInfo.PosX = -88;
                    player.Info.PosInfo.PosY = -63;
                    player.Info.PosInfo.RotZ = 0;
                }
                else if (_players.Count == 4)
                {
                    player.Info.PosInfo.PosX = 69;
                    player.Info.PosInfo.PosY = -60;
                    player.Info.PosInfo.RotZ = 0;
                }
                player.CurrentArea = Map.GetAreaByPos(player.PosInfo);
                player.Init();

                if (_players.Count == RoomManager.Instance.MaxPlayer)
                {
                    // 딜레이 타이머를 적용 한 후에 게임이 시작한 걸 유저들에게 알려주기
                    StartDelayTimer();
                }
                // 본인한테 맵안의 플레이어, 몬스터 정보 전송
                {
                    S_EnterGame enterPacket = new S_EnterGame();
                    enterPacket.Object = player.Info;
                    player.Session.Send(enterPacket);

                    S_Spawn spawnPacket = new S_Spawn();

                    // 나를 제외하고 접속한 플레이어를 spawnPacket에 저장
                    foreach (Player p in _players.Values)
                    {
                        if (player != p)
                            spawnPacket.Objects.Add(p.Info);
                    }
                    // 맵의 몬스터를 spawnPacket에 저장
                    foreach (Monster m in _monsters.Values)
                    {
                        spawnPacket.Objects.Add(m.Info);
                    }
                    // 맵의 Projectile을 spawnPacket에 저장
                    foreach (Projectile projectile in _projectiles.Values)
                    {
                        spawnPacket.Objects.Add(projectile.Info);
                    }
                    // 맵의 Buff를 spawnPacket에 저장
                    foreach (Buff buff in _buffs.Values)
                    {
                        spawnPacket.Objects.Add(buff.Info);
                    }
                    player.Session.Send(spawnPacket);
                }
            }
            else if (type == GameObjectType.Monster)
            {
                Monster monster = gameObject as Monster;
                _monsters.Add(gameObject.Id, monster);
                monster.Room = this;
                monster.BackUpRoom = monster.Room;
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = gameObject as Projectile;
                _projectiles.Add(gameObject.Id, projectile);
                projectile.Room = this;
                projectile.BackUpRoom = projectile.Room;
            }
            else if (type == GameObjectType.Buff)
            {
                Buff buff = gameObject as Buff;
                _buffs.Add(gameObject.Id, buff);
                buff.Room = this;
                buff.BackUpRoom = buff.Room;
            }
            // 타인한테 정보 전송
            {
                S_Spawn spawnPacket = new S_Spawn();
                spawnPacket.Objects.Add(gameObject.Info);
                foreach (Player p in _players.Values)
                {
                    if (p.Id != gameObject.Id)
                        p.Session.Send(spawnPacket);
                }
            }
            // 일정 수의 플레이어가 모이면 방을 잠그는 게 아니라 게임 시작시로 조건 바꾸기
            if (_players.Count >= RoomManager.Instance.MaxPlayer)
            {
                IsGameStart = true;
            }
        }
        public void LeaveGame(int objectId, bool isAttacked = true)
        {
            GameObjectType type = ObjectManager.Instance.GetObjectTypeById(objectId);

            if (type == GameObjectType.Player)
            {
                Player player = null;
                if (_players.Remove(objectId, out player) == false)
                    return;

                player.Room = null;

                // 본인한테 정보 전송
                {
                    player.IsInGame = false;
                    S_LeaveGame leavePacket = new S_LeaveGame();
                    leavePacket.PlayerCount = _players.Count;
                    leavePacket.IsAttacked = isAttacked;
                    player.Session.Send(leavePacket);
                }

                // 플레이어가 한명 나갔으므로 IsFull 해제
                IsFull = false;

                // 플레이어가 나갔을 때 남은 플레이어의 수가 1명이면 게임 종료
                // 해당 플레이어의 승리
                // 방 종료

                // 플레이어가 있었던 방인데 0명이돼서 터진방일 때
                if (IsPlayerIn && _players.Count == 0)
                {
                    ConsoleLogManager.Instance.Log($"Dont enough player so Room {RoomId} Delete!");
                    // 터진 방의 플레이어 정보 밀어주기
                    foreach (Player p in _players.Values)
                        p.Init();
                    RoomManager.Instance.Remove(RoomId);
                }

                if (_players.Count == 1 && IsGameStart)
                {
                    IsGameOver = true;
                    // 최후의 플레이어를 승자로 판정
                    foreach (Player p in _players.Values)
                    {
                        p.IsWinner = true;
                        Winner = p;
                        // 이제 P에게 패킷 전송
                        S_EndGame endPacket = new S_EndGame();
                        endPacket.IsGameEnd = true;
                        p.Session.Send(endPacket);
                    }
                    if (IsGameOver == true)
                    {
                        ConsoleLogManager.Instance.Log($"Game is end so Room {RoomId} Delete!");
                        // 터진 방의 플레이어 정보 밀어주기
                        foreach (Player p in _players.Values)
                            p.Init();
                        RoomManager.Instance.Remove(RoomId);
                    }
                    ConsoleLogManager.Instance.Log($"Remain User Count in Room {RoomId}: " + _players.Count);
                }


            }
            else if (type == GameObjectType.Monster)
            {
                Monster monster = null;
                if (_monsters.Remove(objectId, out monster) == false)
                    return;
                MonsterType monsterType = monster.MonsterType;
                Vector2Float spawnPos = monster.RespawnPos;
                monster.IsInGame = false;
                monster.Room = null;

                if (isAttacked == true)
                {
                    // 몬스터 리스폰 부분
                    _respawnTimers[monster.Id].Interval = _respawnTick;
                    _respawnTimers[monster.Id].Elapsed += ((s, e) => { RespawnMonster(monsterType, spawnPos, monster.Id); });
                    _respawnTimers[monster.Id].AutoReset = true;
                    _respawnTimers[monster.Id].Enabled = true;
                }
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = null;
                if (_projectiles.Remove(objectId, out projectile) == false)
                    return;
                projectile.IsInGame = false;
                projectile.Room = null;
            }
            else if (type == GameObjectType.Buff)
            {
                Buff buff = null;
                if (_buffs.Remove(objectId, out buff) == false)
                    return;
                buff.IsInGame = false;
                buff.Room = null;
            }

            // 타인한테 정보 전송
            {
                S_Despawn despawnPacket = new S_Despawn();
                despawnPacket.ObjectIds.Add(objectId);
                despawnPacket.PlayerCount = _players.Count;

                foreach (Player p in _players.Values)
                {
                    if (p.Id != objectId)
                        p.Session.Send(despawnPacket);
                }
            }
        }
        public void StartGame()
        {
            delayTimer.Stop();
            IsFull = true;
            S_StartGame startPacket = new S_StartGame();
            startPacket.IsStartGame = true;
            Broadcast(startPacket);
            StartAllTimer();
        }
        public void HandleMove(Player player, C_Move movePacket)
        {
            if (player == null)
                return;
            // 서버에서 클라로 보낼 패킷 생성
            S_Move resMovePacket = new S_Move();
            resMovePacket.PosInfo = new PositionInfo();
            // 움직였나 Moving 체크하는 부분
            // 클래스 취급이라 정보를 복사해오면 call by value가 아니라 call by refernce임
            // 서버에서 State 바꾸는 부분
            ObjectInfo info = player.Info;
            if (info.PosInfo.PosX == movePacket.PosInfo.PosX && info.PosInfo.PosY == movePacket.PosInfo.PosY)
                player.State = CreatureState.Idle;
            else
                player.State = CreatureState.Moving;

            resMovePacket.PosInfo.State = player.State;
            // 서버에서 플레이어 좌표 이동 하는 부분
            info.PosInfo.PosX = movePacket.PosInfo.PosX;
            info.PosInfo.PosY = movePacket.PosInfo.PosY;
            info.PosInfo.RotZ = movePacket.PosInfo.RotZ;
            // 다른 플레이어들한테도 myPlayer가 움직이는 것을 알려준다
            resMovePacket.ObjectId = player.Info.ObjectId;
            resMovePacket.PosInfo = info.PosInfo;
            resMovePacket.UseTeleport = movePacket.UseTeleport;
            Broadcast(resMovePacket);
        }
        public void HandleSkill(Player player, C_Skill skillPacket)
        {
            if (player == null)
                return;

            ObjectInfo info = player.Info;

            info.PosInfo.State = CreatureState.Skill;

            S_Skill skill = new S_Skill() { Info = new SkillInfo() };
            skill.ObjectId = info.ObjectId;
            skill.Info.SkillId = skillPacket.Info.SkillId;
            skill.State = CreatureState.Skill;
            skill.WeaponType = player.WeaponType;
            // 스킬아이디 1 설정
            // 1. 평타
            Broadcast(skill);
            // 플레이어 회전값 서버에 반영
            // 샷건 총알 각도
            float shotgunRotz = 15f;
            if (skillPacket.Info.SkillId == 1)
            {
                for (int i = 0; i < skillPacket.PosInfo.Count; i++)
                {
                    Bullet bullet = ObjectManager.Instance.Add<Bullet>();
                    if (bullet == null)
                        return;
                    bullet.Owner = player;
                    bullet.Stat.Speed += player.BulletSpeedBuff;
                    bullet.PosInfo.State = CreatureState.Moving;
                    bullet.PosInfo.PosX = skillPacket.PosInfo[i].PosX;
                    bullet.PosInfo.PosY = skillPacket.PosInfo[i].PosY;
                    // 각도 부분
                    if (i == 0)
                        bullet.PosInfo.RotZ = player.PosInfo.RotZ;
                    else if (i == 1)
                        bullet.PosInfo.RotZ = player.PosInfo.RotZ + shotgunRotz;
                    else if (i == 2)
                        bullet.PosInfo.RotZ = player.PosInfo.RotZ - shotgunRotz;
                    // 뱡향 벡터 부분
                    bullet.DirVector.x = skillPacket.DirVector[0].PosX;
                    bullet.DirVector.y = skillPacket.DirVector[0].PosY;
                    Push(EnterGame, bullet);
                }
            }
        }
        // 플레이어가 플레이어를 때렸을 때
        public void HandleHitPlayer(Player hitter, GameObject enemy, GameObjectType enemyType, C_HitPlayer hitPacket)
        {
            if (hitter == null)
                return;

            if (enemy == null)
                return;

            if (enemyType == GameObjectType.Player)
                enemy = enemy as Player;

            else if (enemyType == GameObjectType.Monster)
                enemy = enemy as Monster;

            // 자기 자신을 쏘는 현상이 생겨서
            // Bullet을 이용해 서버에서 막아줌
            Bullet bullet = ObjectManager.Instance.Find<Bullet>(hitPacket.BulletId);

            //// 자기 자신을 쏘는 걸 서버에서도 막아줌
            if (enemy.Id == bullet.Owner.Id)
                return;
            Push(LeaveGame, bullet.Id, true);
            if (bullet.IsTouch == false)
            {
                bullet.IsTouch = true;
                enemy.OnDamaged(hitter, hitter.Stat.Attack - enemy.Stat.Defense);
                ConsoleLogManager.Instance.Log($"hitter {hitter.Id} Attack: {hitter.Stat.Attack}");
                ConsoleLogManager.Instance.Log($"enemy {enemy.Id} HP: {enemy.Stat.Hp}");

            }
            else
            {
                //ConsoleLogManager.Instance.Log("HandleHitPlayer Bullet already touched!");
            }

        }

        // 플레이어가 몬스터를 때렸을 때
        public void HandleHitMonster(Player hitter, GameObject enemy, GameObjectType enemyType, C_HitMonster hitPacket)
        {
            if (hitter == null)
                return;

            if (enemy == null)
                return;

            if (enemyType == GameObjectType.Player)
                enemy = enemy as Player;

            else if (enemyType == GameObjectType.Monster)
                enemy = enemy as Monster;

            // 자기 자신을 쏘는 현상이 생겨서
            // Bullet을 이용해 서버에서 막아줌
            Bullet bullet = ObjectManager.Instance.Find<Bullet>(hitPacket.BulletId);

            // 통과 되면 state 맞는것으로 바꿔주기
            float damage = (hitter.Stat.Attack - enemy.Stat.Defense);
            // 자기 자신을 쏘는 걸 서버에서도 막아줌
            if (enemy.Id == bullet.Owner.Id)
                return;

            Push(LeaveGame, bullet.Id, true);

            if (bullet.IsTouch == false)
            {
                bullet.IsTouch = true;
                enemy.OnDamaged(hitter, hitter.Stat.Attack - enemy.Stat.Defense);

                ConsoleLogManager.Instance.Log($"hitter {hitter.Id} Attack: {hitter.Stat.Attack} ");
                ConsoleLogManager.Instance.Log($"enemy {enemy.Id} HP: {enemy.Stat.Hp}");
            }
            else
            {
                //ConsoleLogManager.Instance.Log("HandleHitMonster Bullet already touched!");
            }

            S_HitMonster hit = new S_HitMonster();
            // TODO
            // 필요하면 때린 사람 정보 보내기
            // 설계해보니 아직까진 필요 없는 정보인듯
            //hit.HitterObjectInfo.ObjectId = player.Id;
            //hit.HitterObjectInfo.Stat = player.Stat;
            hit.EnemyObjectInfo = new ObjectInfo();
            hit.EnemyObjectInfo.ObjectId = enemy.Id;
            hit.EnemyObjectInfo.Stat = enemy.Stat;

            Broadcast(hit);
        }
        // 몬스터가 플레이러를 때렸을 때
        public void HandleMonsterHitPlayer(Monster hitter, GameObject enemy, GameObjectType enemyType, C_HitMonster hitPacket)
        {
            if (hitter == null)
                return;

            if (enemy == null)
                return;

            if (enemyType == GameObjectType.Player)
                enemy = enemy as Player;

            else if (enemyType == GameObjectType.Monster)
                enemy = enemy as Monster;

            float damage = (hitter.Stat.Attack - enemy.Stat.Defense);
            // 자기 자신을 쏘는 걸 서버에서도 막아줌
            // 최소 데미지 설정 가능
            if (damage < 0)
                damage = _minDamage;

            enemy.Hp -= damage;

            if (enemy.Hp <= 0)
            {
                enemy.Hp = 0;
                hitter.Gold += enemy.Gold;
                Push(LeaveGame, enemy.Id, true);
            }

            S_HitMonster hit = new S_HitMonster();
            // 필요하면 때린 사람 정보 보내기
            hit.EnemyObjectInfo = new ObjectInfo();
            hit.EnemyObjectInfo.ObjectId = enemy.Id;
            hit.EnemyObjectInfo.Stat = enemy.Stat;

            hit.HitterObjectInfo = new ObjectInfo();
            hit.HitterObjectInfo.ObjectId = hitter.Id;
            hit.HitterObjectInfo.PosInfo = new PositionInfo();
            Broadcast(hit);
        }
        public void HandleEnterPlayerInLobby(Player player, C_EnterRoom C_EnterRoomPacket)
        {
            S_EnterRoom S_EnterRoompPacket = new S_EnterRoom();
            S_EnterRoompPacket.CanEnter = true;
            S_EnterRoompPacket.PlayerCount = _players.Count;
            S_EnterRoompPacket.MaxPlayer = RoomManager.Instance.MaxPlayer;
            S_EnterRoompPacket.DelayBeforeStartGame = RoomManager.Instance.DelayBeforeStartGame;
            Broadcast(S_EnterRoompPacket);
        }
        // 플레이어가 로비(잡은 방)에서 나갈떄
        public void HandleLeaveLobby(Player player)
        {
            Push(LeaveGame, player.Id, false);
            S_LeaveRoom S_LeaveRoomPacket = new S_LeaveRoom();
            S_LeaveRoomPacket.PlayerCount = _players.Count;
            Broadcast(S_LeaveRoomPacket);
        }
        // 플레이어 레벨업
        public void HandlePlayerLevelUp(Player player)
        {
            player.Level += 1;
            S_LevelUp levelUpPacket = new S_LevelUp();
            levelUpPacket.ObjectId = player.Id;
            levelUpPacket.Exp = player.Exp;
            Broadcast(levelUpPacket);
        }
        public void HandlePlayerGetExp(Player player, C_GetExp expPacket)
        {
            player.TotalExp += expPacket.Exp;
            S_GetExp exp = new S_GetExp();
            exp.Exp = expPacket.Exp;
            exp.ObjectId = player.Id;
            Broadcast(exp);
        }
        public void HandleChangeStat(Player player, C_ChangeStat statPacket)
        {
            // 클라에서 보낸 스탯 정보 서버에 저장
            player.Stat = statPacket.Stat;
            // 브로드 캐스팅 부분
            S_ChangeStat stat = new S_ChangeStat();
            stat.Stat = new Stat();
            stat.Stat = player.Stat;
            stat.ObjectId = player.Id;
            Broadcast(stat);
        }
        public void HandleChangeState(Player player, C_ChangeState statePacket)
        {
            // 클라에서 보낸 스탯 정보 서버에 저장
            player.State = statePacket.State;
            // 브로드 캐스팅 부분
            S_ChangeState state = new S_ChangeState();
            state.State = player.State;
            state.ObjectId = player.Id;
            Broadcast(state);
        }
        public void HandleChangeRotZ(Player player, C_ChangeRotz rotZPacket)
        {
            // 클라에서 보낸 각도 정보 서버에 저장
            player.PosInfo.RotZ = rotZPacket.RotZ;
            // 브로드 캐스팅 부분
            S_ChangeRotz rotZ = new S_ChangeRotz();
            rotZ.RotZ = player.PosInfo.RotZ;
            rotZ.ObjectId = player.Id;
            Broadcast(rotZ);
        }
        public void HandleChangeWeaponType(Player player, C_ChangeWeaponType weaponPacket)
        {
            // 클라에서 정보 서버에 저장
            player.WeaponType = weaponPacket.WeaponType;
            // 서버에서 클라로 보낼 패킷 조립
            S_ChangeWeaponType weapon = new S_ChangeWeaponType();
            weapon.WeaponType = player.WeaponType;
            weapon.ObjectId = player.Id;
            // 브로드 캐스팅 부분
            Broadcast(weapon);
        }
        public void HandlePlaySound(Player player, C_PlaySound soundPacket)
        {
            S_PlaySound sound = new S_PlaySound();
            sound.Path = soundPacket.Path;
            // 플레이어한테서 나는 소리가 아닐 때
            if (soundPacket.IsNotPlayer)
            {
                // 서버에서 클라로 보낼 패킷 조립
                sound.ObjectId = soundPacket.NotPlayerId;
            }
            // 플레이어한테서 나는 소리일 때
            else
            {
                // 서버에서 클라로 보낼 패킷 조립
                sound.ObjectId = player.Id;
            }
            // 브로드 캐스팅 부분
            Broadcast(sound);
        }
        void HandleChangeTime()
        {
            _elapsedTime++;
            S_CountTime timePacket = new S_CountTime();
            timePacket.Time = _elapsedTime;
            Broadcast(timePacket);
        }
        void HandleCloseAreaText()
        {
            if (IsGameOver)
            {
                StopAllTimer();
                return;
            }
            if (_closedAreaIds.Count != 4)
            {
                int areaId = GetRandomClosingAreaId();
                _closedAreaIds.Add(areaId);
                _closedAreas.Add(Map.Areas[areaId]);
                S_ViewText textPacket = new S_ViewText();
                textPacket.AreaId = areaId;
                textPacket.MsgType = SystemMSGType.MapCloseReminder;
                foreach (Player player in _players.Values)
                    player.Session.Send(textPacket);
                closedTextTimer.Stop();
            }
            else
            {
                closedTimer.Stop();
            }
        }
        void HandleCloseArea()
        {
            int areaId = _closedAreaIds[_closedAreaIds.Count - 1];
            S_ViewText textPacket = new S_ViewText();
            textPacket.AreaId = areaId;
            textPacket.MsgType = SystemMSGType.MapCloseNow;
            foreach (Player player in _players.Values)
                player.Session.Send(textPacket);
            CheckPlayersInClosedArea(Map.Areas[areaId]);

            // 클라이언트에서 폐쇄 구역 포탈 파괴 패킷 보내기
            S_DestroyPortal destroyPacket = new S_DestroyPortal();
            destroyPacket.AreaId = areaId;
            Broadcast(destroyPacket);

            // 지역 폐쇄 미리 알림 타이머 리셋
            ResetCloseTextTimer();
        }
        void HandleMatchAreaText()
        {
            S_ViewText textPacket = new S_ViewText();
            textPacket.MsgType = SystemMSGType.MapMatch;
            foreach (Player player in _players.Values)
                player.Session.Send(textPacket);

            // 모든 몬스터 삭제
            foreach (Monster monster in _monsters.Values)
                LeaveGame(monster.Id);

            // 몬스터 스폰 타이머 삭제
            foreach (Timer timer in _respawnTimers.Values)
                timer.Stop();

            matchTimer.Stop();
        }
        // 게임 시간이 다 흐른 뒤에 플레이어가 1명이 아닐 때 실행
        void HandleEndGame()
        {
            ConsoleLogManager.Instance.Log($"End Game Room {RoomId}");
            Random random = new Random();
            int winnerIndex = random.Next(0, _players.Count);
            int count = 0;
            ConsoleLogManager.Instance.Log($"Remain player: {_players.Count} players");
            foreach (Player player in _players.Values)
            {
                if (winnerIndex != count)
                {
                    count++;
                    ConsoleLogManager.Instance.Log("Player " + player.Id + " Dead");
                    LeaveGame(player.Id);
                }
            }
            lastTimer.Stop();
        }
        // 닫힌 구역에 플레이어가 있는지 체크 후 사망시키기
        void CheckPlayersInClosedArea(Area area)
        {
            foreach (Player player in _players.Values)
            {
                Area lastClosedArea = area;
                if (lastClosedArea.IsInArea(player.PosInfo))
                {
                    GameObject system = new GameObject();
                    player.OnDamaged(system, 10000);
                    ConsoleLogManager.Instance.Log($"Player {player.Id} is in Cloesd Area so Dead");
                }
            }
        }
        public void HandleChangeGold(Player player, C_ChangeGold goldPacket)
        {
            // 클라에서 보낸 스탯 정보 서버에 저장
            player.Gold = goldPacket.Gold;
            // 브로드 캐스팅 부분
            S_ChangeGold gold = new S_ChangeGold();
            gold.Gold = player.Gold;
            gold.ObjectId = player.Id;
            Broadcast(gold);
        }
        public void HandleUseTeleport(Player player, C_UseTeleport teleportPacket)
        {
            // 클라에서 보낸 텔레포트한 위치 정보 서버에 저장
            player.PosInfo = teleportPacket.PosInfo;
            player.CurrentArea = Map.GetAreaByPos(player.PosInfo);

            // 브로드 캐스팅 부분
            S_UseTeleport teleport = new S_UseTeleport();
            teleport.PosInfo = player.PosInfo;
            teleport.ObjectId = player.Id;
            Broadcast(teleport);
        }
        public void HandleChangeHp(Player player, C_ChangeHp hpPacket)
        {
            if (_buffs[hpPacket.BuffId].IsBuffed == false)
            {
                _buffs[hpPacket.BuffId].IsBuffed = true;
                if (hpPacket.IsBuff)
                {
                    player.Hp *= DataManager.Instance.BuffData[BuffType.Hp].IncreaseRate;
                    player.Gold -= DataManager.Instance.BuffData[BuffType.Hp].Cost;
                }
                // 브로드 캐스팅 부분
                S_ChangeHp hp = new S_ChangeHp();
                hp.IsBuff = hpPacket.IsBuff;
                hp.Hp = player.Hp;
                hp.ObjectId = player.Id;
                Broadcast(hp);
            }
        }
        public void HandleChangeSpeed(Player player, C_ChangeSpeed speedPacket)
        {
            if (_buffs[speedPacket.BuffId].IsBuffed == false)
            {
                _buffs[speedPacket.BuffId].IsBuffed = true;
                if (speedPacket.IsBuff)
                {
                    player.Stat.Speed *= DataManager.Instance.BuffData[BuffType.Speed].IncreaseRate;
                    player.Gold -= DataManager.Instance.BuffData[BuffType.Speed].Cost;
                }
                // 브로드 캐스팅 부분
                S_ChangeSpeed speed = new S_ChangeSpeed();
                speed.IsBuff = speedPacket.IsBuff;
                speed.Speed = player.Stat.Speed;
                speed.ObjectId = player.Id;
                Broadcast(speed);
            }
        }
        public void HandleChangeAttack(Player player, C_ChangeAttack attackPacket)
        {
            if (_buffs[attackPacket.BuffId].IsBuffed == false)
            {
                _buffs[attackPacket.BuffId].IsBuffed = true;
                if (attackPacket.IsBuff)
                {
                    player.Stat.Attack *= DataManager.Instance.BuffData[BuffType.Attack].IncreaseRate;
                    player.Gold -= DataManager.Instance.BuffData[BuffType.Attack].Cost;
                }
                // 브로드 캐스팅 부분
                S_ChangeAttack attack = new S_ChangeAttack();
                attack.IsBuff = attackPacket.IsBuff;
                attack.Attack = player.Stat.Attack;
                attack.ObjectId = player.Id;
                Broadcast(attack);
            }
        }
        public void HandleChangeSight(Player player, C_ChangeSight sightPacket)
        {
            if (_buffs[sightPacket.BuffId].IsBuffed == false)
            {
                _buffs[sightPacket.BuffId].IsBuffed = true;
                if (sightPacket.IsBuff)
                {
                    player.Stat.CameraSize *= DataManager.Instance.BuffData[BuffType.Sight].IncreaseRate;
                    player.Gold -= DataManager.Instance.BuffData[BuffType.Sight].Cost;
                }
                // 브로드 캐스팅 부분
                S_ChangeSight sight = new S_ChangeSight();
                sight.IsBuff = sightPacket.IsBuff;
                sight.Sight = player.Stat.CameraSize;
                sight.ObjectId = player.Id;
                Broadcast(sight);
            }
        }
        public void HandleShopBuff(Player player, C_ShopBuff buffPacket)
        {
            if (buffPacket.IsBuff == true)
            {
                switch (buffPacket.Type)
                {
                    case ShopBuffType.ShopBlock:
                        S_AddBlock block = new S_AddBlock();
                        block.IsBuff = buffPacket.IsBuff;
                        block.ObjectId = player.Id;
                        player.BlockCount++;
                        Broadcast(block);
                        break;
                    case ShopBuffType.ShopAttack:
                        S_ChangeAttack attack = new S_ChangeAttack();
                        attack.IsBuff = buffPacket.IsBuff;
                        player.Stat.Attack *= DataManager.Instance.ShopBuffData[buffPacket.Type].IncreaseRate;
                        attack.Attack = player.Stat.Attack;
                        attack.ObjectId = player.Id;
                        Broadcast(attack);
                        break;
                    case ShopBuffType.ShopSpeed:
                        S_ChangeSpeed speed = new S_ChangeSpeed();
                        speed.IsBuff = buffPacket.IsBuff;
                        player.Stat.Speed *= DataManager.Instance.ShopBuffData[buffPacket.Type].IncreaseRate;
                        speed.Speed = player.Stat.Speed;
                        speed.ObjectId = player.Id;
                        Broadcast(speed);
                        break;
                    case ShopBuffType.ShopSight:
                        S_ChangeSight sight = new S_ChangeSight();
                        sight.IsBuff = buffPacket.IsBuff;
                        player.Stat.CameraSize *= DataManager.Instance.ShopBuffData[buffPacket.Type].IncreaseRate;
                        sight.Sight = player.Stat.CameraSize;
                        sight.ObjectId = player.Id;
                        Broadcast(sight);
                        break;
                }
            }
        }
        public void HandleMapManipulation(Player player, C_MapManipulation mapPacket)
        {
            if (mapPacket.MapData != DataManager.Instance.MapData)
            {
                ConsoleLogManager.Instance.Log($"Player {player.Id} use map manipulate");
                S_ForceQuit quitPacket = new S_ForceQuit();
                player.Session.Send(quitPacket);
                player.Session.Disconnect();
            }
        }
        public Player FindPlayer(Func<GameObject, bool> condition)
        {
            foreach (Player player in _players.Values)
            {
                if (condition.Invoke(player))
                    return player;
            }

            return null;
        }
        public void Broadcast(IMessage packet)
        {
            foreach (Player player in _players.Values)
            {
                player.Session.Send(packet);
            }
        }
        // 이미 닫혀 있는 곳을 제외하고 닫힐 수 있는 Area의 Id를 뽑아오는 함수
        int GetRandomClosingAreaId()
        {
            Random random = new Random();
            List<int> numbers = Enumerable.Range(1, 4).Except(_closedAreaIds).ToList();
            int randIndex = numbers[random.Next(numbers.Count)];
            return randIndex;
        }
        void ResetCloseTextTimer()
        {
            closedTextTimer = new System.Timers.Timer();
            closedTextTimer.Interval = _closeTextTick;
            closedTextTimer.Elapsed += ((s, e) => { HandleCloseAreaText(); });
            closedTextTimer.AutoReset = true;
            closedTextTimer.Enabled = true;
        }
        void StopAllTimer()
        {
            timer.Stop();
            closedTimer.Stop();
            closedTextTimer.Stop();
        }
    }
}