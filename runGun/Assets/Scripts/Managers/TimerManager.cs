using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages game time tracking and provides timer functionality
/// </summary>
public class TimerManager : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private bool autoStart = true;
    
    [Header("Events")]
    // Making events public so they can be accessed by other scripts
    public UnityEvent onTimerStart = new UnityEvent();
    public UnityEvent<float> onTimerUpdate = new UnityEvent<float>(); // Passes current time
    public UnityEvent onTimerStop = new UnityEvent();
    
    // Timer state
    private float elapsedTime = 0f;
    private bool isRunning = false;
    
    /// <summary>
    /// Singleton instance
    /// </summary>
    private static TimerManager _instance;
    public static TimerManager Instance 
    {
        get { return _instance; }
    }
    
    /// <summary>
    /// Initialize the timer
    /// </summary>
    private void Awake()
    {
        // Singleton pattern
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        
        // Optional: make the timer persist between scenes
        // DontDestroyOnLoad(gameObject);
    }
    
    /// <summary>
    /// Start the timer if autoStart is enabled
    /// </summary>
    private void Start()
    {
        if (autoStart)
        {
            StartTimer();
        }
    }
    
    /// <summary>
    /// Update the timer each frame if running
    /// </summary>
    private void Update()
    {
        if (isRunning)
        {
            // Increment time
            elapsedTime += Time.deltaTime;
            
            // Notify listeners
            onTimerUpdate?.Invoke(elapsedTime);
        }
    }
    
    /// <summary>
    /// Starts the timer
    /// </summary>
    public void StartTimer()
    {
        isRunning = true;
        onTimerStart?.Invoke();
    }
    
    /// <summary>
    /// Stops the timer
    /// </summary>
    public void StopTimer()
    {
        isRunning = false;
        onTimerStop?.Invoke();
    }
    
    /// <summary>
    /// Resets the timer to zero
    /// </summary>
    public void ResetTimer()
    {
        elapsedTime = 0f;
        onTimerUpdate?.Invoke(elapsedTime);
    }
    
    /// <summary>
    /// Restarts the timer from zero
    /// </summary>
    public void RestartTimer()
    {
        ResetTimer();
        StartTimer();
    }
    
    /// <summary>
    /// Toggles the timer between running and stopped
    /// </summary>
    public void ToggleTimer()
    {
        if (isRunning)
            StopTimer();
        else
            StartTimer();
    }
    
    /// <summary>
    /// Gets the current elapsed time in seconds
    /// </summary>
    public float GetElapsedTime() => elapsedTime;
    
    /// <summary>
    /// Checks if the timer is currently running
    /// </summary>
    public bool IsRunning() => isRunning;
    
    /// <summary>
    /// Gets the formatted time string (HH:MM:SS or MM:SS)
    /// </summary>
    public string GetFormattedTime()
    {
        int hours = Mathf.FloorToInt(elapsedTime / 3600);
        int minutes = Mathf.FloorToInt((elapsedTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        
        if (hours > 0)
            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        else
            return $"{minutes:D2}:{seconds:D2}";
    }
    
    /// <summary>
    /// Gets the time components separately
    /// </summary>
    public void GetTimeComponents(out int hours, out int minutes, out int seconds, out int milliseconds)
    {
        hours = Mathf.FloorToInt(elapsedTime / 3600);
        minutes = Mathf.FloorToInt((elapsedTime % 3600) / 60);
        seconds = Mathf.FloorToInt(elapsedTime % 60);
        milliseconds = Mathf.FloorToInt((elapsedTime * 1000) % 1000);
    }
}