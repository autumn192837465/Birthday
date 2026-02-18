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
    public int Fatigue { get; private set; } = 0;
    public int DayCount { get; private set; } = 1;
    
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

        if (scale == 0) return;

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
            ShowMessage($"Bonus! +${finalAmount} (x{multiplier:F1})");
        }
    }

    /// <summary>
    /// Earn work salary with both EarningsMultiplier and SalaryBonusPercent applied.
    /// </summary>
    public int EarnWorkSalary(int baseSalary)
    {
        float salaryBonus = GetTotalSalaryBonusPercent();
        float earningsMultiplier = GetTotalEarningsMultiplier();

        int adjustedSalary = Mathf.RoundToInt(baseSalary * (1f + salaryBonus));
        int finalAmount = Mathf.RoundToInt(adjustedSalary * earningsMultiplier);
        finalAmount = Mathf.Max(1, finalAmount); // Always earn at least 1

        Money += finalAmount;
        NotifyStatsChanged();

        return finalAmount;
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
            ShowMessage("Not enough money!");
            return false;
        }

        Money -= amount;
        NotifyStatsChanged();
        return true;
    }

    // =============================================
    // Fatigue Operations
    // =============================================

    /// <summary>
    /// Add fatigue. Returns false if it would exceed max.
    /// Respects BlocksFatigue modifier from active effects.
    /// </summary>
    public bool AddFatigue(int amount)
    {
        if (HasBlocksFatigue())
        {
            ShowMessage("Magic power! No fatigue consumed.");
            return true;
        }

        if (Fatigue + amount > Settings.MaxFatigue)
        {
            ShowMessage("Too tired! You need to sleep first.");
            return false;
        }

        Fatigue += amount;
        NotifyStatsChanged();
        return true;
    }

    public async Awaitable Sleep()
    {
        EnableInput(false);
        await uiManager.FadeOutAsync(2);
        uiManager.ToMainView();
        ResetFatigue();
        AdvanceDay();
        await Awaitable.WaitForSecondsAsync(1);
        await uiManager.FadeInAsync(2);
        EnableInput(true);
    }
    
    public async Awaitable TryWork()
    {
        if (HasBlocksWork())
        {
            ShowMessage("The Hermit says: rest today. No work allowed!");
        }
        
        if (IsTooTired())
        {
            ShowMessage("The Hermit says: rest today. No work allowed!");
            return;
        }
        
        if (!AddFatigue(Settings.WorkFatigueCost))
        {
            return;
        }
        
        EnableInput(false);
        await uiManager.FadeOutAsync(1);
        EarnWorkSalary(Settings.WorkSalary);
        NotifyWorkPerformed();
        uiManager.ToMainView();
        await uiManager.FadeInAsync(1);
        EnableInput(true);
    }

    /// <summary>
    /// Reset fatigue to 0 (Sleep or Star card).
    /// </summary>
    public void ResetFatigue()
    {
        Fatigue = 0;
        NotifyStatsChanged();
    }

    /// <summary>
    /// Check if the player is too fatigued to act.
    /// </summary>
    public bool IsTooTired()
    {
        return Fatigue >= Settings.MaxFatigue;
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
        if (type == TarotType.None) return;

        ITarotEffect card = TarotCardFactory.Create(type);
        if (card == null) return;

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
    /// Notify all active effects that a work action was performed, then clean up.
    /// </summary>
    public void NotifyWorkPerformed()
    {
        foreach (var effect in activeEffects)
        {
            effect.OnWorkPerformed();
        }

        CleanupExpiredEffects();
    }

    /// <summary>
    /// Notify all active effects that a mini-game was played, then clean up.
    /// </summary>
    public void NotifyMiniGamePlayed()
    {
        foreach (var effect in activeEffects)
        {
            effect.OnMiniGamePlayed();
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

    /// <summary>
    /// Sum of all active SalaryBonusPercent values.
    /// </summary>
    public float GetTotalSalaryBonusPercent()
    {
        float bonus = 0f;
        foreach (var effect in activeEffects)
        {
            bonus += effect.SalaryBonusPercent;
        }
        return bonus;
    }

    /// <summary>True if any active effect blocks work.</summary>
    public bool HasBlocksWork()
    {
        return activeEffects.Any(e => e.BlocksWork);
    }

    /// <summary>True if any active effect blocks fatigue cost.</summary>
    public bool HasBlocksFatigue()
    {
        return activeEffects.Any(e => e.BlocksFatigue);
    }

    /// <summary>True if any active effect guarantees a mini-game win.</summary>
    public bool HasGuaranteedMiniGameWin()
    {
        return activeEffects.Any(e => e.GuaranteesMiniGameWin);
    }

    /// <summary>True if any active effect makes the work button chaotic.</summary>
    public bool HasChaoticWorkButton()
    {
        return activeEffects.Any(e => e.MakesWorkButtonChaotic);
    }

    /// <summary>
    /// Get a formatted string of all active buff names for UI display.
    /// </summary>
    public string GetActiveEffectsDisplay()
    {
        if (activeEffects.Count == 0) return "";

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

    private void NotifyStatsChanged()
    {
        OnStatsChanged?.Invoke();
    }
    
    public void EnableInput(bool enable)
    {
        raycastBlocker.SetActive(!enable);
    }
}
