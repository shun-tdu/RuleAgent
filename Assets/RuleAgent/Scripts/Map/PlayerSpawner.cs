using System;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Transform goalTransform;
    [SerializeField] private Transform spawnPoint;

    private void Start()
    {
        Debug.Log("Player Spawner has called");
        var model = ModelPersistence.Load();
        var cfg = CustomizationState.CurrentConfig;
        if (cfg == null)
        {
            Debug.LogError("Player Spawner: CurrentConfigがnullです。workshopを先に開いてください");
            return;
        }
        
        if (model != null)
        {
            Debug.Log("Model has existed");
            //重みの反映
            if (model.weights != null && model.weights.Length == cfg.evaluatorWeights.Length)
            {
                cfg.evaluatorWeights = (float[])model.weights.Clone();
            }
            else
            {
                Debug.LogWarning("Loaded model の weights 長さが config と一致しません。読み飛ばします。");
            }
            //センサのEnableの反映
            if(model.sensorEnabled != null&&model.sensorEnabled.Length == cfg.sensorEnabled.Length)
            {
                cfg.sensorEnabled = (bool[])model.sensorEnabled.Clone();
            }
            else
            {
                Debug.LogWarning("Loaded model の sensorEnabled 長さが config と一致しません。読み飛ばします。");
            }
        }
        
        /*----Agent生成処理----*/
        var go = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        var agent = go.GetComponent<AgentController>();
        if (agent == null)
        {
            Debug.LogError("PlayerPrefab に AgentController がアタッチされていません。");
            return;
        }
        
        /*----依存性の注入----*/
        agent.Initialize(cfg, gridManager, goalTransform);

        UIManager.I.UpdateHP(100, 100);
    }
}