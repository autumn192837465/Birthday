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
    [SerializeField] private TextMeshProUGUI inventoryCountText;
    [SerializeField] private TextMeshProUGUI displayedCountText;
    [SerializeField] private TextMeshProUGUI rentedCountText;

    [Header("Painting Lists")]
    [SerializeField] private Transform inventoryListParent;
    [SerializeField] private Transform displayListParent;
    [SerializeField] private GameObject paintingEntryPrefab;

    [Header("Promotion View")]
    [SerializeField] private PromotionView promotionView;

    [SerializeField] private CreateArtAnimationView createArtAnimationView;

    private void Start()
    {
        if (galleryLobby != null)
            galleryLobby.OnEnterClicked += OnEnterGalleryClicked;
        
        if (galleryView != null)
        {
            galleryView.CreateArtClicked += OnCreateArtClickedAsync;
            galleryView.SettingsClicked += OnSettingsClicked;
        }

        if (promotionView != null)
        {
            promotionView.OnCloseClicked += HidePromotionView;
            promotionView.OnPromotionClicked += OnPromotePaintingByType;
            promotionView.Hide();
        }

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
        if (gm == null || GalleryManager.Instance == null || galleryView == null || ui == null)
            return;

        if (gm.HasBlocksCreation())
        {
            gm.ShowMessage("The Hermit says: rest today. No creating allowed!");
            return;
        }

        if (!gm.AddFatigue(gm.Settings.PaintingFatigueCost))
            return;

        gm.EnableInput(false);
        
        createArtAnimationView.Open();
        var (result, painting) = GalleryManager.Instance.CreatePainting();

        
        
        await UniTask.WaitForSeconds(2);
        createArtAnimationView.Close();

        await UniTask.WaitUntil(() => createArtAnimationView.IsClosed);
        
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

    private void OnSettingsClicked()
    {
        if (promotionView == null || GalleryManager.Instance == null) return;

        var completed = GalleryManager.Instance.GetCompletedPaintings();
        promotionView.Refresh(completed);
        promotionView.Show();
    }

    private void HidePromotionView()
    {
        if (promotionView != null)
            promotionView.Hide();
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
            gm.ShowMessage($"\"{painting.Title}\" 今晚已經推廣過了！");
            return;
        }

        if (!gm.SpendMoney(gm.Settings.PromotionCost))
            return;

        gallery.TogglePaintingPromotionByType(drawingType);
        gm.ShowMessage($"\"{painting.Title}\" 推廣成功！租售機率加倍！");
        promotionView.UpdatePromotionState(drawingType, true);
    }

    // =============================================
    // Display / Remove from wall
    // =============================================

    public void OnDisplayPaintingClicked(string paintingId)
    {
        if (GalleryManager.Instance == null) return;

        if (GalleryManager.Instance.DisplayPainting(paintingId))
        {
            RefreshUI();
        }
    }

    public void OnRemoveFromDisplayClicked(string paintingId)
    {
        if (GalleryManager.Instance == null) return;

        if (GalleryManager.Instance.RemoveFromDisplay(paintingId))
        {
            RefreshUI();
        }
    }

    // =============================================
    // UI Refresh
    // =============================================

    private void RefreshUI()
    {
        UpdateCostLabels();
        UpdateCounts();
        UpdateProgress();
        RefreshPaintingWall();
        RebuildPaintingLists();
    }

    private void RefreshPaintingWall()
    {
        if (galleryView == null || GalleryManager.Instance == null) return;

        var completed = GalleryManager.Instance.GetCompletedPaintingsForWall();
        galleryView.RefreshPaintingWall(completed);
    }

    private void UpdateCostLabels()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (galleryView != null)
        {
            galleryView.SetCreateArtCost(gm.Settings.PaintingFatigueCost.ToString());
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

        var settings = GameManager.Instance?.Settings;

        if (inventoryCountText != null)
            inventoryCountText.text = $"Inventory: {gallery.GetInventoryPaintings().Count}";

        if (displayedCountText != null)
        {
            int displayed = gallery.GetDisplayedPaintings().Count;
            int max = settings != null ? settings.MaxDisplaySlots : 0;
            displayedCountText.text = $"Displayed: {displayed}/{max}";
        }

        if (rentedCountText != null)
            rentedCountText.text = $"Rented: {gallery.GetRentedPaintings().Count}";
    }

    private void RebuildPaintingLists()
    {
        if (paintingEntryPrefab == null) return;

        RebuildList(inventoryListParent, GalleryManager.Instance.GetInventoryPaintings(), true);
        RebuildList(displayListParent, GalleryManager.Instance.GetDisplayedPaintings(), false);
    }

    private void RebuildList(Transform parent, List<Painting> paintings, bool isInventory)
    {
        if (parent == null) return;

        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);

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
                    image.sprite = painting.Image;
            }

            var button = entry.GetComponentInChildren<Button>();
            if (button != null)
            {
                string id = painting.ID;
                if (isInventory)
                    button.onClick.AddListener(() => OnDisplayPaintingClicked(id));
                else
                    button.onClick.AddListener(() => OnRemoveFromDisplayClicked(id));
            }
        }
    }

    private void OnDestroy()
    {
        if (galleryLobby != null) galleryLobby.OnEnterClicked -= OnEnterGalleryClicked;
        if (galleryView != null)
        {
            galleryView.CreateArtClicked -= OnCreateArtClickedAsync;
            galleryView.SettingsClicked -= OnSettingsClicked;
        }
        if (promotionView != null)
        {
            promotionView.OnCloseClicked -= HidePromotionView;
            promotionView.OnPromotionClicked -= OnPromotePaintingByType;
        }
    }
}
