/// <summary>
/// Factory that creates the correct ITarotEffect instance from a TarotType enum.
/// </summary>
public static class TarotCardFactory
{
    /// <summary>
    /// Create a new tarot card effect instance for the given type.
    /// Returns null for TarotType.None.
    /// </summary>
    public static ITarotEffect Create(TarotType type)
    {
        switch (type)
        {
            // Positive: Money
            case TarotType.AceOfPentacles:  return new AceOfPentaclesCard();
            case TarotType.TheEmpress:      return new TheEmpressCard();
            case TarotType.TenOfPentacles:  return new TenOfPentaclesCard();
            case TarotType.WheelOfFortune:  return new WheelOfFortuneCard();

            // Positive: Work
            case TarotType.TheMagician:     return new TheMagicianCard();

            // Positive: Health
            case TarotType.TheStar:         return new TheStarCard();

            // Neutral
            case TarotType.TwoOfPentacles:  return new TwoOfPentaclesCard();
            case TarotType.TheHermit:       return new TheHermitCard();

            // Playful Debuffs
            case TarotType.TheFool:         return new TheFoolCard();
            case TarotType.TheHangedMan:    return new TheHangedManCard();
            case TarotType.TheMoon:         return new TheMoonCard();

            default: return null;
        }
    }
}
