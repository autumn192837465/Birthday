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

    /// <summary>Called after each painting creation. Use to decrement creation counters.</summary>
    void OnPaintingCreated();

    // === Passive Modifiers (queried by systems) ===

    /// <summary>Multiplier for all earnings. 1.0 = normal, 2.0 = double.</summary>
    float EarningsMultiplier { get; }

    /// <summary>If true, the player cannot create art today.</summary>
    bool BlocksCreation { get; }

    /// <summary>If true, creating art does not consume fatigue.</summary>
    bool BlocksFatigue { get; }

    /// <summary>Multiplier on rent chance during nightly market. 1.0 = normal, 0.5 = halved.</summary>
    float RentChanceMultiplier { get; }

    /// <summary>Multiplier on newly created painting base value. 1.0 = normal, 0.9 = -10%.</summary>
    float PaintingValueMultiplier { get; }
}
