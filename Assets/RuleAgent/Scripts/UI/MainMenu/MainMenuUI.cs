using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button mapSelectButton;
    [SerializeField] private Button workShopButton;
    [SerializeField] private Button optionButton;

    private void Awake()
    {
        mapSelectButton.onClick.AddListener(GameManager.I.LoadMapSelect);
        workShopButton.onClick.AddListener(GameManager.I.LoadWorkShop);
        optionButton.onClick.AddListener(GameManager.I.LoadOption);
    }
}
