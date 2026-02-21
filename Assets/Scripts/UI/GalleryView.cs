using System;
using UnityEngine;

/// <summary>
/// Gallery main view: create art button, promotion button, and optional future content.
/// Raises OnCreateArtClicked and OnPromotionClicked for the parent system to handle logic.
/// </summary>
public class GalleryView : MonoBehaviour
{
    [Header("Actions")]
    [SerializeField] private CostButton createArtButton;
    [SerializeField] private CostButton promotionButton;

    /// <summary>Fired when the player clicks Create Art.</summary>
    public event Action CreateArtClicked;

    /// <summary>Fired when the player clicks Promotion.</summary>
    public event Action PromotionClicked;

    private void Awake()
    {
        createArtButton.OnClick += OnCreateArtClicked;
        promotionButton.OnClick += OnPromotionClicked;
    }

    private void OnDestroy()
    {
        createArtButton.OnClick -= OnCreateArtClicked;
        promotionButton.OnClick -= OnPromotionClicked;
    }

    private void OnCreateArtClicked()
    {
        CreateArtClicked?.Invoke();
    }

    private void OnPromotionClicked()
    {
        PromotionClicked?.Invoke();
    }

    /// <summary>Set the create art button label (e.g. "Create Art (Fatigue +5)").</summary>
    public void SetCreateArtCost(string text)
    {
        if (createArtButton != null)
            createArtButton.SetCostText(text);
    }

    /// <summary>Set the promotion button label (e.g. "Promote ($100)").</summary>
    public void SetPromotionCost(string text)
    {
        if (promotionButton != null)
            promotionButton.SetCostText(text);
    }
}
