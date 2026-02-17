using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using MoreMountains.Feedbacks;

/// <summary>
/// Single tarot card in the selection table. Player clicks one of 3 cards to choose their fortune.
/// </summary>
public class TarotCard : MonoBehaviour
{
    public enum Facing
    {
        Back,
        Front
    }
    
    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private Image cardImage;
    [SerializeField] private MMF_Player flipFeedback;
    [SerializeField] private MMF_Player showFeedback;

    [SerializeField] private Transform cardRoot;
    [SerializeField] private MMF_Player onPointerDownFeedback;
    [SerializeField] private MMF_Player onPointerUpFeedback;

    /// <summary>Fired when this card is selected. Passes the assigned TarotType.</summary>
    public event Action<TarotCard> OnSelected;

    public TarotType AssignedType { get; private set; }
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
        AssignedType = type;
        _interactable = true;

        var sprite = GameManager.Instance.DataManager.GetTarotSprite(type);
        cardImage.sprite = sprite;
    }

    public void SetFacing(Facing facing)
    {
        switch (facing)
        {
            case Facing.Back:
                cardRoot.transform.localScale = new Vector3(-1, 1, 0);
                break;
            case Facing.Front:
                cardRoot.transform.localScale = new Vector3(1, 1, 0);
                break;
        }
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
  
       
    }

    public void SetInteractable(bool value)
    {
        _interactable = value;
        if (button != null)
        {
            button.interactable = value;
        }
        
        onPointerUpFeedback.SkipToTheEnd();
    }

    private void OnCardClicked()
    {
        if (!_interactable)
        {
            return;
        }
        
        _interactable = false;
        OnSelected?.Invoke(this);
    }

    public async Awaitable FlipCardAsync()
    {
        await flipFeedback.PlayFeedbacksTask();
    } 
    
    public void OnPointerDown()
    {
        if (!_interactable)
        {
            return;
        }

        if (onPointerUpFeedback.IsPlaying)
        {
            onPointerUpFeedback.StopFeedbacks();
        }
        
        onPointerDownFeedback.PlayFeedbacks();
    }
    
    public void OnPointerUp()
    {
        if (!_interactable)
        {
            return;
        }

        if (onPointerDownFeedback.IsPlaying)
        {
            onPointerDownFeedback.StopFeedbacks();
        }
        
        onPointerUpFeedback.PlayFeedbacks();
    }
}
