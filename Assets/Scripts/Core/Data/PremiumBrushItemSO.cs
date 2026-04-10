using UnityEngine;

[CreateAssetMenu(fileName = "PremiumBrushItem", menuName = "Game/Shop Items/Premium Brush (最高級の筆セット)")]
public class PremiumBrushItemSO : ShopItemSO
{
    public override void OnBuy(GameManager gm, GalleryManager galleryManager, MarketManager marketManager)
    {
        if (galleryManager != null)
        {
            galleryManager.PaintingCostMultiplier *= 0.8f;
        }
    }
}
