using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AgentControllerEpsilonGreedy : MonoBehaviour, ITeleportable
{
    public GridManager grid;
    public Transform goalTransform;

    [Header("移動設定")] public float moveSpeed = 3f;
    public int visionRange = 5;

    [Header("学習設定")] [Tooltip("Wander 時に使う最大モジュール数")]
    public int maxModules = 5;

    [Header("モジュール設定")] public SensorModuleSO[] sensors;
    public EvaluationModuleSO[] evaluators;


    private Vector2Int _currentGrid;
    private Vector2Int _targetGrid;
    private bool _isMoving;
    private bool _isGoal = false;
    private Rigidbody _rb;
    private AgentStatus _agentStatus;

    private void Awake()
    {
        _agentStatus = GetComponent<AgentStatus>();
        _agentStatus.OnDeath += HandleDeath;
    }

    private void Start()
    {
        //Agent位置初期化
        _currentGrid = grid.WorldToCell(transform.position);
        _targetGrid = _currentGrid;
        transform.position = grid.CellToWorld(_currentGrid.x, _currentGrid.y) + Vector3.up * 0.5f;

        //センサーの初期化
        foreach (var s in sensors)
        {
            if (s is DistanceSensorSO ds)
                ds.Initialize(goalTransform, grid, visionRange);
        }

        //VisitedManagerの初期化
        VisitedManager.I.MarkVisited(_currentGrid);
    }

    private void Update()
    {
        if (!_isGoal)
        {
            UddateEpsilonGreedy();
        }
    }

    /// <summary>
    /// Epsilon Greedyによるランダムウォーク and 評価関数による移動先候補の計算
    /// </summary>
    void UddateEpsilonGreedy()
    {
        if (!_isMoving)
        {
            //εを計算:モジュールが大きいほどεは小さく、賢い選択に寄る
            float eps = 1f - Mathf.Clamp01((float)sensors.Length / maxModules);

            //ランダムウォーク vs. 評価関数選択
            if (Random.value < eps)
                DoRandomWalk();
            else
                DoEvaluatedWalk();
        }
        else MoveToTarget();
    }

    /// <summary>
    /// ランダムウォークを計算する関数
    /// </summary>
    void DoRandomWalk()
    {
        var dirs = new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right, };
        var choices = new List<Vector2Int>();
        foreach (var dir in dirs)
        {
            var cand = _currentGrid + dir;
            if (grid.InBounds(cand) && grid.IsWalkable(cand) && grid.IsOneWayAllowed(_currentGrid, cand))
                choices.Add(cand);
        }

        if (choices.Count > 0)
        {
            _targetGrid = choices[Random.Range(0, choices.Count)];
            _isMoving = true;
        }
    }

    /// <summary>
    /// センサーを元にAgentの上下左右セルの評価値を計算
    /// 評価値が最も高いマスに進む
    /// </summary>
    void DoEvaluatedWalk()
    {
        var dirs = new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right, };
        float bestScore = float.MinValue;
        var bestCands = new List<Vector2Int>();

        //上下左右に対してスコアを計算
        foreach (var cand in dirs.Select(d => _currentGrid + d))
        {
            if (!grid.InBounds(cand) || !grid.IsWalkable(cand) || !grid.IsOneWayAllowed(_currentGrid, cand))
                continue;
            float sum = 0f;
            for (int i = 0; i < sensors.Length; i++)
            {
                float sv = sensors[i].Sense(cand, grid);
                sum += evaluators[i].Evaluate(cand, sv);
            }

            if (sum > bestScore)
            {
                bestScore = sum;
                bestCands.Clear();
                bestCands.Add(cand);
            }
            else if (Mathf.Approximately(sum, bestScore))
            {
                bestCands.Add(cand);
            }
        }

        if (bestCands.Count > 0)
        {
            //同点候補からランダムに選ぶ
            _targetGrid = bestCands[Random.Range(0, bestCands.Count)];
            _isMoving = true;
        }
    }


    /// <summary>
    /// Targetセルに移動するための関数
    /// </summary>
    void MoveToTarget()
    {
        var targetPos = grid.CellToWorld(_targetGrid.x, _targetGrid.y) + Vector3.up * 0.5f;
        transform.position = Vector3.MoveTowards(
            transform.position, targetPos, moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            _isMoving = false;
            _currentGrid = _targetGrid;
            VisitedManager.I.MarkVisited(_currentGrid);

            if (_currentGrid == grid.WorldToCell(goalTransform.position))
            {
                Debug.Log("Arrived at Goal!");
                _isGoal = true;
            }
        }
    }


    /// <summary>
    /// worldPosにテレポートする
    /// </summary>
    public void TeleportTo(Vector3 worldPos)
    {
        transform.position = worldPos;
        _currentGrid = grid.WorldToCell(worldPos);
        VisitedManager.I.MarkVisited(_currentGrid);
    }


    /// <summary>
    /// Teleporterから呼ばれる強制停止用メソッド
    /// </summary>
    public void ForceStopMovement()
    {
        _isMoving = false;
        _targetGrid = _currentGrid;
        if (_rb != null)
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }

    /// <summary>
    /// Agent死亡時の処理
    /// </summary>
    private void HandleDeath()
    {
        _isMoving = false;
        // todo ゲームオーバ処理を追加
    }
}