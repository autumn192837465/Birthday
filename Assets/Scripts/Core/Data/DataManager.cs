using UnityEngine;

/// <summary>
/// 遊戲資料集中存取（例如塔羅圖、名稱、敘述；畫作名稱、敘述、圖、價格）。須由 GameManager 先初始化。
/// </summary>
public class DataManager : MonoBehaviour
{
    [SerializeField] private TarotData tarotData;
    [SerializeField] private DrawingData drawingData;

    /// <summary>啟動時由 GameManager 呼叫。</summary>
    public void Initialize()
    {
        if (tarotData != null)
            tarotData.Initialize();
        if (drawingData != null)
            drawingData.Initialize();
    }

    /// <summary>取得該塔羅類型的圖片，未設定則回傳 null。</summary>
    public Sprite GetTarotSprite(TarotType type)
    {
        return tarotData != null ? tarotData.GetSprite(type) : null;
    }

    /// <summary>取得該塔羅類型的顯示名稱，未設定則回傳 type.ToString()。</summary>
    public string GetTarotName(TarotType type)
    {
        return tarotData != null ? tarotData.GetName(type) : type.ToString();
    }

    /// <summary>取得該塔羅類型的效果敘述，未設定則回傳空字串。</summary>
    public string GetTarotDescription(TarotType type)
    {
        return tarotData != null ? tarotData.GetDescription(type) : "";
    }

    // === Drawing Data ===

    /// <summary>取得畫作資料筆數。</summary>
    public int DrawingCount => drawingData != null ? drawingData.Count : 0;

    /// <summary>依索引取得畫作條目，超出範圍回傳 null。</summary>
    public DrawingData.DrawingEntry GetDrawingEntry(int index)
    {
        return drawingData != null ? drawingData.GetEntry(index) : null;
    }

    /// <summary>隨機取得一筆畫作條目，無資料時回傳 null。</summary>
    public DrawingData.DrawingEntry GetRandomDrawingEntry()
    {
        return drawingData != null ? drawingData.GetRandomEntry() : null;
    }
}
