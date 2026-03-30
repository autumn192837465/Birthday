using System.Collections.Generic;
using UnityEngine;

public enum CreateResult
{
    InProgress, 
    Completed
}

/// <summary>
/// Manages the player's painting collection: creation, inventory, and display slots.
/// Works with MarketManager for daily rent/sell processing.
/// </summary>
public class GalleryManager : MonoBehaviour
{
    public static GalleryManager Instance { get; private set; }

    private readonly Dictionary<string, Painting> _paintings = new Dictionary<string, Painting>();
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

    public Painting GetInProgressPainting()
    {
        foreach (var p in _paintings.Values)
        {
            if (p.State == PaintingState.InProgress)
            {
                return p;
            }
        }
        return null;
    }

    /// <summary>
    /// Progress-based painting creation. If no painting is in progress, starts a new one.
    /// Otherwise continues the existing one. Always succeeds — caller is responsible
    /// for checking fatigue/block conditions before calling.
    /// </summary>
    public (CreateResult result, Painting painting) CreatePainting()
    {
        var gm = GameManager.Instance;

        var existing = GetInProgressPainting();
        Painting painting = existing ?? CreateNewPainting(gm);

        float progressGain = Random.Range(gm.Settings.MinPaintingProgress, gm.Settings.MaxPaintingProgress);
        painting.Progress = Mathf.Min(painting.Progress + progressGain, 100f);

        gm.NotifyPaintingCreated();

        if (painting.Progress >= 100f)
        {
            painting.Progress = 100f;
            painting.State = PaintingState.Displayed;
            return (CreateResult.Completed, painting);
        }
        
        return (CreateResult.InProgress, painting);
    }

    private Painting CreateNewPainting(GameManager gm)
    {
        string title;
        string description = "";
        Sprite sprite = null;
        int basePrice;
        DrawingType drawingType = default;

        var entry = gm.DataManager != null ? GetNextAvailableDrawingEntry(gm.DataManager) : null;
        if (entry != null)
        {
            title = string.IsNullOrEmpty(entry.Name) ? $"Untitled #{_nextPaintingId}" : entry.Name;
            description = entry.Description;
            sprite = entry.Sprite;
            basePrice = Mathf.Max(1, entry.Price);
            drawingType = entry.Type;
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
        var painting = new Painting(id, drawingType, title, description, sprite, basePrice);
        _paintings.Add(id, painting);
        return painting;
    }

    /// <summary>
    /// 取得下一個尚未擁有的畫作條目（隨機選取）。
    /// </summary>
    private DrawingData.DrawingEntry GetNextAvailableDrawingEntry(DataManager dataManager)
    {
        if (dataManager == null || dataManager.DrawingCount == 0)
        {
            return null;
        }

        var ownedTypes = new HashSet<DrawingType>();
        foreach (var p in _paintings.Values)
        {
            ownedTypes.Add(p.DrawingType);
        }

        var availableEntries = new List<DrawingData.DrawingEntry>();
        for (int i = 0; i < dataManager.DrawingCount; i++)
        {
            var entry = dataManager.GetDrawingEntry(i);
            if (entry != null && !ownedTypes.Contains(entry.Type))
            {
                availableEntries.Add(entry);
            }
        }

        if (availableEntries.Count == 0)
        {
            return null;
        }

        return availableEntries[Random.Range(0, availableEntries.Count)];
    }

    /// <summary>
    /// 檢查是否所有畫作都已完成（沒有可創作的新畫作）。
    /// </summary>
    public bool AreAllPaintingsCompleted()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.DataManager == null)
        {
            return false;
        }

        var dataManager = gm.DataManager;
        if (dataManager.DrawingCount == 0)
        {
            return true;
        }

        var ownedTypes = new HashSet<DrawingType>();
        foreach (var p in _paintings.Values)
        {
            ownedTypes.Add(p.DrawingType);
        }

        for (int i = 0; i < dataManager.DrawingCount; i++)
        {
            var entry = dataManager.GetDrawingEntry(i);
            if (entry != null && !ownedTypes.Contains(entry.Type))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Returns one (DrawingType, Sprite) per completed painting type for refreshing the gallery wall.
    /// </summary>
    public List<(DrawingType drawingType, Sprite sprite)> GetCompletedPaintingsForWall()
    {
        var seen = new HashSet<DrawingType>();
        var result = new List<(DrawingType, Sprite)>();
        foreach (var p in _paintings.Values)
        {
            if (p.State == PaintingState.Sold || p.State == PaintingState.InProgress)
            {
                continue;
            }
            
            if (seen.Contains(p.DrawingType))
            {
                continue;
            }
            
            if (p.Image == null)
            {
                continue;
            }
            seen.Add(p.DrawingType);
            result.Add((p.DrawingType, p.Image));
        }
        return result;
    }
    
    public Dictionary<string, Painting> GetPaintings()
    {
        return _paintings;
    }


    private const int PromotionDuration = 3;

    /// <summary>
    /// Toggle promotion on a displayed painting. Returns true if toggled successfully.
    /// </summary>
    public bool TogglePaintingPromotion(string paintingId)
    {
        if (!_paintings.TryGetValue(paintingId, out var painting) || painting.State != PaintingState.Displayed)
        {
            return false;
        }

        if (painting.PromotionDaysLeft > 0)
        {
            painting.PromotionDaysLeft = 0;
        }
        else
        {
            painting.PromotionDaysLeft = PromotionDuration;
        }
        return true;
    }

    /// <summary>
    /// Decrease promotion days for all paintings by 1. Called after nightly market processing.
    /// </summary>
    public void DecreasePromotionDays()
    {
        foreach (var p in _paintings.Values)
        {
            if (p.PromotionDaysLeft > 0)
            {
                p.PromotionDaysLeft--;
            }
        }
    }

    public List<Painting> GetDisplayedPaintings()
    {
        var list = new List<Painting>();
        foreach (var p in _paintings.Values)
            if (p.State == PaintingState.Displayed) list.Add(p);
        return list;
    }

    public List<Painting> GetRentedPaintings()
    {
        var list = new List<Painting>();
        foreach (var p in _paintings.Values)
            if (p.State == PaintingState.Rented) list.Add(p);
        return list;
    }

    public List<Painting> GetAllPaintings()
    {
        var list = new List<Painting>();
        foreach (var p in _paintings.Values)
            if (p.State != PaintingState.Sold) list.Add(p);
        return list;
    }

    /// <summary>
    /// 取得所有已完成的畫作（不含 InProgress、Sold）。
    /// </summary>
    public List<Painting> GetCompletedPaintings()
    {
        var list = new List<Painting>();
        foreach (var p in _paintings.Values)
        {
            if (p.State != PaintingState.InProgress && p.State != PaintingState.Sold)
                list.Add(p);
        }
        return list;
    }

    /// <summary>
    /// 依 DrawingType 設定推廣（允許對已完成的畫作進行推廣，持續 3 天）。
    /// </summary>
    public bool TogglePaintingPromotionByType(DrawingType drawingType)
    {
        foreach (var p in _paintings.Values)
        {
            if (p.DrawingType == drawingType && 
                p.State != PaintingState.InProgress && 
                p.State != PaintingState.Sold)
            {
                if (p.PromotionDaysLeft > 0)
                {
                    p.PromotionDaysLeft = 0;
                }
                else
                {
                    p.PromotionDaysLeft = PromotionDuration;
                }
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 依 DrawingType 取得畫作（第一筆符合的）。
    /// </summary>
    public Painting GetPaintingByType(DrawingType drawingType)
    {
        foreach (var p in _paintings.Values)
        {
            if (p.DrawingType == drawingType && 
                p.State != PaintingState.InProgress && 
                p.State != PaintingState.Sold)
                return p;
        }
        return null;
    }

    /// <summary>
    /// Remove sold paintings from the list permanently.
    /// Called after market processing to clean up.
    /// </summary>
    public void PurgeSoldPaintings()
    {
        var toRemove = new List<string>();
        foreach (var kv in _paintings)
        {
            if (kv.Value.State == PaintingState.Sold)
                toRemove.Add(kv.Key);
        }
        foreach (var id in toRemove)
            _paintings.Remove(id);
    }
}
