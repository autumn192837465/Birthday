/// <summary>
/// The Fool: Next 3 work sessions earn 10% less.
/// Category: Playful Debuff. Action-count based.
/// </summary>
public class TheFoolCard : TarotCardBase
{
    private int remainingWorkSessions = 3;

    public override TarotType Type => TarotType.TheFool;
    public override string CardName => "The Fool";
    public override string Description => $"Clumsy! Salary -10% ({remainingWorkSessions} sessions left)";
    public override string Symbol => "OOF";

    public override bool IsExpired => remainingWorkSessions <= 0;
    public override float SalaryBonusPercent => -0.10f;

    public override void OnApply(GameManager gm)
    {
        gm.ShowMessage("The Fool: Oops! Salary -10% for the next 3 work sessions.");
    }

    public override void OnWorkPerformed()
    {
        remainingWorkSessions--;
    }
}
