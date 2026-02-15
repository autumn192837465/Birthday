using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Arcade Hub Controller.
/// Manages the state of "In Hall" vs "Playing a specific mini-game".
/// When an ArcadeMachine is clicked, this system transitions to the corresponding game panel.
/// Uses DOTween for smooth pop-up transitions.
/// </summary>
public class GameSystem : PanelBase
{
    [Header("Hall View")]
    [Tooltip("The root GameObject of the Arcade Hall (contains all ArcadeMachine sprites).")]
    [SerializeField] private GameObject hallView;

    [Header("Mini-Game Panels")]
    [SerializeField] private GamePanelEntry[] gamePanels;

    /// <summary>
    /// Maps GameType to a specific mini-game panel container.
    /// </summary>
    [Serializable]
    public struct GamePanelEntry
    {
        public GameType Type;
        public RectTransform PanelRoot;
    }

    private Dictionary<GameType, RectTransform> _panelLookup;
    private GameType _currentGameType = GameType.None;

    protected override void Awake()
    {
        base.Awake();
        BuildPanelLookup();
        HideAllGamePanels();
    }

    /// <summary>
    /// Build the dictionary for quick GameType -> panel lookup.
    /// </summary>
    private void BuildPanelLookup()
    {
        _panelLookup = new Dictionary<GameType, RectTransform>();

        if (gamePanels == null) return;

        foreach (var entry in gamePanels)
        {
            if (entry.Type != GameType.None && entry.PanelRoot != null)
            {
                _panelLookup[entry.Type] = entry.PanelRoot;
            }
        }
    }

    /// <summary>
    /// Hide all mini-game panels at startup so only the Hall is visible.
    /// </summary>
    private void HideAllGamePanels()
    {
        if (_panelLookup == null) return;

        foreach (var kvp in _panelLookup)
        {
            kvp.Value.gameObject.SetActive(false);
        }
    }

    // =============================================
    // Public API (called by ArcadeMachine)
    // =============================================

    /// <summary>
    /// Launch a specific mini-game. Hides the hall and shows the game panel with a pop-up effect.
    /// Called by ArcadeMachine when a machine is clicked.
    /// </summary>
    public void LaunchGame(GameType gameType)
    {
        if (gameType == GameType.None) return;

        if (!_panelLookup.TryGetValue(gameType, out RectTransform panel))
        {
            Debug.LogWarning($"[GameSystem] No panel registered for GameType: {gameType}");
            return;
        }

        _currentGameType = gameType;

        // Hide the arcade hall
        if (hallView != null)
            hallView.SetActive(false);

        // Show the game panel with a pop-up animation
        panel.gameObject.SetActive(true);
        panel.localScale = Vector3.zero;
        panel.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack);
    }

    /// <summary>
    /// Return from the current mini-game back to the Arcade Hall.
    /// Called by back buttons inside each mini-game panel.
    /// </summary>
    public void ReturnToHall()
    {
        if (_currentGameType == GameType.None) return;

        if (_panelLookup.TryGetValue(_currentGameType, out RectTransform panel))
        {
            // Scale-down animation then hide
            panel.DOScale(Vector3.zero, 0.2f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    panel.gameObject.SetActive(false);

                    // Show the arcade hall again
                    if (hallView != null)
                        hallView.SetActive(true);

                    _currentGameType = GameType.None;
                });
        }
        else
        {
            // Fallback: just show the hall
            if (hallView != null)
                hallView.SetActive(true);

            _currentGameType = GameType.None;
        }
    }
}
