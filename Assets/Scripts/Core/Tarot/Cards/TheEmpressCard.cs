/// <summary>
/// The Empress: All earnings doubled for the next 3 days.
/// Category: Positive (Money). Duration-based buff.
/// </summary>
public class TheEmpressCard : TarotCardBase
{
    private int remainingDays = 3;

    public override TarotType Type => TarotType.TheEmpress;
    public override string CardName => "The Empress";
    public override string Description => $"Earnings doubled! ({remainingDays} days left)";
    public override string Symbol => "x2";

    public override bool IsExpired => remainingDays <= 0;
    public override float EarningsMultiplier => 2f;

    public override void OnApply(GameManager gm)
    {
        gm.ShowMessage("The Empress: All earnings x2 for 3 days!");
    }

    public override void OnDayAdvanced()
    {
        remainingDays--;
    }
}
