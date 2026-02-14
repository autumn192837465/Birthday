/// <summary>
/// Interface for all tarot card effects.
/// Each card implements this to define its behavior and modifiers.
/// </summary>
public interface ITarotEffect
{
    // === Identity ===

    /// <summary>Enum type of this card.</summary>
    TarotType Type { get; }

    /// <summary>Display name shown in UI (e.g. "The Empress").</summary>
    string CardName { get; }

    /// <summary>Short description of the effect.</summary>
    string Description { get; }

    /// <summary>Symbol shown on the card face (e.g. "x2", "$$$").</summary>
    string Symbol { get; }

    // === Lifecycle ===

    /// <summary>Called when the card is drawn. Apply instant effects here.</summary>
    void OnApply(GameManager gm);

    /// <summary>Called when the effect is removed from the active list.</summary>
    void OnRemove(GameManager gm);

    /// <summary>True if this effect should be removed from the active list.</summary>
    bool IsExpired { get; }

    // === Event Hooks ===

    /// <summary>Called when a new day begins. Use to decrement day counters.</summary>
    void OnDayAdvanced();

    /// <summary>Called after each work action. Use to decrement work counters.</summary>
    void OnWorkPerformed();

    /// <summary>Called after a mini-game is played. Use for one-shot triggers.</summary>
    void OnMiniGamePlayed();

    // === Passive Modifiers (queried by systems) ===

    /// <summary>Multiplier for all earnings. 1.0 = normal, 2.0 = double.</summary>
    float EarningsMultiplier { get; }

    /// <summary>Bonus percentage on base salary. 0.0 = none, 0.1 = +10%, -0.1 = -10%.</summary>
    float SalaryBonusPercent { get; }

    /// <summary>If true, the player cannot work today.</summary>
    bool BlocksWork { get; }

    /// <summary>If true, work does not consume fatigue.</summary>
    bool BlocksFatigue { get; }

    /// <summary>If true, the next mini-game is a guaranteed big win.</summary>
    bool GuaranteesMiniGameWin { get; }

    /// <summary>If true, the work button moves around chaotically.</summary>
    bool MakesWorkButtonChaotic { get; }
}
