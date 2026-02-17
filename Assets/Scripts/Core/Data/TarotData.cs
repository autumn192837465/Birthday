using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject 儲存每張塔羅牌的圖、名稱、敘述。
/// 建立方式：Assets > Create > Game > Tarot Data。
/// </summary>
[CreateAssetMenu(fileName = "TarotData", menuName = "Game/Tarot Data")]
public class TarotData : ScriptableObject
{
    [Serializable]
    public class TarotEntry
    {
        [Tooltip("塔羅牌類型。")]
        public TarotType Type;

        [Tooltip("顯示用圖片（牌面、圖示）。")]
        public Sprite Sprite;

        [Tooltip("顯示名稱（例如：女皇）。")]
        public string Name;

        [Tooltip("效果說明。")]
        [TextArea(2, 4)]
        public string Description;
    }

    [Header("塔羅牌資料")]
    [Tooltip("每種牌一筆，順序不拘。")]
    [SerializeField] private TarotEntry[] entries = new TarotEntry[0];

    private Dictionary<TarotType, TarotEntry> _lookup;

    /// <summary>取得該類型的圖片，找不到則回傳 null。</summary>
    public Sprite GetSprite(TarotType type)
    {
        return GetEntry(type)?.Sprite;
    }

    /// <summary>取得該類型的顯示名稱，找不到則回傳 type.ToString()。</summary>
    public string GetName(TarotType type)
    {
        var entry = GetEntry(type);
        return string.IsNullOrEmpty(entry?.Name) ? type.ToString() : entry.Name;
    }

    /// <summary>取得該類型的效果敘述，找不到則回傳空字串。</summary>
    public string GetDescription(TarotType type)
    {
        return GetEntry(type)?.Description ?? "";
    }

    public bool HasEntry(TarotType type)
    {
        return GetEntry(type) != null;
    }

    private TarotEntry GetEntry(TarotType type)
    {
        if (_lookup == null)
            return null;
        return _lookup.TryGetValue(type, out var entry) ? entry : null;
    }

    public void Initialize()
    {
        _lookup = new Dictionary<TarotType, TarotEntry>();
        if (entries == null) return;
        foreach (var entry in entries)
        {
            if (entry != null)
                _lookup[entry.Type] = entry;
        }
    }
}
