using Google.Protobuf.Protocol;
using Server.Game.Room;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Timers;

namespace Server.Game
{
	public class Bullet : Projectile
	{
        // 서버에서 패킷이 처리되는 동안 총알 피해 연산이 한번만 일어나는게 아니라 여러번 일어남
        public bool IsTouch = false;
        public Bullet()
        {
            Stat.Speed = 10f;
            StartCountTime();
        }
        public GameObject Owner { get; set; }

        //long _nextMoveTick = 0;
        //int _updateTick = 50;
        bool _isUpdateFirst = true;
        public int LifeTime = 1000;
        Timer timer = new System.Timers.Timer();
        public void StartCountTime()
        {
            timer.Interval = LifeTime;
            timer.Elapsed += ((s, e) => { Destroy(); });
            timer.AutoReset = true;
            timer.Enabled = true;
        }
        void Destroy()
        {
            if (Room != null)
                Room.Push(Room.LeaveGame, Id, false);
        }
        public override void Update()
        {
            if (_isUpdateFirst == false)
                return;

            if (Owner == null || Room == null)
                return;

            //if (_nextMoveTick >= Environment.TickCount64)
            //    return;

            _isUpdateFirst = false;
            //_nextMoveTick = Environment.TickCount64 + _updateTick;
            S_Move movePacket = new S_Move();
            Vector2Float destPos = new Vector2Float
                (PosInfo.PosX + (DirVector.x * deltaTime * Stat.Speed), PosInfo.PosY + (DirVector.y * deltaTime * Stat.Speed));
            // 서버에서 목표 좌표로 이동
            PosInfo.PosX = destPos.x;
            PosInfo.PosY = destPos.y;
            //PosInfo.RotZ = Owner.PosInfo.RotZ;
            //// 패킷 조립
            //movePacket.PosInfo = new PositionInfo();
            //movePacket.ObjectId = Id;
            //movePacket.PosInfo.PosX = destPos.x;
            //movePacket.PosInfo.PosY = destPos.y;
            //movePacket.PosInfo.RotZ = PosInfo.RotZ;
            // 패킷 조립
            movePacket.PosInfo = new PositionInfo();
            movePacket.ObjectId = Id;
            movePacket.PosInfo.PosX = DirVector.x;
            movePacket.PosInfo.PosY = DirVector.y;
            movePacket.PosInfo.RotZ = PosInfo.RotZ;
            movePacket.BulletScaleBuff = (Owner as Player).BulletScaleBuff;
            //ConsoleLogManager.Instance.Log($"Bullet DirVector ({DirVector.x}, {DirVector.y})");
            Room.Broadcast(movePacket);
        }
    }
}
