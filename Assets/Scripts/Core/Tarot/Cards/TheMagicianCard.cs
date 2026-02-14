/// <summary>
/// The Magician: Work does not consume fatigue for the rest of today.
/// Category: Positive (Work). Lasts until next day.
/// </summary>
public class TheMagicianCard : TarotCardBase
{
    private bool dayPassed = false;

    public override TarotType Type => TarotType.TheMagician;
    public override string CardName => "The Magician";
    public override string Description => "Work costs no fatigue today!";
    public override string Symbol => "ZAP";

    public override bool IsExpired => dayPassed;
    public override bool BlocksFatigue => true;

    public override void OnApply(GameManager gm)
    {
        gm.ShowMessage("The Magician: Work without getting tired today!");
    }

    public override void OnDayAdvanced()
    {
        dayPassed = true;
    }
}
