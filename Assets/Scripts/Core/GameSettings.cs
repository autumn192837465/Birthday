using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// ScriptableObject holding all game configuration values.
/// Create via Assets > Create > Game > GameSettings.
/// </summary>
[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Data")]
    [Tooltip("ScriptableObject 儲存塔羅牌圖、名稱、敘述。由 DataManager 使用。")]
    public TarotData TarotData;

    [Header("Stamina")]
    [Tooltip("Base maximum stamina (full bar). Morning events may raise effective max for the day.")]
    [FormerlySerializedAs("MaxFatigue")]
    public int BaseMaxStamina = 100;

    [Header("Gallery")]
    [Tooltip("Stamina consumed per painting creation step.")]
    [FormerlySerializedAs("PaintingFatigueCost")]
    public int PaintingStaminaCost = 30;

    [Tooltip("Minimum base price for a newly created painting.")]
    public int PaintingBasePriceMin = 80;

    [Tooltip("Maximum base price for a newly created painting.")]
    public int PaintingBasePriceMax = 250;

    [Tooltip("Maximum number of paintings that can be displayed on the wall at once.")]
    public int MaxDisplaySlots = 5;
    
    [Tooltip("Max and Minimum progress gain when creating/upgrading a painting.")]
    public int MaxPaintingProgress = 60;
    public int MinPaintingProgress = 30;

    [Header("Market")]
    [Tooltip("Base probability a displayed painting gets rented each night (0-1).")]
    [Range(0f, 1f)]
    public float BaseRentChance = 0.3f;

    [Tooltip("Base probability a displayed painting gets sold each night (0-1). Checked before rent.")]
    [Range(0f, 1f)]
    public float BaseSellChance = 0.05f;

    [Tooltip("Daily rent income as a fraction of BasePrice.")]
    public float RentIncomeMultiplier = 0.1f;

    [Tooltip("Minimum daily probability (0-1) that a rented painting is returned.")]
    [Range(0f, 1f)]
    public float DailyReturnChanceMin = 0.15f;

    [Tooltip("Maximum daily probability (0-1) that a rented painting is returned.")]
    [Range(0f, 1f)]
    public float DailyReturnChanceMax = 0.4f;

    [Tooltip("Sell price = BasePrice * this multiplier.")]
    public float SellPriceMultiplier = 10f;

    [Tooltip("Cost to buy a one-day promotion (doubles rent/sell chances).")]
    public int PromotionCost = 300;

    [Header("Fortune")]
    [Tooltip("Cost to draw a fortune card.")]
    public int FortuneCost = 200;

}
