/// <summary>
/// The Fool: Next 3 paintings have -10% base value.
/// Category: Playful Debuff. Creation-count based.
/// </summary>
public class TheFoolCard : TarotCardBase
{
    private int remainingSessions = 3;

    public override TarotType Type => TarotType.TheFool;
    public override string CardName => "The Fool";
    public override string Description => $"Clumsy! Painting value -10% ({remainingSessions} paintings left)";
    public override string Symbol => "OOF";

    public override bool IsExpired => remainingSessions <= 0;
    public override float PaintingValueMultiplier => 0.9f;

    public override void OnApply(GameManager gm)
    {
        gm.ShowMessage("The Fool: Oops! Next 3 paintings lose 10% value.");
    }

    public override void OnPaintingCreated()
    {
        remainingSessions--;
    }
}
