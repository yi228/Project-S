using Google.Protobuf.Protocol;
using Server.Game.Room;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class Buff : GameObject
    {
        private BuffType _type;
        // 중복 버핑을 방지하기 위함
        public BuffType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                Cost = DataManager.Instance.BuffData[Type].Cost;
            }
        }
        public bool IsBuffed { get; set; } = false;
        public int Cost { get { return Stat.Cost; } set { Stat.Cost = value; } }
        public Buff()
        {
            ObjectType = GameObjectType.Buff;
        }
    }
}
