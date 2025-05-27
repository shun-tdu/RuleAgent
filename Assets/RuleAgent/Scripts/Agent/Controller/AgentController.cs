using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AgentController : MonoBehaviour, ITeleportable
{
    /*----強化学習関連のフィールド----*/
    [Header("強化学習設定")] [Tooltip("線形近似の学習率")] [SerializeField]
    private float alpha = 0.1f;

    [Tooltip("割引率")] [SerializeField] private float gamma = 0.9f;

    private float[] _weights; //wベクトル(センサー数分)
    private Queue<(List<float>features, float reward)> _creditBuffer;
    private int _bufferLength = 5; //遡って報酬を割り当てるステップ数
    private int _pendingHumanReward = 0; //次のUpdateで消費するフィードバック


    /*----グリッドベース移動制御のフィールド----*/
    private GridManager _grid;
    private Transform _goalTransform;
    private SensorModuleSO[] _sensors;
    private EvaluationModuleSO[] _evaluators;
    private float _moveSpeed = 3f;
    private int _visionRange = 5;
    private int _maxModules = 5;

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

        //強化学習 : 重みとバッファ初期化
        int featureCount = _sensors.Length;
        _weights = new float[featureCount];
        for (int i = 0; i < featureCount; i++) _weights[i] = 0f;
        _creditBuffer = new Queue<(List<float> features, float reward)>(_bufferLength + 1);
    }

    private void OnEnable()
    {
        FeedbackUI.OnFeedback += ReceiveFeedback;
    }

    private void OnDisable()
    {
        FeedbackUI.OnFeedback -= ReceiveFeedback;
    }

    /// <summary>
    /// AgentConfigから依存をまとめて注入するメソッド
    /// </summary>
    public void Initialize(AgentConfig config, GridManager grid, Transform goalTransform)
    {
        _sensors = config.allSensors.Where((s, i) => config.sensorEnabled[i]).ToArray();
        _evaluators = config.allEvaluators;
        _maxModules = config.maxModules;
        _moveSpeed = config.moveSpeed;
        _visionRange = config.visionRange;

        _grid = grid;
        _goalTransform = goalTransform;

        foreach (var s in _sensors)
            if (s is DistanceSensorSO ds)
                ds.Initialize(_goalTransform, _grid, _visionRange);

        _currentGrid = _grid.WorldToCell(transform.position);
        _targetGrid = _currentGrid;
        transform.position = _grid.CellToWorld(_currentGrid.x, _currentGrid.y) + Vector3.up * 0.5f;

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
            float eps = 1f - Mathf.Clamp01((float)_sensors.Length / _maxModules);

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
            if (_grid.InBounds(cand) && _grid.IsWalkable(cand) && _grid.IsOneWayAllowed(_currentGrid, cand))
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
            if (!_grid.InBounds(cand) || !_grid.IsWalkable(cand) || !_grid.IsOneWayAllowed(_currentGrid, cand))
                continue;
            float sum = 0f;
            for (int i = 0; i < _sensors.Length; i++)
            {
                float sv = _sensors[i].Sense(cand, _grid);
                sum += _evaluators[i].Evaluate(cand, sv);
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
        var targetPos = _grid.CellToWorld(_targetGrid.x, _targetGrid.y) + Vector3.up * 0.5f;
        transform.position = Vector3.MoveTowards(
            transform.position, targetPos, _moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            _isMoving = false;
            _currentGrid = _targetGrid;
            VisitedManager.I.MarkVisited(_currentGrid);

            if (_currentGrid == _grid.WorldToCell(_goalTransform.position))
            {
                _isGoal = true;
            }
        }
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
    /// worldPosにテレポートする
    /// </summary>
    public void TeleportTo(Vector3 worldPos)
    {
        Debug.Log("Teleport!!!");
        transform.position = worldPos;
        _currentGrid = _grid.WorldToCell(worldPos);
        VisitedManager.I.MarkVisited(_currentGrid);
    }


    /// <summary>
    /// Agent死亡時の処理
    /// </summary>
    private void HandleDeath()
    {
        _isMoving = false;
        UIManager.I.ShowGameOverUI();
    }

    /*----強化学習周りのメソッド----*/
    /// <summary>
    /// Good(+1)/Bad(-1)が飛んできたらキューに投げる
    /// </summary>
    private void ReceiveFeedback(int humanReward)
    {
        _pendingHumanReward += humanReward;
    }

    /// <summary>
    /// 候補セルに対するセンサー×評価モジュールによる特徴量φベクトル
    /// </summary>
    private List<float> ComputeFeatures(Vector2Int cand)
    {
        var phy = new List<float>(_sensors.Length);
        for (int i = 0; i < _sensors.Length; i++)
        {
            float sv = _sensors[i].Sense(cand, _grid);
            float score = _evaluators[i].Evaluate(cand, sv);
            phy.Add(score);
        }

        return phy;
    }

    /// <summary>
    /// 線形近似 Q(s,a) = W・φ
    /// </summary>
    private float EstimateQ(IReadOnlyList<float> phy)
    {
        float q = 0f;
        for (int i = 0; i < phy.Count; i++)
            q += _weights[i] * phy[i];
        return q;
    }

    /// <summary>
    /// バッファに入った過去 k ステップ分に遡って報酬を割り当て、wを更新
    /// </summary>
    private void ApplyCreditAssignment(float envReward)
    {
        //環境報酬　+ 人間報酬
        float r = envReward + _pendingHumanReward;
        _pendingHumanReward = 0;

        //ステップ分割当
        float perReward = r;
        while (_creditBuffer.Count > 0)
        {
            var (phy, _) = _creditBuffer.Dequeue();
            float qPred = EstimateQ(phy);
            float target = perReward;
            float delta = target - qPred;
            for (int i = 0; i < _weights.Length; i++)
                _weights[i] += alpha * delta * phy[i];
            //減衰させる場合は perReward * gamma
        }
    }
}