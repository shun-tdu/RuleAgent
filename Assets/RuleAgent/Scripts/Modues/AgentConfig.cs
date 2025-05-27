using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Agent/AgentConfig")]
public class AgentConfig : ScriptableObject
{
    [Tooltip("使用可能なセンサー一覧.ON/OFFで切り替えられる")]
    public SensorModuleSO[] allSensors;

    [Tooltip("対応する評価関数モジュール（重みはEvaluatorWeightに格納）")]
    public EvaluationModuleSO[] allEvaluators;

    [Tooltip("センサーを有効にするかどうか.allSensorsと同順序で管理")]
    public bool[] sensorEnabled;

    [Tooltip("評価関数ごとの重み.allEvaluatorと同順序で管理")]
    public float[] evaluatorWeights;

    [Tooltip("センサの最大数")] public int maxModules = 5;

    [Tooltip("Agentの移動速度")] public float moveSpeed = 5;

    [Tooltip("Agentの視野範囲")] public int visionRange = 5;
}