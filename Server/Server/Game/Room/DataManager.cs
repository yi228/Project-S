using Google.Protobuf.Protocol;
using Server.Game.Room;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Net;
using Newtonsoft;
using Newtonsoft.Json;
using System.IO;

namespace Server.Game.Room
{
    public class DataManager
    {
        public static DataManager Instance { get; } = new DataManager();
        // 레벨당 레벨업에 필요한 경험치
        public int[] ReqExpData;
        public Dictionary<WeaponType, PlayerStat> PlayerStatData = new Dictionary<WeaponType, PlayerStat>();
        public Dictionary<BuffType, BuffInfo> BuffData = new Dictionary<BuffType, BuffInfo>();
        public Dictionary<ShopBuffType, ShopBuffInfo> ShopBuffData = new Dictionary<ShopBuffType, ShopBuffInfo>();
        public string MapData;
        public float MovePacketTick = 0.04f;
        public void LoadAllData()
        {
            LoadMapData();
            LoadReqExpData();
            LoadPlayerStatData();
            LoadBuffData();
            LoadShopBuffData();
            ConsoleLogManager.Instance.Log("All data has been downloaded...");
        }
        void LoadReqExpData()
        {
            try
            {
                string url = "https://evenidemonickitchen.s3.ap-northeast-2.amazonaws.com/ReqExpData.json";

                using (WebClient client = new WebClient())
                {
                    // JSON 데이터 가져오기
                    string json = client.DownloadString(url);

                    // JSON 데이터를 배열로 변환
                    ReqExp[] dataArray = JsonConvert.DeserializeObject<ReqExp[]>(json);

                    // reqExp 값을 int 배열에 넣기
                    ReqExpData = new int[dataArray.Length];
                    for (int i = 0; i < dataArray.Length; i++)
                    {
                        ReqExpData[i] = dataArray[i].reqExp;
                    }

                    //// 결과 출력
                    //foreach (int reqExp in ReqExpData)
                    //{
                    //    ConsoleLogManager.Instance.Log(reqExp);
                    //}
                    ConsoleLogManager.Instance.Log("ReqExpData Downloaded");
                }
            }
            catch (Exception ex)
            {
                ConsoleLogManager.Instance.Log("Error Detected: " + ex.Message);
            }
        }
        void LoadPlayerStatData()
        {
            try
            {
                string url = "https://evenidemonickitchen.s3.ap-northeast-2.amazonaws.com/PlayerStatData.json";

                using (WebClient client = new WebClient())
                {
                    // JSON 데이터 가져오기
                    string json = client.DownloadString(url);

                    // JSON 데이터를 List<PlayerStat>으로 변환
                    List<PlayerStat> playerStats = JsonConvert.DeserializeObject<List<PlayerStat>>(json);

                    // 결과 출력
                    foreach (PlayerStat playerStat in playerStats)
                    {
                        WeaponType type = WeaponType.Default;

                        if (playerStat.WeaponType == "Pistol")
                            type = WeaponType.Pistol;
                        else if (playerStat.WeaponType == "Rifle")
                            type = WeaponType.Rifle;
                        else if (playerStat.WeaponType == "Sniper")
                            type = WeaponType.Sniper;
                        else if (playerStat.WeaponType == "Shotgun")
                            type = WeaponType.Shotgun;
                        else
                            ConsoleLogManager.Instance.Log($"Cant find WeaponType: {playerStat.WeaponType}");
                        PlayerStatData.Add(type, playerStat);
                    }
                    ConsoleLogManager.Instance.Log("PlayerStatData Downloaded");
                }
            }
            catch (Exception ex)
            {
                ConsoleLogManager.Instance.Log("Error Detected: " + ex.Message);
            }
        }
        void LoadBuffData()
        {
            try
            {
                string url = "https://evenidemonickitchen.s3.ap-northeast-2.amazonaws.com/BuffData.json";

                using (WebClient client = new WebClient())
                {
                    // JSON 데이터 가져오기
                    string json = client.DownloadString(url);

                    // JSON 데이터를 List<PlayerStat>으로 변환
                    List<BuffInfo> buffInfos = JsonConvert.DeserializeObject<List<BuffInfo>>(json);

                    // 결과 출력
                    foreach (BuffInfo buffInfo in buffInfos)
                    {
                        BuffType type = BuffType.BuffNone;

                        if (buffInfo.Type == "Hp")
                            type = BuffType.Hp;
                        else if (buffInfo.Type == "Speed")
                            type = BuffType.Speed;
                        else if (buffInfo.Type == "Attack")
                            type = BuffType.Attack;
                        else if (buffInfo.Type == "Sight")
                            type = BuffType.Sight;
                        else if (buffInfo.Type == "Light")
                            type = BuffType.Light;
                        else
                            ConsoleLogManager.Instance.Log($"Cant find BuffType: {buffInfo.Type}");
                        BuffData.Add(type, buffInfo);
                    }
                    ConsoleLogManager.Instance.Log("BuffData Downloaded");
                }
            }
            catch (Exception ex)
            {
                ConsoleLogManager.Instance.Log("Error Detected: " + ex.Message);
            }
            //BuffData.Add(BuffType.Hp, new BuffInfo(BuffType.Hp, 1.30f, 60));
            //BuffData.Add(BuffType.Speed, new BuffInfo(BuffType.Speed, 1.20f, 45));
            //BuffData.Add(BuffType.Attack, new BuffInfo(BuffType.Attack, 1.20f, 45));
            //BuffData.Add(BuffType.Sight, new BuffInfo(BuffType.Sight, 1.15f, 50));
            //BuffData.Add(BuffType.Light, new BuffInfo(BuffType.Light, 2.0f, 75));
        }
        void LoadShopBuffData()
        {
            try
            {
                string url = "https://evenidemonickitchen.s3.ap-northeast-2.amazonaws.com/ShopBuffData.json";

                using (WebClient client = new WebClient())
                {
                    // JSON 데이터 가져오기
                    string json = client.DownloadString(url);

                    // JSON 데이터를 List<PlayerStat>으로 변환
                    List<ShopBuffInfo> shopBuffInfos = JsonConvert.DeserializeObject<List<ShopBuffInfo>>(json);

                    // 결과 출력
                    foreach (ShopBuffInfo shopBuffInfo in shopBuffInfos)
                    {
                        ShopBuffType type = ShopBuffType.ShopBuffNone;

                        if (shopBuffInfo.Type == "ShopBlock")
                            type = ShopBuffType.ShopBlock;
                        else if (shopBuffInfo.Type == "ShopAttack")
                            type = ShopBuffType.ShopAttack;
                        else if (shopBuffInfo.Type == "ShopSpeed")
                            type = ShopBuffType.ShopSpeed;
                        else if (shopBuffInfo.Type == "ShopSight")
                            type = ShopBuffType.ShopSight;
                        else
                            ConsoleLogManager.Instance.Log($"Cant find ShopBuffType: {shopBuffInfo.Type}");
                        ShopBuffData.Add(type, shopBuffInfo);
                    }
                    ConsoleLogManager.Instance.Log("ShopBuffData Downloaded");
                }
            }
            catch (Exception ex)
            {
                ConsoleLogManager.Instance.Log("Error Detected: " + ex.Message);
            }
            //ShopBuffData.Add(ShopBuffType.ShopBlock, new ShopBuffInfo(ShopBuffType.ShopBlock, 1f));
            //ShopBuffData.Add(ShopBuffType.ShopAttack, new ShopBuffInfo(ShopBuffType.ShopAttack, 1.2f));
            //ShopBuffData.Add(ShopBuffType.ShopSpeed, new ShopBuffInfo(ShopBuffType.ShopSpeed, 1.2f));
            //ShopBuffData.Add(ShopBuffType.ShopSight, new ShopBuffInfo(ShopBuffType.ShopSight, 1.2f));
        }
        public class ReqExp
        {
            public int reqExp { get; set; }
        }
        public class PlayerStat
        {
            public string WeaponType;
            public int MaxHp;
            public int Hp;
            public int Attack;
            public int Defense;
            public float Speed;
            public float CameraSize;
            public float CoolTime;
        }
        public class BuffInfo
        {
            public string Type;
            public float IncreaseRate;
            public int Cost;
            public BuffInfo(string type, float increaseRate, int cost)
            {
                Type = type;
                IncreaseRate = increaseRate;
                Cost = cost;
            }
        }
        public class ShopBuffInfo
        {
            public string Type;
            public float IncreaseRate;
            public ShopBuffInfo(string type, float increaseRate)
            {
                Type = type;
                IncreaseRate = increaseRate;
            }
        }
        void LoadMapData()
        {
            string directory = "../../../../../Common/MapData/";
            string filePath = Path.Combine(directory, "Map_1.txt");

            if (File.Exists(filePath))
            {
                MapData = File.ReadAllText(filePath);
            }
            else
            {
                Console.WriteLine("no exist MapData");
            }
        }
    }
}