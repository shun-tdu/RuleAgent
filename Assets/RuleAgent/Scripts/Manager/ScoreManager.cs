using System;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager I { get; private set; }

    public int Score { get; private set; }

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// スコアを加算してUIを更新する
    /// </summary>
    public void AddScore(int points)
    {
        Score += points;
        UIManager.I.UpdateScoreDisplay(Score);
    }
    
    /// <summary>
    /// スコアをリセットする(新しいステージ開始時などに呼ぶ)
    /// </summary>
    public void ResetScore()
    {
        Score = 0;
        UIManager.I.UpdateScoreDisplay(Score);
    }
}