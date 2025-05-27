using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpikeTrap : MonoBehaviour
{
    [Tooltip("プレイヤーに与えるダメージ量")] [SerializeField]
    private int damage = 10;

    [Tooltip("一度トリガーしたら無効化するならチェック")] [SerializeField]
    private bool oneShoot = true;

    private Collider _col;

    private void Awake()
    {
        _col = GetComponent<Collider>();
        _col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<AgentStatus>(out var status))
        {
            //Agentにダメージを与える
            status.TakeDamage(damage);
            //一度発動したら無効化and消去
            if (oneShoot)
            {
                _col.enabled = false;
                Destroy(gameObject, 0.1f);
            }
        }
    }
}