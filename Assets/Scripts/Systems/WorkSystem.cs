using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Random = UnityEngine.Random;

/// <summary>
/// Work system: earn money at the cost of fatigue.
/// Respects tarot card modifiers: TheHermit (blocks work), TheMagician (no fatigue),
/// TheFool (salary penalty), TheMoon (chaotic button).
/// </summary>
public class WorkSystem : PanelBase
{
    [Header("UI References")]
    [SerializeField] private Button workButton;
    [SerializeField] private RectTransform workButtonRect;
    [SerializeField] private Transform characterSprite;

    [Header("Chaotic Button Settings")]
    [Tooltip("How far the button randomly moves when TheMoon is active.")]
    [SerializeField] private float chaoticMoveRange = 120f;

    private bool isWorking = false;
    private Vector2 originalButtonPos;
    private bool hasStoredOriginalPos = false;
    
    private void Start()
    {
        // Store original button position for TheMoon reset
        if (workButtonRect != null)
        {
            originalButtonPos = workButtonRect.anchoredPosition;
            hasStoredOriginalPos = true;
        }
        
        workButton.onClick.AddListener(OnWorkButtonClicked);
    }

    private void Update()
    {
        // TheMoon effect: periodically move the work button to a random position
        if (GameManager.Instance != null && GameManager.Instance.HasChaoticWorkButton()
            && workButtonRect != null && !isWorking)
        {
            ApplyChaoticButtonMovement();
        }
        else if (hasStoredOriginalPos && workButtonRect != null && !isWorking)
        {
            // Smoothly return to original position when TheMoon is not active
            workButtonRect.anchoredPosition = Vector2.Lerp(workButtonRect.anchoredPosition, originalButtonPos, Time.deltaTime * 5f);
        }
    }

    /// <summary>
    /// Called by the "Work" button OnClick.
    /// </summary>
    public void OnWorkButtonClicked()
    {
        _ = GameManager.Instance.TryWork();
    }

    /// <summary>
    /// Simple bounce animation to represent working.
    /// </summary>
    private void PlayWorkAnimation(TweenCallback onComplete)
    {
        if (characterSprite != null)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(characterSprite.DOPunchScale(Vector3.one * 0.2f, 0.4f, 8));
            seq.AppendInterval(0.3f);
            seq.Append(characterSprite.DOPunchPosition(new Vector3(10f, 0f, 0f), 0.3f, 6));
            seq.OnComplete(onComplete);
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    /// <summary>
    /// TheMoon effect: randomly move the work button to annoy the player (playfully).
    /// </summary>
    private void ApplyChaoticButtonMovement()
    {
        // Move to a random offset from original position every few frames
        if (Time.frameCount % 15 == 0)
        {
            Vector2 randomOffset = new Vector2(
                Random.Range(-chaoticMoveRange, chaoticMoveRange),
                Random.Range(-chaoticMoveRange, chaoticMoveRange)
            );

            workButtonRect.DOAnchorPos(originalButtonPos + randomOffset, 0.2f)
                .SetEase(Ease.OutQuad);
        }
    }
}
