using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the game over state when player reaches the final wave
/// </summary>
public class GameOverManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private TimerManager timerManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] PlayerStats playerStats;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip victorySound;

    private float gameStartTime;
    private bool gameEnded = false;

    private void Start()
    {
        // Find managers
        if (waveManager == null)
        {
            waveManager = FindFirstObjectByType<WaveManager>();
        }
        if (scoreManager == null)
        {
            scoreManager = FindFirstObjectByType<ScoreManager>();
        }

        if (timerManager == null)
        {
            timerManager = FindFirstObjectByType<TimerManager>();
        }
        if (playerStats == null)
        {
            playerStats = FindFirstObjectByType<PlayerStats>();
        }

    }

    private void Update()
    {
        CheckDeath();
    }


    public void TriggerGameComplete(bool win = true)
    {
        if (gameEnded) return;

        gameEnded = true;
        Time.timeScale = 0f;
        timerManager.ToggleTimer();

        string gameDuration = timerManager.GetFormattedTime();
        int score = scoreManager.GetScore();
        int waveNumber = waveManager.GetCurrentWave();
        Debug.Log("trying to display game over");
        gameOverUI.ShowGameOverUI(gameDuration, score, waveNumber, win);

        // Play victory sound
        if (win && audioSource != null && victorySound != null)
        {
            audioSource.PlayOneShot(victorySound);
        }
        // else play loss sound?
    }
    void CheckDeath()
    {
        if (playerStats.currentHealth <= 0)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            TriggerGameComplete(false);
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Reset time scale
        SceneManager.LoadScene("Main Menu"); // Replace with your main menu scene name
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Reset time scale
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}