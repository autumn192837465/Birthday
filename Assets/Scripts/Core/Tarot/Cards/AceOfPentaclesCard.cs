/// <summary>
/// Ace of Pentacles: Instantly gain 100 coins.
/// Category: Positive (Money). Instant effect.
/// </summary>
public class AceOfPentaclesCard : TarotCardBase
{
    public override TarotType Type => TarotType.AceOfPentacles;
    public override string CardName => "Ace of Pentacles";
    public override string Description => "A gift of gold! +100 coins instantly.";
    public override string Symbol => "$";

    // Instant effect: expires right after apply
    public override bool IsExpired => true;

    public override void OnApply(GameManager gm)
    {
        gm.AddMoney(100);
        gm.ShowMessage("Ace of Pentacles: +$100!");
    }
}
