using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tarot table: uses pre-placed TarotCard references. Assigns card types, plays show animation, subscribes to selection, and raises OnCardSelected.
/// </summary>
public class TarotTable : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TarotCard[] tarotCards;

    /// <summary>Fired when the player selects a card on the table.</summary>
    public event Action<TarotCard> OnCardSelected;

    private void Awake()
    {
        HideAllCards();
    }

    private void HideAllCards()
    {
        foreach(var card in tarotCards)
        {
            if (card != null)
            {
                card.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Assigns each pre-placed card with a type from tarotTypes, sets face-down and interactable, plays show animation, then subscribes to selection.
    /// </summary>
    public async Awaitable InitializeTableAsync(List<TarotType> tarotTypes)
    {
        if (tarotCards == null || tarotTypes == null)
        {
            return;
        }

        HideAllCards();
        UnsubscribeCards();

        int count = Mathf.Min(tarotCards.Length, tarotTypes.Count);
        for (int i = 0; i < count; i++)
        {
            TarotCard card = tarotCards[i];
            if (card == null) continue;

            card.SetCard(tarotTypes[i]);
            card.SetFacing(TarotCard.Facing.Back);
            card.SetInteractable(true);
        }

        List<Awaitable> showTasks = new List<Awaitable>();
        for (int i = 0; i < count; i++)
        {
            TarotCard card = tarotCards[i];
            if (card == null)
            {
                continue;
            }

            card.gameObject.SetActive(true);
            showTasks.Add(card.ShowCardAsync());
            await Awaitable.WaitForSecondsAsync(0.2f);
        }

        foreach (var task in showTasks)
        {
            await task;
        }

        SubscribeCards();
    }

    /// <summary>Sets whether all cards on the table are clickable.</summary>
    public void SetAllInteractable(bool interactable)
    {
        if (tarotCards == null) return;
        foreach (var card in tarotCards)
        {
            if (card != null)
                card.SetInteractable(interactable);
        }
    }

    private void SubscribeCards()
    {
        if (tarotCards == null) return;
        foreach (var card in tarotCards)
        {
            if (card != null)
            {
                card.OnSelected += OnCardSelectedInternal;
            }
        }
    }

    private void UnsubscribeCards()
    {
        if (tarotCards == null) return;
        foreach (var card in tarotCards)
        {
            if (card != null)
            {
                card.OnSelected -= OnCardSelectedInternal;
            }
        }
    }

    private void OnCardSelectedInternal(TarotCard card)
    {
        UnsubscribeCards();
        SetAllInteractable(false);
        OnCardSelected?.Invoke(card);
    }

    private void OnDestroy()
    {
        UnsubscribeCards();
    }
}
