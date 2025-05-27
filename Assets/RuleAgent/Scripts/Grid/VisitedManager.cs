using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Agentが訪れたCellを管理するクラス
/// </summary>
public class VisitedManager : MonoBehaviour
{
    public static VisitedManager I { get; private set; }

    private HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

    private void Awake()
    {
        if (I == null) I = this;
        else if(I != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void MarkVisited(Vector2Int cell)
    {
        visited.Add(cell);
    }

    public bool HasVisited(Vector2Int cell)
    {
        return visited.Contains(cell);
    }

    public void ResetVisited()
    {
        visited.Clear();
    }
    
    /// <summary>
    /// 訪問済みのセルを列挙する
    /// </summary>
    /// <returns>訪問済みのセル</returns>
    public IEnumerable<Vector2Int> GetVisitedCells()
    {
        return visited;
    }
}
