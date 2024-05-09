using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    public class Listener
    {
        Socket _listenSocket;
        Func<Session> _sessionFactory;
        object _lock = new object();

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 100)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory += sessionFactory;

            // 문지기 교육
            _listenSocket.Bind(endPoint);

            // 영업 시작
            // backlog : 최대 대기수
            _listenSocket.Listen(backlog);

            for (int i = 0; i < register; i++)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
                RegisterAccept(args);
            }
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            try
            {
                args.AcceptSocket = null;

                bool pending = _listenSocket.AcceptAsync(args);
                if (pending == false)
                    OnAcceptCompleted(null, args);
            }
            catch (Exception ex)
            {
                ConsoleLogManager.Instance.Log(ex);
            }
        }

        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                try
                {
                    if (args.SocketError == SocketError.Success)
                    {
                        Session session = _sessionFactory.Invoke();
                        session.Start(args.AcceptSocket);
                        // 소켓 연결된 상태인지 확인
                        if (args.AcceptSocket.Connected)
                        {
                            session.OnConnected(args.AcceptSocket.RemoteEndPoint);
                        }
                    }
                    else
                        ConsoleLogManager.Instance.Log(args.SocketError.ToString());
                }
                catch (Exception ex)
                {
                    ConsoleLogManager.Instance.Log(ex);
                }
                RegisterAccept(args);
            }
        }
    }
}
