using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapEditor
{

#if UNITY_EDITOR

    // % (Ctrl), # (Shift), & (Alt)

    [MenuItem("Tools/GenerateMap %#q")]
    private static void GenerateMap()
    {
        GenerateMapByPath("Assets/Resources/Prefabs/Data/Map");
        GenerateMapByPath("../Common/MapData");
    }
    private static void GenerateMapByPath(string pathPrefix)
    {
        // 임시로 1번만 넣기 - 추후 여유되면 추가 개발
        GameObject go = Resources.Load<GameObject>("Prefabs/Map/Map_1");
        
        Tilemap tmBase = Util.FindChild<Tilemap>(go, "Tilemap_Base", true);
        Tilemap tm = Util.FindChild<Tilemap>(go, "Tilemap_RealCollision", true);
        //Tilemap tm = Util.FindChild<Tilemap>(go, "Tilemap_Collision", true);
        int count = 0;
        var writer = File.CreateText($"{pathPrefix}/{go.name}.txt");
        writer.WriteLine(tmBase.cellBounds.xMin);
        writer.WriteLine(tmBase.cellBounds.xMax);
        writer.WriteLine(tmBase.cellBounds.yMin);
        writer.WriteLine(tmBase.cellBounds.yMax);
        
        for (int y = tmBase.cellBounds.yMax; y >= tmBase.cellBounds.yMin; y--)
        {
            string line = "";
            for (int x = tmBase.cellBounds.xMin; x <= tmBase.cellBounds.xMax; x++)
            {
                TileBase tile = tm.GetTile(new Vector3Int(x, y, 0));
                if (tile != null)
                {
                    line += "1";
                    count++;
                }
                else
                    line += "0";
            }
            writer.WriteLine(line);
        }
        writer.Close();

        Debug.Log("Generate Maps");
    }
#endif

}