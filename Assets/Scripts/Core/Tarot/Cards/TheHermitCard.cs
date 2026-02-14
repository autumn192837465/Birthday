/// <summary>
/// The Hermit: Forced rest. Cannot work for the rest of today.
/// Category: Neutral. Lasts until next day.
/// </summary>
public class TheHermitCard : TarotCardBase
{
    private bool dayPassed = false;

    public override TarotType Type => TarotType.TheHermit;
    public override string CardName => "The Hermit";
    public override string Description => "Forced rest. No work today.";
    public override string Symbol => "REST";

    public override bool IsExpired => dayPassed;
    public override bool BlocksWork => true;

    public override void OnApply(GameManager gm)
    {
        gm.ShowMessage("The Hermit: Take the day off... no work today.");
    }

    public override void OnDayAdvanced()
    {
        dayPassed = true;
    }
}
