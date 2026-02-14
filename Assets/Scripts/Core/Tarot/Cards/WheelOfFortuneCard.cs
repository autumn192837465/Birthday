/// <summary>
/// Wheel of Fortune: The next mini-game is a guaranteed big win.
/// Category: Positive (Money). One-shot trigger.
/// </summary>
public class WheelOfFortuneCard : TarotCardBase
{
    private bool consumed = false;

    public override TarotType Type => TarotType.WheelOfFortune;
    public override string CardName => "Wheel of Fortune";
    public override string Description => "Next mini-game is a guaranteed BIG WIN!";
    public override string Symbol => "WIN";

    public override bool IsExpired => consumed;
    public override bool GuaranteesMiniGameWin => !consumed;

    public override void OnApply(GameManager gm)
    {
        gm.ShowMessage("Wheel of Fortune: Your next game is a guaranteed win!");
    }

    public override void OnMiniGamePlayed()
    {
        consumed = true;
    }
}
