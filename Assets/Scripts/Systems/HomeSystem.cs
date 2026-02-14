using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// Home is the player's safe haven. Sleeping is always FREE.
/// Resets fatigue, advances the day, and plays a fade transition.
/// </summary>
public class HomeSystem : MonoBehaviour
{
    [SerializeField] private Button sleepButton;

    public void Initialize()
    {
        sleepButton.onClick.AddListener(OnSleepButtonClicked);
    }
    
    /// <summary>
    /// Called by the "Sleep / Rest" button OnClick.
    /// </summary>
    public void OnSleepButtonClicked()
    {
        var gm = GameManager.Instance;
        var ui = UIManager.Instance;
        if (gm == null || ui == null) return;

        // Play fade transition, then reset fatigue and advance day
        Sequence fadeSeq = ui.PlayFadeTransition(0.5f);

        fadeSeq.OnStepComplete(() =>
        {
            // This fires after the fade-out is complete (mid-transition)
        });

        // Insert logic at the midpoint of the fade (after fade-out, before fade-in)
        fadeSeq.InsertCallback(0.8f, () =>
        {
            gm.ResetFatigue();
            gm.AdvanceDay();
        });

        fadeSeq.OnComplete(() =>
        {
            gm.ShowMessage("Good morning! Energy fully recovered.");
        });
    }
}
