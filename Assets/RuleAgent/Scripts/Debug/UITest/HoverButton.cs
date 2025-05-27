using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Button))]
public class HoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("アニメーション設定")] [SerializeField] private float scaleUp = 1.1f;
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private Color hoverColor = new Color(0f, 0f, 0f, 0.2f);

    private RectTransform _rt;
    private Image _bgImage;
    private Color _origColor;
    private Vector3 _origScale;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _bgImage = GetComponent<Image>();
        if (_bgImage == null)
            _bgImage = GetComponentInChildren<Image>();

        _origScale = _rt.localScale;
        _origColor = _bgImage != null ? _bgImage.color : Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("HOver");
        AudioManager.I.PlayHover(0.02f, 1.0f);
        //背景色を少し暗くする
        if (_bgImage != null)
            _bgImage.DOColor(hoverColor, duration).SetUpdate(true);
        //少し大きくポップアップ
        _rt.DOScale(_origScale * scaleUp, duration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_bgImage != null)
            _bgImage.DOColor(_origColor, duration).SetUpdate(true);

        _rt.DOScale(_origScale, duration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }
}