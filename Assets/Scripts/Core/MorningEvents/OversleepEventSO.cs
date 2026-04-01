using UnityEngine;

/// <summary>
/// Wake up with only part of the stamina bar filled.
/// </summary>
[CreateAssetMenu(fileName = "OversleepMorningEvent", menuName = "Game/Morning Event/Oversleep")]
public class OversleepEventSO : MorningEventSO
{
    [Tooltip("Fraction of max stamina remaining after waking (e.g. 0.8 = 80%).")]
    [Range(0.05f, 1f)]
    [SerializeField] private float wakeStaminaRatio = 0.8f;

    public override void ApplyEffect(GameManager gameManager, GalleryManager galleryManager)
    {
        int effectiveMax = gameManager.GetBaseMaxStamina();
        int current = Mathf.Max(0, Mathf.RoundToInt(effectiveMax * wakeStaminaRatio));
        gameManager.SetDailyStaminaState(current, effectiveMax);
    }
}
