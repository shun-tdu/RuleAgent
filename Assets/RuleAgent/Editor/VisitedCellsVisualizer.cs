using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// VisitedManagerを可視化するクラス
/// </summary>
public class VisitedCellsVisualizer : MonoBehaviour
{
    [Header("参照するグリッドマネージャー")] public GridManager grid;
    [Header("描画色")] public Color visitedColor = new Color(0f, 0.5f, 1f, 0.3f);

    private void OnDrawGizmos()
    {
        if (grid == null)
            grid = FindFirstObjectByType<GridManager>();
        if (grid == null || VisitedManager.I == null)
            return;

        Gizmos.color = visitedColor;
        foreach (var cell in VisitedManager.I.GetVisitedCells())
        {
            Vector3 center = grid.CellToWorld(cell.x, cell.y);
            center.y += 0.1f;
            float s = grid.CellSize;
            Gizmos.DrawCube(center, new Vector3(s, 0.1f, s));
        }
    }
}