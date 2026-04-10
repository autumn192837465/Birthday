using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public const string SoldOutText = "売り切れ";

    public ShopItemType ItemType;

    [Tooltip("綁定 ShopItemSO 資產，有值時走 PurchaseItem 流程（新道具系統）。")]
    public ShopItemSO ShopItemSO;

    public CostButton CostButton;
    public Image Icon;

    /// <summary>
    /// 購買完成後：隱藏價格、按鈕文字改為「売り切れ」並設為不可點擊。
    /// </summary>
    public void SetSoldOut()
    {
        if (CostButton == null)
        {
            return;
        }

        CostButton.ShowCost(false);
        CostButton.SetText(SoldOutText);
        CostButton.SetInteractable(false);
    }
}