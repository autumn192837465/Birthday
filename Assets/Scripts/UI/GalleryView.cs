using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gallery main view: create art button, settings button, progress display,
/// painting display wall, and a completion popup for finished paintings.
/// </summary>
public class GalleryView : MonoBehaviour
{
    [System.Serializable]
    public class PaintingInfo
    {
        public DrawingType Id;
        public Image Image;
    }

    [Header("Actions")]
    [SerializeField] private CostButton createArtButton;
    [SerializeField] private Button promotionButton;

    [Header("Progress Display")]
    [SerializeField] private GameObject progressPanel;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Slider progressBar;

    [Header("Painting Display Wall")]
    [SerializeField] private PaintingInfo[] paintingSlots;

    [Header("Completion Popup")]
    [SerializeField] private GameObject completionPopup;
    [SerializeField] private Image completionImage;
    [SerializeField] private TextMeshProUGUI completionTitleText;
    [SerializeField] private TextMeshProUGUI completionPriceText;
    [SerializeField] private Button completionCloseButton;
    
    [Header("Create Animation")]
    [SerializeField] private CreateArtAnimationView createArtAnimationView;

    public event Action CreateArtClicked;
    public event Action PromotionClicked;

    private void Awake()
    {
        createArtButton.OnClick += OnCreateArtClicked;
        
        promotionButton.onClick.AddListener(OnPromotionButtonClicked);
            
        
        if (completionCloseButton != null)
            completionCloseButton.onClick.AddListener(HideCompletionPopup);

        if (completionPopup != null)
            completionPopup.SetActive(false);

        promotionButton.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        createArtButton.OnClick -= OnCreateArtClicked;
        
        promotionButton.onClick.RemoveListener(OnPromotionButtonClicked);
            
        if (completionCloseButton != null)
            completionCloseButton.onClick.RemoveListener(HideCompletionPopup);
    }

    private void OnCreateArtClicked() => CreateArtClicked?.Invoke();
    private void OnPromotionButtonClicked() => PromotionClicked?.Invoke();

    public void SetCreateArtCost(string text)
    {
        if (createArtButton != null)
            createArtButton.SetCostText(text);
    }

    public void UpdateProgress(Painting inProgress)
    {
        if (progressPanel == null) return;

        if (inProgress == null)
        {
            progressPanel.SetActive(false);
            return;
        }

        progressPanel.SetActive(true);
        if (progressText != null)
            progressText.text = $"\"{inProgress.Title}\" - {inProgress.Progress:F0}%";
        if (progressBar != null)
            progressBar.value = inProgress.Progress / 100f;
    }

    public void ShowCompletionPopup(Painting painting)
    {
        if (completionPopup == null)
        {
            return;
        }

        completionPopup.SetActive(true);

        if (completionImage != null)
        {
            completionImage.sprite = painting.Image;
        }
        if (completionTitleText != null)
        {
            completionTitleText.text = painting.Title;
        }
        if (completionPriceText != null)
        {
            completionPriceText.text = $"Value: ${painting.BasePrice}";
        }
    }

    public void HideCompletionPopup()
    {
        if (completionPopup != null)
            completionPopup.SetActive(false);
    }

    /// <summary>
    /// Activate and set the sprite for the painting slot matching the given DrawingType.
    /// </summary>
    public void RevealPainting(DrawingType drawingType, Sprite sprite)
    {
        if (paintingSlots == null) return;

        foreach (var slot in paintingSlots)
        {
            if (slot.Id == drawingType && slot.Image != null)
            {
                slot.Image.gameObject.SetActive(true);
                slot.Image.sprite = sprite;
                return;
            }
        }
    }

    /// <summary>
    /// Refresh the painting wall: hide all slots, then show each completed painting by DrawingType.
    /// Pass the result of GalleryManager.GetCompletedPaintingsForWall().
    /// </summary>
    public void RefreshPaintingWall(IReadOnlyList<(DrawingType drawingType, Sprite sprite)> completed)
    {
        if (paintingSlots == null) return;

        int paintingCount = completed?.Count ?? 0;
        promotionButton.gameObject.SetActive(paintingCount > 0);

        foreach (var slot in paintingSlots)
        {
            if (slot.Image != null)
            {
                slot.Image.gameObject.SetActive(false);
            }
        }

        if (completed == null)
        {
            return;
        }

        foreach (var (drawingType, sprite) in completed)
        {
            RevealPainting(drawingType, sprite);
        }
    }
}
