using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 推廣面板：顯示所有已完成畫作的 Scroller，
/// 每個 Cell 可點擊推廣按鈕。
/// </summary>
public class PromotionView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Button closeButton;

    [Header("Cells")]
    [SerializeField] private PromotionCell[] cells;

    public event Action OnCloseClicked;
    public event Action<DrawingType> OnPromotionClicked;

    private void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(HandleCloseClick);

        foreach (var cell in cells)
        {
            cell.OnPromotionClicked += HandleCellPromotionClick;
        }
    }

    private void OnDestroy()
    {
        if (closeButton != null)
            closeButton.onClick.RemoveListener(HandleCloseClick);

        foreach (var cell in cells)
        {
            cell.OnPromotionClicked -= HandleCellPromotionClick;
        }
    }

    private void HandleCloseClick()
    {
        OnCloseClicked?.Invoke();
    }

    private void HandleCellPromotionClick(DrawingType drawingType)
    {
        OnPromotionClicked?.Invoke(drawingType);
    }

    /// <summary>
    /// 顯示面板。
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 隱藏面板。
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 刷新所有 Cell：只顯示已完成（非 InProgress、非 Sold）的畫作。
    /// </summary>
    public void Refresh(IReadOnlyList<Painting> completedPaintings)
    {
        var dm = GameManager.Instance?.DataManager;

        var paintingsByType = new Dictionary<DrawingType, Painting>();
        if (completedPaintings != null)
        {
            foreach (var p in completedPaintings)
            {
                if (!paintingsByType.ContainsKey(p.DrawingType))
                {
                    paintingsByType[p.DrawingType] = p;
                }
            }
        }

        foreach (var cell in cells)
        {
            if (cell == null) continue;

            if (paintingsByType.TryGetValue(cell.DrawingType, out var painting))
            {
                cell.SetData(painting.IsPromoted);
                cell.Show();
            }
            else
            {
                cell.Hide();
            }
        }

        scrollRect.horizontalNormalizedPosition = 0f;
            
    }

    /// <summary>
    /// 更新指定 DrawingType 的推廣狀態。
    /// </summary>
    public void UpdatePromotionState(DrawingType drawingType, bool isPromoted)
    {
        foreach (var cell in cells)
        {
            if (cell != null && cell.DrawingType == drawingType)
            {
                cell.SetPromoted(isPromoted);
                return;
            }
        }
    }
}
