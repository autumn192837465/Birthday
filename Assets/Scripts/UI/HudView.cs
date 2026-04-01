using TMPro;
using UnityEngine;

public class HudView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI fatigueText;
    [SerializeField] private TextMeshProUGUI dayText;

    public void SetMoney(int amount)
    {
        moneyText.text = amount.ToString();
    }

    /// <summary>Display remaining stamina vs today's effective maximum.</summary>
    public void SetStamina(int current, int effectiveMax)
    {
        fatigueText.text = $"{current}/{effectiveMax}";
    }
    
    public void SetDay(int day)
    {
        dayText.text = $"Day {day}";
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
    