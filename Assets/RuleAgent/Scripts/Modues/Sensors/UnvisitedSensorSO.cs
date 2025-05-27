using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Modules/Sensor/Unvisited")]
public class UnvisitedSensorSO : SensorModuleSO
{
    private static readonly Vector2Int[] dirs =
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
    };

    public override float Sense(Vector2Int agentPos, GridManager grid)
    {
        int count = 0;
        foreach (var dir in dirs)
        {
            var n = agentPos + dir;
            if (grid.InBounds(n) && !VisitedManager.I.HasVisited(n))
                count++;
        }
        return count;
    }
}