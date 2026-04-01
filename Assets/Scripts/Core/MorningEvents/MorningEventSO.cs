using UnityEngine;

/// <summary>
/// Base ScriptableObject for a daily morning event. Use polymorphism (Strategy pattern) — no string or enum dispatch.
/// </summary>
public abstract class MorningEventSO : ScriptableObject
{
    [Header("Display (Japanese)")]
    [Tooltip("Event title shown in the morning popup.")]
    public string EventName;

    [Tooltip("Event body text shown in the morning popup.")]
    [TextArea(2, 6)]
    public string Description;
    
    public float Weight = 1f;

    /// <summary>
    /// Apply this morning's gameplay state (stamina, multipliers, etc.).
    /// </summary>
    public abstract void ApplyEffect(GameManager gameManager, GalleryManager galleryManager);

    /// <summary>
    /// Clear state from the previous day before a new morning event is rolled (e.g. reset painting cost multiplier).
    /// </summary>
    public virtual void ResetEffect(GameManager gameManager, GalleryManager galleryManager)
    {
    }
}
