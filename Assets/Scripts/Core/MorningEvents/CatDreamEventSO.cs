using UnityEngine;

/// <summary>
/// Higher effective max stamina for the day; wake up with a full bar at the new cap.
/// </summary>
[CreateAssetMenu(fileName = "CatDreamMorningEvent", menuName = "Game/Morning Event/Cat Dream")]
public class CatDreamEventSO : MorningEventSO
{
    [Tooltip("Multiplier applied to base max stamina for today's effective cap (e.g. 1.2 = 120%).")]
    [SerializeField] private float effectiveMaxStaminaMultiplier = 1.2f;

    public override void ApplyEffect(GameManager gameManager, GalleryManager galleryManager)
    {
        int baseMax = gameManager.GetBaseMaxStamina();
        int effectiveMax = Mathf.Max(1, Mathf.RoundToInt(baseMax * effectiveMaxStaminaMultiplier));
        gameManager.SetDailyStaminaState(effectiveMax, effectiveMax);
    }
}
