using System.Collections.Generic;
using UnityEngine;


public class AgentController1 : MonoBehaviour
{
    public GridManager grid;
    public Transform goalTransform;

    [Header("移動設定")] public float moveSpeed = 3f;

    public int visionRange = 5;

    enum State
    {
        Wander,
        Chase
    };

    private State currentState = State.Wander;

    private Vector2Int currentGrid;
    private Vector2Int targetGrid;
    private bool isMovibg;
    private bool _isGoal = false;

    private List<Vector2Int> _currentPath;

    private void Start()
    {
        currentGrid = grid.WorldToCell(transform.position);
        targetGrid = currentGrid;
        transform.position = grid.CellToWorld(currentGrid.x, currentGrid.y) + Vector3.up * 0.5f;
    }

    private void Update()
    {
        // Debug.Log(currentState);
        
        switch(currentState)
        {
            case State.Wander:
                UpdateWander();
                break;
            case State.Chase:
                UpdateChase();
                break;
        }
    }

    void UpdateWander()
    {
        //ゴールが視野範囲に入ったらチェイス状態へ
        var goalGrid = grid.WorldToCell(goalTransform.position);
        if (IsInVision(currentGrid, goalGrid))
        {
            currentState = State.Chase;
            _currentPath = FindPath(currentGrid, goalGrid);
            return;
        }

        //ランダムウォーク
        if (!isMovibg)
        {
            var dirs = new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right, };
            var choices = new List<Vector2Int>();
            foreach (var dir in dirs)
            {
                var cand = currentGrid + dir;
                if (grid.InBounds(cand) && grid.IsWalkable(cand))
                    choices.Add(cand);
            }

            if (choices.Count > 0)
            {
                targetGrid = choices[Random.Range(0, choices.Count)];
                isMovibg = true;
            }
        }
        else MoveToTarget();
    }

    void UpdateChase()
    {
        if (_currentPath == null || _currentPath.Count == 0)
        {
            currentState = State.Wander;
            return;
        }

        if (!isMovibg)
        {
            if (_currentPath.Count >= 2)
            {
                targetGrid = _currentPath[1];
                isMovibg = true;
                _currentPath.RemoveAt(0);
            }
            else
            {
                _currentPath.Clear();
            }
        }
        else MoveToTarget();
    }

    void MoveToTarget()
    {
        var targetPos = grid.CellToWorld(targetGrid.x, targetGrid.y) + Vector3.up*0.5f;
        transform.position = Vector3.MoveTowards(
            transform.position, targetPos, moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            isMovibg = false;
            currentGrid = targetGrid;

            if (currentGrid == grid.WorldToCell(goalTransform.position))
            {
                Debug.Log("Arrived at Goal!");
                _isGoal = true;
            }
        }
    }

    bool IsInVision(Vector2Int from, Vector2Int to)
    {
        int md = Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
        if (md > visionRange) return false;
        //簡易LOSチェック
        //AgentとGoalの間に壁がある場合は視野を通さない
        foreach (var cell in BresenhamLine(from, to))
            if (!grid.IsWalkable(cell))
                return false;
        return true;
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

    // 単純 BFS で最短経路を返す（start と goal を含むリスト）
    List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        var q = new Queue<Vector2Int>();
        var visited = new Dictionary<Vector2Int, Vector2Int>();
        q.Enqueue(start);
        visited[start] = start;

        var dirs = new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        while (q.Count > 0)
        {
            var cur = q.Dequeue();
            if (cur == goal) break;
            foreach (var d in dirs)
            {
                var nxt = cur + d;
                if (!grid.InBounds(nxt) || !grid.IsWalkable(nxt)) continue;
                if (visited.ContainsKey(nxt)) continue;
                visited[nxt] = cur;
                q.Enqueue(nxt);
            }
        }

        // ゴールに到達できなかった
        if (!visited.ContainsKey(goal)) return null;

        // パス復元
        var path = new List<Vector2Int>();
        var node = goal;
        while (true)
        {
            path.Add(node);
            if (node == start) break;
            node = visited[node];
        }

        path.Reverse();
        return path;
    }
}