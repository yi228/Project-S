using Google.Protobuf.Protocol;
using Server.Game.Room;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Server.Game
{
	public class Monster : GameObject
	{
		float _searchRange = 7.0f;
		float _chaseRange = 20.0f;
		float _skillRange = 2.0f;
		Player _target;
		MonsterType _monsterType;
		public MonsterType MonsterType
        {
			get { return _monsterType; }
			set
			{
				_monsterType = value;
				// TODO- JSON으로 빼오기
				switch (MonsterType)
				{
					case MonsterType.Bat:
						Stat.Hp = 50;
						Stat.MaxHp = 50;
						Stat.Attack = 12;
						Stat.Defense = 0;
						Stat.Speed = 5;
						Exp = 50;
						Gold = 10;
						break;
					case MonsterType.MonsterPlant:
						Stat.Hp = 70;
						Stat.MaxHp = 70;
						Stat.Attack = 6;
						Stat.Defense = 0;
						Stat.Speed = 5;
						Exp = 20;
						Gold = 10;
						break;
					case MonsterType.Dragon:
						Stat.Hp = 125;
						Stat.MaxHp = 125;
						Stat.Attack = 18;
						Stat.Defense = 0;
						Stat.Speed = 5;
						Exp = 100;
						Gold = 30;
						break;
					case MonsterType.EvilMage:
						Stat.Hp = 60;
						Stat.MaxHp = 60;
						Stat.Attack = 20;
						Stat.Defense = 0;
						Stat.Speed = 5;
						Exp = 70;
						Gold = 20;
						break;
					case MonsterType.Golem:
						Stat.Hp = 150;
						Stat.MaxHp = 150;
						Stat.Attack = 5;
						Stat.Defense = 0;
						Stat.Speed = 5;
						Exp = 100;
						Gold = 30;
						break;
					case MonsterType.Lizard:
						Stat.Hp = 80;
						Stat.MaxHp = 80;
						Stat.Attack = 11;
						Stat.Defense = 0;
						Stat.Speed = 5;
						Exp = 70;
						Gold = 20;
						break;
					case MonsterType.Spector:
						Stat.Hp = 50;
						Stat.MaxHp = 50;
						Stat.Attack = 5;
						Stat.Defense = 0;
						Stat.Speed = 5;
						Exp = 50;
						Gold = 10;
						break;
					default:
						Stat.Hp = 50;
						Stat.MaxHp = 50;
						Stat.Attack = 15;
						Stat.Defense = 0;
						Stat.Speed = 5;
						Exp = 50;
						Gold = 10;
						break;
				}
				State = CreatureState.Idle;
			}
		}
        public Monster()
		{
			ObjectType = GameObjectType.Monster;
			// 몬스터는 리스폰 가능이 Default
			CanRespawn = true;
			// TODO- JSON으로 빼오기
			Stat.Hp = 50;
			Stat.MaxHp = 50;
			Stat.Attack = 5;
			Stat.Defense = 0;
			Stat.Speed = 5;
			Exp = 50;
			Gold = 10;
			State = CreatureState.Idle;
		}
		// FSM
        public override void Update()
		{
			switch (State)
			{
				case CreatureState.Idle:
					UpdateIdle();
					break;
				case CreatureState.Moving:
					UpdateMoving();
					break;
				case CreatureState.Chasing:
					UpdateChasing();
					break;
				case CreatureState.Fire:
					UpdateFire();
					break;
				case CreatureState.Skill:
					UpdateSkill();
					break;
				case CreatureState.Dead:
					UpdateDead();
					break;
			}
		}

		long _nextSearchTick = 0;
		int _searchTick = 500;
		public void UpdateIdle()
		{
			if (_nextSearchTick > Environment.TickCount64)
				return;
			_nextSearchTick = Environment.TickCount64 + _searchTick;

			Player target = Room.FindPlayer(p =>
			{
                Vector2Float dir = p.CurrentPos - CurrentPos;
                return dir.cellDistFromZero <= _searchRange;
			});

			if (target == null)
				return;

			_target = target;
			State = CreatureState.Moving;
 
			S_FindPlayer findPacket = new S_FindPlayer();
			findPacket.MonsterId = Id;
			findPacket.PlayerId = _target.Id;
			Room.Broadcast(findPacket);
		}

		long _nextMoveTick = 0;
		protected virtual void UpdateMoving()
		{
			if (_nextMoveTick > Environment.TickCount64)
				return;

            // 실제 스피드 - 이게 업데이트 틱
            long _moveTick = (long)(400 / 2f);
            _nextMoveTick = Environment.TickCount64 + _moveTick;

			// 타겟이나 타겟의 방이 없을 때
			if (_target == null || _target.Room == null)
			{
				_target = null;
				State = CreatureState.Idle;
				BroadcastMove();
				return;
			}
			// 도달했거나 쫓는 범위 밖일 때
			float dist = (_target.CurrentPos - CurrentPos).cellDistFromZero;
			if (dist == 0 || dist > _chaseRange)
			{
				_target = null;
				State = CreatureState.Idle;
				BroadcastMove();
				return;
			}

			Vector2Int startCellPos = new Vector2Int((int)Math.Round(PosInfo.PosX), (int)Math.Round(PosInfo.PosY));
			Vector2Int destCellPos = new Vector2Int((int)Math.Round(_target.PosInfo.PosX), (int)Math.Round(_target.PosInfo.PosY));
			List<Vector2Int> path = Room.Map.FindPath(startCellPos, destCellPos, true);
			// 쫓는 본인 좌표도 포함이라 0번쨰는 본인 1번째는 타겟이라면 2 보다는 커야 한칸이라도 움직이겠단 거임
			// 길이 없거나 너무 멀리 있다			

			// 스킬 사정 거리 안쪽이지만 path.count가 3보다 작을 때 공격 안함을 방지하기 위함
			if (path.Count < 3 || path.Count > (int)_chaseRange)
			{
				if (dist > _skillRange)
				{
					_target = null;
					State = CreatureState.Idle;
					BroadcastMove();
					return;
				}
			}

			// 몬스터 이동 패킷 조립 부분
	 		S_Move movePacket = new S_Move();

			// 서버에서 이동 가능한지 체크
			if (path.Count >= 3 && Room.Map.CanGo(path[1], true) == true)
			{
				{ 
					// 몬스터 각도 갱신 (플레이어 방향 참고)
					PosInfo.RotZ = -(float)(Math.Atan2(PosInfo.PosX - _target.PosInfo.PosX, PosInfo.PosY - _target.PosInfo.PosY) * (180.0f / Math.PI));
					S_ChangeRotz rotZ = new S_ChangeRotz();
					rotZ.RotZ = PosInfo.RotZ;
					rotZ.ObjectId = Id;
					Room.Broadcast(rotZ);

					// 실제 좌표 이동
					PosInfo.PosX = path[1].x;
					PosInfo.PosY = path[1].y;
				}
			}

			// 패킷 조립
			if (path.Count >= 3)
			{
				movePacket.PosInfo = new PositionInfo();
				movePacket.ObjectId = Id;
				movePacket.PosInfo.PosX = path[1].x;
				movePacket.PosInfo.PosY = path[1].y;
				movePacket.PosInfo.RotZ = PosInfo.RotZ;
				movePacket.PosInfo.State = State;
				Room.Broadcast(movePacket);
			}

			// 스킬로 넘어갈지 체크 하는 부분
			if (dist <= _skillRange)
			{
				State = CreatureState.Skill;
				BroadcastMove();
				return;
			}

		}

		long _nextSkillTick = 0;
		protected virtual void UpdateSkill()
		{
			// 스킬 사용 가능 체크
			if (_nextSkillTick == 0)
			{
				// 유효 타겟인가?
				if (_target == null || _target.Room != Room || _target.Hp == 0)
				{
					_target = null;
					State = CreatureState.Moving;
					BroadcastMove();
					return;
				}

				// 스킬이 아직 사용 가능한가?
				Vector2Float dir = CurrentPos - _target.CurrentPos;
				float dist = dir.cellDistFromZero;
				bool canUseSkill = (dist <= _skillRange);
				if (canUseSkill == false)
				{
					State = CreatureState.Moving;
					BroadcastMove();
					return;
				}
                // 데미지 판정
                _target.OnDamaged(this, Stat.Attack - _target.Stat.Defense);
				if (_target.Hp == 0)
					Gold += _target.Gold;

				// 스킬 사용 Broadcast
				S_Skill skill = new S_Skill() { Info = new SkillInfo() };
				skill.ObjectId = Id;

				// 몬스터의 기본 공격은 1
				skill.Info.SkillId = 1;
				skill.State = CreatureState.Skill;
				Room.Broadcast(skill);

				// 스킬 쿨타임 적용
				float skillCool = 1f;
				int coolTick = (int)skillCool * 1000;
				_nextSkillTick = Environment.TickCount64 + coolTick;
			}

			// 준비 안 됨
			if (_nextSkillTick > Environment.TickCount64)
			{
				return;
			}

            // 스킬 쓴 후에 초기화
            _nextSkillTick = 0;
		}

		protected virtual void UpdateChasing()
		{

		}

		protected virtual void UpdateFire()
		{

		}

		protected virtual void UpdateDead()
		{

		}
        public override void OnDamaged(GameObject hitter, float damage)
        {
            base.OnDamaged(hitter, damage);
			// 맞으면 거리 멀어도 따라가게 함
			if (ObjectManager.Instance.GetObjectTypeById(hitter.Id) == GameObjectType.Player)
			{
				_target = hitter as Player;
				State = CreatureState.Moving;
				ConsoleLogManager.Instance.Log($"new target Id: {hitter.Id}");
			}
		}
        public override void OnDead(GameObject hitter)
        {
            base.OnDead(hitter);
		}
        // 타겟을 더이상 쫓지 않을 때의 몬스터 상태를 서버에 반영후 Broadcast
        void BroadcastMove()
        {
			S_Move movePacket = new S_Move();
			movePacket.ObjectId = Id;
			movePacket.PosInfo = new PositionInfo();
			movePacket.PosInfo.PosX = PosInfo.PosX;
			movePacket.PosInfo.PosY = PosInfo.PosY;
			movePacket.PosInfo.RotZ = PosInfo.RotZ;
			movePacket.PosInfo.State = State;
			Room.Broadcast(movePacket);
		}
	}
}
