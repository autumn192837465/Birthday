/// <summary>
/// The Magician: Creating art does not consume stamina for the rest of today.
/// Category: Positive (Gallery). Lasts until next day.
/// </summary>
public class TheMagicianCard : TarotCardBase
{
    private bool dayPassed = false;

    public override TarotType Type => TarotType.TheMagician;
    public override string CardName => "The Magician";
    public override string Description => "Creating art costs no stamina today!";
    public override string Symbol => "ZAP";

    public override bool IsExpired => dayPassed;
    public override bool SkipsStaminaCost => true;

    public override void OnDayAdvanced()
    {
        dayPassed = true;
    }
}
