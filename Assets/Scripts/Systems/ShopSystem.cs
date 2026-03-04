using UnityEngine;
using DG.Tweening;

/// <summary>
/// Shop system: buy gifts to trigger the "Game Clear" ending.
/// Uses shop data from DataManager to display items with prices and sprites.
/// </summary>
public class ShopSystem : PanelBase
{
    [Header("Shop Slots")]
    [SerializeField] private ShopSlot[] shopSlots;

    private bool _slotsSubscribed;
    private DataManager _dataManager;

    private void Start()
    {
        _dataManager = GameManager.Instance != null ? GameManager.Instance.DataManager : null;
        SetupShopSlots();
    }

    /// <summary>
    /// Initialize shop slots with data from DataManager.
    /// Updates sprite and price for each slot based on its ItemType.
    /// </summary>
    private void SetupShopSlots()
    {
        if (shopSlots == null || _dataManager == null)
        {
            return;
        }

        foreach (var slot in shopSlots)
        {
            var entry = _dataManager.GetShopEntryByType(slot.ItemType);
            if (entry == null)
            {
                continue;
            }

            UpdateSlotVisual(slot, entry);

            if (slot.CostButton != null && !_slotsSubscribed)
            {
                var itemType = slot.ItemType;
                slot.CostButton.OnClick += () => OnShopItemClicked(itemType);
            }
        }

        _slotsSubscribed = true;
    }

    private void UpdateSlotVisual(ShopSlot slot, ShopData.ShopEntry entry)
    {
        if (slot == null || entry == null)
        {
            return;
        }

        if (slot.Icon != null)
        {
            slot.Icon.sprite = entry.Sprite;
        }

        if (slot.CostButton != null)
        {
            slot.CostButton.SetCostText($"{entry.Price}");
        }
    }

    /// <summary>
    /// Refresh all shop slots with current data.
    /// Call this when shop data might have changed.
    /// </summary>
    public void RefreshShopDisplay()
    {
        if (shopSlots == null || _dataManager == null)
        {
            return;
        }

        foreach (var slot in shopSlots)
        {
            var entry = _dataManager.GetShopEntryByType(slot.ItemType);
            if (entry != null)
            {
                UpdateSlotVisual(slot, entry);
            }
        }
    }

    /// <summary>
    /// Update a specific slot's sprite and price by ItemType.
    /// </summary>
    public void UpdateSlot(ShopItemType itemType, Sprite sprite, int price)
    {
        if (shopSlots == null)
        {
            return;
        }

        foreach (var slot in shopSlots)
        {
            if (slot.ItemType == itemType)
            {
                if (slot.Icon != null)
                {
                    slot.Icon.sprite = sprite;
                }

                if (slot.CostButton != null && _dataManager != null)
                {
                    var entry = _dataManager.GetShopEntryByType(itemType);
                    string name = entry?.Name ?? itemType.ToString();
                    slot.CostButton.SetCostText($"{price}");
                }
                return;
            }
        }
    }

    /// <summary>
    /// Called when a shop item is clicked.
    /// </summary>
    public void OnShopItemClicked(ShopItemType itemType)
    {
        var gm = GameManager.Instance;
        if (gm == null || _dataManager == null)
        {
            return;
        }

        var entry = _dataManager.GetShopEntryByType(itemType);
        if (entry == null)
        {
            return;
        }

        if (!gm.SpendMoney(entry.Price))
        {
            return;
        }

        ShopSlot clickedSlot = GetSlotByType(itemType);
        if (clickedSlot != null)
        {
            clickedSlot.SetSoldOut();
        }

        gm.ShowMessage($"{entry.Name}を購入しました！誕生日おめでとう！");

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameClear(entry.Name);
        }
    }

    private ShopSlot GetSlotByType(ShopItemType itemType)
    {
        if (shopSlots == null)
        {
            return null;
        }

        foreach (var slot in shopSlots)
        {
            if (slot.ItemType == itemType)
            {
                return slot;
            }
        }
        return null;
    }

    /// <summary>
    /// Animate the shop slot on purchase.
    /// </summary>
    private void PlayPurchaseAnimation(ShopSlot slot, TweenCallback onComplete)
    {
        onComplete?.Invoke();
    }
}
