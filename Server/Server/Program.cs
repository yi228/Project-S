using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using Server.Game;
using Server.Game.Room;
using ServerCore;

namespace Server
{
	class Program
	{
		static Listener _listener = new Listener();

		static void FlushRoom()
		{
			//JobTimer.Instance.Push(FlushRoom, 250);
		}

		static void Main(string[] args)
		{
			// Json 데이터 역직렬화
			DataManager.Instance.LoadAllData();

			// DNS (Domain Name System)
			string host = Dns.GetHostName();
			IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[1]; // for ec2
            //IPAddress ipAddr = ipHost.AddressList[0]; // for test
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

			_listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
			ConsoleLogManager.Instance.Log("Server Starting...");

			while (true)
			{
				List<GameRoom> rooms = new List<GameRoom>(RoomManager.Instance.Rooms.Values);

				for (int i = 0; i < rooms.Count; i++)
				{
					GameRoom room = rooms[i];
					if (room != null)
                    {
						GameRoom gr = RoomManager.Instance.Find(room.RoomId);
						if (gr != null)
							gr.Push(gr.Update);
					}
				}
                Thread.Sleep(1);
            }
        }
	}
}
