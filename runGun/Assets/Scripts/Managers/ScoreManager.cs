using UnityEngine;
using TMPro;
using UnityEngine.Events;

/// <summary>
/// Manages the player's score and provides methods to modify it
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private ScoreDisplay scoreDisplay;
    
    [SerializeField] [Tooltip("Format string for score display, {0} is replaced with score value")]
    private string scoreFormat = "Score: {0}";
    
    [SerializeField] [Tooltip("Initial score when game starts")]
    private int initialScore = 0;
    
    [Header("Events")]
    [Tooltip("Event triggered when score changes")]
    public UnityEvent<int> OnScoreChanged;
    
    [Tooltip("Event triggered when score reaches highscore")]
    public UnityEvent<int> OnHighScoreReached;
    
    private int currentScore;
    private int highScore;
    
    /// <summary>
    /// Initialize the score system
    /// </summary>
    private void Start()
    {
        // Set initial score
        currentScore = initialScore;
        
        // Load high score from player prefs
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        
        // Update UI
        UpdateScoreDisplay();
    }
    
    /// <summary>
    /// Adds points to the current score
    /// </summary>
    /// <param name="points">Amount to add</param>
    public void AddScore(int points)
    {
        currentScore += points;
        
        // Check for new high score
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            OnHighScoreReached?.Invoke(highScore);
        }
        
        // Update UI and invoke event
        UpdateScoreDisplay();
        OnScoreChanged?.Invoke(currentScore);
    }
    
    /// <summary>
    /// Updates the score display text if available
    /// </summary>
    private void UpdateScoreDisplay()
    {
        if (scoreDisplay != null)
        {
            scoreDisplay.UpdateScore(currentScore);
        }
    }
    
    /// <summary>
    /// Gets the current score
    /// </summary>
    public int GetScore()
    {
        return currentScore;
    }
    
    /// <summary>
    /// Gets the current high score
    /// </summary>
    public int GetHighScore()
    {
        return highScore;
    }
    
    /// <summary>
    /// Resets the current score to initial value
    /// </summary>
    public void ResetScore()
    {
        currentScore = initialScore;
        UpdateScoreDisplay();
        OnScoreChanged?.Invoke(currentScore);
    }
}