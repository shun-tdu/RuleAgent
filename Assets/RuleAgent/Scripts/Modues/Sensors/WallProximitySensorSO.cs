using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

[CreateAssetMenu(menuName = "Modules/Sensor/WallProximity")]
public class WallProximitySensorSO : SensorModuleSO
{
    private static readonly Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right, };

    /// <summary>
    /// 壁セルに隣接している方向の数を返す(0-4)
    /// </summary>
    public override float Sense(Vector2Int agentPos, GridManager grid)
    {
        int count = 0;
        foreach (var dir in dirs)
        {
            var n = agentPos + dir;
            if (!grid.InBounds(n) || !grid.IsWalkable(n))
                count++;
        }
        return count;
    }
}