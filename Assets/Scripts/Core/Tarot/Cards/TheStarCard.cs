/// <summary>
/// The Star: Instantly reset fatigue to 0.
/// Category: Positive (Health). Instant effect.
/// </summary>
public class TheStarCard : TarotCardBase
{
    public override TarotType Type => TarotType.TheStar;
    public override string CardName => "The Star";
    public override string Description => "Full recovery! Fatigue reset to 0.";
    public override string Symbol => "HEAL";

    // Instant: expires right after apply
    public override bool IsExpired => true;

    public override void OnApply(GameManager gm)
    {
        gm.ResetFatigue();
        gm.ShowMessage("The Star: Fatigue fully healed!");
    }
}
