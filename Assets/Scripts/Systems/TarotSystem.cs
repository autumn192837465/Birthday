using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// Tarot reading system: pay money to flip 3 cards and reveal a tarot card effect.
/// Player-friendly: ~67% positive, ~15% neutral, ~18% mild debuff.
/// </summary>
public class TarotSystem : MonoBehaviour
{
    [Header("Card References (3 cards)")]
    [SerializeField] private RectTransform[] cardTransforms;
    [SerializeField] private Image[] cardFrontImages;
    [SerializeField] private TextMeshProUGUI[] cardTexts;

    [Header("UI")]
    [SerializeField] private Button drawButton;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI resultNameText;
    [SerializeField] private TextMeshProUGUI resultDescriptionText;

    [Header("Card Sprites")]
    [SerializeField] private Sprite cardBackSprite;
    [SerializeField] private Sprite cardFrontSprite;

    private bool isDrawing = false;

    /// <summary>
    /// Weighted draw table. Weights are relative (do not need to sum to 1).
    /// </summary>
    private static readonly TarotWeightEntry[] drawTable = new TarotWeightEntry[]
    {
        // Positive: Money (~37%)
        new TarotWeightEntry(TarotType.AceOfPentacles, 15),
        new TarotWeightEntry(TarotType.TheEmpress,     12),
        new TarotWeightEntry(TarotType.TenOfPentacles,  12),
        new TarotWeightEntry(TarotType.WheelOfFortune,   8),

        // Positive: Work/Health (~20%)
        new TarotWeightEntry(TarotType.TheMagician,     10),
        new TarotWeightEntry(TarotType.TheStar,         10),

        // Neutral (~15%)
        new TarotWeightEntry(TarotType.TwoOfPentacles,   8),
        new TarotWeightEntry(TarotType.TheHermit,         7),

        // Playful Debuffs (~18%)
        new TarotWeightEntry(TarotType.TheFool,           7),
        new TarotWeightEntry(TarotType.TheHangedMan,      6),
        new TarotWeightEntry(TarotType.TheMoon,           5),
    };

    /// <summary>
    /// Total weight sum (cached).
    /// </summary>
    private static readonly float totalWeight;

    static TarotSystem()
    {
        totalWeight = 0;
        foreach (var entry in drawTable)
        {
            totalWeight += entry.Weight;
        }
    }

    private void Start()
    {
        if (costText != null && GameManager.Instance != null)
        {
            costText.text = $"Draw Tarot (${GameManager.Instance.Settings.FortuneCost})";
        }
    }

    /// <summary>
    /// Called by the "Draw Tarot" button.
    /// </summary>
    public void OnDrawButtonClicked()
    {
        var gm = GameManager.Instance;
        if (gm == null || isDrawing) return;

        if (!gm.SpendMoney(gm.Settings.FortuneCost))
        {
            return;
        }

        isDrawing = true;

        // Determine result via weighted random
        TarotType drawnType = GetWeightedDraw();
        ITarotEffect card = TarotCardFactory.Create(drawnType);

        // Reset cards to face-down
        ResetCards();

        // Animate then apply
        PlayCardFlipAnimation(card);
    }

    /// <summary>
    /// Weighted random draw from the tarot table.
    /// </summary>
    private TarotType GetWeightedDraw()
    {
        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var entry in drawTable)
        {
            cumulative += entry.Weight;
            if (roll <= cumulative)
            {
                return entry.Type;
            }
        }

        // Fallback (should not reach)
        return TarotType.AceOfPentacles;
    }

    /// <summary>
    /// Reset all cards to their back (face-down) state.
    /// </summary>
    private void ResetCards()
    {
        for (int i = 0; i < cardTransforms.Length; i++)
        {
            cardTransforms[i].localScale = new Vector3(1f, 1f, 1f);
            cardTransforms[i].localRotation = Quaternion.identity;

            if (cardFrontImages != null && i < cardFrontImages.Length && cardBackSprite != null)
            {
                cardFrontImages[i].sprite = cardBackSprite;
            }

            if (cardTexts != null && i < cardTexts.Length)
            {
                cardTexts[i].text = "?";
            }
        }

        // Clear result display
        if (resultNameText != null) resultNameText.text = "";
        if (resultDescriptionText != null) resultDescriptionText.text = "";
    }

    /// <summary>
    /// Animate 3 cards flipping sequentially, then reveal and apply the tarot effect.
    /// </summary>
    private void PlayCardFlipAnimation(ITarotEffect card)
    {
        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < cardTransforms.Length; i++)
        {
            int index = i;
            RectTransform cardTransform = cardTransforms[i];

            // Flip: scale X to 0, change visual, scale X back to 1
            seq.Append(cardTransform.DOScaleX(0f, 0.25f).SetEase(Ease.InQuad));
            seq.AppendCallback(() =>
            {
                if (cardFrontImages != null && index < cardFrontImages.Length && cardFrontSprite != null)
                {
                    cardFrontImages[index].sprite = cardFrontSprite;
                }

                if (cardTexts != null && index < cardTexts.Length)
                {
                    cardTexts[index].text = card.Symbol;
                }
            });
            seq.Append(cardTransform.DOScaleX(1f, 0.25f).SetEase(Ease.OutQuad));
            seq.AppendInterval(0.15f);
        }

        // After all cards flip, show result and apply
        seq.AppendInterval(0.5f);
        seq.OnComplete(() =>
        {
            // Display card name and description
            if (resultNameText != null)
                resultNameText.text = card.CardName;

            if (resultDescriptionText != null)
                resultDescriptionText.text = card.Description;

            // Apply the card effect through GameManager
            var gm = GameManager.Instance;
            if (gm != null)
            {
                gm.ApplyTarotCard(card.Type);
            }

            isDrawing = false;
        });
    }

    /// <summary>
    /// Simple data holder for the weighted draw table.
    /// </summary>
    private struct TarotWeightEntry
    {
        public TarotType Type;
        public float Weight;

        public TarotWeightEntry(TarotType type, float weight)
        {
            Type = type;
            Weight = weight;
        }
    }
}
