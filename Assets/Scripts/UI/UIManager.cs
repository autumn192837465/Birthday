using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Serialization;

/// <summary>
/// Handles HUD updates, screen transitions, and toast messages.
/// </summary>
public class UIManager : MonoBehaviour
{
    public enum PanelType
    {
        Home,
        Work,
        Tarot,
        Game,
        Shop
    }
    
    [Serializable]
    public struct PanelInfo
    {
        public PanelType Type;
        public MainViewIcon MainViewIcon;
        public PanelBase PanelObject;
    }
    
    public static UIManager Instance { get; private set; }

    [Header("HUD Elements")]
    [SerializeField] private HudView hudView;

    [Header("Message Toast")]
    [SerializeField] private CanvasGroup messagePanel;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Screen Fade")]
    [SerializeField] private CanvasGroup fadeOverlay;

    [Header("Panels")]
    [SerializeField] private PanelInfo homePanelInfo;
    [SerializeField] private PanelInfo workPanelInfo;
    [SerializeField] private PanelInfo tarotPanelInfo;
    [SerializeField] private PanelInfo gamePanelInfo;
    [SerializeField] private PanelInfo shopPanelInfo;
    [SerializeField] private GameObject gameClearPanel;
    
    
    
    private PanelInfo? _currentPanelInfo;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        homePanelInfo.MainViewIcon.OnIconClicked += () => _ = ShowPanelAsync(PanelType.Home);
        workPanelInfo.MainViewIcon.OnIconClicked += () => _ = ShowPanelAsync(PanelType.Work);
        tarotPanelInfo.MainViewIcon.OnIconClicked += () => _ = ShowPanelAsync(PanelType.Tarot);
        gamePanelInfo.MainViewIcon.OnIconClicked += () => _ = ShowPanelAsync(PanelType.Game);
        shopPanelInfo.MainViewIcon.OnIconClicked += () => _ = ShowPanelAsync(PanelType.Shop);
        
        homePanelInfo.PanelObject.Hide();
        workPanelInfo.PanelObject.Hide();
        tarotPanelInfo.PanelObject.Hide();
        gamePanelInfo.PanelObject.Hide();
        shopPanelInfo.PanelObject.Hide();


        
        homePanelInfo.PanelObject.SetBackAction(ToMainView);
        workPanelInfo.PanelObject.SetBackAction(ToMainView);
        tarotPanelInfo.PanelObject.SetBackAction(ToMainView);
        gamePanelInfo.PanelObject.SetBackAction(ToMainView);
        shopPanelInfo.PanelObject.SetBackAction(ToMainView);
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStatsChanged += RefreshHUD;
            GameManager.Instance.OnShowMessage += ShowToast;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStatsChanged -= RefreshHUD;
            GameManager.Instance.OnShowMessage -= ShowToast;
        }
    }

    private void Start()
    {
        // Initialize fade overlay to transparent
        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 0f;
            fadeOverlay.blocksRaycasts = false;
        }

        // Initialize message panel hidden
        if (messagePanel != null)
        {
            messagePanel.alpha = 0f;
            messagePanel.blocksRaycasts = false;
        }

        // Hide game clear panel
        if (gameClearPanel != null)
            gameClearPanel.SetActive(false);

        RefreshHUD();
    }

    /// <summary>
    /// Refresh all HUD text elements to current values.
    /// </summary>
    public void RefreshHUD()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        hudView.SetMoney(gm.Money);
        hudView.SetFatigue(gm.Fatigue, gm.Settings.MaxFatigue);
        hudView.SetDay(gm.DayCount);
    }

    /// <summary>
    /// Show a panel by name, hiding all others.
    /// </summary>
    public async Awaitable ShowPanelAsync(PanelType panelType)
    {
        GameManager.Instance.EnableInput(false);
        await FadeOutAsync();
        if (_currentPanelInfo != null)
        {
            _currentPanelInfo.Value.PanelObject.Hide();
        }
        
        _currentPanelInfo = panelType switch
        {
            PanelType.Home => homePanelInfo,
            PanelType.Work => workPanelInfo,
            PanelType.Tarot => tarotPanelInfo,
            PanelType.Game => gamePanelInfo,
            PanelType.Shop => shopPanelInfo,
            _ => null
        };
       
        if (_currentPanelInfo != null)
        {
            _currentPanelInfo.Value.PanelObject.Show();
        }
        
        await FadeInAsync();
        GameManager.Instance.EnableInput(true);
    }
    
    public async Awaitable FadeInAsync(float duration = 0.5f)
    {
        fadeOverlay.blocksRaycasts = true;
        Sequence seq = DOTween.Sequence();
        seq.Append(fadeOverlay.DOFade(0f, duration));
        await seq.AsyncWaitForCompletion();
    }
    
    public async Awaitable FadeOutAsync(float duration = 0.5f)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(fadeOverlay.DOFade(1f, duration));
        seq.OnComplete(() => fadeOverlay.blocksRaycasts = false);
        await seq.AsyncWaitForCompletion();
    }

    /// <summary>
    /// Display a toast message that auto-fades.
    /// </summary>
    public void ShowToast(string message)
    {
        if (messagePanel == null || messageText == null) return;

        // Kill any existing toast animation
        DOTween.Kill(messagePanel);

        messageText.text = message;
        messagePanel.alpha = 0f;
        messagePanel.blocksRaycasts = false;

        Sequence seq = DOTween.Sequence().SetId(messagePanel);
        seq.Append(messagePanel.DOFade(1f, 0.3f));
        seq.AppendInterval(2f);
        seq.Append(messagePanel.DOFade(0f, 0.3f));
    }

    /// <summary>
    /// Show the Game Clear panel with celebration.
    /// </summary>
    public void ShowGameClear(string giftName)
    {
        if (gameClearPanel == null) return;

        gameClearPanel.SetActive(true);

        var clearText = gameClearPanel.GetComponentInChildren<TextMeshProUGUI>();
        if (clearText != null)
        {
            clearText.text = $"Happy Birthday!\nYou gifted: {giftName}";
        }

        // Punch scale animation
        gameClearPanel.transform.localScale = Vector3.zero;
        gameClearPanel.transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack);
    }

    public void ToMainView()
    {
        if (_currentPanelInfo != null)
        {
            _currentPanelInfo.Value.PanelObject.Hide();
            _currentPanelInfo = null;
        }
    }
    
    public async Awaitable FadeToMainViewAsync()
    {
        GameManager.Instance.EnableInput(false);
        await FadeOutAsync();
        ToMainView();
        await FadeInAsync();
        GameManager.Instance.EnableInput(true);
    }
}
