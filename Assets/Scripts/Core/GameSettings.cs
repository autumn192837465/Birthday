using UnityEngine;

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

    [Header("Fatigue")]
    [Tooltip("Maximum fatigue before the player must sleep.")]
    public int MaxFatigue = 100;

    [Header("Gallery")]
    [Tooltip("Fatigue added per painting created.")]
    public int PaintingFatigueCost = 30;

    [Tooltip("Minimum base price for a newly created painting.")]
    public int PaintingBasePriceMin = 80;

    [Tooltip("Maximum base price for a newly created painting.")]
    public int PaintingBasePriceMax = 250;

    [Tooltip("Maximum number of paintings that can be displayed on the wall at once.")]
    public int MaxDisplaySlots = 5;

    [Header("Market")]
    [Tooltip("Base probability a displayed painting gets rented each night (0-1).")]
    [Range(0f, 1f)]
    public float BaseRentChance = 0.3f;

    [Tooltip("Base probability a displayed painting gets sold each night (0-1). Checked before rent.")]
    [Range(0f, 1f)]
    public float BaseSellChance = 0.05f;

    [Tooltip("Daily rent income as a fraction of BasePrice.")]
    public float RentIncomeMultiplier = 0.1f;

    [Tooltip("Minimum days a painting stays rented.")]
    public int MinRentDays = 2;

    [Tooltip("Maximum days a painting stays rented.")]
    public int MaxRentDays = 5;

    [Tooltip("Sell price = BasePrice * this multiplier.")]
    public float SellPriceMultiplier = 10f;

    [Tooltip("Cost to buy a one-day promotion (doubles rent/sell chances).")]
    public int PromotionCost = 300;

    [Header("Fortune")]
    [Tooltip("Cost to draw a fortune card.")]
    public int FortuneCost = 200;

    [Header("Shop")]
    [Tooltip("Gift names displayed on buttons.")]
    public string[] GiftNames = new string[] { "Teddy Bear", "Music Box", "Birthday Cake" };

    [Tooltip("Gift prices matching GiftNames array.")]
    public int[] GiftPrices = new int[] { 500, 800, 1200 };
}
