using UnityEngine;
using UnityEngine.Serialization;

public enum PaintingState
{
    InProgress,
    Inventory,
    Displayed,
    Rented,
    Sold
}

/// <summary>
/// Represents a single painting created by the player.
/// Tracks its lifecycle from creation through display, rental, and potential sale.
/// </summary>
[System.Serializable]
public class Painting
{
    public string ID;
    public DrawingType DrawingType;
    public string Title;
    public string Description;
    public Sprite Image;
    public int BasePrice;
    public PaintingState State;
    public int RentDaysLeft;
    public float Progress;
    public bool IsPromoted;

    public Painting(string id, DrawingType drawingType, string title, string description, Sprite image, int basePrice)
    {
        ID = id;
        DrawingType = drawingType;
        Title = title;
        Description = description;
        Image = image;
        BasePrice = basePrice;
        State = PaintingState.InProgress;
        RentDaysLeft = 0;
        Progress = 0f;
        IsPromoted = false;
    }
}
