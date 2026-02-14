/// <summary>
/// Ten of Pentacles: Base salary increased by 10% for the next 3 days.
/// Category: Positive (Money). Duration-based buff.
/// </summary>
public class TenOfPentaclesCard : TarotCardBase
{
    private int remainingDays = 3;

    public override TarotType Type => TarotType.TenOfPentacles;
    public override string CardName => "Ten of Pentacles";
    public override string Description => $"Salary +10%! ({remainingDays} days left)";
    public override string Symbol => "+10%";

    public override bool IsExpired => remainingDays <= 0;
    public override float SalaryBonusPercent => 0.10f;

    public override void OnApply(GameManager gm)
    {
        gm.ShowMessage("Ten of Pentacles: Salary +10% for 3 days!");
    }

    public override void OnDayAdvanced()
    {
        remainingDays--;
    }
}
