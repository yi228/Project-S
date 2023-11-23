using Google.Protobuf.Protocol;
using Server.Game.Room;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
	public class GameObject
	{
		public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
		public int Id
		{
			get { return Info.ObjectId; }
			set { Info.ObjectId = value; }
		}
		// 필요하면 쓰기(PosInfo로 해결 가능할듯)
		public Vector2Float CurrentPos
		{
			get
			{
				return new Vector2Float(PosInfo.PosX, PosInfo.PosY);
			}

			set
			{
				PosInfo.PosX = value.x;
				PosInfo.PosY = value.y;
			}
		}
		// 플레이어의 무기 타입, 나중에 몬스터도 사용할 수 있을듯
		public WeaponType WeaponType
        {
            get
            {
				return Info.WeaponType;
            }
            set
            {
				Info.WeaponType = value;
            }
        }
		// 바라보는 곳의 방향 벡터
		public Vector2Float DirVector; 
		protected float deltaTime = 0.03f;
		public int _minDamage = 1;
		private int _totalExp;
		private int _gold = -1;
		public bool CanRespawn = false;
		// 방어막 개수
		public int BlockCount = 0;
		private Vector2Float _respawnPos;
		private Area _currentArea = new Area();
		public GameRoom Room { get; set; }
		public GameRoom BackUpRoom { get; set; }

		public ObjectInfo Info { get; set; } = new ObjectInfo();
		public PositionInfo PosInfo { get; set; } = new PositionInfo();
		public Stat Stat { get; set; } = new Stat();
		public CreatureState State
		{
			get { return PosInfo.State; }
			set { PosInfo.State = value; }
		}
		public float Hp { get { return Stat.Hp; } set { Stat.Hp = Math.Clamp(value, 0, Stat.MaxHp); } }
		public int Level { get; set; }
		// 현재 경험치
		public int TotalExp
		{
			get { return _totalExp; }
			set
			{
				_totalExp = value;
				LevelUp();
			}
		}
		// 나를 잡은 오브젝트가 얻는 경험치
		public int Exp { get; set; }
		// 서버에서 전투 일어나는 거 하나하나 패킷 보내는 코드
		// 작성하는 것 보다는 Setter를 바꿔주기
		public int Gold 
        {
            get { return Info.Gold; }
            set
            {
				Info.Gold = value;
                S_ChangeGold goldPacket = new S_ChangeGold();
                goldPacket.ObjectId = Id;
                goldPacket.Gold = Info.Gold;
                if (Room != null && Room.IsGameStart == true)
                    Room.Broadcast(goldPacket);
            }
        }
		// 게임에 들어와 있나 검사
		public bool IsInGame { get; set; } = false;
		// 맵에서 현재 속해있는 지역
		public Area CurrentArea
        {
            get { return _currentArea; }
            set
            {
				_currentArea = value;
			}
        }
		public Vector2Float RespawnPos { get { return _respawnPos; } set { _respawnPos = value; } }
		public GameObject()
		{
			Info.PosInfo = PosInfo;
			Info.Stat = Stat;
		}
		public virtual void LevelUp()
        {
			if (TotalExp >= 50)
            {
				Level++;
				IncreaseStat();
			}
        }
		// 서버에서 스탯 올려주고 브로드 캐스팅 하는 부분
		public virtual void IncreaseStat()
		{
			Stat.Speed += 2;
			Stat.Attack += 10;
			Stat.MaxHp += 20;
			Stat.Hp += 20;
			S_ChangeStat statPacket = new S_ChangeStat();
			statPacket.ObjectId = Id;
			statPacket.Stat = new Stat();
			statPacket.Stat = Stat;
			Room.Broadcast(statPacket);
		}
		public virtual void Update()
		{

		}
		public virtual void OnDamaged(GameObject hitter, float damage)
		{
			if (Room == null)
				return;
			// 방어막 있으면 무효0
				
			if (BlockCount > 0)
			{
				BlockCount--;
				S_BreakBlock breakPacket = new S_BreakBlock();
				breakPacket.ObjectId = Id;
				Room.Broadcast(breakPacket);
				return;
			}
			// 최소 데미지 존재
			if (damage < _minDamage)
				damage = _minDamage;

			Stat.Hp = Math.Max(Stat.Hp - damage, 0);

			S_ChangeHp changePacket = new S_ChangeHp();
			changePacket.ObjectId = Id;
			changePacket.Hp = Stat.Hp;

			if (Room == null)
				BackUpRoom.Broadcast(changePacket);
			else
				Room.Broadcast(changePacket);

			if (Stat.Hp <= 0)
			{
				// 플레이어에게 경험치 주는 부분
				if (hitter.ObjectType == GameObjectType.Player &&
					this.Room == hitter.Room &&
					this.IsInGame == true)
				{
					// 서버에서 경험치 관리 부분
					hitter.TotalExp += this.Exp;
					//ConsoleLogManager.Instance.Log($"{hitter.Id}가 {this.Exp}의 경험치를 얻음");
					//ConsoleLogManager.Instance.Log($"총 경험치: {hitter.TotalExp} 현재 레벨: {hitter.Level}");
					S_GetExp expPacket = new S_GetExp();
					expPacket.ObjectId = hitter.Id;
					expPacket.Exp = this.Exp;
					expPacket.TotalExp = hitter.TotalExp;
					expPacket.Level = hitter.Level;
					hitter.Room.Broadcast(expPacket);
					// 골드 전달 부분
					hitter.Gold += Gold;
				}
                // 죽음 처리 부분
                OnDead(hitter);
			}
		}

		public virtual void OnDead(GameObject hitter)
		{
			if (Room == null)
				return;


			S_Die diePacket = new S_Die();
			diePacket.ObjectId = Id;
			diePacket.HitterId = hitter.Id;

			if (Room == null)
				BackUpRoom.Broadcast(diePacket);
			else
				Room.Broadcast(diePacket);

			if (Room == null)
			{
				GameRoom room = BackUpRoom;
                room.LeaveGame(Id);
            }
            else
			{
				GameRoom room = Room;
				room.LeaveGame(Id);
			}

			//// 다시 리스폰 해주기
			//Stat.Hp = Stat.MaxHp;
			//PosInfo.State = CreatureState.Idle;
			//PosInfo.PosX = 0;
			//PosInfo.PosY = 0;

			//room.EnterGame(this);
		}
		// 사용 안할듯
		public virtual void GetGold(int gold)
        {
			Gold += gold;
			// 브로드 캐스팅 부분
			S_ChangeGold goldPacket = new S_ChangeGold();
			goldPacket.Gold = Gold;
			goldPacket.ObjectId = Id;
			Room.Broadcast(goldPacket);
		}
	}
}