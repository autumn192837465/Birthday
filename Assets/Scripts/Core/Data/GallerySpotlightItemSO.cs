using UnityEngine;

[CreateAssetMenu(fileName = "GallerySpotlightItem", menuName = "Game/Shop Items/Gallery Spotlight (ギャラリー専用スポットライト)")]
public class GallerySpotlightItemSO : ShopItemSO
{
    public override void OnBuy(GameManager gm, GalleryManager galleryManager, MarketManager marketManager)
    {
        if (marketManager != null)
        {
            marketManager.IncomeMultiplier *= 1.5f;
        }
    }
}
