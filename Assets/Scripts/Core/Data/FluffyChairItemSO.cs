using UnityEngine;

[CreateAssetMenu(fileName = "FluffyChairItem", menuName = "Game/Shop Items/Fluffy Chair")]
public class FluffyChairItemSO : ShopItemSO
{
    public override void OnBuy(GameManager gm, GalleryManager galleryManager, MarketManager marketManager)
    {
        if (gm != null)
        {
            gm.MaxFatigue += 20;
            gm.RefreshStaminaAfterMaxFatigueIncrease(20);
        }
    }
}
