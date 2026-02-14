/// <summary>
/// The Moon: Work button moves around chaotically for the rest of today.
/// Category: Playful Debuff. Lasts until next day.
/// </summary>
public class TheMoonCard : TarotCardBase
{
    private bool dayPassed = false;

    public override TarotType Type => TarotType.TheMoon;
    public override string CardName => "The Moon";
    public override string Description => "Mischievous ghost! Work button is jumpy today.";
    public override string Symbol => "BOO";

    public override bool IsExpired => dayPassed;
    public override bool MakesWorkButtonChaotic => true;

    public override void OnApply(GameManager gm)
    {
        gm.ShowMessage("The Moon: A playful ghost is haunting the work button!");
    }

    public override void OnDayAdvanced()
    {
        dayPassed = true;
    }
}
