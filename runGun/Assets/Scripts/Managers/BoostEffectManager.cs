using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages visual effects for active temporary boosts
/// </summary>
public class BoostEffectManager : MonoBehaviour
{
    [Header("Vignette Settings")]
    [SerializeField, Tooltip("Image component for the vignette effect")]
    private Image vignetteImage;
    
    [SerializeField, Tooltip("Color for health boost vignette")]
    private Color healthBoostColor = new Color(0, 1, 0, 0.2f);
    
    [SerializeField, Tooltip("Color for speed boost vignette")]
    private Color speedBoostColor = new Color(0, 0.8f, 1, 0.2f);
    
    [SerializeField, Tooltip("Color for strength boost vignette")]
    private Color strengthBoostColor = new Color(1, 0, 0, 0.2f);
    
    [SerializeField, Tooltip("Color for defense boost vignette")]
    private Color defenseBoostColor = new Color(0.5f, 0.5f, 1, 0.2f);
    
    [SerializeField, Tooltip("Color for jump boost vignette")]
    private Color jumpBoostColor = new Color(1, 1, 0, 0.2f);
    
    [SerializeField, Tooltip("Fade in time for vignette in seconds")]
    private float fadeInTime = 0.2f;
    
    [SerializeField, Tooltip("Fade out time for vignette in seconds")]
    private float fadeOutTime = 0.5f;
    
    [Header("Timer Display")]
    [SerializeField, Tooltip("Parent container for boost timer displays")]
    private RectTransform boostTimersContainer;
    
    [SerializeField, Tooltip("Prefab for boost timer display")]
    private GameObject boostTimerPrefab;
    
    // Dictionary to track active boost timers
    private Dictionary<string, BoostTimerDisplay> activeTimers = new Dictionary<string, BoostTimerDisplay>();
    
    // Dictionary to track active vignette effects
    private Dictionary<string, Coroutine> activeVignettes = new Dictionary<string, Coroutine>();
    
    // Reference to player stats
    private PlayerStats playerStats;
    
    /// <summary>
    /// Initialize the manager
    /// </summary>
    private void Awake()
    {
        // Find player stats reference
        playerStats = FindFirstObjectByType<PlayerStats>();
        
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats not found in scene!");
        }
        
        // Initialize vignette
        if (vignetteImage != null)
        {
            // Set initial transparency to zero
            Color color = vignetteImage.color;
            color.a = 0;
            vignetteImage.color = color;
        }
        else
        {
            Debug.LogWarning("Vignette image not assigned!");
        }
        
        if (boostTimersContainer == null)
        {
            Debug.LogWarning("Boost timers container not assigned!");
        }
        
        if (boostTimerPrefab == null)
        {
            Debug.LogWarning("Boost timer prefab not assigned!");
        }
    }
    
    /// <summary>
    /// Displays a boost effect with vignette and timer
    /// </summary>
    /// <param name="boostType">Type of boost being applied</param>
    /// <param name="amount">Amount of the boost</param>
    /// <param name="duration">Duration of the boost in seconds</param>
    public void ShowBoostEffect(string boostType, float amount, float duration)
    {
        // Start vignette effect
        StartVignetteEffect(boostType, duration);
        
        // Create or update timer
        CreateOrUpdateTimer(boostType, amount, duration);
    }
    
    /// <summary>
    /// Creates or updates a timer display for a boost
    /// </summary>
    /// <param name="boostType">Type of boost</param>
    /// <param name="amount">Amount of boost</param>
    /// <param name="duration">Duration in seconds</param>
    private void CreateOrUpdateTimer(string boostType, float amount, float duration)
    {
        // If we already have a timer for this boost, remove it
        if (activeTimers.ContainsKey(boostType))
        {
            Destroy(activeTimers[boostType].gameObject);
            activeTimers.Remove(boostType);
        }
        
        // Create a new timer if container and prefab exist
        if (boostTimersContainer != null && boostTimerPrefab != null)
        {
            // Instantiate the timer prefab
            GameObject timerObj = Instantiate(boostTimerPrefab, boostTimersContainer);
            BoostTimerDisplay timerDisplay = timerObj.GetComponent<BoostTimerDisplay>();
            
            if (timerDisplay != null)
            {
                // Initialize the timer
                timerDisplay.Initialize(boostType, amount, duration, GetBoostColor(boostType));
                activeTimers[boostType] = timerDisplay;
            }
        }
    }
    
    /// <summary>
    /// Starts the vignette effect for a boost
    /// </summary>
    /// <param name="boostType">Type of boost</param>
    /// <param name="duration">Duration in seconds</param>
    private void StartVignetteEffect(string boostType, float duration)
    {
        // Stop existing vignette for this boost if it exists
        if (activeVignettes.ContainsKey(boostType) && activeVignettes[boostType] != null)
        {
            StopCoroutine(activeVignettes[boostType]);
        }
        
        // Start new vignette effect
        if (vignetteImage != null)
        {
            activeVignettes[boostType] = StartCoroutine(VignetteEffectCoroutine(GetBoostColor(boostType), duration));
        }
    }
    
    /// <summary>
    /// Gets the color associated with a boost type
    /// </summary>
    /// <param name="boostType">Type of boost</param>
    /// <returns>Color for the boost type</returns>
    private Color GetBoostColor(string boostType)
    {
        switch (boostType)
        {
            case "health":
                return healthBoostColor;
            case "speed":
                return speedBoostColor;
            case "strength":
                return strengthBoostColor;
            case "defense":
                return defenseBoostColor;
            case "jump":
                return jumpBoostColor;
            default:
                return Color.white;
        }
    }
    
    /// <summary>
    /// Coroutine that handles the vignette effect over time
    /// </summary>
    /// <param name="color">Color of vignette</param>
    /// <param name="duration">Duration in seconds</param>
    private IEnumerator VignetteEffectCoroutine(Color color, float duration, float fadeInTime = -1f)
    {
        // Set vignette color (with zero alpha)
        Color targetColor = color;
        Color currentColor = targetColor;
        currentColor.a = 0;
        vignetteImage.color = currentColor;
        
        // Fade in
        float elapsed = 0;
        if (fadeInTime == -1f) fadeInTime = this.fadeInTime;
        while (elapsed < fadeInTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeInTime;
            currentColor.a = Mathf.Lerp(0, targetColor.a, t);
            vignetteImage.color = currentColor;
            yield return null;
        }
        
        // Hold for duration
        yield return new WaitForSeconds(duration - fadeInTime - fadeOutTime);
        
        // Fade out
        elapsed = 0;
        float startAlpha = currentColor.a;
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeOutTime;
            currentColor.a = Mathf.Lerp(startAlpha, 0, t);
            vignetteImage.color = currentColor;
            yield return null;
        }
        
        // Ensure it's fully transparent
        currentColor.a = 0;
        vignetteImage.color = currentColor;
    }

    public void TakeDamageEffect()
    {
        StartCoroutine(VignetteEffectCoroutine(Color.red, .5f, .1f));
    }
}