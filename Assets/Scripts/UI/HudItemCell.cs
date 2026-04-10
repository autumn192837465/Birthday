using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD 物品欄中的單一格子：一個 Button 搭配一張 Image。
/// 點擊時透過 <see cref="OnClicked"/> 回呼通知外部。
/// </summary>
[RequireComponent(typeof(Button))]
public class HudItemCell : MonoBehaviour
{
    [SerializeField] private Image icon;

    private Button _button;
    private string _description;

    public event Action<string> OnClicked;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(HandleClick);
    }

    private void OnDestroy()
    {
        if (_button != null)
        {
            _button.onClick.RemoveListener(HandleClick);
        }
    }

    /// <summary>
    /// 初始化格子的圖片與說明文字。
    /// </summary>
    public void Setup(Sprite sprite, string description)
    {
        _description = description;

        if (icon != null && sprite != null)
        {
            icon.sprite = sprite;
        }
    }

    private void HandleClick()
    {
        OnClicked?.Invoke(_description);
    }
}
