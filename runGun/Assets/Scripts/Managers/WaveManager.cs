using System;
using UnityEngine;

/// <summary>
/// Manages wave progression, timing, and difficulty scaling
/// </summary>
public class WaveManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] 
    [Tooltip("Reference to the GameOverManager for handling game completion")]
    private GameOverManager gameOverManager;
    
    [SerializeField] 
    [Tooltip("Reference to the EnemyManager for spawning enemies")]
    private EnemyManager enemyManager;

    [Header("UI")]
    [SerializeField] 
    [Tooltip("Reference to the WaveDisplay for UI updates")]
    private WaveDisplay waveDisplay;

    [Header("Wave Settings")]
    [SerializeField] 
    [Tooltip("Wave number to start the game on")]
    private int startingWave = 0;
    
    [SerializeField] 
    [Tooltip("Maximum number of waves before game completion")]
    private int maxWaves = 30;
    
    [SerializeField] 
    [Tooltip("Time in seconds between each wave")]
    private float timeBetweenWaves = 10f;
    
    [SerializeField] 
    [Tooltip("Delay in seconds after all enemies are defeated before confirming wave completion")]
    private float waveCompletionDelay = 3f;
    [SerializeField, Tooltip("Time it takes for game to end if stuck on final wave in seconds")]
    private float maxElapsedTimeOnFinalWave = 30f;

    [Header("Difficulty Settings")]
    [SerializeField] 
    [Tooltip("Factor by which enemy count increases with each wave")]
    private float enemyCountIncreaseFactor = 0.1f;
    
    [SerializeField] 
    [Tooltip("Curve that controls how difficulty scales throughout the waves (X: 0-1 progress, Y: difficulty multiplier)")]
    private AnimationCurve difficultyCurve = AnimationCurve.Linear(0, 1, 100, 3);


    // Wave state
    private int currentWave;
    private float elapsedTime = 8f;
    private bool isWaveActive = false;
    private bool isCheckingCompletion = false;

    private float elapsedTimeOnFinalWave = 0f;

    /// <summary>
    /// Initialize the wave system
    /// </summary>
    void Start()
    {
        // Validate required reference
        if (enemyManager == null)
        {
            enemyManager = FindFirstObjectByType<EnemyManager>();
            if (enemyManager == null)
            {
                Debug.LogError("EnemyManager not assigned and couldn't be found!");
            }
        }

        if (gameOverManager == null)
        {
            gameOverManager = FindFirstObjectByType<GameOverManager>();
            if (gameOverManager == null)
            {
                Debug.LogError("GameOverManager not assigned and couldn't be found!");
            }
        }

        // Start from the configured wave
        currentWave = startingWave;
    }

    /// <summary>
    /// Handles wave timing and completion checks
    /// </summary>
    void Update()
    {
        elapsedTime += Time.deltaTime;
        // Start a new wave when it's time
        if (elapsedTimeOnFinalWave >= maxElapsedTimeOnFinalWave)
        {
            CompleteWave();
        }
        if (currentWave == maxWaves)
        {
            elapsedTimeOnFinalWave += Time.deltaTime;
        } else if (elapsedTime >= timeBetweenWaves && currentWave < maxWaves)
        {
            StartNextWave();
        }

        // Check for wave completion if a wave is active
        if (!isCheckingCompletion && enemyManager.GetActiveEnemyCount() < 10)
        {
            StartCoroutine(CheckWaveCompletion());
        }
    }

    /// <summary>
    /// Starts the next wave by spawning enemies
    /// </summary>
    private void StartNextWave()
    {
        currentWave++;

        // Check if we've reached the final wave
        if (currentWave > maxWaves)
        {
            // Game complete logic could go here
            Debug.Log("Maximum wave reached, this should not happen here");
            return;
        }

        isWaveActive = true;

        // Calculate difficulty multiplier based on wave number
        float difficultyMultiplier = GetWaveDifficultyMultiplier();

        // Spawn the wave with the current difficulty
        enemyManager.SpawnWave(currentWave, difficultyMultiplier);
        elapsedTime = 0;

        Debug.Log($"Wave {currentWave} started with {enemyManager.GetCurrentWaveSize()} enemies!");

    }

    /// <summary>
    /// Waits briefly after all enemies are defeated to confirm wave completion
    /// </summary>
    private System.Collections.IEnumerator CheckWaveCompletion()
    {
        isCheckingCompletion = true;

        // Wait to make sure all enemies are truly gone
        yield return new WaitForSeconds(waveCompletionDelay);

        // Double-check that enemies are still gone
        if (enemyManager.GetActiveEnemyCount() == 0)
        {
            CompleteWave();
        }

        isCheckingCompletion = false;
    }

    /// <summary>
    /// Marks the current wave as complete and prepares for the next one
    /// </summary>
    private void CompleteWave()
    {
        isWaveActive = false;

        Debug.Log($"Wave {currentWave} completed");

        // If this was the final wave, trigger the game end event
        if (currentWave >= maxWaves)
        {
            gameOverManager.TriggerGameComplete();
        }
        else
        {
            StartNextWave();
        }
    }

    /// <summary>
    /// Calculates the difficulty multiplier for the current wave
    /// </summary>
    private float GetWaveDifficultyMultiplier()
    {
        // Base multiplier from difficulty curve
        float baseMultiplier = difficultyCurve.Evaluate((float)currentWave / maxWaves);

        // Additional enemy count scaling
        float enemyCountMultiplier = 1.0f + (currentWave * enemyCountIncreaseFactor);

        return baseMultiplier * enemyCountMultiplier;
    }

    /// <summary>
    /// Gets the current wave number
    /// </summary>
    public int GetCurrentWave()
    {
        return currentWave;
    }

    /// <summary>
    /// Gets the maximum number of waves
    /// </summary>
    public int GetMaxWaves()
    {
        return maxWaves;
    }

    /// <summary>
    /// Checks if the final wave has been completed
    /// </summary>
    public bool IsFinalWaveCompleted()
    {
        return currentWave >= maxWaves && !isWaveActive;
    }
}