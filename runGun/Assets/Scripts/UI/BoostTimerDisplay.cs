using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays a single boost timer UI element
/// </summary>
public class BoostTimerDisplay : MonoBehaviour
{
    [SerializeField, Tooltip("Text component for the boost name")]
    private TextMeshProUGUI boostNameText;
    
    [SerializeField, Tooltip("Text component for the boost amount")]
    private TextMeshProUGUI boostAmountText;
    
    [SerializeField, Tooltip("Text component for the timer countdown")]
    private TextMeshProUGUI timerText;
    
    [SerializeField, Tooltip("Image for the boost icon")]
    private Image boostIcon;
    
    [SerializeField, Tooltip("Image for the timer fill")]
    private Image timerFill;
    
    private float duration;
    private float remainingTime;
    private string boostType;
    private float boostAmount;
    
    /// <summary>
    /// Initialize the timer display
    /// </summary>
    /// <param name="type">Type of boost</param>
    /// <param name="amount">Amount of the boost</param>
    /// <param name="boostDuration">Duration in seconds</param>
    /// <param name="color">Color for the boost</param>
    public void Initialize(string type, float amount, float boostDuration, Color color)
    {
        boostType = type;
        boostAmount = amount;
        duration = boostDuration;
        remainingTime = duration;
        
        // Set boost name
        if (boostNameText != null)
        {
            boostNameText.text = GetDisplayName(boostType);
        }
        
        // Set boost amount
        if (boostAmountText != null)
        {
            string prefix = amount > 0 ? "+" : "";
            boostAmountText.text = $"{prefix}{amount}";
        }
        
        // Set color
        if (boostIcon != null)
        {
            boostIcon.color = color;
        }
        
        if (timerFill != null)
        {
            timerFill.color = color;
        }
        
        // Initialize timer
        UpdateTimerDisplay();
    }
    
    /// <summary>
    /// Update the timer each frame
    /// </summary>
    private void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            remainingTime = Mathf.Max(0, remainingTime);
            
            UpdateTimerDisplay();
            
            // Destroy when timer reaches zero
            if (remainingTime <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
    
    /// <summary>
    /// Updates the visual display of the timer
    /// </summary>
    private void UpdateTimerDisplay()
    {
        // Update fill amount
        if (timerFill != null)
        {
            timerFill.fillAmount = remainingTime / duration;
        }
        
        // Update timer text
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(remainingTime);
            timerText.text = seconds.ToString();
        }
    }
    
    /// <summary>
    /// Gets a display name for the boost type
    /// </summary>
    /// <param name="type">Boost type string</param>
    /// <returns>User-friendly display name</returns>
    private string GetDisplayName(string type)
    {
        switch (type)
        {
            case "health":
                return "Health";
            case "speed":
                return "Speed";
            case "strength":
                return "Strength";
            case "defense":
                return "Defense";
            case "jump":
                return "Jump";
            case "common pickup range":
                return "Pickup Range";
            default:
                return type;
        }
    }
}