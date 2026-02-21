using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Reusable button with integrated label/cost text.
/// Assign Button + TMP in inspector; set text via code; subscribe to OnClick.
/// </summary>
public class CostButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI costText;

    public bool Interactable => button.interactable;

    /// <summary>Fired when the button is clicked.</summary>
    public event Action OnClick;

    private void Awake()
    {
        button.onClick.AddListener(OnButtonClicked);            
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();            
    }

    private void OnButtonClicked()
    {
        OnClick?.Invoke();
    }

    /// <summary>Set the full button label (e.g. "Create Art (Fatigue +5)" or "Promote ($100)").</summary>
    public void SetCostText(string text)
    {
        costText.text = text;         
    }


    /// <summary>Set whether the button is interactable.</summary>
    public void SetInteractable(bool interactable)
    {
        button.interactable = interactable;            
    }
}