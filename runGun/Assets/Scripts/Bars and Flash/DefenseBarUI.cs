using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class DefenseBarUI : MonoBehaviour
{
    public PlayerStats playerStats;
    private Slider defenseSlider;
    public TextMeshProUGUI defenseText; 

    void Start()
    {
        defenseSlider = GetComponent<Slider>();

        if (playerStats == null)
            Debug.LogError("DefenseBarUI: PlayerStats reference not assigned.");

        if (defenseSlider == null)
            Debug.LogError("DefenseBarUI: Slider component missing.");

        if (defenseText == null)
            Debug.LogError("DefenseBarUI: Defense text reference not assigned.");

        UpdateDefenseBar();
    }

    void Update()
    {
        UpdateDefenseBar();
    }

    void UpdateDefenseBar()
    {
        if (playerStats != null && defenseSlider != null)
        {
            defenseSlider.maxValue = 200f;
            defenseSlider.value = playerStats.defense;

            if (defenseText != null)
            {
                defenseText.text = $"DEFENSE: {Mathf.RoundToInt(playerStats.defense)}";
            }
        }
    }
}


