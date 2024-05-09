using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
	public class ObjectManager
	{
		public static ObjectManager Instance { get; } = new ObjectManager();

		object _lock = new object();
		Dictionary<int, Player> _players = new Dictionary<int, Player>();
		Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
		Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();
		Dictionary<int, Buff> _buffs = new Dictionary<int, Buff>();

		// [UNUSED(1)][TYPE(7)][ID(24)]
		int _counter = 0;

		public T Add<T>() where T : GameObject, new()
		{
			T gameObject = new T();

			lock (_lock)
			{
                try
				{
					gameObject.Id = GenerateId(gameObject.ObjectType);

					if (gameObject.ObjectType == GameObjectType.Player)
					{
						_players.Add(gameObject.Id, gameObject as Player);
					}
					else if (gameObject.ObjectType == GameObjectType.Monster)
					{
						_monsters.Add(gameObject.Id, gameObject as Monster);
					}
					else if (gameObject.ObjectType == GameObjectType.Projectile)
					{
						_projectiles.Add(gameObject.Id, gameObject as Projectile);
					}
					else if (gameObject.ObjectType == GameObjectType.Buff)
					{
						_buffs.Add(gameObject.Id, gameObject as Buff);
					}

				}
				catch(Exception e)
                {
                    ConsoleLogManager.Instance.Log(e);
                    ConsoleLogManager.Instance.Log("Dictionary Key-Value Problem");	
                }
			}

			return gameObject;
		}

		public int GenerateId(GameObjectType type)
		{
			lock (_lock)
			{
				return ((int)type << 24) | (_counter++);
			}
		}

		public GameObjectType GetObjectTypeById(int id)
		{
			int type = (id >> 24) & 0x7F;
			return (GameObjectType)type;
		}

		public bool Remove(int objectId)
		{
			GameObjectType objectType = GetObjectTypeById(objectId);

			lock (_lock)
			{
				if (objectType == GameObjectType.Player)
					return _players.Remove(objectId);
			}

			return false;
		}

		public T Find<T>(int objectId) where T : GameObject
		{
			GameObjectType objectType = GetObjectTypeById(objectId);

			lock (_lock)
			{
				if (objectType == GameObjectType.Player)
				{
					Player player = null;
					if (_players.TryGetValue(objectId, out player))
						return player as T;
				}
				if (objectType == GameObjectType.Monster)
				{
					Monster monster = null;
					if (_monsters.TryGetValue(objectId, out monster))
						return monster as T;
				}
				if (objectType == GameObjectType.Projectile)
				{
					Projectile projectile = null;
					if (_projectiles.TryGetValue(objectId, out projectile))
						return projectile as T;
				}
				if (objectType == GameObjectType.Buff)
				{
					Buff buff = null;
					if (_buffs.TryGetValue(objectId, out buff))
						return buff as T;
				}
			}

			return null;
		}
	}
}
