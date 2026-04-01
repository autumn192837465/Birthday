using UnityEngine;

/// <summary>
/// Normal morning: full stamina at base maximum.
/// </summary>
[CreateAssetMenu(fileName = "NormalMorningEvent", menuName = "Game/Morning Event/Normal")]
public class NormalEventSO : MorningEventSO
{
    public override void ApplyEffect(GameManager gameManager, GalleryManager galleryManager)
    {
        int max = gameManager.GetBaseMaxStamina();
        gameManager.SetDailyStaminaState(max, max);
    }
}
