
using UnityEngine;

public class CrystalPickUp : MonoBehaviour
{
    [HideInInspector] public int pointValue;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<AgentStatus>(out var status))
        {
            CurrencyManager.I.AddCurrency(pointValue);
            Destroy(gameObject);
        }
    }
}
