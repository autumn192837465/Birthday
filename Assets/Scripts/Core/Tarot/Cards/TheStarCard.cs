/// <summary>
/// The Star: Instantly restore stamina to full.
/// Category: Positive (Health). Instant effect.
/// </summary>
public class TheStarCard : TarotCardBase
{
    public override TarotType Type => TarotType.TheStar;
    public override string CardName => "The Star";
    public override string Description => "Full recovery! Stamina restored to full.";
    public override string Symbol => "HEAL";

    // Instant: expires right after apply
    public override bool IsExpired => true;

    public override void OnApply(GameManager gm)
    {
        gm.RestoreStaminaToFull();
    }
}
