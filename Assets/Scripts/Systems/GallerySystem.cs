using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gallery panel UI controller.
/// Provides buttons for creating art, displaying/removing paintings, and buying promotions.
/// </summary>
public class GallerySystem : PanelBase
{
    [Header("Create Art")]
    [SerializeField] private Button createArtButton;
    [SerializeField] private TextMeshProUGUI createCostText;

    [Header("Promotion")]
    [SerializeField] private Button promotionButton;
    [SerializeField] private TextMeshProUGUI promotionText;

    [Header("Info Display")]
    [SerializeField] private TextMeshProUGUI inventoryCountText;
    [SerializeField] private TextMeshProUGUI displayedCountText;
    [SerializeField] private TextMeshProUGUI rentedCountText;

    [Header("Painting Lists")]
    [SerializeField] private Transform inventoryListParent;
    [SerializeField] private Transform displayListParent;
    [SerializeField] private GameObject paintingEntryPrefab;

    private void Start()
    {
        if (createArtButton != null)
            createArtButton.onClick.AddListener(OnCreateArtClicked);

        if (promotionButton != null)
            promotionButton.onClick.AddListener(OnPromotionClicked);

        UpdateCostLabels();
    }

    protected override void OnPanelShow()
    {
        RefreshUI();
    }

    private void OnCreateArtClicked()
    {
        if (GalleryManager.Instance == null) return;

        if (GalleryManager.Instance.TryCreatePainting())
        {
            RefreshUI();
        }
    }

    private void OnPromotionClicked()
    {
        var gm = GameManager.Instance;
        if (gm == null || MarketManager.Instance == null) return;

        if (MarketManager.Instance.IsPromotionActive)
        {
            gm.ShowMessage("Promotion already active for tonight!");
            return;
        }

        if (!gm.SpendMoney(gm.Settings.PromotionCost))
            return;

        MarketManager.Instance.IsPromotionActive = true;
        gm.ShowMessage("Promotion active! Rent & sell chances doubled tonight!");
        RefreshUI();
    }

    /// <summary>
    /// Display a painting from inventory onto the gallery wall.
    /// Called by painting entry buttons.
    /// </summary>
    public void OnDisplayPaintingClicked(string paintingId)
    {
        if (GalleryManager.Instance == null) return;

        if (GalleryManager.Instance.DisplayPainting(paintingId))
        {
            RefreshUI();
        }
    }

    /// <summary>
    /// Remove a painting from the gallery wall back to inventory.
    /// Called by displayed painting entry buttons.
    /// </summary>
    public void OnRemoveFromDisplayClicked(string paintingId)
    {
        if (GalleryManager.Instance == null) return;

        if (GalleryManager.Instance.RemoveFromDisplay(paintingId))
        {
            RefreshUI();
        }
    }

    private void RefreshUI()
    {
        UpdateCostLabels();
        UpdateCounts();
        RebuildPaintingLists();
    }

    private void UpdateCostLabels()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (createCostText != null)
            createCostText.text = $"Create Art (Fatigue +{gm.Settings.PaintingFatigueCost})";

        if (promotionText != null)
            promotionText.text = $"Promote (${gm.Settings.PromotionCost})";
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

    private void RebuildList(Transform parent, System.Collections.Generic.List<Painting> paintings, bool isInventory)
    {
        if (parent == null) return;

        // Clear existing entries
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
}
