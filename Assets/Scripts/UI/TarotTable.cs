using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 塔羅牌桌：負責在 cardRoot 下動態生成卡牌、設定牌面、訂閱選取，並對外發出 OnCardSelected。
/// </summary>
public class TarotTable : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TarotCard tarotCardPrefab;
    
    [SerializeField] private Transform[] cardRoots;

    private List<TarotCard> tarotCards = new List<TarotCard>();

    /// <summary>當玩家在桌面上選中一張牌時觸發。</summary>
    public event Action<TarotType> OnCardSelected;

    /// <summary>
    /// 依 tarotTypes 在 cardRoot 下生成卡牌、設定牌面並設為可點選，並訂閱選取事件。
    /// 若已有舊卡牌會先清除再建立。
    /// </summary>
    public async Awaitable InitializeTableAsync(List<TarotType> tarotTypes)
    {
        ClearTable();

        List<Awaitable> showTasks = new List<Awaitable>();
        for (int i = 0; i < tarotTypes.Count; i++)
        {
            TarotType type = tarotTypes[i];
            // Use i as the index for cardRoots
            if (i >= cardRoots.Length)
                break; // Prevent out-of-bounds

            TarotCard card = Instantiate(tarotCardPrefab, cardRoots[i]);
            card.SetCard(type);
            card.SetInteractable(true);
            tarotCards.Add(card);
            showTasks.Add(card.ShowCardAsync());

            await Awaitable.WaitForSecondsAsync(0.2f);
        }

        for (int i = 0; i < showTasks.Count; i++)
        {
            await showTasks[i];
        }
        SubscribeCards();
    }

    /// <summary>清除桌面上所有卡牌（銷毀實例）。</summary>
    public void ClearTable()
    {
        UnsubscribeCards();
        foreach (var card in tarotCards)
        {
            if (card != null && card.gameObject != null)
                Destroy(card.gameObject);
        }
        tarotCards.Clear();
    }

    /// <summary>設定桌上所有卡牌是否可點選。</summary>
    public void SetAllInteractable(bool interactable)
    {
        foreach (var card in tarotCards)
        {
            if (card != null)
                card.SetInteractable(interactable);
        }
    }

    private void SubscribeCards()
    {
        foreach (var card in tarotCards)
        {
            if (card != null)
                card.OnSelected += OnCardSelectedInternal;
        }
    }

    private void UnsubscribeCards()
    {
        foreach (var card in tarotCards)
        {
            if (card != null)
                card.OnSelected -= OnCardSelectedInternal;
        }
    }

    private void OnCardSelectedInternal(TarotType type)
    {
        UnsubscribeCards();
        SetAllInteractable(false);
        OnCardSelected?.Invoke(type);
    }

    private void OnDestroy()
    {
        UnsubscribeCards();
    }
}
