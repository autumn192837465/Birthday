using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// Whac-A-Mole mini-game with 3x3 grid.
/// Player-friendly: low bomb rate (~12%), high gold mole rate (~35%).
/// </summary>
public class WhacAMoleGame : MonoBehaviour
{
    /// <summary>
    /// Types of moles that can spawn.
    /// </summary>
    private enum MoleType
    {
        Normal,
        Gold,
        Bomb
    }

    [Header("Mole Hole References (9 holes, 3x3 grid)")]
    [Tooltip("Assign 9 transforms representing the mole positions in the grid.")]
    [SerializeField] private RectTransform[] moleHoles;

    [Header("Mole Visual References")]
    [Tooltip("Image components on each hole to show mole sprite.")]
    [SerializeField] private Image[] moleImages;

    [Header("Mole Sprites")]
    [SerializeField] private Sprite normalMoleSprite;
    [SerializeField] private Sprite goldMoleSprite;
    [SerializeField] private Sprite bombSprite;
    [SerializeField] private Sprite emptyHoleSprite;

    [Header("UI")]
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI costText;

    private bool isPlaying = false;
    private float timeRemaining;
    private int currentScore;
    private MoleType[] holeStates;
    private bool[] holeActive;

    private void Start()
    {
        holeStates = new MoleType[9];
        holeActive = new bool[9];

        if (costText != null && GameManager.Instance != null)
        {
            costText.text = $"Play (${GameManager.Instance.Settings.MoleGameCost})";
        }

        ResetAllHoles();
    }

    /// <summary>
    /// Whether WheelOfFortune was active when this round started.
    /// If true, bombs are disabled and all moles are gold.
    /// </summary>
    private bool isGuaranteedWinRound = false;

    /// <summary>
    /// Called by the "Start Game" button.
    /// </summary>
    public void OnStartButtonClicked()
    {
        var gm = GameManager.Instance;
        if (gm == null || isPlaying) return;

        // Deduct cost
        if (!gm.SpendMoney(gm.Settings.MoleGameCost))
        {
            return;
        }

        // Check WheelOfFortune: guaranteed win round (no bombs, all gold)
        isGuaranteedWinRound = gm.HasGuaranteedMiniGameWin();
        if (isGuaranteedWinRound)
        {
            gm.ShowMessage("Wheel of Fortune! All moles are golden!");
        }

        // Consume the one-shot effect
        gm.NotifyMiniGamePlayed();

        StartGame();
    }

    /// <summary>
    /// Called when a mole hole is clicked. Pass the hole index (0-8).
    /// </summary>
    public void OnMoleClicked(int holeIndex)
    {
        if (!isPlaying || holeIndex < 0 || holeIndex >= 9) return;
        if (!holeActive[holeIndex]) return;

        var gm = GameManager.Instance;
        if (gm == null) return;

        MoleType type = holeStates[holeIndex];

        switch (type)
        {
            case MoleType.Normal:
                currentScore += gm.Settings.MoleHitReward;
                PlayHitEffect(holeIndex);
                break;

            case MoleType.Gold:
                currentScore += gm.Settings.GoldMoleHitReward;
                PlayHitEffect(holeIndex);
                break;

            case MoleType.Bomb:
                // Penalty: lose some points but not devastating
                currentScore = Mathf.Max(0, currentScore - 50);
                PlayBombEffect(holeIndex);
                break;
        }

        HideMole(holeIndex);
        UpdateScoreUI();
    }

    private void StartGame()
    {
        var gm = GameManager.Instance;
        isPlaying = true;
        currentScore = 0;
        timeRemaining = gm.Settings.MoleGameDuration;

        if (startButton != null)
            startButton.interactable = false;

        UpdateScoreUI();
        ResetAllHoles();

        StartCoroutine(GameLoop());
        StartCoroutine(TimerLoop());
    }

    /// <summary>
    /// Main game loop: spawn moles at intervals.
    /// </summary>
    private IEnumerator GameLoop()
    {
        while (isPlaying)
        {
            float spawnDelay = Random.Range(0.6f, 1.2f);
            yield return new WaitForSeconds(spawnDelay);

            if (!isPlaying) break;

            // Pick a random empty hole
            int hole = GetRandomEmptyHole();
            if (hole < 0) continue;

            // Determine mole type (player-friendly distribution)
            MoleType type = GetRandomMoleType();

            SpawnMole(hole, type);

            // Auto-hide after some time if not clicked
            float displayTime = Random.Range(1.2f, 2.5f);
            StartCoroutine(AutoHideMole(hole, displayTime));
        }
    }

    /// <summary>
    /// Countdown timer.
    /// </summary>
    private IEnumerator TimerLoop()
    {
        while (timeRemaining > 0f && isPlaying)
        {
            timeRemaining -= Time.deltaTime;

            if (timerText != null)
                timerText.text = $"Time: {Mathf.CeilToInt(timeRemaining)}s";

            yield return null;
        }

        EndGame();
    }

    /// <summary>
    /// Player-friendly mole type distribution.
    /// Normal: ~53%, Gold: ~35%, Bomb: ~12%.
    /// If WheelOfFortune round: no bombs, all gold.
    /// </summary>
    private MoleType GetRandomMoleType()
    {
        // Guaranteed win round: no bombs, heavily gold
        if (isGuaranteedWinRound)
        {
            return (Random.value < 0.7f) ? MoleType.Gold : MoleType.Normal;
        }

        var gm = GameManager.Instance;
        float roll = Random.value;

        if (roll < gm.Settings.BombSpawnRate)
            return MoleType.Bomb;
        else if (roll < gm.Settings.BombSpawnRate + gm.Settings.GoldMoleSpawnRate)
            return MoleType.Gold;
        else
            return MoleType.Normal;
    }

    /// <summary>
    /// Find a random hole that currently has no active mole.
    /// </summary>
    private int GetRandomEmptyHole()
    {
        // Collect empty holes
        int[] emptyHoles = new int[9];
        int count = 0;

        for (int i = 0; i < 9; i++)
        {
            if (!holeActive[i])
            {
                emptyHoles[count] = i;
                count++;
            }
        }

        if (count == 0) return -1;
        return emptyHoles[Random.Range(0, count)];
    }

    /// <summary>
    /// Spawn a mole at the given hole with pop-up animation.
    /// </summary>
    private void SpawnMole(int holeIndex, MoleType type)
    {
        holeActive[holeIndex] = true;
        holeStates[holeIndex] = type;

        // Set sprite based on type
        if (moleImages != null && holeIndex < moleImages.Length)
        {
            switch (type)
            {
                case MoleType.Normal:
                    if (normalMoleSprite != null) moleImages[holeIndex].sprite = normalMoleSprite;
                    break;
                case MoleType.Gold:
                    if (goldMoleSprite != null) moleImages[holeIndex].sprite = goldMoleSprite;
                    break;
                case MoleType.Bomb:
                    if (bombSprite != null) moleImages[holeIndex].sprite = bombSprite;
                    break;
            }

            moleImages[holeIndex].enabled = true;
        }

        // Pop-up animation using DOTween DOMoveY
        if (moleHoles != null && holeIndex < moleHoles.Length)
        {
            RectTransform hole = moleHoles[holeIndex];
            Vector2 startPos = hole.anchoredPosition;
            Vector2 hiddenPos = startPos + Vector2.down * 80f;

            hole.anchoredPosition = hiddenPos;
            hole.DOAnchorPosY(startPos.y, 0.3f).SetEase(Ease.OutBack);
        }
    }

    /// <summary>
    /// Hide a mole at a specific hole with slide-down animation.
    /// </summary>
    private void HideMole(int holeIndex)
    {
        if (!holeActive[holeIndex]) return;

        holeActive[holeIndex] = false;

        if (moleHoles != null && holeIndex < moleHoles.Length)
        {
            RectTransform hole = moleHoles[holeIndex];
            float targetY = hole.anchoredPosition.y - 80f;

            hole.DOAnchorPosY(targetY, 0.2f)
                .SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    if (moleImages != null && holeIndex < moleImages.Length)
                    {
                        moleImages[holeIndex].enabled = false;
                    }
                });
        }
    }

    /// <summary>
    /// Auto-hide mole if not clicked in time.
    /// </summary>
    private IEnumerator AutoHideMole(int holeIndex, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (holeActive[holeIndex])
        {
            HideMole(holeIndex);
        }
    }

    /// <summary>
    /// Play a satisfying hit effect on a mole.
    /// </summary>
    private void PlayHitEffect(int holeIndex)
    {
        if (moleHoles != null && holeIndex < moleHoles.Length)
        {
            moleHoles[holeIndex].DOPunchScale(Vector3.one * 0.3f, 0.25f, 8);
        }
    }

    /// <summary>
    /// Play a bomb explosion effect.
    /// </summary>
    private void PlayBombEffect(int holeIndex)
    {
        if (moleHoles != null && holeIndex < moleHoles.Length)
        {
            moleHoles[holeIndex].DOShakePosition(0.3f, 15f, 20);
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: ${currentScore}";
    }

    private void ResetAllHoles()
    {
        for (int i = 0; i < 9; i++)
        {
            holeActive[i] = false;
            holeStates[i] = MoleType.Normal;

            if (moleImages != null && i < moleImages.Length)
            {
                moleImages[i].enabled = false;
            }
        }
    }

    private void EndGame()
    {
        isPlaying = false;

        // Hide all active moles
        for (int i = 0; i < 9; i++)
        {
            if (holeActive[i])
            {
                HideMole(i);
            }
        }

        // Award money
        var gm = GameManager.Instance;
        if (gm != null && currentScore > 0)
        {
            gm.EarnMoney(currentScore);
            gm.ShowMessage($"Whac-A-Mole complete! Earned ${currentScore}!");
        }
        else if (gm != null)
        {
            gm.ShowMessage("Whac-A-Mole complete! Better luck next time!");
        }

        if (startButton != null)
            startButton.interactable = true;

        if (timerText != null)
            timerText.text = "Time: 0s";
    }
}
