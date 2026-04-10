/// <summary>
/// 商店道具類型。
/// ScriptableObject 道具請使用 <see cref="ShopItemSO"/>／<see cref="IPurchasable"/>，購買由 <see cref="ShopSystem.PurchaseItem"/> 處理。
/// 已購買狀態由 <see cref="GameManager.IsItemPurchased"/> 管理。
/// </summary>
public enum ShopItemType
{
    PremiumBrush,
    FluffyChair,
    LuckyCat,
    GallerySpotlight,
}
