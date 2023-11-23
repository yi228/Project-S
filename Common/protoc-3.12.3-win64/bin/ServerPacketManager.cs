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
		_onRecv.Add((ushort)MsgId.CMove, MakePacket<C_Move>);
		_handler.Add((ushort)MsgId.CMove, PacketHandler.C_MoveHandler);		
		_onRecv.Add((ushort)MsgId.CSkill, MakePacket<C_Skill>);
		_handler.Add((ushort)MsgId.CSkill, PacketHandler.C_SkillHandler);		
		_onRecv.Add((ushort)MsgId.CHitPlayer, MakePacket<C_HitPlayer>);
		_handler.Add((ushort)MsgId.CHitPlayer, PacketHandler.C_HitPlayerHandler);		
		_onRecv.Add((ushort)MsgId.CHitMonster, MakePacket<C_HitMonster>);
		_handler.Add((ushort)MsgId.CHitMonster, PacketHandler.C_HitMonsterHandler);		
		_onRecv.Add((ushort)MsgId.CEnterRoom, MakePacket<C_EnterRoom>);
		_handler.Add((ushort)MsgId.CEnterRoom, PacketHandler.C_EnterRoomHandler);		
		_onRecv.Add((ushort)MsgId.CLeaveGame, MakePacket<C_LeaveGame>);
		_handler.Add((ushort)MsgId.CLeaveGame, PacketHandler.C_LeaveGameHandler);		
		_onRecv.Add((ushort)MsgId.CLevelUp, MakePacket<C_LevelUp>);
		_handler.Add((ushort)MsgId.CLevelUp, PacketHandler.C_LevelUpHandler);		
		_onRecv.Add((ushort)MsgId.CGetExp, MakePacket<C_GetExp>);
		_handler.Add((ushort)MsgId.CGetExp, PacketHandler.C_GetExpHandler);		
		_onRecv.Add((ushort)MsgId.CChangeStat, MakePacket<C_ChangeStat>);
		_handler.Add((ushort)MsgId.CChangeStat, PacketHandler.C_ChangeStatHandler);		
		_onRecv.Add((ushort)MsgId.CChangeState, MakePacket<C_ChangeState>);
		_handler.Add((ushort)MsgId.CChangeState, PacketHandler.C_ChangeStateHandler);		
		_onRecv.Add((ushort)MsgId.CChangeRotz, MakePacket<C_ChangeRotz>);
		_handler.Add((ushort)MsgId.CChangeRotz, PacketHandler.C_ChangeRotzHandler);		
		_onRecv.Add((ushort)MsgId.COutGame, MakePacket<C_OutGame>);
		_handler.Add((ushort)MsgId.COutGame, PacketHandler.C_OutGameHandler);		
		_onRecv.Add((ushort)MsgId.CCheckInfo, MakePacket<C_CheckInfo>);
		_handler.Add((ushort)MsgId.CCheckInfo, PacketHandler.C_CheckInfoHandler);		
		_onRecv.Add((ushort)MsgId.CChangeWeaponType, MakePacket<C_ChangeWeaponType>);
		_handler.Add((ushort)MsgId.CChangeWeaponType, PacketHandler.C_ChangeWeaponTypeHandler);		
		_onRecv.Add((ushort)MsgId.CPlaySound, MakePacket<C_PlaySound>);
		_handler.Add((ushort)MsgId.CPlaySound, PacketHandler.C_PlaySoundHandler);		
		_onRecv.Add((ushort)MsgId.CChangeGold, MakePacket<C_ChangeGold>);
		_handler.Add((ushort)MsgId.CChangeGold, PacketHandler.C_ChangeGoldHandler);		
		_onRecv.Add((ushort)MsgId.CUseTeleport, MakePacket<C_UseTeleport>);
		_handler.Add((ushort)MsgId.CUseTeleport, PacketHandler.C_UseTeleportHandler);		
		_onRecv.Add((ushort)MsgId.CChangeHp, MakePacket<C_ChangeHp>);
		_handler.Add((ushort)MsgId.CChangeHp, PacketHandler.C_ChangeHpHandler);		
		_onRecv.Add((ushort)MsgId.CChangeSpeed, MakePacket<C_ChangeSpeed>);
		_handler.Add((ushort)MsgId.CChangeSpeed, PacketHandler.C_ChangeSpeedHandler);		
		_onRecv.Add((ushort)MsgId.CChangeAttack, MakePacket<C_ChangeAttack>);
		_handler.Add((ushort)MsgId.CChangeAttack, PacketHandler.C_ChangeAttackHandler);		
		_onRecv.Add((ushort)MsgId.CChangeSight, MakePacket<C_ChangeSight>);
		_handler.Add((ushort)MsgId.CChangeSight, PacketHandler.C_ChangeSightHandler);		
		_onRecv.Add((ushort)MsgId.CShopBuff, MakePacket<C_ShopBuff>);
		_handler.Add((ushort)MsgId.CShopBuff, PacketHandler.C_ShopBuffHandler);		
		_onRecv.Add((ushort)MsgId.CMapManipulation, MakePacket<C_MapManipulation>);
		_handler.Add((ushort)MsgId.CMapManipulation, PacketHandler.C_MapManipulationHandler);
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