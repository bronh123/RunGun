using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class SpeedBarUI : MonoBehaviour
{
    public PlayerStats playerStats;
    private Slider speedSlider;
    public TextMeshProUGUI speedText; 

    void Start()
    {
        speedSlider = GetComponent<Slider>();

        if (playerStats == null)
            Debug.LogError("SpeedBarUI: PlayerStats reference not assigned.");

        if (speedSlider == null)
            Debug.LogError("SpeedBarUI: Slider component missing.");

        if (speedText == null)
            Debug.LogError("SpeedBarUI: Speed text reference not assigned.");

        UpdateSpeedBar();
    }

    void Update()
    {
        UpdateSpeedBar();
    }

    void UpdateSpeedBar()
    {
        if (playerStats != null && speedSlider != null)
        {
            speedSlider.maxValue = 300f;
            speedSlider.value = playerStats.moveSpeed;

            if (speedText != null)
            {
                speedText.text = $"SPEED: {Mathf.RoundToInt(playerStats.moveSpeed)}";
            }
        }
    }
}
