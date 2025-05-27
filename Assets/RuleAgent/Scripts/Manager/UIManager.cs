using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager I { get; private set; }

    [Header("HUD Elements")] [SerializeField]
    private Slider hpSlider;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private CanvasGroup gameOverGroup;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float popDuration = 0.4f;
    [SerializeField] private Vector3 hiddenScale = Vector3.zero;
    [SerializeField] private Vector3 shownScale = new Vector3(0.7f,0.8f,1.0f);

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);

        //GameOverGroupの初期化
        gameOverGroup.alpha = 0f;
        gameOverGroup.interactable = false;
        gameOverGroup.blocksRaycasts = false;
        gameOverGroup.transform.localScale = hiddenScale;
    }

    private void Start()
    {
        CurrencyManager.I.OnCurrencyChanged.AddListener(UpdateCurrencyDisplay);
    }

    /// <summary>
    /// PlayerHPの更新
    /// </summary>
    public void UpdateHP(int current, int max)
    {
        hpSlider.value = (float)current / max;
    }
    
    /// <summary>
    /// 表示するスコアを更新
    /// </summary>
    public void UpdateScoreDisplay(int score)
    {
        scoreText.text = $"Score:{score}";
    }
    
    
    /// <summary>
    /// 表示するクリスタルの個数を更新
    /// </summary>
    public void UpdateCurrencyDisplay(int amount)
    {
        currencyText.text = $"Crystals: {amount}";
    }
    
    /// <summary>
    /// GameOver処理 
    /// </summary>
    public void ShowGameOverUI()
    {
        Debug.Log("GameOver");
        
        gameOverGroup.gameObject.SetActive(true);
        
        Time.timeScale = 0f;

        var seq = DOTween.Sequence();
        seq.SetUpdate(true);
        seq.Append(gameOverGroup.DOFade(1f, fadeDuration));
        seq.Join(gameOverGroup.transform
            .DOScale(shownScale * 1.2f, popDuration)
            .SetEase(Ease.OutBack)
        );
        seq.Append(gameOverGroup.transform
            .DOScale(shownScale, popDuration * 0.5f)
            .SetEase(Ease.OutBack)
        );
        seq.OnComplete(() =>
        {
            gameOverGroup.interactable = true;
            gameOverGroup.blocksRaycasts = true;
        });
    }
}