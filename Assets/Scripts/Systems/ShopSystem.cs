using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// Shop system: buy gifts to trigger the "Game Clear" ending.
/// 3 gift buttons with different prices.
/// </summary>
public class ShopSystem : PanelBase
{
    [Header("Gift Button References")]
    [SerializeField] private Button[] giftButtons;
    [SerializeField] private TextMeshProUGUI[] giftButtonTexts;

    [Header("Gift Preview")]
    [SerializeField] private Image[] giftPreviewImages;
    [SerializeField] private Sprite[] giftSprites;

    private void Start()
    {
        SetupGiftButtons();
    }

    /// <summary>
    /// Initialize gift button text with names and prices from GameSettings.
    /// </summary>
    private void SetupGiftButtons()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        for (int i = 0; i < giftButtons.Length; i++)
        {
            if (i >= gm.Settings.GiftNames.Length || i >= gm.Settings.GiftPrices.Length)
                break;

            int giftIndex = i;

            // Set button text
            if (giftButtonTexts != null && i < giftButtonTexts.Length)
            {
                giftButtonTexts[i].text = $"{gm.Settings.GiftNames[i]}\n${gm.Settings.GiftPrices[i]}";
            }

            // Set preview image
            if (giftPreviewImages != null && i < giftPreviewImages.Length
                && giftSprites != null && i < giftSprites.Length)
            {
                giftPreviewImages[i].sprite = giftSprites[i];
            }

            // Add click listener
            if (giftButtons != null && i < giftButtons.Length)
            {
                giftButtons[i].onClick.RemoveAllListeners();
                giftButtons[i].onClick.AddListener(() => OnGiftButtonClicked(giftIndex));
            }
        }
    }

    /// <summary>
    /// Called when a gift button is clicked.
    /// </summary>
    public void OnGiftButtonClicked(int giftIndex)
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (giftIndex >= gm.Settings.GiftPrices.Length) return;

        int price = gm.Settings.GiftPrices[giftIndex];
        string giftName = gm.Settings.GiftNames[giftIndex];

        // Check if player can afford it
        if (!gm.SpendMoney(price))
        {
            return;
        }

        // Play purchase animation
        PlayPurchaseAnimation(giftIndex, () =>
        {
            // Trigger Game Clear
            gm.ShowMessage($"You bought a {giftName}! Happy Birthday!");

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowGameClear(giftName);
            }
        });
    }

    /// <summary>
    /// Animate the gift button on purchase.
    /// </summary>
    private void PlayPurchaseAnimation(int giftIndex, TweenCallback onComplete)
    {
        if (giftButtons == null || giftIndex >= giftButtons.Length)
        {
            onComplete?.Invoke();
            return;
        }

        Transform btnTransform = giftButtons[giftIndex].transform;

        Sequence seq = DOTween.Sequence();
        seq.Append(btnTransform.DOScale(Vector3.one * 1.2f, 0.2f).SetEase(Ease.OutQuad));
        seq.Append(btnTransform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack));
        seq.AppendInterval(0.2f);
        seq.OnComplete(onComplete);
    }
}
