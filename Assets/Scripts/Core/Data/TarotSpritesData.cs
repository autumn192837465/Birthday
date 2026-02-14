using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that stores sprites for each tarot card type.
/// Create via Assets > Create > Game > Tarot Sprites Data.
/// Assign a sprite for each TarotType; missing entries return null.
/// </summary>
[CreateAssetMenu(fileName = "TarotSpritesData", menuName = "Game/Tarot Sprites Data")]
public class TarotSpritesData : ScriptableObject
{
    [Serializable]
    public class TarotSpriteEntry
    {
        [Tooltip("The tarot card type.")]
        public TarotType Type;

        [Tooltip("Sprite used for this card (e.g. card face, icon).")]
        public Sprite Sprite;
    }

    [Header("Tarot Card Sprites")]
    [Tooltip("One entry per tarot card type. Order does not matter.")]
    [SerializeField] private TarotSpriteEntry[] entries = new TarotSpriteEntry[0];

    private Dictionary<TarotType, Sprite> spriteLookup;
    
    /// <summary>
    /// Get the sprite for the given tarot type. Returns null if not found.
    /// </summary>
    public Sprite GetSprite(TarotType type)
    {
        if (entries == null) return null;

        for (int i = 0; i < entries.Length; i++)
        {
            if (entries[i].Type == type)
            {
                return entries[i].Sprite;
            }
        }

        return null;
    }

    /// <summary>
    /// Check if a sprite is assigned for the given type.
    /// </summary>
    public bool HasSprite(TarotType type)
    {
        return GetSprite(type) != null;
    }
    
    public void Initialize()
    {
        spriteLookup = new Dictionary<TarotType, Sprite>();
        foreach (TarotSpriteEntry entry in entries)
        {
            spriteLookup[entry.Type] = entry.Sprite;
        }
    }
}
