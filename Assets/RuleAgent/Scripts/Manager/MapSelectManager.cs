using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MapSelectManager : MonoBehaviour
{
    [Header("マップリスト")] [SerializeField] private string[] mapSceneNames;
    [Header("UI設定")] [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject mapButtonPrefab;

    [Header("戻るボタン")] [SerializeField] private Button backButton;

    [Header("AgentEditボタン")] [SerializeField]
    private Button editAgentButton;

    private void Start()
    {
        foreach (var sceneName in mapSceneNames)
        {
            var btnObj = Instantiate(mapButtonPrefab, contentParent);

            var label = btnObj.GetComponentInChildren<TextMeshProUGUI>();

            label.text = sceneName;

            var button = btnObj.GetComponent<Button>();
            string target = sceneName;
            button.onClick.AddListener((() => SceneManager.LoadScene(target)));
        }

        if (backButton != null)
            backButton.onClick.AddListener((() => SceneManager.LoadScene("StartMenuScene")));

        if (editAgentButton != null)
            editAgentButton.onClick.AddListener(() => { SceneManager.LoadScene("WorkShopScene"); });
    }
}