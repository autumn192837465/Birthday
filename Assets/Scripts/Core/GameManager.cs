using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Singleton manager that tracks core player stats and manages active tarot card effects.
/// Systems query this manager for aggregated modifiers from all active effects.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public GameSettings Settings;

    [SerializeField] private DataManager dataManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameObject raycastBlocker;

    // === Player Stats ===
    public int Money { get; private set; } = 1000;

    /// <summary>Remaining stamina (depletes when creating art; 0 = exhausted).</summary>
    public int CurrentStamina { get; private set; }

    /// <summary>Maximum stamina for the current day (morning events may raise above base).</summary>
    public int EffectiveMaxStamina { get; private set; }

    /// <summary>Multiplier on painting stamina cost (e.g. stiff neck morning event).</summary>
    public float PaintingCostMultiplier { get; private set; } = 1f;

    public int DayCount { get; private set; } = 1;

    /// <summary>Game day index when the player last completed a tarot reading (same as <see cref="DayCount"/> when read).</summary>
    public int LastTarotFortuneDay { get; private set; } = 0;
    
    public DataManager DataManager => dataManager;

    // === Active Tarot Effects ===
    private readonly List<ITarotEffect> activeEffects = new List<ITarotEffect>();

    /// <summary>Read-only view of all active tarot effects.</summary>
    public IReadOnlyList<ITarotEffect> ActiveEffects => activeEffects;

    // === Events ===
    public event Action OnStatsChanged;
    public event Action<string> OnShowMessage;

    // =============================================
    // Lifecycle
    // =============================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize DataManager with data from Settings
        dataManager.Initialize();
    }

    private void Start()
    {
        int baseMax = Settings.BaseMaxStamina;
        EffectiveMaxStamina = baseMax;
        CurrentStamina = baseMax;
        PaintingCostMultiplier = 1f;
        NotifyStatsChanged();
    }

    private void Update()
    {
#if UNITY_EDITOR
        bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        int scale = 0;
        if (Input.GetKeyDown(KeyCode.Alpha1)) scale = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha2)) scale = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha3)) scale = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha4)) scale = 4;
        else if (Input.GetKeyDown(KeyCode.Alpha5)) scale = 5;

        if (scale == 0)
        {
            return;
        }

        if (shift)
        {
            Time.timeScale = 1f / scale;
        }
        else
        {
            Time.timeScale = scale;
        }
#endif
    }

    // =============================================
    // Money Operations
    // =============================================

    /// <summary>
    /// Earn money with all active tarot modifiers applied.
    /// Aggregates EarningsMultiplier from all active effects.
    /// </summary>
    public void EarnMoney(int baseAmount)
    {
        float multiplier = GetTotalEarningsMultiplier();
        int finalAmount = Mathf.RoundToInt(baseAmount * multiplier);

        Money += finalAmount;
        NotifyStatsChanged();

        if (multiplier > 1f)
        {
            ShowMessage(GameMessages.EarnMoneyBonus(finalAmount, multiplier));
        }
    }

    /// <summary>
    /// Add money directly without multipliers (used by instant card effects).
    /// </summary>
    public void AddMoney(int amount)
    {
        Money += amount;
        NotifyStatsChanged();
    }

    /// <summary>
    /// Deduct money directly (used by card penalty effects). Never goes below 0.
    /// </summary>
    public void DeductMoney(int amount)
    {
        Money = Mathf.Max(0, Money - amount);
        NotifyStatsChanged();
    }

    /// <summary>
    /// Spend money. Returns false if insufficient funds.
    /// </summary>
    public bool SpendMoney(int amount)
    {
        if (Money < amount)
        {
            ShowMessage(GameMessages.InsufficientMoney);
            return false;
        }

        Money -= amount;
        NotifyStatsChanged();
        return true;
    }

    // =============================================
    // Stamina Operations
    // =============================================

    /// <summary>
    /// Base max stamina from settings (before morning event modifiers).
    /// </summary>
    public int GetBaseMaxStamina()
    {
        return Settings.BaseMaxStamina;
    }

    /// <summary>
    /// Stamina cost for one painting step after morning multipliers (e.g. stiff neck).
    /// </summary>
    public int GetPaintingStaminaCost()
    {
        return Mathf.Max(1, Mathf.RoundToInt(Settings.PaintingStaminaCost * PaintingCostMultiplier));
    }

    /// <summary>
    /// Consume stamina for an action. Returns false if not enough stamina.
    /// Respects SkipsStaminaCost from active tarot effects.
    /// </summary>
    public bool TryConsumeStamina(int amount)
    {
        if (HasSkipsStaminaCost())
        {
            ShowMessage(GameMessages.MagicPowerNoStaminaCost);
            return true;
        }

        if (CurrentStamina < amount)
        {
            ShowMessage(GameMessages.ExhaustedNeedSleep);
            return false;
        }

        CurrentStamina -= amount;
        NotifyStatsChanged();
        return true;
    }

    /// <summary>
    /// Restore stamina to the current effective maximum (Star card, etc.).
    /// </summary>
    public void RestoreStaminaToFull()
    {
        CurrentStamina = EffectiveMaxStamina;
        NotifyStatsChanged();
    }

    /// <summary>
    /// Sets current and max stamina for the day (used by morning events).
    /// </summary>
    public void SetDailyStaminaState(int currentStamina, int effectiveMaxStamina)
    {
        effectiveMaxStamina = Mathf.Max(1, effectiveMaxStamina);
        CurrentStamina = Mathf.Clamp(currentStamina, 0, effectiveMaxStamina);
        EffectiveMaxStamina = effectiveMaxStamina;
        NotifyStatsChanged();
    }

    /// <summary>
    /// Sets painting cost multiplier for the day (stiff neck event).
    /// </summary>
    public void SetPaintingCostMultiplier(float multiplier)
    {
        PaintingCostMultiplier = Mathf.Max(0.01f, multiplier);
        NotifyStatsChanged();
    }

    /// <summary>
    /// Default morning when no <see cref="MorningEventManager"/> is present: full stamina, base cap, normal cost.
    /// </summary>
    public void ApplyDefaultMorningStamina()
    {
        PaintingCostMultiplier = 1f;
        int baseMax = Settings.BaseMaxStamina;
        SetDailyStaminaState(baseMax, baseMax);
    }

    /// <summary>
    /// Check if the player cannot act due to stamina.
    /// </summary>
    public bool IsExhausted()
    {
        return CurrentStamina <= 0;
    }

    public async Awaitable Sleep()
    {
        EnableInput(false);
        await uiManager.FadeOutAsync(1.5f);
        uiManager.ToMainView();

        // Process nightly gallery market before advancing the day
        MarketResult? marketResult = null;
        if (MarketManager.Instance != null)
        {
            marketResult = MarketManager.Instance.ProcessDailyMarket();
        }

        AdvanceDay();

        if (MorningEventManager.Instance != null)
        {
            MorningEventManager.Instance.TriggerDailyEvent();
        }
        else
        {
            ApplyDefaultMorningStamina();
        }

        await Awaitable.WaitForSecondsAsync(1);
        await uiManager.FadeInAsync(2);
        // Keep input blocked until morning toast (if any) and market UI finish.

        bool hasMorningEvent = MorningEventManager.Instance != null &&
            MorningEventManager.Instance.CurrentActiveMorningEvent != null;

        if (hasMorningEvent)
        {
            var ev = MorningEventManager.Instance.CurrentActiveMorningEvent;
            string morningText = $"{ev.EventName}\n{ev.Description}";
            await ShowMessageAndWaitAsync(morningText);
        }

        EnableInput(true);
        
        if (marketResult.HasValue && uiManager.HasDailySummaryPopup)
        {
            await uiManager.ShowDailySummaryPopupAsync(marketResult.Value, DayCount);
        }
        else if (marketResult.HasValue)
        {
            await ShowMessageAndWaitAsync(GameMessages.FormatMarketResult(marketResult.Value));
        }
        else if (!hasMorningEvent)
        {
            await ShowMessageAndWaitAsync(GameMessages.GoodMorningDay(DayCount));
        }
    }

    // =============================================
    // Day Management
    // =============================================

    /// <summary>
    /// Advance to the next day. Notifies all active effects and cleans up expired ones.
    /// </summary>
    public void AdvanceDay()
    {
        DayCount++;

        // Notify all effects about the new day
        foreach (var effect in activeEffects)
        {
            effect.OnDayAdvanced();
        }

        CleanupExpiredEffects();
        NotifyStatsChanged();
    }

    // =============================================
    // Tarot Effect Management
    // =============================================

    /// <summary>
    /// Apply a new tarot card effect. Creates the effect, calls OnApply, and adds to active list.
    /// </summary>
    public void ApplyTarotCard(TarotType type)
    {
        if (type == TarotType.None)
        {
            return;
        }

        ITarotEffect card = TarotCardFactory.Create(type);
        if (card == null)
        {
            return;
        }

        // Apply instant effects
        card.OnApply(this);

        // Only keep non-expired effects in the active list
        if (!card.IsExpired)
        {
            activeEffects.Add(card);
        }

        NotifyStatsChanged();
    }

    /// <summary>
    /// Notify all active effects that a painting was created, then clean up.
    /// </summary>
    public void NotifyPaintingCreated()
    {
        foreach (var effect in activeEffects)
        {
            effect.OnPaintingCreated();
        }

        CleanupExpiredEffects();
    }

    // =============================================
    // Aggregated Modifier Queries
    // =============================================

    /// <summary>
    /// Product of all active EarningsMultiplier values.
    /// </summary>
    public float GetTotalEarningsMultiplier()
    {
        float multiplier = 1f;
        foreach (var effect in activeEffects)
        {
            multiplier *= effect.EarningsMultiplier;
        }
        return multiplier;
    }

    /// <summary>True if any active effect skips stamina cost for creating art.</summary>
    public bool HasSkipsStaminaCost()
    {
        return activeEffects.Any(e => e.SkipsStaminaCost);
    }

    /// <summary>True if any active effect blocks painting creation.</summary>
    public bool HasBlocksCreation()
    {
        return activeEffects.Any(e => e.BlocksCreation);
    }

    /// <summary>Product of all active RentChanceMultiplier values.</summary>
    public float GetTotalRentChanceMultiplier()
    {
        float multiplier = 1f;
        foreach (var effect in activeEffects)
        {
            multiplier *= effect.RentChanceMultiplier;
        }
        return multiplier;
    }

    /// <summary>Product of all active PaintingValueMultiplier values.</summary>
    public float GetTotalPaintingValueMultiplier()
    {
        float multiplier = 1f;
        foreach (var effect in activeEffects)
        {
            multiplier *= effect.PaintingValueMultiplier;
        }
        return multiplier;
    }

    /// <summary>
    /// Get a formatted string of all active buff names for UI display.
    /// </summary>
    public string GetActiveEffectsDisplay()
    {
        if (activeEffects.Count == 0)
        {
            return "";
        }

        var names = activeEffects.Select(e => e.CardName);
        return string.Join(", ", names);
    }

    // =============================================
    // Utility
    // =============================================

    /// <summary>
    /// Remove all expired effects from the active list.
    /// </summary>
    private void CleanupExpiredEffects()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            if (activeEffects[i].IsExpired)
            {
                activeEffects[i].OnRemove(this);
                activeEffects.RemoveAt(i);
            }
        }
    }

    public void ShowMessage(string message)
    {
        OnShowMessage?.Invoke(message);
    }

    /// <summary>
    /// Same toast channel as <see cref="ShowMessage"/>, but waits until the notification animation completes.
    /// </summary>
    public async Awaitable ShowMessageAndWaitAsync(string message)
    {
        if (uiManager == null)
        {
            return;
        }

        await uiManager.ShowToastAndWaitAsync(message);
    }

    /// <summary>True if the player already completed a tarot reading on the current in-game day.</summary>
    public bool HasTarotFortuneToday()
    {
        return LastTarotFortuneDay == DayCount;
    }

    /// <summary>
    /// If already read tarot today: shows <see cref="ShowMessage"/> and returns true (caller should abort).
    /// </summary>
    public bool TryRejectTarotFortuneBecauseAlreadyDoneToday()
    {
        if (!HasTarotFortuneToday())
        {
            return false;
        }

        ShowMessage(GameMessages.TarotFortuneAlreadyDoneToday);
        return true;
    }

    /// <summary>Call when a tarot reading is fully completed (card chosen and applied).</summary>
    public void RecordTarotFortuneToday()
    {
        LastTarotFortuneDay = DayCount;
    }

    private void NotifyStatsChanged()
    {
        OnStatsChanged?.Invoke();
    }
    
    public void EnableInput(bool enable)
    {
        raycastBlocker.SetActive(!enable);
    }
}
