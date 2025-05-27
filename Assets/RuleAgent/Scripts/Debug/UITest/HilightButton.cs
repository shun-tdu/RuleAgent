using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;


[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Shadow))]
public class HilightButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale")] [SerializeField] private float scaleUp = 1.1f;
    [SerializeField] private float duration = 0.2f;

    [Header("Shadow")] [SerializeField] private Vector2 shadowDistance = new Vector2(4f, 4f);

    private RectTransform _rect;
    private Shadow _shadow;
    private Vector3 _originalScale;
    private Vector2 _originalDistance;
    private Button _button;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _shadow = GetComponent<Shadow>();
        _button = GetComponent<Button>();

        _button.onClick.AddListener(OnClick);

        _originalScale = _rect.localScale;
        _originalDistance = _shadow.effectDistance;

        _shadow.effectDistance = Vector2.zero;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DOTween.Kill(_rect);
        DOTween.Kill(_shadow);

        AudioManager.I.PlayHover(0.02f, 1.0f);

        _rect.DOScale(_originalScale * scaleUp, duration)
            .SetEase(Ease.OutBack);

        DOTween.To(() => _shadow.effectDistance, x => _shadow.effectDistance = x, shadowDistance, duration)
            .SetEase(Ease.Linear)
            .SetTarget(_shadow);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DOTween.Kill(_rect);
        DOTween.Kill(_shadow);

        // 元のサイズに戻す
        _rect.DOScale(_originalScale, duration)
            .SetEase(Ease.OutBack);

        // Shaowを消す
        DOTween.To(
                () => _shadow.effectDistance,
                x => _shadow.effectDistance = x,
                _originalDistance, // 通常は zero
                duration
            )
            .SetEase(Ease.Linear)
            .SetTarget(_shadow);
    }

    private void OnClick()
    {
        AudioManager.I.PlayClick(0.02f, 1.5f);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OnClick);
    }

    private void OnDisable()
    {
        DOTween.Kill(_rect);
        DOTween.Kill(_shadow);
    }
}