using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Tarot reading: Pay to play → FadeOut/FadeIn → TarotTable with 3 cards → player picks one → TarotResultView → Confirm returns to MainView.
/// </summary>
public class TarotSystem : PanelBase
{
    [Header("Entry (before draw)")]
    [SerializeField] private GameObject entryView;
    [SerializeField] private Button playButton;
    [SerializeField] private TextMeshProUGUI costText;

    [Header("Tarot Table (3 cards)")]
    [SerializeField] private GameObject tarotTableView;
    [SerializeField] private TarotTable tarotTable;

    [Header("Result View (after pick)")]
    [SerializeField] private GameObject tarotResultView;
    [SerializeField] private TextMeshProUGUI resultNameText;
    [SerializeField] private TextMeshProUGUI resultDescriptionText;
    [SerializeField] private Button resultConfirmButton;

    private bool _isDrawing;
    private readonly List<TarotType> _drawPool = new List<TarotType>(3);

    private static readonly TarotWeightEntry[] DrawTable = new TarotWeightEntry[]
    {
        new TarotWeightEntry(TarotType.AceOfPentacles, 15),
        new TarotWeightEntry(TarotType.TheEmpress, 12),
        new TarotWeightEntry(TarotType.TenOfPentacles, 12),
        new TarotWeightEntry(TarotType.WheelOfFortune, 8),
        new TarotWeightEntry(TarotType.TheMagician, 10),
        new TarotWeightEntry(TarotType.TheStar, 10),
        new TarotWeightEntry(TarotType.TwoOfPentacles, 8),
        new TarotWeightEntry(TarotType.TheHermit, 7),
        new TarotWeightEntry(TarotType.TheFool, 7),
        new TarotWeightEntry(TarotType.TheHangedMan, 6),
        new TarotWeightEntry(TarotType.TheMoon, 5),
    };

    private static float _totalWeight;

    static TarotSystem()
    {
        _totalWeight = 0f;
        foreach (var e in DrawTable)
        {
            _totalWeight += e.Weight;
        }
    }

    private void Start()
    {
        if (costText != null && GameManager.Instance != null)
        {
            costText.text = GameManager.Instance.Settings.FortuneCost.ToString();
        }

        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayClicked);
        }

        if (resultConfirmButton != null)
        {
            resultConfirmButton.onClick.AddListener(OnResultConfirmClicked);
        }

        ShowEntryOnly();
    }

    private void OnDestroy()
    {
        if (tarotTable != null)
            tarotTable.OnCardSelected -= OnCardSelected;
        if (playButton != null) playButton.onClick.RemoveAllListeners();
        if (resultConfirmButton != null) resultConfirmButton.onClick.RemoveAllListeners();
    }

    private void ShowEntryOnly()
    {
        if (entryView != null) entryView.SetActive(true);
        if (tarotTableView != null) tarotTableView.SetActive(false);
        if (tarotResultView != null) tarotResultView.SetActive(false);
    }

    private void ShowTarotTableOnly()
    {
        if (entryView != null) entryView.SetActive(false);
        if (tarotTableView != null) tarotTableView.SetActive(true);
        if (tarotResultView != null) tarotResultView.SetActive(false);
    }

    private void ShowResultOnly()
    {
        if (entryView != null) entryView.SetActive(false);
        if (tarotTableView != null) tarotTableView.SetActive(false);
        if (tarotResultView != null) tarotResultView.SetActive(true);
    }

    /// <summary>
    /// Play button: deduct cost, FadeOut → FadeIn → open TarotTable with 3 cards.
    /// </summary>
    private async void OnPlayClicked()
    {
        var gm = GameManager.Instance;
        var ui = UIManager.Instance;
        if (gm == null || ui == null || _isDrawing)
        {
            return;
        }

        if (!gm.SpendMoney(gm.Settings.FortuneCost))
        {
            return;
        }

        _isDrawing = true;

        await ui.FadeOut();
        ShowTarotTableOnly();
        await ui.FadeIn();

        // Assign 3 weighted-random types to the 3 cards (no duplicate in one draw)
        _drawPool.Clear();
        for (int i = 0; i < 3; i++)
        {
            TarotType t = GetWeightedDrawNoDuplicate(_drawPool);
            _drawPool.Add(t);
        }

        if (tarotTable != null)
        {
            tarotTable.InitializeTable(_drawPool);
            tarotTable.OnCardSelected += OnCardSelected;
        }

    }

    private void OnCardSelected(TarotType type)
    {
        if (tarotTable != null)
            tarotTable.OnCardSelected -= OnCardSelected;

        ITarotEffect effect = TarotCardFactory.Create(type);
        var gm = GameManager.Instance;
        if (gm != null)
            gm.ApplyTarotCard(type);

        if (resultNameText != null)
            resultNameText.text = effect?.CardName ?? type.ToString();
        if (resultDescriptionText != null)
            resultDescriptionText.text = effect?.Description ?? "";

        ShowResultOnly();
    }

    private void OnResultConfirmClicked()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.ToMainView();
        _isDrawing = false;
    }

    /// <summary>Weighted random from draw table, excluding already chosen types for this round.</summary>
    private TarotType GetWeightedDrawNoDuplicate(List<TarotType> exclude)
    {
        float total = 0f;
        foreach (var entry in DrawTable)
        {
            if (!exclude.Contains(entry.Type))
            {
                total += entry.Weight;
            }
        }

        if (total <= 0f)
        {
            return DrawTable[0].Type;
        }

        float roll = UnityEngine.Random.Range(0f, total);
        float cumulative = 0f;
        foreach (var entry in DrawTable)
        {
            if (exclude.Contains(entry.Type))
            {
                continue;
            }
            cumulative += entry.Weight;
            if (roll <= cumulative)
            {
                return entry.Type;
            }
        }

        return DrawTable[0].Type;
    }

    private struct TarotWeightEntry
    {
        public TarotType Type;
        public float Weight;
        public TarotWeightEntry(TarotType type, float weight) { Type = type; Weight = weight; }
    }
}
