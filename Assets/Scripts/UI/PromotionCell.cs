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
    [SerializeField] private Image paintingImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button promotionButton;
    [SerializeField] private TextMeshProUGUI promotionButtonText;

    public DrawingType DrawingType => drawingType;
    public event Action<DrawingType> OnPromotionClicked;

    private bool _isPromoted;

    private void Awake()
    {
        if (promotionButton != null)
            promotionButton.onClick.AddListener(HandlePromotionClick);
    }

    private void OnDestroy()
    {
        if (promotionButton != null)
            promotionButton.onClick.RemoveListener(HandlePromotionClick);
    }

    private void HandlePromotionClick()
    {
        OnPromotionClicked?.Invoke(drawingType);
    }

    /// <summary>
    /// 設定 Cell 的顯示內容。
    /// </summary>
    public void SetData(Sprite image, string title, string description, bool isPromoted)
    {
        if (paintingImage != null)
            paintingImage.sprite = image;

        if (titleText != null)
            titleText.text = title;

        if (descriptionText != null)
            descriptionText.text = description;

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
        if (promotionButtonText != null)
            promotionButtonText.text = _isPromoted ? "已推廣" : "推廣";

        if (promotionButton != null)
            promotionButton.interactable = !_isPromoted;
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
