using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion

	PacketManager()
	{
		Register();
	}

	Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();
		
	public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }

	public void Register()
	{		
		_onRecv.Add((ushort)MsgId.SEnterGame, MakePacket<S_EnterGame>);
		_handler.Add((ushort)MsgId.SEnterGame, PacketHandler.S_EnterGameHandler);		
		_onRecv.Add((ushort)MsgId.SLeaveGame, MakePacket<S_LeaveGame>);
		_handler.Add((ushort)MsgId.SLeaveGame, PacketHandler.S_LeaveGameHandler);		
		_onRecv.Add((ushort)MsgId.SSpawn, MakePacket<S_Spawn>);
		_handler.Add((ushort)MsgId.SSpawn, PacketHandler.S_SpawnHandler);		
		_onRecv.Add((ushort)MsgId.SDespawn, MakePacket<S_Despawn>);
		_handler.Add((ushort)MsgId.SDespawn, PacketHandler.S_DespawnHandler);		
		_onRecv.Add((ushort)MsgId.SMove, MakePacket<S_Move>);
		_handler.Add((ushort)MsgId.SMove, PacketHandler.S_MoveHandler);		
		_onRecv.Add((ushort)MsgId.SSkill, MakePacket<S_Skill>);
		_handler.Add((ushort)MsgId.SSkill, PacketHandler.S_SkillHandler);		
		_onRecv.Add((ushort)MsgId.SHitPlayer, MakePacket<S_HitPlayer>);
		_handler.Add((ushort)MsgId.SHitPlayer, PacketHandler.S_HitPlayerHandler);		
		_onRecv.Add((ushort)MsgId.SHitMonster, MakePacket<S_HitMonster>);
		_handler.Add((ushort)MsgId.SHitMonster, PacketHandler.S_HitMonsterHandler);		
		_onRecv.Add((ushort)MsgId.SFindPlayer, MakePacket<S_FindPlayer>);
		_handler.Add((ushort)MsgId.SFindPlayer, PacketHandler.S_FindPlayerHandler);		
		_onRecv.Add((ushort)MsgId.SChangeHp, MakePacket<S_ChangeHp>);
		_handler.Add((ushort)MsgId.SChangeHp, PacketHandler.S_ChangeHpHandler);		
		_onRecv.Add((ushort)MsgId.SDie, MakePacket<S_Die>);
		_handler.Add((ushort)MsgId.SDie, PacketHandler.S_DieHandler);		
		_onRecv.Add((ushort)MsgId.SEnterRoom, MakePacket<S_EnterRoom>);
		_handler.Add((ushort)MsgId.SEnterRoom, PacketHandler.S_EnterRoomHandler);		
		_onRecv.Add((ushort)MsgId.SStartGame, MakePacket<S_StartGame>);
		_handler.Add((ushort)MsgId.SStartGame, PacketHandler.S_StartGameHandler);		
		_onRecv.Add((ushort)MsgId.SEndGame, MakePacket<S_EndGame>);
		_handler.Add((ushort)MsgId.SEndGame, PacketHandler.S_EndGameHandler);		
		_onRecv.Add((ushort)MsgId.SLeaveRoom, MakePacket<S_LeaveRoom>);
		_handler.Add((ushort)MsgId.SLeaveRoom, PacketHandler.S_LeaveRoomHandler);		
		_onRecv.Add((ushort)MsgId.SLevelUp, MakePacket<S_LevelUp>);
		_handler.Add((ushort)MsgId.SLevelUp, PacketHandler.S_LevelUpHandler);		
		_onRecv.Add((ushort)MsgId.SGetExp, MakePacket<S_GetExp>);
		_handler.Add((ushort)MsgId.SGetExp, PacketHandler.S_GetExpHandler);		
		_onRecv.Add((ushort)MsgId.SChangeStat, MakePacket<S_ChangeStat>);
		_handler.Add((ushort)MsgId.SChangeStat, PacketHandler.S_ChangeStatHandler);		
		_onRecv.Add((ushort)MsgId.SChangeState, MakePacket<S_ChangeState>);
		_handler.Add((ushort)MsgId.SChangeState, PacketHandler.S_ChangeStateHandler);		
		_onRecv.Add((ushort)MsgId.SChangeRotz, MakePacket<S_ChangeRotz>);
		_handler.Add((ushort)MsgId.SChangeRotz, PacketHandler.S_ChangeRotzHandler);		
		_onRecv.Add((ushort)MsgId.SChangeWeaponType, MakePacket<S_ChangeWeaponType>);
		_handler.Add((ushort)MsgId.SChangeWeaponType, PacketHandler.S_ChangeWeaponTypeHandler);		
		_onRecv.Add((ushort)MsgId.SPlaySound, MakePacket<S_PlaySound>);
		_handler.Add((ushort)MsgId.SPlaySound, PacketHandler.S_PlaySoundHandler);		
		_onRecv.Add((ushort)MsgId.SCountTime, MakePacket<S_CountTime>);
		_handler.Add((ushort)MsgId.SCountTime, PacketHandler.S_CountTimeHandler);		
		_onRecv.Add((ushort)MsgId.SChangeGold, MakePacket<S_ChangeGold>);
		_handler.Add((ushort)MsgId.SChangeGold, PacketHandler.S_ChangeGoldHandler);		
		_onRecv.Add((ushort)MsgId.SUseTeleport, MakePacket<S_UseTeleport>);
		_handler.Add((ushort)MsgId.SUseTeleport, PacketHandler.S_UseTeleportHandler);		
		_onRecv.Add((ushort)MsgId.SViewText, MakePacket<S_ViewText>);
		_handler.Add((ushort)MsgId.SViewText, PacketHandler.S_ViewTextHandler);		
		_onRecv.Add((ushort)MsgId.SDestroyPortal, MakePacket<S_DestroyPortal>);
		_handler.Add((ushort)MsgId.SDestroyPortal, PacketHandler.S_DestroyPortalHandler);		
		_onRecv.Add((ushort)MsgId.SChangeSpeed, MakePacket<S_ChangeSpeed>);
		_handler.Add((ushort)MsgId.SChangeSpeed, PacketHandler.S_ChangeSpeedHandler);		
		_onRecv.Add((ushort)MsgId.SChangeAttack, MakePacket<S_ChangeAttack>);
		_handler.Add((ushort)MsgId.SChangeAttack, PacketHandler.S_ChangeAttackHandler);		
		_onRecv.Add((ushort)MsgId.SChangeSight, MakePacket<S_ChangeSight>);
		_handler.Add((ushort)MsgId.SChangeSight, PacketHandler.S_ChangeSightHandler);		
		_onRecv.Add((ushort)MsgId.SAddBlock, MakePacket<S_AddBlock>);
		_handler.Add((ushort)MsgId.SAddBlock, PacketHandler.S_AddBlockHandler);		
		_onRecv.Add((ushort)MsgId.SBreakBlock, MakePacket<S_BreakBlock>);
		_handler.Add((ushort)MsgId.SBreakBlock, PacketHandler.S_BreakBlockHandler);		
		_onRecv.Add((ushort)MsgId.SMapManipulation, MakePacket<S_MapManipulation>);
		_handler.Add((ushort)MsgId.SMapManipulation, PacketHandler.S_MapManipulationHandler);		
		_onRecv.Add((ushort)MsgId.SForceQuit, MakePacket<S_ForceQuit>);
		_handler.Add((ushort)MsgId.SForceQuit, PacketHandler.S_ForceQuitHandler);
	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Action<PacketSession, ArraySegment<byte>, ushort> action = null;
		if (_onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{
		T pkt = new T();
		pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);

		if (CustomHandler != null)
        {
			CustomHandler.Invoke(session, pkt, id);	
		}
        else
		{
			Action<PacketSession, IMessage> action = null;
			if (_handler.TryGetValue(id, out action))
				action.Invoke(session, pkt);
		}
	}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			return action;
		return null;
	}
}