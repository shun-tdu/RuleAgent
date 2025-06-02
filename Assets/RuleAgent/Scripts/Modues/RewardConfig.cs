using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RewardConfig",menuName = "Modules/RewardConfig")]
public class RewardConfig : ScriptableObject
{
    [Header("探索報酬")] [Tooltip("未訪問セルに訪れたときに与える報酬")]
    public float unvisitedCellReward = 0.1f;

    [Tooltip("各ステップごとに与えるペナルティ")] public float stepPenalty = -0.01f;

    [Header("罠・ペナルティ")] [Tooltip("罠セルに触れたときに与える報酬")]
    public float trapPenalty = -1f;
    
    [Tooltip("罠予兆（距離センサーで察知）のときの報酬")] 
    public float nearTrapPenalty = -0.5f;

    [Header("ゴール報酬")]
    [Tooltip("ゴールに到達したときに与える報酬")] 
    public float goalReward = +1f;

    [Header("その他")]
    [Tooltip("人間フィードバック (Good/Bad) に掛け合わせる倍率")]
    public float humanFeedbackMultiplier = 1f;

}
