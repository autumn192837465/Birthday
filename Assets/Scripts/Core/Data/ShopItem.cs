using UnityEngine;

/// <summary>
/// Types of items available in the shop.
/// </summary>
public enum ShopItemType
{
    HairOil,
}

/// <summary>
/// Represents a single shop item that can be purchased.
/// </summary>
[System.Serializable]
public class ShopItem
{
    public string ID;
    public ShopItemType ItemType;
    public string Name;
    public string Description;
    public Sprite Image;
    public int Price;
    public bool IsPurchased;

    public ShopItem(string id, ShopItemType itemType, string name, string description, Sprite image, int price)
    {
        ID = id;
        ItemType = itemType;
        Name = name;
        Description = description;
        Image = image;
        Price = price;
        IsPurchased = false;
    }
}
