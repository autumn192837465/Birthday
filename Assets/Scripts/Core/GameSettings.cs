using UnityEngine;

/// <summary>
/// ScriptableObject holding all game configuration values.
/// Create via Assets > Create > Game > GameSettings.
/// </summary>
[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Data")]
    [Tooltip("ScriptableObject that stores tarot card sprites. Used by DataManager.")]
    public TarotSpritesData TarotSpritesData;

    [Header("Fatigue")]
    [Tooltip("Maximum fatigue before the player must sleep.")]
    public int MaxFatigue = 100;

    [Header("Work")]
    [Tooltip("Money earned per work action.")]
    public int WorkSalary = 500;

    [Tooltip("Fatigue added per work action.")]
    public int WorkFatigueCost = 30;

    [Header("Fortune")]
    [Tooltip("Cost to draw a fortune card.")]
    public int FortuneCost = 200;

    [Header("Slot Machine")]
    [Tooltip("Cost per slot machine spin.")]
    public int SlotMachineCost = 50;

    [Tooltip("Probability of winning (0-1). 0.7 = 70%.")]
    [Range(0f, 1f)]
    public float SlotWinRate = 0.7f;

    [Tooltip("Slot rewards: [0] = Small Win, [1] = Big Win.")]
    public int[] SlotRewards = new int[] { 100, 300 };

    [Header("Whac-A-Mole")]
    [Tooltip("Duration of a single round in seconds.")]
    public float MoleGameDuration = 20f;

    [Tooltip("Money reward per normal mole hit.")]
    public int MoleHitReward = 30;

    [Tooltip("Money reward per gold mole hit.")]
    public int GoldMoleHitReward = 100;

    [Tooltip("Probability of a bomb spawning (0-1). Keep low for player-friendly.")]
    [Range(0f, 1f)]
    public float BombSpawnRate = 0.12f;

    [Tooltip("Probability of a gold mole spawning (0-1).")]
    [Range(0f, 1f)]
    public float GoldMoleSpawnRate = 0.35f;

    [Tooltip("Cost to play Whac-A-Mole.")]
    public int MoleGameCost = 50;

    [Header("Shop")]
    [Tooltip("Gift names displayed on buttons.")]
    public string[] GiftNames = new string[] { "Teddy Bear", "Music Box", "Birthday Cake" };

    [Tooltip("Gift prices matching GiftNames array.")]
    public int[] GiftPrices = new int[] { 500, 800, 1200 };
}
