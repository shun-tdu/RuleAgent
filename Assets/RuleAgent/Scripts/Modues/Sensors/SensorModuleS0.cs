using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Modules/Sensor")]
public class SensorModuleSO : ScriptableObject
{
    public virtual float Sense(Vector2Int agentPos, GridManager grid)
    {
        return 0f;
    }
}