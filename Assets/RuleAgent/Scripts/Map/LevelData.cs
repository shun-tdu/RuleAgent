using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(menuName = "Level/LevelData2D")]
public class LevelData : ScriptableObject
{
    public enum TileType
    {
        Floor = 0,
        Wall = 1,
        OneWayUp = 2,
        OneWayDown = 3,
        OneWayLeft = 4,
        OneWayRight = 5,
        TeleportPad = 6
    };

    public int width = 20, height = 10;
    public float cellSize = 1f;
    
    //レベルデータ
    [Tooltip("0=床,1=壁,2=一方通行(上),3=一方通行(下),4=一方通行(左),5=一方通行(右)")]
    [Serializable]
    public struct Row
    {
        public int[] cols;
    }
    public Row[] rows;
    
    //テレポーター情報
    [Serializable]
    public struct TeleportInfo
    {
        public Vector2Int source;
        public Vector2Int destination;
    }
    public List<TeleportInfo> teleportList;
    
    //トラップ情報
    [Serializable]
    public struct TrapInfo
    {
        public Vector2Int position;
        public GameObject trapPrefab;
    }
    public List<TrapInfo> trapList;
    
    //クリスタル情報
    [Serializable]
    public struct CrystalInfo
    {
        public Vector2Int position;
        public CrystalType type;
    }

    public List<CrystalInfo> crystalList;

    private void OnValidate()
    {
        if (rows == null || rows.Length != height)
            System.Array.Resize(ref rows, height);
        for (int i = 0; i < rows.Length; i++)
        {
            if (rows[i].cols == null || rows[i].cols.Length != width)
                System.Array.Resize(ref rows[i].cols, width);
        }
    }

    public int GetTile(int x, int y)
    {
        return rows[y].cols[x];
    }

    public TileType GetTileType(int x, int y)
    {
        return (TileType)rows[y].cols[x];
    }

    public void SetTileType(int x, int y, TileType t)
        => rows[y].cols[x] = (int)t;


    public bool IsWalkable(int x, int y)
    {
        return rows[y].cols[x] != 1;
    }
}