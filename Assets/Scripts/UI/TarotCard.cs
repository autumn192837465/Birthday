using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using MoreMountains.Feedbacks;
using Unity.Android.Gradle.Manifest;

/// <summary>
/// Single tarot card in the selection table. Player clicks one of 3 cards to choose their fortune.
/// </summary>
public class TarotCard : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private Image cardImage;
    [SerializeField] private TextMeshProUGUI symbolText;
    [SerializeField] private MMF_Player flipFeedback;
    [SerializeField] private MMF_Player showFeedback;

    /// <summary>Fired when this card is selected. Passes the assigned TarotType.</summary>
    public event Action<TarotType> OnSelected;

    private TarotType _assignedType;
    private bool _interactable = true;

    private void Awake()
    {
        if (button != null)
            button.onClick.AddListener(OnCardClicked);
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    /// <summary>
    /// Assign the tarot type for this card (called by TarotSystem when showing the table).
    /// Card is shown as face-down (back) until selected.
    /// </summary>
    public void SetCard(TarotType type)
    {
        _assignedType = type;
        _interactable = true;

        var sprite = GameManager.Instance.DataManager.GetTarotSprite(type);
        cardImage.sprite = sprite;
    }

    public async Awaitable ShowCardAsync()
    {
        await showFeedback.PlayFeedbacksTask();
    }

    /// <summary>
    /// Reveal this card's face (symbol) with a flip animation. Call after selection if desired.
    /// </summary>
    public void RevealFace(ITarotEffect effect)
    {
        if (effect == null) return;
        if (symbolText != null)
        {
            symbolText.text = effect.Symbol;
        }
       
    }

    public void SetInteractable(bool value)
    {
        _interactable = value;
        if (button != null)
        {
            button.interactable = value;
        }
    }

    private void OnCardClicked()
    {
        if (!_interactable)
        {
            return;
        }
        
        _interactable = false;

        
        OnSelected?.Invoke(_assignedType);
    }

    public async Awaitable FlipCard()
    {
        await flipFeedback.PlayFeedbacksTask();
    } 
}
