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
                OnStageClear();
                break;
            case GameState.GameOver:
                OnGameOver();
                break;
        }
    }

    private void OnStageClear()
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
    }

    private void OnGameOver()
    {
        Time.timeScale = 0f;
        
        // UIManager.I.ShowGameOverUI();
    }

    public void RestartStage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        ChangeState(GameState.Playing);
    }

    public void LoadNextStage(string nextScene)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextScene);
        ChangeState(GameState.Playing);
    }

    public void LoadMapSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MapSelectScene");
        ChangeState(GameState.MapSelect);
    }

    public void LoadWorkShop()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("WorkShopScene");
        ChangeState(GameState.WorkShop);
    }
    
    public void LoadOption()
    {
        Time.timeScale = 1f;
        //
    }
}