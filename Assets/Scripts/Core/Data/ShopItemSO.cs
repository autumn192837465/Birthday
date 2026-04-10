using UnityEngine;

/// <summary>
/// 商店道具基底（ScriptableObject）。子類別覆寫 <see cref="OnBuy"/> 以定義購買效果。
/// 已購買狀態由 <see cref="GameManager.IsItemPurchased"/> 管理，不存在 SO 上。
/// </summary>
public abstract class ShopItemSO : ScriptableObject, IPurchasable
{
    [Header("Display")]
    public string ItemName;

    [TextArea(2, 4)]
    public string Description;

    public Sprite Sprite;

    [Header("Commerce")]
    public int Price;

    /// <summary>對應的 <see cref="ShopItemType"/>，用於追蹤已購買狀態。</summary>
    public ShopItemType ItemType;

    public abstract void OnBuy(GameManager gm, GalleryManager galleryManager, MarketManager marketManager);
}
