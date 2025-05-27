using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Modules/Evaluator/InvDistance")]
public class InverseDistanceEvaluatorSO : EvaluationModuleSO
{
    public override float Evaluate(Vector2Int agentPos, float sensorValue)
    {
        // 距離が近いほど大きい評価
        return 1f / (1f + sensorValue);
    }
}