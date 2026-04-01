using UnityEngine;

/// <summary>
/// Increases painting stamina cost for the day. Reset on the next morning before a new event applies.
/// </summary>
[CreateAssetMenu(fileName = "StiffNeckMorningEvent", menuName = "Game/Morning Event/Stiff Neck")]
public class StiffNeckEventSO : MorningEventSO
{
    [Tooltip("Multiplier on painting stamina cost until the next morning.")]
    [SerializeField] private float paintingCostMultiplier = 1.5f;

    public override void ApplyEffect(GameManager gameManager, GalleryManager galleryManager)
    {
        int max = gameManager.GetBaseMaxStamina();
        gameManager.SetDailyStaminaState(max, max);
        gameManager.SetPaintingCostMultiplier(paintingCostMultiplier);
    }

    public override void ResetEffect(GameManager gameManager, GalleryManager galleryManager)
    {
        gameManager.SetPaintingCostMultiplier(1f);
    }
}
