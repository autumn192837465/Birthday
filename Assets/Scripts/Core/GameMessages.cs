using System.Collections.Generic;

/// <summary>
/// 集中管理 <see cref="GameManager.ShowMessage"/> 等 UI 用日文訊息。
/// </summary>
public static class GameMessages
{
    public const string InsufficientMoney = "お金が足りません！";
    public const string MagicPowerNoFatigue = "魔力のおかげで、疲労は消費されませんでした。";
    public const string TooTiredNeedSleep = "疲れすぎです。先に寝てください。";
    public const string TarotFortuneAlreadyDoneToday = "本日はすでに占いを行いました。今日はもう占えません。";
    public const string HermitBlocksCreation = "隠者：今日は休もう。創作は禁止！";

    public static string EarnMoneyBonus(int finalAmount, float multiplier)
    {
        return $"ボーナス！ +${finalAmount} (x{multiplier:F1})";
    }

    public static string GoodMorningDay(int dayCount)
    {
        return $"おはようございます！{dayCount}日目が始まります。";
    }

    public static string PaintingInProgress(string title, float progress)
    {
        return $"「{title}」制作中… {progress:F0}%";
    }

    public static string PromotionAlreadyActive(string title, int daysLeft)
    {
        return $"\"{title}\" プロモーション中！（残り{daysLeft}日）";
    }

    public static string PromotionSuccess(string title)
    {
        return $"\"{title}\" プロモーション成功！レンタル・販売の確率が2倍（3日間）！";
    }

    public static string ShopPurchaseBirthday(string itemName)
    {
        return $"{itemName}を購入しました！誕生日おめでとう！";
    }

    /// <summary>
    /// 夜間ギャラリー結果をトースト用に整形する。
    /// </summary>
    public static string FormatMarketResult(MarketResult result)
    {
        var parts = new List<string>();

        if (result.TotalRentIncome > 0)
        {
            parts.Add($"家賃収入: ${result.TotalRentIncome}");
        }

        if (result.NewlyRentedTitles.Count > 0)
        {
            parts.Add($"新規レンタル: {string.Join(", ", result.NewlyRentedTitles)}");
        }

        if (result.SoldTitles.Count > 0)
        {
            parts.Add($"売却: {string.Join(", ", result.SoldTitles)} (+${result.TotalSellIncome})");
        }

        if (result.ReturnedTitles.Count > 0)
        {
            parts.Add($"返却: {string.Join(", ", result.ReturnedTitles)}");
        }

        return parts.Count > 0
            ? string.Join(" | ", parts)
            : "静かな夜でした…ギャラリーの動きはありません。";
    }
}
