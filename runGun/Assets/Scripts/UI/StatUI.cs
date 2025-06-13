using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatUI : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    
    [Header("UI Elements")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI strengthText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI jumpText;
    public TextMeshProUGUI commonPickupRangeText;

    void Update()
    {
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        if (playerStats == null)
            return;
            
        // Update health display
        if (healthText != null)
            healthText.text = $"Health: {playerStats.currentHealth}/{playerStats.maxHealth}";
            
        // Update speed display
        if (speedText != null)
            speedText.text = $"Speed: {playerStats.moveSpeed:F1}";
            
        // Update strength display
        if (strengthText != null)
            strengthText.text = $"Strength: {playerStats.strength:F1}";
            
        // Update defense display
        if (defenseText != null)
            defenseText.text = $"Defense: {playerStats.defense:F1}";
        
        if (jumpText != null)
            jumpText.text = $"Jump Force: {playerStats.jumpForce:F1}";

        if (commonPickupRangeText != null)
            commonPickupRangeText.text = $"Pickup Range: {playerStats.commonPickupRange:F1}";

    }
}