using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatBar : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Image statBar;
    [SerializeField] private TextMeshProUGUI statText;

    public enum StatType { Health, Speed, Strength, Defense, Jump, CommonPickupRange }
    public StatType statToDisplay;
    private float statPercentage;
    private float statAmount = -1f;
    private Color color;
    public float jumpMax = 100f;
    public float speedMax = 100f;
    public float defenseMax = 100f;
    public float strengthMax = 100f;
    private string statString;

    private void GetVars()
    {
        if (playerStats != null)
        {
            switch (statToDisplay)
            {
                case StatType.Health:
                    statPercentage = playerStats.GetHealthPercentage();
                    statAmount = playerStats.currentHealth;
                    color = Color.red;
                    statString = $"{statToDisplay.ToString()}: {statAmount:F1} / {playerStats.maxHealth}";
                    break;
                case StatType.Speed:
                    statPercentage = playerStats.moveSpeed / speedMax;
                    statAmount = playerStats.moveSpeed;
                    color = Color.blue;
                    break;
                case StatType.Defense:
                    statAmount = playerStats.defense;
                    statPercentage = statAmount / defenseMax;
                    color = Color.grey;
                    break;
                case StatType.Strength:
                    statAmount = playerStats.strength;
                    statPercentage = statAmount / strengthMax;
                    color = Color.yellow;
                    break;
                case StatType.Jump:
                    statAmount = playerStats.jumpForce;
                    statPercentage = statAmount / jumpMax;
                    color = Color.green;
                    break;

            }
        }
    }

    private void Update()
    {
        if (playerStats != null)
        {
            GetVars();
            if (statToDisplay != StatType.Health)
            {
                statString = $"{statToDisplay.ToString()}: {statAmount:F1}";
            }
            // Update exp bar
            statBar.fillAmount = statPercentage;
            statText.text = statString;
            statText.color = color;
            statBar.color = color;

        }
    }
}
