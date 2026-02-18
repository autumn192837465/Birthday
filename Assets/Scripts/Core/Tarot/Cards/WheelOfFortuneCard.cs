/// <summary>
/// Wheel of Fortune: The next displayed painting is instantly rented at premium rate.
/// Category: Positive (Money). Consumed when a painting is created.
/// </summary>
public class WheelOfFortuneCard : TarotCardBase
{
    private bool consumed = false;

    public override TarotType Type => TarotType.WheelOfFortune;
    public override string CardName => "Wheel of Fortune";
    public override string Description => "Next painting you display will be rented instantly tonight!";
    public override string Symbol => "WIN";

    public override bool IsExpired => consumed;

    /// <summary>
    /// Doubles the rent chance, effectively guaranteeing rental for the next market cycle.
    /// </summary>
    public override float RentChanceMultiplier => consumed ? 1f : 2f;

    public override void OnApply(GameManager gm)
    {
        gm.ShowMessage("Wheel of Fortune: Your gallery is buzzing with interest!");
    }

    public override void OnDayAdvanced()
    {
        consumed = true;
    }
}
