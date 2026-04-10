using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HudView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI fatigueText;
    [SerializeField] private TextMeshProUGUI dayText;

    [Header("Inventory")]
    [SerializeField] private Transform inventoryContainer;
    [SerializeField] private HudItemCell itemCellPrefab;

    private readonly Dictionary<ShopItemType, HudItemCell> _cells = new Dictionary<ShopItemType, HudItemCell>();

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
            if (gm.IsItemPurchased(type) && !_cells.ContainsKey(type))
            {
                CreateCell(type);
            }
        }
    }

    public void SetMoney(int amount)
    {
        moneyText.text = amount.ToString();
    }

    /// <summary>Display remaining stamina vs today's effective maximum.</summary>
    public void SetStamina(int current, int effectiveMax)
    {
        fatigueText.text = $"{current}/{effectiveMax}";
    }

    public void SetDay(int day)
    {
        dayText.text = $"Day {day}";
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
        if (_cells.ContainsKey(type))
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
        cell.OnClicked += description => gm.ShowMessage(description);

        _cells[type] = cell;
    }
}
    