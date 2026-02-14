using UnityEngine;

/// <summary>
/// The Hanged Man: Instantly lose 20 coins.
/// Category: Playful Debuff. Instant effect.
/// Player-friendly: loss is very small.
/// </summary>
public class TheHangedManCard : TarotCardBase
{
    public override TarotType Type => TarotType.TheHangedMan;
    public override string CardName => "The Hanged Man";
    public override string Description => "Feeling heavy... lost 20 coins.";
    public override string Symbol => "-$";

    public override bool IsExpired => true;

    public override void OnApply(GameManager gm)
    {
        int loss = Mathf.Min(20, gm.Money); // Never go below 0
        gm.DeductMoney(loss);
        gm.ShowMessage($"The Hanged Man: Lost ${loss}... but it's just pocket change!");
    }
}
