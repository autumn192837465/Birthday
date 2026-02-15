using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Simple, reusable script attached to each Arcade Machine sprite in the Hall.
/// Detects clicks and notifies GameSystem to launch the associated mini-game.
/// Supports both world-space clicks (OnMouseDown) and UI buttons.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ArcadeMachine : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Which mini-game this machine launches.")]
    [SerializeField] private GameType gameType;

    [Header("Optional UI Button")]
    [Tooltip("If assigned, this button will also trigger the machine (for UI-based interaction).")]
    [SerializeField] private Button interactButton;

    [Header("Visual Feedback")]
    [Tooltip("Punch scale amount on click.")]
    [SerializeField] private float punchScale = 0.15f;

    private GameSystem _gameSystem;

    private void Start()
    {
        // Cache the GameSystem reference
        _gameSystem = GetComponentInParent<GameSystem>();

        if (_gameSystem == null)
        {
            _gameSystem = FindFirstObjectByType<GameSystem>();
        }

        if (_gameSystem == null)
        {
            Debug.LogWarning($"[ArcadeMachine] No GameSystem found for machine: {gameObject.name}");
        }

        // Wire up optional UI button
        if (interactButton != null)
        {
            interactButton.onClick.AddListener(OnMachineClicked);
        }
    }

    private void OnDestroy()
    {
        if (interactButton != null)
        {
            interactButton.onClick.RemoveAllListeners();
        }
    }

    /// <summary>
    /// World-space click detection for 2D sprites.
    /// Requires a Collider2D on this GameObject.
    /// </summary>
    private void OnMouseDown()
    {
        OnMachineClicked();
    }

    /// <summary>
    /// Handle machine interaction: play feedback and launch the game.
    /// </summary>
    private void OnMachineClicked()
    {
        if (_gameSystem == null || gameType == GameType.None) return;

        // Visual feedback: punch scale
        transform.DOPunchScale(Vector3.one * punchScale, 0.3f, 6);

        // Launch the game via GameSystem
        _gameSystem.LaunchGame(gameType);
    }
}
