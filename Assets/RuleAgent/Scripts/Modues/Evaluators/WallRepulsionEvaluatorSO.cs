using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Modules/Evaluator/WallRepulsion")]
public class WallRepulsionEvaluatorSO : EvaluationModuleSO
{
    [Tooltip("壁1マスあたりのペナルティ（正の値）")] public float weight = 5f;

    /// <summary>
    /// センサーが大きいほどペナルティを大きく返す
    /// </summary>
    public override float Evaluate(Vector2Int agentPos, float sensorValue)
    {
        return -sensorValue * weight;
    }
}
