using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class StrengthBarUI : MonoBehaviour
{
    public PlayerStats playerStats;
    private Slider strengthSlider;
    public TextMeshProUGUI strengthText; 

    void Start()
    {
        strengthSlider = GetComponent<Slider>();

        if (playerStats == null)
            Debug.LogError("StrengthBarUI: PlayerStats reference not assigned.");

        if (strengthSlider == null)
            Debug.LogError("StrengthBarUI: Slider component missing.");

        if (strengthText == null)
            Debug.LogError("StrengthBarUI: Strength text reference not assigned.");

        UpdateStrengthBar();
    }

    void Update()
    {
        UpdateStrengthBar();
    }

    void UpdateStrengthBar()
    {
        if (playerStats != null && strengthSlider != null)
        {
            strengthSlider.maxValue = 150f;
            strengthSlider.minValue = 10f;
            strengthSlider.value = playerStats.strength;

            if (strengthText != null)
            {
                strengthText.text = $"STRENGTH: {Mathf.RoundToInt(playerStats.strength)}";
            }
        }
    }
}


