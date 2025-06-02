using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapManager : MonoBehaviour
{
    [SerializeField] private LevelData levelData;
    [SerializeField] private GridManager grid;
    private void Awake()
    {
        foreach (var info in levelData.trapList)
        {
            Vector3 worldPos = grid.CellToWorld(info.position.x, info.position.y) + Vector3.up * 0.5f;
            Instantiate(info.trapPrefab, worldPos, Quaternion.identity, transform);
        }
    }
        
    /// <summary>
    /// 指定した位置にトラップが存在するかを返す
    /// </summary>
    public bool IsTrap(Vector2Int targetPosition)
    {
        return levelData.trapList.Exists(trap => trap.position == targetPosition);
    }
}
