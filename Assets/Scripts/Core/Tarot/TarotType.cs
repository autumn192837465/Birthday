/// <summary>
/// All tarot card types available in the fortune reading system.
/// </summary>
public enum TarotType
{
    None,

    // === Positive: Money ===
    AceOfPentacles,   // Instant +100 coins
    TheEmpress,       // Earnings doubled for 3 days
    TenOfPentacles,   // Painting value +10% for 3 days
    WheelOfFortune,   // Next displayed painting is instantly rented

    // === Positive: Gallery ===
    TheMagician,      // Creating art costs no fatigue (today only)

    // === Positive: Health ===
    TheStar,          // Instantly reset fatigue to 0

    // === Neutral ===
    TwoOfPentacles,   // No effect (balanced, nothing happens)
    TheHermit,        // Forced rest: cannot create art today

    // === Playful Debuffs ===
    TheFool,          // Next 3 paintings have -10% base value
    TheHangedMan,     // Instantly lose 20 coins
    TheMoon           // Rent chance halved tonight
}
