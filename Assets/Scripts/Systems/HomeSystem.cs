using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// Home is the player's safe haven. Sleeping is always FREE.
/// Advances the day, applies morning events (stamina), and plays a fade transition.
/// </summary>
public class HomeSystem : PanelBase
{
    [SerializeField] private Button sleepButton;
    
    protected override void Awake()
    {
        base.Awake();
        sleepButton.onClick.AddListener(OnSleepButtonClicked);
    }

    /// <summary>
    /// Called by the "Sleep / Rest" button OnClick.
    /// </summary>
    public void OnSleepButtonClicked()
    {
        var gm = GameManager.Instance;
        _ = gm.Sleep();
    }
    
    
    
}
