using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorkShopUI : MonoBehaviour
{
    [Header("設定データ")] [SerializeField] private AgentConfig config;

    [Header("UIプレファブ/コンテナ")] [SerializeField]
    private Transform sensorListParent;

    [SerializeField] private GameObject sensorTogglePrefab;
    [SerializeField] private Transform evaluatorListParent;
    [SerializeField] private GameObject evaluatorSliderPrefab;
    [SerializeField] private Button applyButton;

    private void Start()
    {
        //センサー一覧を動的生成
        for (int i = 0; i < config.allSensors.Length; i++)
        {
            var go = Instantiate(sensorTogglePrefab, sensorListParent);
            var ui = go.GetComponent<SensorToggleUI>();
            ui.index = i;
            ui.toggle.isOn = config.sensorEnabled[i];
            ui.label.text = config.allSensors[i].name;
            ui.toggle.onValueChanged.AddListener(val => { config.sensorEnabled[ui.index] = val; });
        }

        //評価関数スライダー一覧を動的生成
        for (int i = 0; i < config.allEvaluators.Length; i++)
        {
            var go = Instantiate(evaluatorSliderPrefab, evaluatorListParent);
            var ui = go.GetComponent<EvaluatorSliderUI>();
            ui.index = i;
            ui.label.text = config.allEvaluators[i].name;
            ui.slider.value = config.evaluatorWeights[i];
            ui.inputField.text = config.evaluatorWeights[i].ToString("0.00");
            ui.slider.onValueChanged.AddListener(val =>
            {
                config.evaluatorWeights[ui.index] = val;
                ui.inputField.text = val.ToString("0.00");
            });
            ui.inputField.onEndEdit.AddListener(text =>
            {
                if (float.TryParse(text, out var v))
                {
                    config.evaluatorWeights[ui.index] = v;
                    ui.slider.value = v;
                }
            });
        }
        
        //ApplyボタンにLoadSceneを追加
        applyButton.onClick.AddListener(() =>
        {
            //PlayerSpawnerにConfigを渡す仕組みがあればここで実装
            CustomizationState.CurrentConfig = config;
            SceneManager.LoadScene("StartMenuScene");
        });
    }
}