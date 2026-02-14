/// <summary>
/// All tarot card types available in the fortune reading system.
/// </summary>
public enum TarotType
{
    None,

    // === Positive: Money ===
    AceOfPentacles,   // Instant +100 coins
    TheEmpress,       // Earnings doubled for 3 days
    TenOfPentacles,   // Base salary +10% for 3 days
    WheelOfFortune,   // Next mini-game is a guaranteed big win

    // === Positive: Work ===
    TheMagician,      // Work costs no fatigue (today only)

    // === Positive: Health ===
    TheStar,          // Instantly reset fatigue to 0

    // === Neutral ===
    TwoOfPentacles,   // No effect (balanced, nothing happens)
    TheHermit,        // Forced rest: cannot work today

    // === Playful Debuffs ===
    TheFool,          // Next 3 work sessions earn 10% less
    TheHangedMan,     // Instantly lose 20 coins
    TheMoon           // Work button moves around chaotically (today only)
}
