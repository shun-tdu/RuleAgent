using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStatus : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP { get; private set; }

    public event Action OnDeath;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        currentHP = Mathf.Max(0, currentHP - amount);
        UIManager.I.UpdateHP(currentHP, maxHP);
        if (currentHP == 0)
            OnDeath?.Invoke();
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(maxHP, currentHP + amount);
        UIManager.I.UpdateHP(currentHP, maxHP);
    }
}