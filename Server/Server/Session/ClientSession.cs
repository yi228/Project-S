using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;
using Google.Protobuf.Protocol;
using Google.Protobuf;
using Server.Game;
using Server.Game.Room;

namespace Server
{
	public class ClientSession : PacketSession
	{
		public Player MyPlayer { get; set; }

		public int RoomId;
		public int SessionId { get; set; }

		public void Send(IMessage packet)
		{
			string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
			MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);
			ushort size = (ushort)packet.CalculateSize();
			byte[] sendBuffer = new byte[size + 4];
			Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
			Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
			Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);
			Send(new ArraySegment<byte>(sendBuffer));
		}

		public override void OnConnected(EndPoint endPoint)
		{
			ConsoleLogManager.Instance.Log($"OnConnected : {endPoint}");
            // 맵 조작 감지 패킷
            //S_MapManipulation mapPacket = new S_MapManipulation();
            //// TODO - MSSQL 사용시 DB에서 긁어올 부분, 유저 정보
            MyPlayer = ObjectManager.Instance.Add<Player>();
            MyPlayer.Session = this;
            //mapPacket.MapData = DataManager.Instance.MapData;
            //MyPlayer.Session.Send(mapPacket);
        }

		public override void OnRecvPacket(ArraySegment<byte> buffer)
		{
			PacketManager.Instance.OnRecvPacket(this, buffer);
		}

		public override void OnDisconnected(EndPoint endPoint)
		{
			GameRoom room = RoomManager.Instance.Find(RoomId);
			if (room == null)
			{
                ConsoleLogManager.Instance.Log("Cant Leave the game because room is null");
				return;
			}
			if (MyPlayer == null)
				return;
			room.Push(room.LeaveGame, MyPlayer.Info.ObjectId, true);
			SessionManager.Instance.Remove(this);
			ConsoleLogManager.Instance.Log($"OnDisconnected : {endPoint}");
		}

		public override void OnSend(int numOfBytes)
		{
			//ConsoleLogManager.Instance.Log($"Transferred bytes: {numOfBytes}");
		}
		// 로비에서 멀티게임 누르면 실행되는 함수
		public void EnterRoom()
		{
			GameRoom room = RoomManager.Instance.FindGameRoomAndEnter(MyPlayer);
			RoomId = room.RoomId;
			ConsoleLogManager.Instance.Log($"Player Connected in GameRoom {room.RoomId}");
		}
	}
}
