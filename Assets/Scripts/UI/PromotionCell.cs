using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 單一畫作的推廣 Cell：顯示圖片、標題、描述，以及推廣按鈕。
/// 在 Inspector 中為每個 DrawingType 各放一個 Cell。
/// </summary>
public class PromotionCell : MonoBehaviour
{
    [Header("Identification")]
    [SerializeField] private DrawingType drawingType;

    [Header("UI References")]
    [SerializeField] private CostButton promotionButton;

    public DrawingType DrawingType => drawingType;
    public event Action<DrawingType> OnPromotionClicked;

    private bool _isPromoted;

    private void Awake()
    {
        promotionButton.OnClick += HandlePromotionClick;
    }

    private void OnDestroy()
    {        
        promotionButton.OnClick -= HandlePromotionClick;
    }

    private void HandlePromotionClick()
    {
        OnPromotionClicked?.Invoke(drawingType);
    }

    /// <summary>
    /// 設定 Cell 的顯示內容。
    /// </summary>
    public void SetData(bool isPromoted)
    {
        _isPromoted = isPromoted;
        UpdatePromotionButtonState();
    }

    /// <summary>
    /// 更新推廣狀態（已推廣/未推廣）。
    /// </summary>
    public void SetPromoted(bool isPromoted)
    {
        _isPromoted = isPromoted;
        UpdatePromotionButtonState();
    }

    private void UpdatePromotionButtonState()
    {
        promotionButton.SetInteractable(!_isPromoted);
        promotionButton.ShowCost(!_isPromoted);
        promotionButton.SetText(_isPromoted ? "プロモーション済み" : "プロモーション");
            
    }

    /// <summary>
    /// 顯示此 Cell。
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 隱藏此 Cell。
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
