using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

/// <summary>
/// Gallery panel UI controller.
/// Entry flow: Lobby View -> (click enter) -> Gallery View (create art, settings, etc.)
/// Settings panel allows per-painting promotion management.
/// </summary>
public class GallerySystem : PanelBase
{
    [Header("Lobby View (Entry)")]
    [SerializeField] private GalleryLobby galleryLobby;

    [Header("Gallery View (Main)")]
    [SerializeField] private GalleryView galleryView;

    [Header("Info Display")]
    [SerializeField] private TextMeshProUGUI displayedCountText;
    [SerializeField] private TextMeshProUGUI rentedCountText;

    [Header("Painting Lists")]
    [SerializeField] private Transform displayListParent;
    [SerializeField] private GameObject paintingEntryPrefab;

    [Header("Promotion View")]
    [SerializeField] private PromotionView promotionView;

    [SerializeField] private CreateArtAnimationView createArtAnimationView;

    protected override void Awake()
    {
        base.Awake();
        
        galleryLobby.OnEnterClicked += OnEnterGalleryClicked;
        
        galleryView.CreateArtClicked += OnCreateArtClickedAsync;
        galleryView.PromotionClicked += OnPromotionClicked;

        promotionView.OnCloseClicked += HidePromotionView;
        promotionView.OnPromotionClicked += OnPromotePaintingByType;
        promotionView.Hide();

        UpdateCostLabels();
        ShowLobbyOnly();
    }

    protected override void OnPanelShow()
    {
        ResetToLobbyState();
    }

    private void ResetToLobbyState()
    {
        if (galleryLobby != null)
            galleryLobby.ResetState();
        ShowLobbyOnly();
    }

    private void ShowLobbyOnly()
    {
        if (galleryLobby != null) galleryLobby.gameObject.SetActive(true);
        if (galleryView != null) galleryView.gameObject.SetActive(false);
        if (promotionView != null) promotionView.Hide();
    }

    private void ShowGalleryOnly()
    {
        if (galleryLobby != null) galleryLobby.gameObject.SetActive(false);
        if (galleryView != null) galleryView.gameObject.SetActive(true);
        if (promotionView != null) promotionView.Hide();
    }

    private async void OnEnterGalleryClicked()
    {
        var gm = GameManager.Instance;
        var ui = UIManager.Instance;
        if (gm == null || ui == null) return;

        gm.EnableInput(false);
        await ui.FadeOutAsync();
        ShowGalleryOnly();
        RefreshUI();
        await ui.FadeInAsync();
        gm.EnableInput(true);
    }

    // =============================================
    // Create Art (progress-based)
    // =============================================

    private async void OnCreateArtClickedAsync()
    {
        var gm = GameManager.Instance;
        var ui = UIManager.Instance;

        if (gm.HasBlocksCreation())
        {
            gm.ShowMessage(GameMessages.HermitBlocksCreation);
            return;
        }

        if (!gm.AddFatigue(gm.Settings.PaintingFatigueCost))
        {
            return;
        }

        gm.EnableInput(false);
        
        createArtAnimationView.Open();
        var (result, painting) = GalleryManager.Instance.CreatePainting();
        
        await UniTask.WaitForSeconds(2);
        createArtAnimationView.Close();
        await UniTask.WaitUntil(() => createArtAnimationView.IsClosed);

        if (result == CreateResult.InProgress)
        {
            gm.ShowMessage(GameMessages.PaintingInProgress(painting.Title, painting.Progress));
        }
            
        gm.EnableInput(true);
        
        if (result == CreateResult.Completed)
        {
            galleryView.RevealPainting(painting.DrawingType, painting.Image);
            galleryView.ShowCompletionPopup(painting);
        }
        RefreshUI();
    }

    // =============================================
    // Promotion View
    // =============================================

    private void OnPromotionClicked()
    {
        var completed = GalleryManager.Instance.GetCompletedPaintings();
        promotionView.Refresh(completed);
        promotionView.Show();
        UIManager.Instance.HideHudView();
    }

    private void HidePromotionView()
    {
        promotionView.Hide();
        UIManager.Instance.ShowHudView();
    }

    private void OnPromotePaintingByType(DrawingType drawingType)
    {
        var gm = GameManager.Instance;
        var gallery = GalleryManager.Instance;
        if (gm == null || gallery == null || promotionView == null) return;

        var painting = gallery.GetPaintingByType(drawingType);
        if (painting == null) return;

        if (painting.IsPromoted)
        {
            gm.ShowMessage(GameMessages.PromotionAlreadyActive(painting.Title, painting.PromotionDaysLeft));
            return;
        }

        if (!gm.SpendMoney(gm.Settings.PromotionCost))
        {
            return;
        }

        gallery.TogglePaintingPromotionByType(drawingType);
        gm.ShowMessage(GameMessages.PromotionSuccess(painting.Title));
        promotionView.UpdatePromotionState(drawingType, true);
    }

    // =============================================
    // UI Refresh
    // =============================================

    private void RefreshUI()
    {
        UpdateCostLabels();
        UpdateCounts();
        RefreshPaintingWall();
        UpdateProgress();
        RebuildPaintingLists();
    }

    private void RefreshPaintingWall()
    {
        if (galleryView == null || GalleryManager.Instance == null)
        {
            return;
        }

        
        galleryView.RefreshPaintingWall();
    }

    private void UpdateCostLabels()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (galleryView != null)
        {
            galleryView.SetCreateArtCost(gm.Settings.PaintingFatigueCost.ToString());

            bool allCompleted = GalleryManager.Instance != null && GalleryManager.Instance.AreAllPaintingsCompleted();
            galleryView.SetCreateArtButtonVisible(!allCompleted);
        }
    }

    private void UpdateProgress()
    {
        if (galleryView == null || GalleryManager.Instance == null) return;

        var inProgress = GalleryManager.Instance.GetInProgressPainting();
        galleryView.UpdateProgress(inProgress);
    }

    private void UpdateCounts()
    {
        var gallery = GalleryManager.Instance;
        if (gallery == null) return;

        if (displayedCountText != null)
        {
            int displayed = gallery.GetDisplayedPaintings().Count;
            displayedCountText.text = $"Displayed: {displayed}";
        }

        if (rentedCountText != null)
        {
            rentedCountText.text = $"Rented: {gallery.GetRentedPaintings().Count}";
        }
    }

    private void RebuildPaintingLists()
    {
        if (paintingEntryPrefab == null) return;

        RebuildList(displayListParent, GalleryManager.Instance.GetDisplayedPaintings());
    }

    private void RebuildList(Transform parent, List<Painting> paintings)
    {
        if (parent == null) return;

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }

        foreach (var painting in paintings)
        {
            var entry = Instantiate(paintingEntryPrefab, parent);
            var text = entry.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = $"{painting.Title} (${painting.BasePrice})";
            }

            if (painting.Image != null)
            {
                var image = entry.GetComponentInChildren<Image>();
                if (image != null)
                {
                    image.sprite = painting.Image;
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (galleryLobby != null) galleryLobby.OnEnterClicked -= OnEnterGalleryClicked;
        if (galleryView != null)
        {
            galleryView.CreateArtClicked -= OnCreateArtClickedAsync;
            galleryView.PromotionClicked -= OnPromotionClicked;
        }
        if (promotionView != null)
        {
            promotionView.OnCloseClicked -= HidePromotionView;
            promotionView.OnPromotionClicked -= OnPromotePaintingByType;
        }
    }
}
