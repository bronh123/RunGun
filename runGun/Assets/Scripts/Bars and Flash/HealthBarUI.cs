using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class HealthBarUI : MonoBehaviour
{
    public PlayerStats playerStats;
    private Slider healthSlider;
    public TextMeshProUGUI healthText; 

    void Start()
    {
        healthSlider = GetComponent<Slider>();

        if (playerStats == null)
        {
            Debug.LogError("HealthBarUI: PlayerStats reference not assigned.");
        }

        if (healthSlider == null)
        {
            Debug.LogError("HealthBarUI: Slider component missing.");
        }

        if (healthText == null)
        {
            Debug.LogError("HealthBarUI: Health text reference not assigned.");
        }

        UpdateHealthBar();
    }

    void Update()
    {
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (playerStats != null && healthSlider != null)
        {
            
            healthSlider.maxValue = playerStats.maxHealth;

           
            healthSlider.value = Mathf.Min(playerStats.currentHealth, 50f);

            
            if (healthText != null)
            {
                healthText.text = $"HEALTH: {playerStats.currentHealth}/{playerStats.maxHealth}";
            }
        }
    }

}



