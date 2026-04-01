using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rolls and applies one morning event per day after sleep. Uses <see cref="MorningEventSO"/> polymorphism only.
/// </summary>
public class MorningEventManager : MonoBehaviour
{
    public static MorningEventManager Instance { get; private set; }

    [Tooltip("Pool of morning events (assign asset instances in the Inspector).")]
    [SerializeField] private List<MorningEventSO> availableMorningEvents;

    /// <summary>Today's active morning event asset, if any was rolled.</summary>
    public MorningEventSO CurrentActiveMorningEvent { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Clears the previous day's event state, rolls a new event (or defaults), and applies it.
    /// Call after <see cref="GameManager.AdvanceDay"/> during sleep.
    /// </summary>
    public void TriggerDailyEvent()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null)
        {
            return;
        }

        GalleryManager gallery = GalleryManager.Instance;

        if (CurrentActiveMorningEvent != null)
        {
            CurrentActiveMorningEvent.ResetEffect(gm, gallery);
        }

        if (availableMorningEvents == null || availableMorningEvents.Count == 0)
        {
            CurrentActiveMorningEvent = null;
            gm.ApplyDefaultMorningStamina();
            return;
        }
        
        var totalWeight = 0f;
        foreach (var evt in availableMorningEvents)        
        {
            totalWeight += evt.Weight;
        }

        var randomValue = Random.Range(0f, totalWeight);
        var cumulativeWeight = 0f;

        foreach (var morningEvent in availableMorningEvents)
        {
            cumulativeWeight += morningEvent.Weight;
            if (randomValue <= cumulativeWeight)
            {
                CurrentActiveMorningEvent = morningEvent;
                CurrentActiveMorningEvent.ApplyEffect(gm, gallery);
                break;
            }
        }
    }
}
