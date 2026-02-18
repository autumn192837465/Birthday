/// <summary>
/// Abstract base class for all tarot card effects.
/// Provides sensible defaults so each card only overrides what it needs.
/// </summary>
public abstract class TarotCardBase : ITarotEffect
{
    // === Identity (must be overridden) ===
    public abstract TarotType Type { get; }
    public abstract string CardName { get; }
    public abstract string Description { get; }
    public abstract string Symbol { get; }

    // === Lifecycle (override as needed) ===
    public virtual void OnApply(GameManager gm) { }
    public virtual void OnRemove(GameManager gm) { }
    public virtual bool IsExpired => true;

    // === Event Hooks (override as needed) ===
    public virtual void OnDayAdvanced() { }
    public virtual void OnPaintingCreated() { }

    // === Passive Modifiers (all default to neutral) ===
    public virtual float EarningsMultiplier => 1f;
    public virtual bool BlocksCreation => false;
    public virtual bool BlocksFatigue => false;
    public virtual float RentChanceMultiplier => 1f;
    public virtual float PaintingValueMultiplier => 1f;
}
