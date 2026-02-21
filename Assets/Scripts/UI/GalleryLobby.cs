using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gallery lobby: entry view before entering the main gallery.
/// Contains an enter button and raises OnEnterClicked when the player wants to proceed.
/// </summary>
public class GalleryLobby : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button enterButton;

    /// <summary>Fired when the player clicks the enter button to proceed to the gallery.</summary>
    public event Action OnEnterClicked;

    private void Awake()
    {
        if (enterButton != null)
        {
            enterButton.onClick.AddListener(OnEnterButtonClicked);
        }
    }

    private void OnDestroy()
    {
        if (enterButton != null)
        {
            enterButton.onClick.RemoveAllListeners();
        }
    }

    private void OnEnterButtonClicked()
    {
        SetInteractable(false);
        OnEnterClicked?.Invoke();
    }

    /// <summary>Sets whether the enter button is clickable.</summary>
    public void SetInteractable(bool interactable)
    {
        if (enterButton != null)
        {
            enterButton.interactable = interactable;
        }
    }

    /// <summary>Resets the lobby to its initial state (button enabled).</summary>
    public void ResetState()
    {
        SetInteractable(true);
    }
}
