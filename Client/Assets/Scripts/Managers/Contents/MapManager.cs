using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Protobuf.Protocol;
using System.Collections;
using UnityEngine;
using System.IO;

public class MapManager
{
    public Grid CurrentGrid { get; private set; }
    public int MinX { get; set; }
    public int MaxX { get; set; }
    public int MinY { get; set; }
    public int MaxY { get; set; }
    bool[,] _collision;
    // 맨 왼쪽의 좌표를 현재 내 좌표에서 빼줘야 collision 에서의 x, y 인덱스가 나옴 - TODO
    public bool CanGo(Vector3 pos)
    {
        // 맵 범위 밖
        if (pos.x < MinX || pos.x > MaxX || pos.y < MinY || pos.y > MaxY)
            return false;

        // 범위 안쪽 계산
        int x = (int)Math.Round(pos.x) - MinX;
        int y = MaxY - (int)Math.Round(pos.y);
        return !_collision[y, x];
    }
    // 처음엔 1개지만 추가 개발 가능하면 2개로 늘려보기
    public void LoadMap(int mapId = 1)
    {
        try
        {
            string mapName = "Map_" + mapId;
            GameObject map = GameObject.Find(mapName);

            //GameObject collision = Util.FindChild(map, "Tilemap_Collision", true);

            CurrentGrid = map.GetComponent<Grid>();
            // Collision 정보 추출
            TextAsset txt = Managers.Resource.Load<TextAsset>($"Data/Map/{mapName}");
            // 문장 단위 추출 용이
            StringReader reader = new StringReader(txt.text);

            MinX = int.Parse(reader.ReadLine());
            MaxX = int.Parse(reader.ReadLine());
            MinY = int.Parse(reader.ReadLine());
            MaxY = int.Parse(reader.ReadLine());

            int xCount = MaxX - MinX + 1;
            int yCount = MaxY - MinY + 1;
            _collision = new bool[yCount, xCount];
            //GameObject folder = new GameObject();
            //folder.name = "folder";
            //GameObject g = Managers.Resource.Load<GameObject>("Circle");


            for (int y = 0; y < yCount; y++)
            {
                string line = reader.ReadLine();
                //if (line == null)
                //Debug.Log($"Line ({y}에서 에러)");
                int lineCount = line.Length;
                //Debug.Log($"Line ({y} 출력중)");

                for (int x = 0; x < xCount; x++)
                {
                    try
                    {
                        char c = line[x];
                        bool isCollision = (c == '1' ? true : false);
                        _collision[y, x] = isCollision;
                        if (_collision[y, x] == true)
                        {
                            int posX = x + MinX;
                            int posY = MaxY - y;
                            //GameObject circle = GameObject.Instantiate(g, new Vector3(posX, posY, 100f), Quaternion.identity);
                            //circle.transform.SetParent(folder.transform);
                            //circle.GetComponent<SpriteRenderer>().sortingOrder = 500;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log($"Error ({y}, {x})");
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    public void DestroyMap(int mapId)
    {
        GameObject map = GameObject.Find($"Map_{mapId}");
        if (map != null)
        {
            Managers.Resource.Destroy(map);
        }
    }
}