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
        Gallery,
        Tarot,
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

    [Header("Notification")]
    [SerializeField] private NotificationUI notificationUI;

    [Header("Screen Fade")]
    [SerializeField] private CanvasGroup fadeOverlay;

    [Header("Panels")]
    [SerializeField] private PanelInfo homePanelInfo;
    [SerializeField] private PanelInfo galleryPanelInfo;
    [SerializeField] private PanelInfo tarotPanelInfo;
    [SerializeField] private PanelInfo shopPanelInfo;
    [SerializeField] private GameObject gameClearPanel;

    [Header("Daily Summary Popup")]
    [SerializeField] private DailySummaryPopupView dailySummaryPopup;

    private PanelInfo? _currentPanelInfo;

    // 儲存委派參考以便正確取消訂閱
    private Action _onHomeClicked;
    private Action _onGalleryClicked;
    private Action _onTarotClicked;
    private Action _onShopClicked;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 初始化委派
        _onHomeClicked = () => _ = ShowPanelAsync(PanelType.Home);
        _onGalleryClicked = () => _ = ShowPanelAsync(PanelType.Gallery);
        _onTarotClicked = () => _ = ShowPanelAsync(PanelType.Tarot);
        _onShopClicked = () => _ = ShowPanelAsync(PanelType.Shop);

        AddUIEvents();

        homePanelInfo.PanelObject.Hide();
        galleryPanelInfo.PanelObject.Hide();
        tarotPanelInfo.PanelObject.Hide();
        shopPanelInfo.PanelObject.Hide();

        homePanelInfo.PanelObject.SetBackAction(() => _ = FadeToMainViewAsync());
        galleryPanelInfo.PanelObject.SetBackAction(() => _ = FadeToMainViewAsync());
        tarotPanelInfo.PanelObject.SetBackAction(() => _ = FadeToMainViewAsync());
        shopPanelInfo.PanelObject.SetBackAction(() => _ = FadeToMainViewAsync());
    }

    private void OnDestroy()
    {
        RemoveUIEvents();
    }

    /// <summary>
    /// 註冊主畫面圖示點擊事件（回到 MainView 時呼叫）。
    /// </summary>
    public void AddUIEvents()
    {
        RemoveUIEvents();
        homePanelInfo.MainViewIcon.OnIconClicked += _onHomeClicked;
        galleryPanelInfo.MainViewIcon.OnIconClicked += _onGalleryClicked;
        tarotPanelInfo.MainViewIcon.OnIconClicked += _onTarotClicked;
        shopPanelInfo.MainViewIcon.OnIconClicked += _onShopClicked;
    }

    /// <summary>
    /// 移除主畫面圖示點擊事件（切換面板時呼叫，防止玩家重複按）。
    /// </summary>
    public void RemoveUIEvents()
    {
        homePanelInfo.MainViewIcon.OnIconClicked -= _onHomeClicked;
        galleryPanelInfo.MainViewIcon.OnIconClicked -= _onGalleryClicked;
        tarotPanelInfo.MainViewIcon.OnIconClicked -= _onTarotClicked;
        shopPanelInfo.MainViewIcon.OnIconClicked -= _onShopClicked;
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
        GameManager.Instance.OnStatsChanged -= RefreshHUD;
        GameManager.Instance.OnShowMessage -= ShowToast;
    }

    private void Start()
    {
        // Initialize fade overlay to transparent
        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 0f;
            fadeOverlay.blocksRaycasts = false;
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
        if (gm == null)
        {
            return;
        }

        hudView.SetMoney(gm.Money);
        hudView.SetFatigue(gm.Fatigue, gm.Settings.MaxFatigue);
        hudView.SetDay(gm.DayCount);

        UpdateShopIconVisibility(gm);
    }

    /// <summary>
    /// 根據金錢是否足夠購買最低價商品來顯示/隱藏 Shop Icon。
    /// </summary>
    private void UpdateShopIconVisibility(GameManager gm)
    {
        if (gm.DataManager == null)
        {
            return;
        }

        int minPrice = gm.DataManager.GetShopMinPrice();
        bool canAfford = gm.Money >= minPrice;
        shopPanelInfo.MainViewIcon.gameObject.SetActive(canAfford);
    }

    /// <summary>
    /// Show a panel by name, hiding all others.
    /// </summary>
    public async Awaitable ShowPanelAsync(PanelType panelType)
    {
        RemoveUIEvents();
        GameManager.Instance.EnableInput(false);
        await FadeOutAsync();
        if (_currentPanelInfo != null)
        {
            _currentPanelInfo.Value.PanelObject.Hide();
        }
        
        _currentPanelInfo = panelType switch
        {
            PanelType.Home => homePanelInfo,
            PanelType.Gallery => galleryPanelInfo,
            PanelType.Tarot => tarotPanelInfo,
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
    /// Display a notification message that auto-fades.
    /// Delegates to NotificationUI which handles kill-and-restart on overlap.
    /// </summary>
    public void ShowToast(string message)
    {
        notificationUI?.ShowNotification(message);
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
        
        AddUIEvents();
    }
    
    public async Awaitable FadeToMainViewAsync()
    {
        GameManager.Instance.EnableInput(false);
        await FadeOutAsync();
        ToMainView();
        await FadeInAsync();
        GameManager.Instance.EnableInput(true);
    }
    
    public void ShowHudView()
    {
        hudView.gameObject.SetActive(true);
    }
    
    public void HideHudView()
    {
        hudView.gameObject.SetActive(false);
    }

    /// <summary>
    /// 顯示每日結算 popup，並等待用戶關閉。
    /// </summary>
    public async Awaitable ShowDailySummaryPopupAsync(MarketResult result, int dayCount)
    {
        if (dailySummaryPopup == null)
        {
            return;
        }

        var tcs = new System.Threading.Tasks.TaskCompletionSource<bool>();

        void OnPopupClosed()
        {
            dailySummaryPopup.OnCloseClicked -= OnPopupClosed;
            tcs.TrySetResult(true);
        }

        dailySummaryPopup.OnCloseClicked += OnPopupClosed;
        dailySummaryPopup.ShowResult(result, dayCount);

        await tcs.Task;
    }

    /// <summary>
    /// 檢查每日結算 popup 是否已設定。
    /// </summary>
    public bool HasDailySummaryPopup => dailySummaryPopup != null;
}
