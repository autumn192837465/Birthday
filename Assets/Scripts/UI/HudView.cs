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

    public void SetFatigue(int amount, int maxFatigue)
    {
        fatigueText.text = $"{amount}/{maxFatigue}";
    }
    
    public void SetDay(int day)
    {
        dayText.text = $"Day {day}";
    }
}
    