using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Transform goalTransform;
    [SerializeField] private Transform spawnPoint;

    private void Start()
    {
        var go = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);

        var agent = go.GetComponent<AgentController>();

        if (agent == null)
        {
            Debug.LogError("PlayerPrefab に AgentController がアタッチされていません。");
            return;
        }

        var cfg = CustomizationState.CurrentConfig;
        
        //依存性の注入
        agent.Initialize(cfg, gridManager, goalTransform);

        UIManager.I.UpdateHP(100, 100);
    }
}