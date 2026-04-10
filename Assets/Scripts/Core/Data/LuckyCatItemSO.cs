using UnityEngine;

[CreateAssetMenu(fileName = "LuckyCatItem", menuName = "Game/Shop Items/Lucky Cat (ピカピカ招き猫)")]
public class LuckyCatItemSO : ShopItemSO
{
    public override void OnBuy(GameManager gm, GalleryManager galleryManager, MarketManager marketManager)
    {
        if (marketManager != null)
        {
            marketManager.BaseRentChance += 0.05f;
            marketManager.BaseSellChance += 0.05f;
        }
    }
}
