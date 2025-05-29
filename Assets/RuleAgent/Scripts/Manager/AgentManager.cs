using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    public static AgentManager I { get; private set; }

    private readonly List<AgentController> _agents = new List<AgentController>();

    private void Awake()
    {
        //シングルトン初期化
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);
    }
    
    /// <summary>
    /// エージェントを登録
    /// </summary>
    public void Register(AgentController agent)
    {
        if (!_agents.Contains(agent))
            _agents.Add(agent);
    }
    
    /// <summary>
    /// エージェントを登録解除
    /// </summary>
    public void Unregister(AgentController agent)
    {
        _agents.Remove(agent);
    }
    
    /// <summary>
    /// 全エージェントを返す
    /// </summary>
    public IReadOnlyList<AgentController> GetAllAgents() => _agents;
    
    /// <summary>
    /// 最初に登録されたエージェントを返す
    /// </summary>
    public AgentController GetPrimaryAgent() => _agents.FirstOrDefault();
}