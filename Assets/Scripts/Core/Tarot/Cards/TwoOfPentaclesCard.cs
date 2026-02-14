/// <summary>
/// Two of Pentacles: Balance. Nothing happens.
/// Category: Neutral. Instant (no effect).
/// </summary>
public class TwoOfPentaclesCard : TarotCardBase
{
    public override TarotType Type => TarotType.TwoOfPentacles;
    public override string CardName => "Two of Pentacles";
    public override string Description => "Balance... nothing happens this time.";
    public override string Symbol => "...";

    public override bool IsExpired => true;

    public override void OnApply(GameManager gm)
    {
        gm.ShowMessage("Two of Pentacles: The stars are balanced. Nothing happens.");
    }
}
