using System;
using System.Collections.Generic;
using System.Linq;
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
        public CanvasGroup CanvasGroup;
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
        {
            progressBar.value = inProgress.Progress / 100f;
        }
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
    /// 同步牆面：已完成格 alpha=1；進行中格 alpha=Progress/100；其餘 alpha=0。
    /// 並依 <see cref="GalleryManager.GetCompletedPaintingsForWall"/> 呼叫 <see cref="RevealPainting"/>。
    /// </summary>
    public void RefreshPaintingWall()
    {
        var gallery = GalleryManager.Instance;
        if (gallery == null || paintingSlots == null)
        {
            return;
        }

        var paintings = gallery.GetPaintings();
        List<Painting> completedPaintings = paintings.Values.Where(p => p.State != PaintingState.InProgress).ToList();
        if (promotionButton != null)
        {
            promotionButton.gameObject.SetActive(completedPaintings.Count > 0);
        }

        var completedForWall = gallery.GetCompletedPaintingsForWall();
        var completedTypes = new HashSet<DrawingType>();
        foreach (var (drawingType, _) in completedForWall)
        {
            completedTypes.Add(drawingType);
        }

        Painting inProgress = gallery.GetInProgressPainting();

        foreach (var slot in paintingSlots)
        {
            if (slot == null)
            {
                continue;
            }

            bool isCompleted = completedTypes.Contains(slot.Id);
            bool isInProgress = inProgress != null && inProgress.DrawingType == slot.Id;

            if (slot.CanvasGroup != null)
            {
                if (isCompleted)
                {
                    slot.CanvasGroup.alpha = 1f;
                }
                else if (isInProgress)
                {
                    slot.CanvasGroup.alpha = Mathf.Clamp01(inProgress.Progress / 100f);
                }
                else
                {
                    slot.CanvasGroup.alpha = 0f;
                }
            }

            if (slot.Paint != null)
            {
                slot.Paint.SetActive(isCompleted || isInProgress);
            }
        }

        foreach (var (drawingType, sprite) in completedForWall)
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
