using UnityEngine;
using TMPro;

/// <summary>
/// Handles displaying the timer value in the UI
/// </summary>
public class TimerDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;
    
    [Header("Display Settings")]
    [SerializeField] private bool showMilliseconds = false;
    [SerializeField] private bool alwaysShowHours = false;
    [SerializeField] private string timeFormat = "{0}:{1}:{2}";
    [SerializeField] private string timeFormatWithMs = "{0}:{1}:{2}.{3}";
    
    // Reference to timer manager
    private TimerManager timerManager;
    
    /// <summary>
    /// Initialize the display
    /// </summary>
    private void Start()
    {
        // Find the timer manager
        timerManager = TimerManager.Instance;
        if (timerManager == null)
        {
            timerManager = FindFirstObjectByType<TimerManager>();
        }
        
        if (timerManager == null)
        {
            Debug.LogError("TimerManager not found in scene!");
        }
        else
        {
            // Subscribe to timer updates with our UpdateDisplay method
            timerManager.onTimerUpdate.AddListener(UpdateDisplay);
        }
        
        // Initial display update
        UpdateDisplay(0f);
    }
    
    /// <summary>
    /// Updates the timer display with current elapsed time
    /// </summary>
    /// <param name="time">Current elapsed time in seconds</param>
    public void UpdateDisplay(float time)
    {
        if (timerText == null) return;
        
        // Get time components
        int hours, minutes, seconds, milliseconds;
        
        if (timerManager != null)
        {
            timerManager.GetTimeComponents(out hours, out minutes, out seconds, out milliseconds);
        }
        else
        {
            // Fallback if manager not available
            hours = Mathf.FloorToInt(time / 3600);
            minutes = Mathf.FloorToInt((time % 3600) / 60);
            seconds = Mathf.FloorToInt(time % 60);
            milliseconds = Mathf.FloorToInt((time * 1000) % 1000);
        }
        
        // Format based on settings
        if (hours > 0 || alwaysShowHours)
        {
            if (showMilliseconds)
                timerText.text = string.Format(timeFormatWithMs, hours.ToString("D2"), 
                    minutes.ToString("D2"), seconds.ToString("D2"), milliseconds.ToString("D3"));
            else
                timerText.text = string.Format(timeFormat, hours.ToString("D2"), 
                    minutes.ToString("D2"), seconds.ToString("D2"));
        }
        else
        {
            if (showMilliseconds)
                timerText.text = string.Format("{0}:{1}.{2}", minutes.ToString("D2"), 
                    seconds.ToString("D2"), milliseconds.ToString("D3"));
            else
                timerText.text = string.Format("{0}:{1}", minutes.ToString("D2"), 
                    seconds.ToString("D2"));
        }
    }
    
    /// <summary>
    /// Toggle display of milliseconds
    /// </summary>
    public void ToggleMilliseconds()
    {
        showMilliseconds = !showMilliseconds;
        if (timerManager != null)
        {
            UpdateDisplay(timerManager.GetElapsedTime());
        }
    }
    
    /// <summary>
    /// Toggle display of hours
    /// </summary>
    public void ToggleHoursDisplay()
    {
        alwaysShowHours = !alwaysShowHours;
        if (timerManager != null)
        {
            UpdateDisplay(timerManager.GetElapsedTime());
        }
    }
    
    private void OnDestroy()
    {
        // Clean up event subscription when this object is destroyed
        if (timerManager != null)
        {
            timerManager.onTimerUpdate.RemoveListener(UpdateDisplay);
        }
    }
}