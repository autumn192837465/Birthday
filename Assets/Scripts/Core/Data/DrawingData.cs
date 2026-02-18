using UnityEngine;

/// <summary>
/// ScriptableObject storing drawing/art entries: name, description, sprite, and base price.
/// Create via Assets > Create > Game > Drawing Data.
/// </summary>
[CreateAssetMenu(fileName = "DrawingData", menuName = "Game/Drawing Data")]
public class DrawingData : ScriptableObject
{
    [System.Serializable]
    public class DrawingEntry
    {
        public DrawingType Type;
        public string Name;
        [TextArea(2, 4)]
        public string Description;
        public Sprite Sprite;
        public int Price;
    }

    [Header("Drawing Data")]
    [SerializeField] private DrawingEntry[] entries;

    private DrawingEntry[] _entries;

    public int Count => _entries != null ? _entries.Length : 0;

    public void Initialize()
    {
        _entries = entries != null ? entries : new DrawingEntry[0];
    }

    /// <summary>Get entry by index. Returns null if out of range.</summary>
    public DrawingEntry GetEntry(int index)
    {
        if (_entries == null || index < 0 || index >= _entries.Length)
            return null;
        return _entries[index];
    }

    /// <summary>Get a random entry, or null if no entries.</summary>
    public DrawingEntry GetRandomEntry()
    {
        if (_entries == null || _entries.Length == 0)
            return null;
        return _entries[Random.Range(0, _entries.Length)];
    }

    public Sprite GetSprite(int index)
    {
        return GetEntry(index)?.Sprite;
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
}
