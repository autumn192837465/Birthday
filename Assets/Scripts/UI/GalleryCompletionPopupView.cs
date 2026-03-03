using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

/// <summary>
/// Completion popup UI: shows finished painting image, title, price, and close button.
/// Attach to the popup root GameObject; show/hide via Show() and Hide().
/// </summary>
public class GalleryCompletionPopupView : AnimatorBase
{
    [SerializeField] private Image completionImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI completionPriceText;
    [SerializeField] private Button closeButton;

    /// <summary>
    /// Fired when the user clicks the close button (after the popup is hidden).
    /// </summary>
    public event Action CloseClicked;

    protected override void Awake()
    {
        base.Awake();
        closeButton.onClick.AddListener(OnCloseButtonClicked);
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveListener(OnCloseButtonClicked);
    }

    private void OnCloseButtonClicked()
    {
        CloseClicked?.Invoke();
    }

    public void SetData(Painting painting)
    {
        if (painting == null)
        {
            return;
        }
        
        completionImage.sprite = painting.Image;
        completionImage.SetNativeSize();
        titleText.text = painting.Title;
        descriptionText.text = painting.Description;

        if (completionPriceText != null)
        {
            completionPriceText.text = $"Value: ${painting.BasePrice}";
        }
    }
}
