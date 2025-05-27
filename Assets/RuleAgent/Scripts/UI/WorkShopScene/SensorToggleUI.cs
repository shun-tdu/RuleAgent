using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Toggle))]
public class SensorToggleUI : MonoBehaviour
{
    [HideInInspector] public int index;
    public Toggle toggle;
    public TMP_Text label;

    private void Reset()
    {
        toggle = GetComponent<Toggle>();
        label = GetComponentInChildren<TMP_Text>();
    }
}