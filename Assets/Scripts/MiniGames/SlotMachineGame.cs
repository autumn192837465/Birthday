using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// Slot Machine mini-game with an explicit 70% win rate.
/// The result is determined BEFORE the reels spin, then animation matches the outcome.
/// Respects WheelOfFortune tarot card (guaranteed big win).
/// </summary>
public class SlotMachineGame : MonoBehaviour
{
    /// <summary>
    /// Slot result categories.
    /// </summary>
    private enum SlotResult
    {
        Lose,
        SmallWin,
        BigWin
    }

    [Header("Reel Image References (3 reels)")]
    [SerializeField] private Image[] reelImages;

    [Header("Reel Result Text (optional overlay)")]
    [SerializeField] private TextMeshProUGUI[] reelTexts;

    [Header("Symbols")]
    [Tooltip("Sprites used for slot symbols. Index 0-4 are different symbols.")]
    [SerializeField] private Sprite[] symbolSprites;

    [Header("UI")]
    [SerializeField] private Button spinButton;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI costText;

    /// <summary>
    /// Symbol names for text-based display if no sprites are assigned.
    /// </summary>
    private readonly string[] symbolNames = { "Cherry", "Star", "Bell", "Seven", "Diamond" };

    private bool isSpinning = false;

    private void Start()
    {
        if (costText != null && GameManager.Instance != null)
        {
            costText.text = $"Spin (${GameManager.Instance.Settings.SlotMachineCost})";
        }

        if (resultText != null)
        {
            resultText.text = "Pull the lever!";
        }
    }

    /// <summary>
    /// Called by the Spin button OnClick.
    /// </summary>
    public void OnSpinButtonClicked()
    {
        var gm = GameManager.Instance;
        if (gm == null || isSpinning) return;

        // Deduct cost
        if (!gm.SpendMoney(gm.Settings.SlotMachineCost))
        {
            return;
        }

        isSpinning = true;

        if (resultText != null)
            resultText.text = "Spinning...";

        // Step 1: Check WheelOfFortune guaranteed win, otherwise use normal win rate
        SlotResult result;
        if (gm.HasGuaranteedMiniGameWin())
        {
            result = SlotResult.BigWin;
            gm.ShowMessage("Wheel of Fortune activated! Guaranteed BIG WIN!");
        }
        else
        {
            result = DetermineResult(gm.Settings.SlotWinRate);
        }

        // Notify tarot effects that a mini-game was played (consumes WheelOfFortune)
        gm.NotifyMiniGamePlayed();

        // Step 2: Determine the final symbols for each reel based on the result
        int[] finalSymbols = GenerateReelSymbols(result);

        // Step 3: Animate reels to land on the predetermined symbols
        PlayReelAnimation(finalSymbols, result);
    }

    /// <summary>
    /// Determine win/lose using the configured win rate.
    /// Win rate of 0.7 means 70% chance to win.
    /// Within wins: 30% chance Big Win, 70% chance Small Win.
    /// </summary>
    private SlotResult DetermineResult(float winRate)
    {
        float roll = Random.value;

        if (roll <= winRate)
        {
            // WIN - decide Big or Small
            float winTypeRoll = Random.value;
            if (winTypeRoll < 0.30f)
            {
                return SlotResult.BigWin;
            }
            else
            {
                return SlotResult.SmallWin;
            }
        }
        else
        {
            return SlotResult.Lose;
        }
    }

    /// <summary>
    /// Generate the 3 reel symbols based on the predetermined result.
    /// </summary>
    private int[] GenerateReelSymbols(SlotResult result)
    {
        int symbolCount = symbolSprites != null && symbolSprites.Length > 0
            ? symbolSprites.Length
            : symbolNames.Length;

        int[] symbols = new int[3];

        switch (result)
        {
            case SlotResult.BigWin:
                // All 3 match
                int matchSymbol = Random.Range(0, symbolCount);
                symbols[0] = matchSymbol;
                symbols[1] = matchSymbol;
                symbols[2] = matchSymbol;
                break;

            case SlotResult.SmallWin:
                // 2 match, 1 different
                int pairSymbol = Random.Range(0, symbolCount);
                symbols[0] = pairSymbol;
                symbols[1] = pairSymbol;
                symbols[2] = (pairSymbol + 1 + Random.Range(0, symbolCount - 1)) % symbolCount;
                ShuffleArray(symbols);
                break;

            case SlotResult.Lose:
                // All different
                symbols[0] = Random.Range(0, symbolCount);
                do { symbols[1] = Random.Range(0, symbolCount); } while (symbols[1] == symbols[0]);
                do { symbols[2] = Random.Range(0, symbolCount); } while (symbols[2] == symbols[0] || symbols[2] == symbols[1]);
                break;
        }

        return symbols;
    }

    /// <summary>
    /// Fisher-Yates shuffle for the reel symbols.
    /// </summary>
    private void ShuffleArray(int[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    /// <summary>
    /// Animate the 3 reels spinning and stopping on final symbols.
    /// Uses DOTween for the spin illusion, then snaps to result.
    /// </summary>
    private void PlayReelAnimation(int[] finalSymbols, SlotResult result)
    {
        Sequence seq = DOTween.Sequence();
        int symbolCount = symbolSprites != null && symbolSprites.Length > 0
            ? symbolSprites.Length
            : symbolNames.Length;

        for (int i = 0; i < 3; i++)
        {
            int reelIndex = i;
            int totalSpins = 8 + i * 4;
            float spinInterval = 0.08f;

            // Rapid symbol changes to simulate spinning
            for (int s = 0; s < totalSpins; s++)
            {
                int randomSym = (s + reelIndex * 3) % symbolCount;
                float time = i * 0.4f + s * spinInterval;

                seq.InsertCallback(time, () =>
                {
                    SetReelSymbol(reelIndex, randomSym);
                });
            }

            // Final symbol landing
            float landTime = i * 0.4f + totalSpins * spinInterval;
            seq.InsertCallback(landTime, () =>
            {
                SetReelSymbol(reelIndex, finalSymbols[reelIndex]);

                if (reelImages != null && reelIndex < reelImages.Length)
                {
                    reelImages[reelIndex].transform.DOPunchScale(Vector3.one * 0.15f, 0.3f, 6);
                }
            });
        }

        // Show result after all reels stop
        float totalDuration = 2 * 0.4f + (8 + 2 * 4) * 0.08f + 0.5f;
        seq.InsertCallback(totalDuration, () =>
        {
            ShowResult(result);
            isSpinning = false;
        });
    }

    /// <summary>
    /// Set a reel's display to a specific symbol.
    /// </summary>
    private void SetReelSymbol(int reelIndex, int symbolIndex)
    {
        if (reelImages != null && reelIndex < reelImages.Length
            && symbolSprites != null && symbolSprites.Length > 0)
        {
            reelImages[reelIndex].sprite = symbolSprites[symbolIndex % symbolSprites.Length];
        }

        if (reelTexts != null && reelIndex < reelTexts.Length)
        {
            reelTexts[reelIndex].text = symbolNames[symbolIndex % symbolNames.Length];
        }
    }

    /// <summary>
    /// Display the result and award money.
    /// </summary>
    private void ShowResult(SlotResult result)
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        switch (result)
        {
            case SlotResult.BigWin:
                int bigReward = gm.Settings.SlotRewards.Length > 1
                    ? gm.Settings.SlotRewards[1] : 300;
                gm.EarnMoney(bigReward);
                if (resultText != null)
                    resultText.text = $"BIG WIN! +${bigReward}!";
                gm.ShowMessage($"JACKPOT! You won ${bigReward}!");

                if (resultText != null)
                    resultText.transform.DOPunchScale(Vector3.one * 0.5f, 0.6f, 8);
                break;

            case SlotResult.SmallWin:
                int smallReward = gm.Settings.SlotRewards.Length > 0
                    ? gm.Settings.SlotRewards[0] : 100;
                gm.EarnMoney(smallReward);
                if (resultText != null)
                    resultText.text = $"WIN! +${smallReward}!";
                gm.ShowMessage($"Nice! You won ${smallReward}!");
                break;

            case SlotResult.Lose:
                if (resultText != null)
                    resultText.text = "No luck... Try again!";
                gm.ShowMessage("Almost! Try again!");
                break;
        }
    }
}
