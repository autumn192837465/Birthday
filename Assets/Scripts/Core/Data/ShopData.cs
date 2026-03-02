using UnityEngine;

/// <summary>
/// ScriptableObject storing shop item entries: name, description, sprite, and price.
/// Create via Assets > Create > Game > Shop Data.
/// </summary>
[CreateAssetMenu(fileName = "ShopData", menuName = "Game/Shop Data")]
public class ShopData : ScriptableObject
{
    [System.Serializable]
    public class ShopEntry
    {
        public ShopItemType Type;
        public string Name;
        [TextArea(2, 4)]
        public string Description;
        public Sprite Sprite;
        public int Price;
    }

    [Header("Shop Items")]
    [SerializeField] private ShopEntry[] entries;

    private ShopEntry[] _entries;

    public int Count => _entries != null ? _entries.Length : 0;

    public void Initialize()
    {
        _entries = entries != null ? entries : new ShopEntry[0];
    }

    /// <summary>Get entry by index. Returns null if out of range.</summary>
    public ShopEntry GetEntry(int index)
    {
        if (_entries == null || index < 0 || index >= _entries.Length)
        {
            return null;
        }
        return _entries[index];
    }

    /// <summary>Get entry by ShopItemType. Returns null if not found.</summary>
    public ShopEntry GetEntryByType(ShopItemType type)
    {
        if (_entries == null)
        {
            return null;
        }
        foreach (var entry in _entries)
        {
            if (entry.Type == type)
            {
                return entry;
            }
        }
        return null;
    }

    public Sprite GetSprite(int index)
    {
        return GetEntry(index)?.Sprite;
    }

    public Sprite GetSpriteByType(ShopItemType type)
    {
        return GetEntryByType(type)?.Sprite;
    }

    public string GetName(int index)
    {
        var entry = GetEntry(index);
        return string.IsNullOrEmpty(entry?.Name) ? "" : entry.Name;
    }

    public string GetDescription(int index)
    {
        return GetEntry(index)?.Description ?? "";
    }

    public int GetPrice(int index)
    {
        return GetEntry(index)?.Price ?? 0;
    }

    public int GetPriceByType(ShopItemType type)
    {
        return GetEntryByType(type)?.Price ?? 0;
    }

    /// <summary>取得所有商品中的最低價格。若無商品則回傳 int.MaxValue。</summary>
    public int GetMinPrice()
    {
        if (_entries == null || _entries.Length == 0)
        {
            return int.MaxValue;
        }

        int minPrice = int.MaxValue;
        foreach (var entry in _entries)
        {
            if (entry.Price < minPrice)
            {
                minPrice = entry.Price;
            }
        }
        return minPrice;
    }
}
