using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 每日結算 popup：顯示過夜後的畫廊市場結果（租金收入、新租賃、賣出、歸還）。
/// 繼承 AnimatorBase 以支援開啟/關閉動畫。
/// </summary>
public class DailySummaryPopupView : AnimatorBase
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI summaryText;
    [SerializeField] private TextMeshProUGUI totalIncomeText;
    [SerializeField] private Button closeButton;

    [Header("Detail Sections (Optional)")]
    [SerializeField] private GameObject rentSection;
    [SerializeField] private TextMeshProUGUI rentDetailText;
    [SerializeField] private GameObject newRentalSection;
    [SerializeField] private TextMeshProUGUI newRentalDetailText;
    [SerializeField] private GameObject soldSection;
    [SerializeField] private TextMeshProUGUI soldDetailText;
    [SerializeField] private GameObject returnedSection;
    [SerializeField] private TextMeshProUGUI returnedDetailText;

    /// <summary>
    /// 當用戶點擊關閉按鈕時觸發。
    /// </summary>
    public event Action OnCloseClicked;

    private MarketResult _currentResult;

    protected override void Awake()
    {
        base.Awake();
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }
    }

    private void OnDestroy()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(OnCloseButtonClicked);
        }
    }

    private void OnCloseButtonClicked()
    {
        Close();
        OnCloseClicked?.Invoke();
    }

    /// <summary>
    /// 設定並顯示市場結果。
    /// </summary>
    public void ShowResult(MarketResult result, int dayCount)
    {
        _currentResult = result;
        UpdateUI(dayCount);
        Open();
    }

    private void UpdateUI(int dayCount)
    {
        titleText.text = $"{dayCount}日目の精算";

        int totalIncome = _currentResult.TotalRentIncome + _currentResult.TotalSellIncome;
        bool hasActivity = totalIncome > 0 || 
                          _currentResult.NewlyRentedTitles.Count > 0 || 
                          _currentResult.ReturnedTitles.Count > 0;

        summaryText.text = BuildSummaryText(hasActivity);

        if (totalIncome > 0)
        {
            totalIncomeText.text = $"今日の収入：{totalIncome}円";
            totalIncomeText.gameObject.SetActive(true);
        }
        else
        {
            totalIncomeText.gameObject.SetActive(false);
        }
        UpdateDetailSections();
    }

    private string BuildSummaryText(bool hasActivity)
    {
        if (!hasActivity)
        {
            return "今夜のギャラリーは特に動きがありませんでした…\n明日も頑張ろう！";
        }

        var sb = new StringBuilder();

        if (_currentResult.TotalRentIncome > 0)
        {
            sb.AppendLine($"<color=#4CAF50>【レンタル収入】</color>");
            sb.AppendLine($"レンタル収入：${_currentResult.TotalRentIncome}");
            sb.AppendLine();
        }

        if (_currentResult.NewlyRentedTitles.Count > 0)
        {
            sb.AppendLine($"<color=#2196F3>【新租賃】</color>");
            foreach (var title in _currentResult.NewlyRentedTitles)
            {
                sb.AppendLine($"• \"{title}\" がレンタルされました！");
            }
            sb.AppendLine();
        }

        if (_currentResult.SoldTitles.Count > 0)
        {
            sb.AppendLine($"<color=#FF9800>【売却！】</color>");
            foreach (var title in _currentResult.SoldTitles)
            {
                sb.AppendLine($"• \"{title}\" が売れました！");
            }
            sb.AppendLine($"売却収入：${_currentResult.TotalSellIncome}");
            sb.AppendLine();
        }

        if (_currentResult.ReturnedTitles.Count > 0)
        {
            sb.AppendLine($"<color=#9C27B0>【返却】</color>");
            foreach (var title in _currentResult.ReturnedTitles)
            {
                sb.AppendLine($"• \"{title}\" は返却されました");
            }
        }

        return sb.ToString().TrimEnd();
    }

    private void UpdateDetailSections()
    {
        if (rentSection != null)
        {
            bool hasRent = _currentResult.TotalRentIncome > 0;
            rentSection.SetActive(hasRent);
            if (hasRent && rentDetailText != null)
            {
                rentDetailText.text = $"${_currentResult.TotalRentIncome}";
            }
        }

        if (newRentalSection != null)
        {
            bool hasNewRental = _currentResult.NewlyRentedTitles.Count > 0;
            newRentalSection.SetActive(hasNewRental);
            if (hasNewRental && newRentalDetailText != null)
            {
                newRentalDetailText.text = string.Join("\n", _currentResult.NewlyRentedTitles);
            }
        }

        if (soldSection != null)
        {
            bool hasSold = _currentResult.SoldTitles.Count > 0;
            soldSection.SetActive(hasSold);
            if (hasSold && soldDetailText != null)
            {
                soldDetailText.text = $"{string.Join(", ", _currentResult.SoldTitles)}\n+${_currentResult.TotalSellIncome}";
            }
        }

        if (returnedSection != null)
        {
            bool hasReturned = _currentResult.ReturnedTitles.Count > 0;
            returnedSection.SetActive(hasReturned);
            if (hasReturned && returnedDetailText != null)
            {
                returnedDetailText.text = string.Join("\n", _currentResult.ReturnedTitles);
            }
        }
    }

    /// <summary>
    /// 檢查市場結果是否有任何活動（用於決定是否顯示 popup）。
    /// </summary>
    public static bool HasAnyActivity(MarketResult result)
    {
        return result.TotalRentIncome > 0 ||
               result.TotalSellIncome > 0 ||
               result.NewlyRentedTitles.Count > 0 ||
               result.SoldTitles.Count > 0 ||
               result.ReturnedTitles.Count > 0;
    }
}
