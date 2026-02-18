using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages the player's painting collection: creation, inventory, and display slots.
/// Works with MarketManager for daily rent/sell processing.
/// </summary>
public class GalleryManager : MonoBehaviour
{
    public static GalleryManager Instance { get; private set; }

    private readonly List<Painting> _paintings = new List<Painting>();
    private int _nextPaintingId = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Attempt to create a new painting. Costs fatigue.
    /// Returns true if successful.
    /// </summary>
    public bool TryCreatePainting()
    {
        var gm = GameManager.Instance;
        if (gm == null) return false;

        if (gm.HasBlocksCreation())
        {
            gm.ShowMessage("The Hermit says: rest today. No creating allowed!");
            return false;
        }

        if (!gm.AddFatigue(gm.Settings.PaintingFatigueCost))
            return false;

        string title;
        Sprite sprite = null;
        int basePrice;

        var entry = gm.DataManager != null ? gm.DataManager.GetRandomDrawingEntry() : null;
        if (entry != null)
        {
            title = string.IsNullOrEmpty(entry.Name) ? $"Untitled #{_nextPaintingId}" : entry.Name;
            sprite = entry.Sprite;
            basePrice = Mathf.Max(1, entry.Price);
        }
        else
        {
            var settings = gm.Settings;
            basePrice = Random.Range(settings.PaintingBasePriceMin, settings.PaintingBasePriceMax + 1);
            title = $"Untitled #{_nextPaintingId}";
        }

        float valueMultiplier = gm.GetTotalPaintingValueMultiplier();
        basePrice = Mathf.RoundToInt(basePrice * valueMultiplier);
        basePrice = Mathf.Max(1, basePrice);

        string id = $"painting_{_nextPaintingId++}";
        var painting = new Painting(id, title, sprite, basePrice);
        _paintings.Add(painting);

        gm.NotifyPaintingCreated();
        gm.ShowMessage($"Created \"{title}\" (Value: ${basePrice})");

        return true;
    }

    /// <summary>
    /// Move a painting from Inventory to a Display slot.
    /// </summary>
    public bool DisplayPainting(string paintingId)
    {
        var painting = _paintings.FirstOrDefault(p => p.ID == paintingId && p.State == PaintingState.Inventory);
        if (painting == null) return false;

        var settings = GameManager.Instance?.Settings;
        if (settings == null) return false;

        int displayedCount = _paintings.Count(p => p.State == PaintingState.Displayed);
        if (displayedCount >= settings.MaxDisplaySlots)
        {
            GameManager.Instance.ShowMessage("Display wall is full!");
            return false;
        }

        painting.State = PaintingState.Displayed;
        return true;
    }

    /// <summary>
    /// Move a painting from Display back to Inventory.
    /// </summary>
    public bool RemoveFromDisplay(string paintingId)
    {
        var painting = _paintings.FirstOrDefault(p => p.ID == paintingId && p.State == PaintingState.Displayed);
        if (painting == null) return false;

        painting.State = PaintingState.Inventory;
        return true;
    }

    public List<Painting> GetInventoryPaintings()
    {
        return _paintings.Where(p => p.State == PaintingState.Inventory).ToList();
    }

    public List<Painting> GetDisplayedPaintings()
    {
        return _paintings.Where(p => p.State == PaintingState.Displayed).ToList();
    }

    public List<Painting> GetRentedPaintings()
    {
        return _paintings.Where(p => p.State == PaintingState.Rented).ToList();
    }

    public List<Painting> GetAllPaintings()
    {
        return _paintings.Where(p => p.State != PaintingState.Sold).ToList();
    }

    /// <summary>
    /// Remove sold paintings from the list permanently.
    /// Called after market processing to clean up.
    /// </summary>
    public void PurgeSoldPaintings()
    {
        _paintings.RemoveAll(p => p.State == PaintingState.Sold);
    }
}
