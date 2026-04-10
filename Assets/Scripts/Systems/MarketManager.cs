using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Result of a single night's market processing, used for the summary toast.
/// </summary>
public struct MarketResult
{
    public int TotalRentIncome;
    public List<string> NewlyRentedTitles;
    public List<string> SoldTitles;
    public int TotalSellIncome;
    public List<string> ReturnedTitles;
}

/// <summary>
/// Processes the nightly gallery market: rent collection, new rentals, and sales.
/// Called by GameManager during the Sleep/AdvanceDay flow.
/// </summary>
public class MarketManager : MonoBehaviour
{
    public static MarketManager Instance { get; private set; }

    /// <summary>加在 <see cref="GameSettings.BaseRentChance"/> 上的商店加成（可累加）。</summary>
    public float BaseRentChance { get; set; }

    /// <summary>加在 <see cref="GameSettings.BaseSellChance"/> 上的商店加成（可累加）。</summary>
    public float BaseSellChance { get; set; }

    /// <summary>乘在租賃／售出收入上的商店倍率。</summary>
    public float IncomeMultiplier { get; set; } = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Run the nightly market simulation on all displayed and rented paintings.
    /// Per-painting promotion is read from painting.IsPromoted and reset after processing.
    /// Returns null when there are no displayed and no rented paintings (nothing to process).
    /// </summary>
    public MarketResult? ProcessDailyMarket()
    {
        var gallery = GalleryManager.Instance;
        var displayed = gallery.GetDisplayedPaintings();
        var rented = gallery.GetRentedPaintings();

        if (displayed.Count == 0 && rented.Count == 0)
        {
            return null;
        }

        var result = new MarketResult
        {
            NewlyRentedTitles = new List<string>(),
            SoldTitles = new List<string>(),
            ReturnedTitles = new List<string>()
        };

        var gm = GameManager.Instance;
        var settings = gm.Settings;
        float rentChanceMultiplier = gm.GetTotalRentChanceMultiplier();

        ProcessDisplayedPaintings(gallery, settings, rentChanceMultiplier, gm, ref result);
        ProcessRentedPaintings(gallery, settings, gm, ref result);

        gallery.PurgeSoldPaintings();
        gallery.DecreasePromotionDays();

        return result;
    }

    private void ProcessDisplayedPaintings(
        GalleryManager gallery, GameSettings settings,
        float rentChanceMultiplier,
        GameManager gm, ref MarketResult result)
    {
        var displayed = gallery.GetDisplayedPaintings();

        foreach (var painting in displayed)
        {
            float promotionMultiplier = painting.IsPromoted ? 2f : 1f;
            float sellChance = (settings.BaseSellChance + BaseSellChance) * promotionMultiplier;
            float rentChance = (settings.BaseRentChance + BaseRentChance) * promotionMultiplier * rentChanceMultiplier;
            sellChance = Mathf.Clamp01(sellChance);
            rentChance = Mathf.Clamp01(rentChance);

            if (Random.value < sellChance)
            {
                int sellPrice = Mathf.RoundToInt(painting.BasePrice * settings.SellPriceMultiplier * IncomeMultiplier);
                painting.State = PaintingState.Sold;
                gm.EarnMoney(sellPrice);
                result.SoldTitles.Add(painting.Title);
                result.TotalSellIncome += sellPrice;
                continue;
            }

            if (Random.value < rentChance)
            {
                painting.State = PaintingState.Rented;
                painting.RentDaysLeft = 0;
                result.NewlyRentedTitles.Add(painting.Title);
            }
        }
    }

    private void ProcessRentedPaintings(
        GalleryManager gallery, GameSettings settings,
        GameManager gm, ref MarketResult result)
    {
        var rented = gallery.GetRentedPaintings();

        foreach (var painting in rented)
        {
            int rentIncome = Mathf.RoundToInt(painting.BasePrice * settings.RentIncomeMultiplier * IncomeMultiplier);
            gm.EarnMoney(rentIncome);
            result.TotalRentIncome += rentIncome;

            float returnChance = Random.Range(settings.DailyReturnChanceMin, settings.DailyReturnChanceMax);
            if (Random.value < returnChance)
            {
                painting.State = PaintingState.Displayed;
                painting.RentDaysLeft = 0;
                result.ReturnedTitles.Add(painting.Title);
            }
        }
    }

    /// <summary>
    /// Format market results into a readable toast message.
    /// </summary>
    public static string FormatResult(MarketResult result)
    {
        return GameMessages.FormatMarketResult(result);
    }
}
