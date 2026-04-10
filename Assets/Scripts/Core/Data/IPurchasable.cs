/// <summary>
/// 商店道具購買時觸發的行為，由具體 ScriptableObject 子類別以多型實作。
/// </summary>
public interface IPurchasable
{
    void OnBuy(GameManager gm, GalleryManager galleryManager, MarketManager marketManager);
}
