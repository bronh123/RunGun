using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpDisplay : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Image expBar;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI expValuesText;
    
    
    private void Update()
    {
        if (playerStats != null)
        {
            // Update exp bar
            expBar.fillAmount = playerStats.GetExpPercentage();
            levelText.text = $"Lv. {playerStats.GetLevel().ToString()}";
            if (expValuesText != null)
            {
                expValuesText.text = $"{playerStats.GetExp().ToString()} / {playerStats.GetExpToLevel()}";
            }
            
        }
    }
}
