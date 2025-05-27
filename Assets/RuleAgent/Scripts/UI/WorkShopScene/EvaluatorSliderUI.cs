using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EvaluatorSliderUI : MonoBehaviour
{
    [HideInInspector] public int index;
    public TMP_Text label;
    public Slider slider;
    public TMP_InputField inputField;

    private void Reset()
    {
        slider = GetComponentInChildren<Slider>();
        inputField = GetComponentInChildren<TMP_InputField>();
        label = GetComponentInChildren<TMP_Text>();
    }

    private void Awake()
    {
        slider.onValueChanged.AddListener(OnSliderChanged);
        inputField.onEndEdit.AddListener(OnInputFieldChanged);
    }

    private void OnSliderChanged(float v)
    {
        inputField.text = v.ToString("0.00");
    }

    private void OnInputFieldChanged(string str)
    {
        if (float.TryParse(str, out var v))
            slider.value = v;
        else
            inputField.text = slider.value.ToString("0.00");
    }
}