using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Modules/Sensor/Distance")]
public class DistanceSensorSO : SensorModuleSO
{
    [Tooltip("視野範囲(マンハッタン距離)")] public int visionRange = 5;

    private Vector2Int goalPos;
    private bool initialized = false;

    // MonoBehaviour 側から Transform と GridManager を受け取る
    public void Initialize(Transform goalTransform, GridManager grid, int visionRange)
    {
        this.visionRange = visionRange;
        if (goalTransform == null)
        {
            Debug.LogError("DistanceSensorSO.Initialize: goalTransform が null です");
            return;
        }

        goalPos = grid.WorldToCell(goalTransform.position);
        initialized = true;
    }

    public override float Sense(Vector2Int agentPos, GridManager grid)
    {
        if (!initialized)
            return float.MaxValue;

        int md = Mathf.Abs(agentPos.x - goalPos.x) + Mathf.Abs(agentPos.y - goalPos.y);
        if (md > visionRange)
            return float.MaxValue;

        foreach (var c in BresenhamLine(agentPos, goalPos))
            if (!grid.IsWalkable(c))
                return float.MaxValue; //壁に遮られて見えない

        return Vector2Int.Distance(agentPos, goalPos);
    }

    //AgentとGoal間にWalkableじゃないマスがあるかを判定
    IEnumerable<Vector2Int> BresenhamLine(Vector2Int a, Vector2Int b)
    {
        int x0 = a.x, y0 = a.y, x1 = b.x, y1 = b.y;
        int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = -Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = dx + dy;
        while (true)
        {
            yield return new Vector2Int(x0, y0);
            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 >= dy)
            {
                err += dy;
                x0 += sx;
            }

            if (e2 <= dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }
}