using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD 通用格子：Icon + 可選文字 + Button。
/// 點擊時透過 <see cref="GameManager.ShowMessage"/> 顯示說明。
/// </summary>
public class HudItemCell : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Button button;

    private string _description;

    private void Awake()
    {
        if (button != null)
        {
            button.onClick.AddListener(HandleClick);
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(HandleClick);
        }
    }

    public void Setup(Sprite sprite, string description)
    {
        _description = description;

        if (icon != null && sprite != null)
        {
            icon.sprite = sprite;
        }
    }

    public void SetDescription(string description)
    {
        _description = description;
    }

    public void SetText(string text)
    {
        if (label != null)
        {
            label.text = text;
        }
    }

    public void SetIcon(Sprite sprite)
    {
        if (icon != null && sprite != null)
        {
            icon.sprite = sprite;
        }
    }

    private void HandleClick()
    {
        if (!string.IsNullOrEmpty(_description))
        {
            GameManager.Instance?.ShowMessage(_description);
        }
    }
}
