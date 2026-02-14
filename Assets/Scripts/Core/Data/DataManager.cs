using UnityEngine;

/// <summary>
/// Central access point for game data (e.g. tarot sprites).
/// Must be initialized by GameManager before use.
/// </summary>
public class DataManager : MonoBehaviour
{
    [SerializeField] private TarotSpritesData tarotSpritesData;
    

    /// <summary>
    /// Initialize the DataManager with the given tarot sprites data.
    /// Called by GameManager on startup.
    /// </summary>
    /// <param name="tarotData">ScriptableObject containing tarot card sprites. Can be null.</param>
    public void Initialize()
    {
        tarotSpritesData.Initialize();
    }

    /// <summary>
    /// Get the sprite for the given tarot card type.
    /// Returns null if DataManager is not initialized or no sprite is assigned.
    /// </summary>
    public Sprite GetTarotSprite(TarotType type)
    {
        return tarotSpritesData.GetSprite(type);
    }
}
