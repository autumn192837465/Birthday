using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

/// <summary>
/// Gallery main view: create art button, settings button, progress display,
/// painting display wall, and a completion popup for finished paintings.
/// </summary>
public class GalleryView : MonoBehaviour
{
    [Serializable]
    public class PaintingInfo
    {
        public DrawingType Id;
        public GameObject Paint;
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

    [FormerlySerializedAs("completionPopup")]
    [Header("Completion Popup")]
    [SerializeField] private GalleryCompletionPopupView galleryCompletionPopup;
    
    public event Action CreateArtClicked;
    public event Action PromotionClicked;

    private void Awake()
    {
        createArtButton.OnClick += OnCreateArtClicked;
        
        promotionButton.onClick.AddListener(OnPromotionButtonClicked);
            
        galleryCompletionPopup.CloseClicked += CloseCompletionPopup;
        promotionButton.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        createArtButton.OnClick -= OnCreateArtClicked;
        
        galleryCompletionPopup.CloseClicked -= CloseCompletionPopup;
        promotionButton.onClick.RemoveListener(OnPromotionButtonClicked);
    }

    private void OnCreateArtClicked() => CreateArtClicked?.Invoke();
    private void OnPromotionButtonClicked() => PromotionClicked?.Invoke();

    public void SetCreateArtCost(string text)
    {
        if (createArtButton != null)
        {
            createArtButton.SetCostText(text);
        }
    }

    public void SetCreateArtButtonVisible(bool visible)
    {
        if (createArtButton != null)
        {
            createArtButton.gameObject.SetActive(visible);
        }
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
        galleryCompletionPopup.SetData(painting);
        galleryCompletionPopup.Open();
        UIManager.Instance.HideHudView();
    }
    

    /// <summary>
    /// Activate and set the sprite for the painting slot matching the given DrawingType.
    /// </summary>
    public void RevealPainting(DrawingType drawingType, Sprite sprite)
    {
        if (paintingSlots == null) return;

        foreach (var slot in paintingSlots)
        {
            if (slot.Id == drawingType)
            {
                slot.Paint.gameObject.SetActive(true);
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
            slot.Paint.gameObject.SetActive(false);
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
    
    public void CloseCompletionPopup()
    {
        galleryCompletionPopup.Close();
        UIManager.Instance.ShowHudView();
    }
}
