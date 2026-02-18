using UnityEngine;

public enum PaintingState
{
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
    public string Title;
    public Sprite Image;
    public int BasePrice;
    public PaintingState State;
    public int RentDaysLeft;

    public Painting(string id, string title, Sprite image, int basePrice)
    {
        ID = id;
        Title = title;
        Image = image;
        BasePrice = basePrice;
        State = PaintingState.Inventory;
        RentDaysLeft = 0;
    }
}
