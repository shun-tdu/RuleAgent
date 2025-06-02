using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;


public enum GameState
{
    StartMenu,
    MapSelect,
    WorkShop,
    Playing,
    Paused,
    Cleared,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }
    public GameState State { get; private set; }

    private void Awake()
    {
        //GameManagerのシングルトン化
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);

        //他のシングルトンを初期化
        if (CurrencyManager.I == null) new GameObject("CurrencyManager").AddComponent<CurrencyManager>();
        if (ScoreManager.I == null) new GameObject("ScoreManager").AddComponent<ScoreManager>();
        if (AgentManager.I == null) new GameObject("AgentManager").AddComponent<AgentManager>();
    }

    private void Start()
    {
        ChangeState(GameState.StartMenu);
    }

    private void ChangeState(GameState newState)
    {
        State = newState;
        switch (newState)
        {
            case GameState.StartMenu:
                SceneManager.LoadScene("StartMenuScene");
                break;
            case GameState.MapSelect:
                SceneManager.LoadScene("MapSelectScene");
                break;
            case GameState.WorkShop:
                SceneManager.LoadScene("WorkShopScene");
                break;
            case GameState.Playing:
                //ステージのロード処理
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
            case GameState.Cleared:
                HandleStageClear();
                break;
            case GameState.GameOver:
                HandleGameOver();
                break;
        }
    }

    /// <summary>
    /// ステージクリア時の一連処理(内部専用)
    /// </summary>
    private void HandleStageClear()
    {
        Time.timeScale = 0f;
        
        var agents = AgentManager.I.GetAllAgents();
        if (agents == null || agents.Count == 0)
        {
            Debug.LogWarning("OnStageClear:登録されたエージェントがいません");
            return;
        }

        var primary = AgentManager.I.GetPrimaryAgent();
        if (primary == null)
        {
            Debug.LogWarning("OnStageClear:代表Agentが見つかりません");
            return;
        }

        //学習済みモデルを組み立て
        var model = new LeanedModel
        {
            weights = primary.GetWeights(),
            sensorEnabled = primary.GetSensorEnabledFlag()
        };

        //学習済みモデルを保存
        ModelPersistence.Save(model);
        Debug.Log("OnStageClear :モデルを保存しました");

        //クリアUI表示
        UIManager.I.ShowStageClearUI();
        
        //スタートメニュへ遷移
        ChangeState(GameState.StartMenu);
    }


    /// <summary>
    /// ゲームオーバ時の一連の処理
    /// </summary>
    private void HandleGameOver()
    {
        Time.timeScale = 0f;
        if (UIManager.I != null)
        {
            UIManager.I.ShowGameOverUI();
        }
        else
        {
            Debug.LogWarning("HandleGameOver: UIManager がアタッチされていません");
        }
    }


    /*--------Publicメソッド--------*/
    
    /// <summary>
    /// ゲームクリア時の処理を一括で行う
    /// </summary>
    public void TriggerStageClear()
    {
        ChangeState(GameState.Cleared);
    }
    
    /// <summary>
    /// ゲームオーバ時の処理を一括で行う
    /// </summary>
    public void TriggerGameOver()
    {
        ChangeState(GameState.GameOver);
    }

    /// <summary>
    /// ステージをリセットする
    /// </summary>
    public void TriggerRestartStage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        ChangeState(GameState.Playing);
    }


    /// <summary>
    /// 次のステージへ遷移
    /// </summary>
    /// <param name="nextScene"></param>
    public void LoadNextStage(string nextScene)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextScene);
        ChangeState(GameState.Playing);
    }

    /// <summary>
    /// マップ選択画面へ
    /// </summary>
    public void LoadMapSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MapSelectScene");
        ChangeState(GameState.MapSelect);
    }


    /// <summary>
    /// ワークショップ画面へ
    /// </summary>
    public void LoadWorkShop()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("WorkShopScene");
        ChangeState(GameState.WorkShop);
    }


    /// <summary>
    /// オプション画面へ
    /// </summary>
    public void LoadOption()
    {
        Time.timeScale = 1f;
        //
    }
}