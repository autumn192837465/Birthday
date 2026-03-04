using DG.Tweening;
using TMPro;
using UnityEngine;

/// <summary>
/// Self-contained notification banner.
/// Fades in via CanvasGroup, holds for a duration, then fades out.
/// Calling ShowNotification while already playing kills the current sequence and restarts.
/// </summary>
public class NotificationUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI notificationText;

    private const float FadeInDuration = 0.25f;
    private const float HoldDuration = 2f;
    private const float FadeOutDuration = 0.35f;

    private Sequence _sequence;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    private void OnDestroy()
    {
        _sequence?.Kill();
    }

    /// <summary>
    /// Show a notification message.
    /// If a notification is already playing, it is killed and the new message plays immediately.
    /// </summary>
    public void ShowNotification(string message)
    {
        if (canvasGroup == null || notificationText == null)
        {
            return;
        }

        _sequence?.Kill();

        notificationText.text = message;
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

        _sequence = DOTween.Sequence();
        _sequence.Append(canvasGroup.DOFade(1f, FadeInDuration));
        _sequence.AppendInterval(HoldDuration);
        _sequence.Append(canvasGroup.DOFade(0f, FadeOutDuration));
        _sequence.OnComplete(() =>
        {
            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = false;
            }
        });
        _sequence.Play();
    }

    /// <summary>
    /// Immediately hide the notification with a short fade out.
    /// </summary>
    public void Hide()
    {
        _sequence?.Kill();
        _sequence = null;

        if (canvasGroup == null)
        {
            return;
        }

        canvasGroup.DOFade(0f, FadeOutDuration).OnComplete(() =>
        {
            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = false;
            }
        });
    }
}
