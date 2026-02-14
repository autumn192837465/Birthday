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
    public virtual bool IsExpired => true; // Default: instant cards expire immediately

    // === Event Hooks (override as needed) ===
    public virtual void OnDayAdvanced() { }
    public virtual void OnWorkPerformed() { }
    public virtual void OnMiniGamePlayed() { }

    // === Passive Modifiers (all default to neutral) ===
    public virtual float EarningsMultiplier => 1f;
    public virtual float SalaryBonusPercent => 0f;
    public virtual bool BlocksWork => false;
    public virtual bool BlocksFatigue => false;
    public virtual bool GuaranteesMiniGameWin => false;
    public virtual bool MakesWorkButtonChaotic => false;
}
