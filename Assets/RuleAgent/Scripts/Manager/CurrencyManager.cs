using UnityEngine;
using UnityEngine.Events;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager I { get; private set; }
    public int Currency { get; private set; }

    public UnityEvent<int> OnCurrencyChanged;

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

    public void AddCurrency(int amount)
    {
        Currency += amount;
        OnCurrencyChanged?.Invoke(Currency);
    }

    public bool SpendCurrency(int amount)
    {
        if (Currency < amount) return false;
        Currency -= amount;
        OnCurrencyChanged?.Invoke(Currency);
        return true;
    }
}