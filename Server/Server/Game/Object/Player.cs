using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using Server.Game.Room;

namespace Server.Game
{
	public class Player : GameObject
	{
		public ClientSession Session { get; set; }
		public bool IsWinner { get; set; }
		public Player()
		{
			Init();
		}
		public float BulletScaleBuff = 0f;

		public float BulletSpeedBuff = 0f;
		// 플레이어 정보 초기화
		public void Init()
		{
			ObjectType = GameObjectType.Player;

			Random rand = new Random();

			// DB 에서 플레이어 정보 빼오기
			Info.Name = $"Player_{Info.ObjectId}";
			Info.PosInfo.State = CreatureState.Idle;

			InitStat();
		}

		// TODO JSON - PlayerType에 따른 stat 변경
		public void InitStat()
		{
			if (WeaponType == WeaponType.Default)
				return;
			Stat.MaxHp = DataManager.Instance.PlayerStatData[WeaponType].MaxHp;
			Stat.Hp = DataManager.Instance.PlayerStatData[WeaponType].Hp;
			Stat.Attack = DataManager.Instance.PlayerStatData[WeaponType].Attack;
			Stat.Defense = DataManager.Instance.PlayerStatData[WeaponType].Defense;
			Stat.Speed = DataManager.Instance.PlayerStatData[WeaponType].Speed;
			Stat.CameraSize = DataManager.Instance.PlayerStatData[WeaponType].CameraSize;

			// 이부분은 어떻게 할지 나중에 선택
			Exp = 150;
			TotalExp = 0;
			Level = 0;
			Gold = 40;
			BulletSpeedBuff = 0f;
			BulletScaleBuff = 0f;
		}
		public override void LevelUp()
		{
			// 레벨업 기준 주기
			if (TotalExp >= DataManager.Instance.ReqExpData[Level])
			{
				Level++;
				IncreaseStat();
			}
		}
        public override void IncreaseStat()
        {
            ConsoleLogManager.Instance.Log($"Player {Id} WeaponType is {WeaponType}");
			switch (WeaponType)
			{
				case WeaponType.Pistol:
					Stat.Speed += 0.5f;
					break;
				case WeaponType.Rifle:
					BulletScaleBuff += 0.1f;
					break;
				case WeaponType.Sniper:
					BulletSpeedBuff += 2f;
					break;
				case WeaponType.Shotgun:
					BulletScaleBuff += 0.2f;
					break;
			}
			S_ChangeStat statPacket = new S_ChangeStat();
			statPacket.ObjectId = Id;
			statPacket.Stat = new Stat();
			statPacket.Stat = Stat;
			Room.Broadcast(statPacket);
		}
    }
}
