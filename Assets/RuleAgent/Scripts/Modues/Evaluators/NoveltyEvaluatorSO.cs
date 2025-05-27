using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Modules/Evaluator/Novelty")]
public class NoveltyEvaluatorSO : EvaluationModuleSO
{
    [Tooltip("未訪問セル1つあたりの得点")] public float weight = 5f;

    public override float Evaluate(Vector2Int agentPos, float sensorValue)
    {
        return sensorValue * weight;
    }
}