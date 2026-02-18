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

    [HideInInspector] public bool IsPromotionActive;

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
    /// Returns a summary of what happened for the player toast.
    /// </summary>
    public MarketResult ProcessDailyMarket()
    {
        var result = new MarketResult
        {
            NewlyRentedTitles = new List<string>(),
            SoldTitles = new List<string>(),
            ReturnedTitles = new List<string>()
        };

        var gm = GameManager.Instance;
        var gallery = GalleryManager.Instance;
        if (gm == null || gallery == null) return result;

        var settings = gm.Settings;
        float rentChanceMultiplier = gm.GetTotalRentChanceMultiplier();
        float promotionMultiplier = IsPromotionActive ? 2f : 1f;

        ProcessDisplayedPaintings(gallery, settings, promotionMultiplier, rentChanceMultiplier, gm, ref result);
        ProcessRentedPaintings(gallery, settings, gm, ref result);

        gallery.PurgeSoldPaintings();
        IsPromotionActive = false;

        return result;
    }

    private void ProcessDisplayedPaintings(
        GalleryManager gallery, GameSettings settings,
        float promotionMultiplier, float rentChanceMultiplier,
        GameManager gm, ref MarketResult result)
    {
        var displayed = gallery.GetDisplayedPaintings();

        foreach (var painting in displayed)
        {
            float sellChance = settings.BaseSellChance * promotionMultiplier;
            float rentChance = settings.BaseRentChance * promotionMultiplier * rentChanceMultiplier;

            // Sell check first (rare but lucrative)
            if (Random.value < sellChance)
            {
                int sellPrice = Mathf.RoundToInt(painting.BasePrice * settings.SellPriceMultiplier);
                painting.State = PaintingState.Sold;
                gm.EarnMoney(sellPrice);
                result.SoldTitles.Add(painting.Title);
                result.TotalSellIncome += sellPrice;
                continue;
            }

            // Rent check (common passive income)
            if (Random.value < rentChance)
            {
                painting.State = PaintingState.Rented;
                painting.RentDaysLeft = Random.Range(settings.MinRentDays, settings.MaxRentDays + 1);
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
            int rentIncome = Mathf.RoundToInt(painting.BasePrice * settings.RentIncomeMultiplier);
            gm.EarnMoney(rentIncome);
            result.TotalRentIncome += rentIncome;

            painting.RentDaysLeft--;
            if (painting.RentDaysLeft <= 0)
            {
                painting.State = PaintingState.Displayed;
                result.ReturnedTitles.Add(painting.Title);
            }
        }
    }

    /// <summary>
    /// Format market results into a readable toast message.
    /// </summary>
    public static string FormatResult(MarketResult result)
    {
        var parts = new List<string>();

        if (result.TotalRentIncome > 0)
            parts.Add($"Rent collected: ${result.TotalRentIncome}");

        if (result.NewlyRentedTitles.Count > 0)
            parts.Add($"Newly rented: {string.Join(", ", result.NewlyRentedTitles)}");

        if (result.SoldTitles.Count > 0)
            parts.Add($"SOLD: {string.Join(", ", result.SoldTitles)} (+${result.TotalSellIncome})");

        if (result.ReturnedTitles.Count > 0)
            parts.Add($"Returned: {string.Join(", ", result.ReturnedTitles)}");

        return parts.Count > 0 ? string.Join(" | ", parts) : "Quiet night... no gallery activity.";
    }
}
