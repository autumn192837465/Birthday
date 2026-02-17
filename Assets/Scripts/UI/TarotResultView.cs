using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 塔羅抽牌結果畫面：顯示選中牌的圖、名稱、敘述。
/// 由 TarotSystem 在玩家選牌後呼叫 InitializeUI(type) 刷新內容。
/// </summary>
public class TarotResultView : AnimatorBase
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI tarotNameText;
    [SerializeField] private TextMeshProUGUI tarotDescriptionText;
    [SerializeField] private Image tarotImage;

    /// <summary>
    /// 依塔羅類型從 DataManager 取得圖、名稱、敘述並更新 UI。
    /// </summary>
    public void InitializeUI(TarotType type)
    {
        var dm = GameManager.Instance != null ? GameManager.Instance.DataManager : null;
        if (dm == null) return;

        if (tarotImage != null)
            tarotImage.sprite = dm.GetTarotSprite(type);
        if (tarotNameText != null)
            tarotNameText.text = dm.GetTarotName(type);
        if (tarotDescriptionText != null)
            tarotDescriptionText.text = dm.GetTarotDescription(type);
    }
}
