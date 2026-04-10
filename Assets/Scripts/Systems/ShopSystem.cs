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

    [Header("Confirm Popup")]
    [SerializeField] private ShopConfirmPopupView confirmPopup;

    private bool _slotsSubscribed;
    private DataManager _dataManager;
    private ShopSlot _pendingSlot;

    private void Start()
    {
        _dataManager = GameManager.Instance != null ? GameManager.Instance.DataManager : null;

        if (confirmPopup != null)
        {
            confirmPopup.BuyClicked += OnConfirmBuyClicked;
            confirmPopup.CloseClicked += OnConfirmCloseClicked;
        }

        SetupShopSlots();
    }

    private void OnDestroy()
    {
        if (confirmPopup != null)
        {
            confirmPopup.BuyClicked -= OnConfirmBuyClicked;
            confirmPopup.CloseClicked -= OnConfirmCloseClicked;
        }
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

        var gm = GameManager.Instance;

        foreach (var slot in shopSlots)
        {
            var entry = _dataManager.GetShopEntryByType(slot.ItemType);
            if (entry == null)
            {
                continue;
            }

            UpdateSlotVisual(slot, entry);

            if (gm != null && gm.IsItemPurchased(slot.ItemType))
            {
                slot.SetSoldOut();
            }

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

        var gm = GameManager.Instance;

        foreach (var slot in shopSlots)
        {
            if (gm != null && gm.IsItemPurchased(slot.ItemType))
            {
                slot.SetSoldOut();
                continue;
            }

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
    /// Called when a shop item is clicked. Opens the confirm popup.
    /// </summary>
    public void OnShopItemClicked(ShopItemType itemType)
    {
        if (_dataManager == null)
        {
            return;
        }

        ShopSlot clickedSlot = GetSlotByType(itemType);
        if (clickedSlot == null)
        {
            return;
        }

        _pendingSlot = clickedSlot;

        if (clickedSlot.ShopItemSO != null && confirmPopup != null)
        {
            confirmPopup.SetData(clickedSlot.ShopItemSO);
            confirmPopup.Open();
            return;
        }

        var entry = _dataManager.GetShopEntryByType(itemType);
        if (entry == null)
        {
            return;
        }

        if (confirmPopup != null)
        {
            confirmPopup.SetData(entry);
            confirmPopup.Open();
            return;
        }

        ExecutePurchase();
    }

    private void OnConfirmBuyClicked()
    {
        if (confirmPopup != null)
        {
            confirmPopup.Close();
        }

        ExecutePurchase();
    }

    private void OnConfirmCloseClicked()
    {
        _pendingSlot = null;

        if (confirmPopup != null)
        {
            confirmPopup.Close();
        }
    }

    private void ExecutePurchase()
    {
        if (_pendingSlot == null)
        {
            return;
        }

        var slot = _pendingSlot;
        _pendingSlot = null;

        if (slot.ShopItemSO != null)
        {
            if (PurchaseItem(slot.ShopItemSO))
            {
                slot.SetSoldOut();
            }
            return;
        }

        var gm = GameManager.Instance;
        if (gm == null || _dataManager == null)
        {
            return;
        }

        var entry = _dataManager.GetShopEntryByType(slot.ItemType);
        if (entry == null)
        {
            return;
        }

        if (!gm.SpendMoney(entry.Price))
        {
            return;
        }

        slot.SetSoldOut();

        gm.ShowMessage(GameMessages.ShopPurchaseBirthday(entry.Name));

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
    /// 嘗試購買指定 <see cref="ShopItemSO"/> 道具。成功時回傳 true。
    /// </summary>
    public bool PurchaseItem(ShopItemSO item)
    {
        var gm = GameManager.Instance;
        if (gm == null || item == null)
        {
            return false;
        }

        if (gm.Money < item.Price)
        {
            return false;
        }

        if (gm.IsItemPurchased(item.ItemType))
        {
            return false;
        }

        if (!gm.SpendMoney(item.Price))
        {
            return false;
        }

        var gallery = GalleryManager.Instance;
        var market = MarketManager.Instance;

        IPurchasable purchasable = item;
        purchasable.OnBuy(gm, gallery, market);

        gm.MarkItemPurchased(item.ItemType);

        RefreshShopDisplay();

        return true;
    }

    /// <summary>
    /// Animate the shop slot on purchase.
    /// </summary>
    private void PlayPurchaseAnimation(ShopSlot slot, TweenCallback onComplete)
    {
        onComplete?.Invoke();
    }
}
