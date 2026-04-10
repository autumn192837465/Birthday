using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 商店購買確認彈窗：顯示道具圖片、名稱、描述，玩家可選擇購買或關閉。
/// </summary>
public class ShopConfirmPopupView : AnimatorBase
{
    private const float MaxIconWidth = 400f;
    private const float MaxIconHeight = 600f;

    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private CostButton buyButton;
    [SerializeField] private Button closeButton;

    public event Action BuyClicked;
    public event Action CloseClicked;

    protected override void Awake()
    {
        base.Awake();
        buyButton.OnClick += OnBuyButtonClicked;
        closeButton.onClick.AddListener(OnCloseButtonClicked);
    }

    private void OnDestroy()
    {
        buyButton.OnClick -= OnBuyButtonClicked;
        closeButton.onClick.RemoveListener(OnCloseButtonClicked);
    }

    public void SetData(ShopItemSO item)
    {
        if (item == null)
        {
            return;
        }

        if (itemImage != null)
        {
            itemImage.sprite = item.Sprite;
            ApplyIconSizeWithinBounds(item.Sprite);
        }

        if (titleText != null)
        {
            titleText.text = item.ItemName;
        }

        if (descriptionText != null)
        {
            descriptionText.text = item.Description;
        }

        if (buyButton != null)
        {
            buyButton.SetCostText($"{item.Price}");
        }
    }

    public void SetData(ShopData.ShopEntry entry)
    {
        if (entry == null)
        {
            return;
        }

        if (itemImage != null)
        {
            itemImage.sprite = entry.Sprite;
            ApplyIconSizeWithinBounds(entry.Sprite);
        }

        if (titleText != null)
        {
            titleText.text = entry.Name;
        }

        if (descriptionText != null)
        {
            descriptionText.text = entry.Description;
        }

        if (buyButton != null)
        {
            buyButton.SetCostText($"{entry.Price}");
        }
    }

    /// <summary>
    /// 依原始比例縮放圖示，寬度不超過 400、高度不超過 600；較小圖片不放大。
    /// </summary>
    private void ApplyIconSizeWithinBounds(Sprite sprite)
    {
        if (itemImage == null || sprite == null)
        {
            return;
        }

        float w = sprite.rect.width;
        float h = sprite.rect.height;
        if (w <= 0f || h <= 0f)
        {
            return;
        }

        float scale = Mathf.Min(MaxIconWidth / w, MaxIconHeight / h, 1f);
        itemImage.rectTransform.sizeDelta = new Vector2(w * scale, h * scale);
    }

    private void OnBuyButtonClicked()
    {
        BuyClicked?.Invoke();
    }

    private void OnCloseButtonClicked()
    {
        CloseClicked?.Invoke();
    }
}
