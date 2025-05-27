using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Modules/Evaluator")]
public class EvaluationModuleSO : ScriptableObject
{
    public virtual float Evaluate(Vector2Int agentPos, float sensorValue)
    {
        return 0f;
    }
}