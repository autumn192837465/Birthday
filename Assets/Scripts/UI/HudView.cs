using System.Collections.Generic;
using UnityEngine;

public class HudView : MonoBehaviour
{
    [Header("Stat Cells")]
    [SerializeField] private HudItemCell moneyCell;
    [SerializeField] private HudItemCell fatigueCell;
    [SerializeField] private HudItemCell dayCell;

    [Header("Inventory")]
    [SerializeField] private Transform inventoryContainer;
    [SerializeField] private HudItemCell itemCellPrefab;

    private readonly HashSet<ShopItemType> _createdCells = new HashSet<ShopItemType>();

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnItemPurchased += HandleItemPurchased;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnItemPurchased -= HandleItemPurchased;
        }
    }

    /// <summary>
    /// 根據 GameManager 已購買清單，補建先前遺漏的格子。
    /// 應在 HUD 首次顯示或場景載入後呼叫。
    /// </summary>
    public void RefreshInventory()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.DataManager == null)
        {
            return;
        }

        foreach (ShopItemType type in System.Enum.GetValues(typeof(ShopItemType)))
        {
            if (gm.IsItemPurchased(type) && !_createdCells.Contains(type))
            {
                CreateCell(type);
            }
        }
    }

    public void SetMoney(int amount)
    {
        if (moneyCell != null)
        {
            moneyCell.SetText(amount.ToString());
        }
    }

    /// <summary>Display remaining stamina vs today's effective maximum.</summary>
    public void SetStamina(int current, int effectiveMax)
    {
        if (fatigueCell != null)
        {
            fatigueCell.SetText($"{current}/{effectiveMax}");
        }
    }

    public void SetDay(int day)
    {
        if (dayCell != null)
        {
            dayCell.SetText($"Day {day}");
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void HandleItemPurchased(ShopItemType type)
    {
        if (_createdCells.Contains(type))
        {
            return;
        }

        CreateCell(type);
    }

    private void CreateCell(ShopItemType type)
    {
        if (itemCellPrefab == null || inventoryContainer == null)
        {
            return;
        }

        var gm = GameManager.Instance;
        if (gm == null || gm.DataManager == null)
        {
            return;
        }

        var entry = gm.DataManager.GetShopEntryByType(type);
        if (entry == null)
        {
            return;
        }

        HudItemCell cell = Instantiate(itemCellPrefab, inventoryContainer);
        cell.Setup(entry.Sprite, entry.Description);

        _createdCells.Add(type);
    }
}
    