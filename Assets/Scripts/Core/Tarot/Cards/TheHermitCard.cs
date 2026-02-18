/// <summary>
/// The Hermit: Forced rest. Cannot create art for the rest of today.
/// Category: Neutral. Lasts until next day.
/// </summary>
public class TheHermitCard : TarotCardBase
{
    private bool dayPassed = false;

    public override TarotType Type => TarotType.TheHermit;
    public override string CardName => "The Hermit";
    public override string Description => "Forced rest. No creating today.";
    public override string Symbol => "REST";

    public override bool IsExpired => dayPassed;
    public override bool BlocksCreation => true;

    public override void OnApply(GameManager gm)
    {
        gm.ShowMessage("The Hermit: Take the day off... no creating today.");
    }

    public override void OnDayAdvanced()
    {
        dayPassed = true;
    }
}
