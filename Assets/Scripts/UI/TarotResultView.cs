using System;
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
    [SerializeField] private Button confirmButton;

    public Action OnConfirm;

    protected override void Awake()
    {
        base.Awake();
        confirmButton.onClick.AddListener(() => OnConfirm?.Invoke());
    }

    /// <summary>
    /// 依塔羅類型從 DataManager 取得圖、名稱、敘述並更新 UI。
    /// </summary>
    public void InitializeUI(TarotType type)
    {
        var dm = GameManager.Instance != null ? GameManager.Instance.DataManager : null;
        if (dm == null) return;

        tarotImage.sprite = dm.GetTarotSprite(type);
        tarotNameText.text = dm.GetTarotName(type);
        tarotDescriptionText.text = dm.GetTarotDescription(type);
    }

    private void OnDestroy()
    {
        confirmButton.onClick.RemoveAllListeners();
    }
}
