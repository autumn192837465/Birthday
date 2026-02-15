using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Base class for all UI panels.
/// Provides a back button with configurable action (set by UIManager),
/// and optional DOTween show/hide animations.
/// </summary>
public class PanelBase : MonoBehaviour
{
    [Header("Panel Base")]
    [SerializeField] private Button backButton;
    

    private Action _onBackClicked;

    protected virtual void Awake()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }
    }

    private void OnDestroy()
    {
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
        }
    }

    private void OnBackButtonClicked()
    {
        _onBackClicked?.Invoke();
    }

    /// <summary>
    /// Set the action invoked when the back button is pressed (called by UIManager).
    /// </summary>
    public void SetBackAction(Action action)
    {
        _onBackClicked = action;
    }

    /// <summary>
    /// Show this panel with optional pop-up animation.
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);

        // Todo: Add animation

        OnPanelShow();
    }

    /// <summary>
    /// Hide this panel with optional scale-down animation.
    /// </summary>
    public void Hide()
    {
        // Todo: Add animation
        gameObject.SetActive(false);
        OnPanelHide();
    }

    /// <summary>
    /// Called after the panel becomes visible. Override for custom logic.
    /// </summary>
    protected virtual void OnPanelShow() { }

    /// <summary>
    /// Called after the panel becomes hidden. Override for custom logic.
    /// </summary>
    protected virtual void OnPanelHide() { }
}
