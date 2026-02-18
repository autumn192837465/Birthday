/// <summary>
/// The Moon: Rent chance is halved for tonight's market processing.
/// Category: Playful Debuff. Lasts until next day.
/// </summary>
public class TheMoonCard : TarotCardBase
{
    private bool dayPassed = false;

    public override TarotType Type => TarotType.TheMoon;
    public override string CardName => "The Moon";
    public override string Description => "Mischievous ghost! Rent chance halved tonight.";
    public override string Symbol => "BOO";

    public override bool IsExpired => dayPassed;
    public override float RentChanceMultiplier => 0.5f;

    public override void OnApply(GameManager gm)
    {
        gm.ShowMessage("The Moon: A playful ghost is scaring away customers tonight!");
    }

    public override void OnDayAdvanced()
    {
        dayPassed = true;
    }
}
