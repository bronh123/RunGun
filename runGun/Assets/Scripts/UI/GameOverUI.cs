using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ScriptManager scriptManager;
    [SerializeField] private GameOverManager gameOverManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI winLossText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button tryAgainButton;

    public void Start()
    {
        if (gameOverPanel && gameOverPanel.activeInHierarchy) gameOverPanel.SetActive(false);
        if (gameOverManager != null)
        {
            // Setup button listeners
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(gameOverManager.GoToMainMenu);
            if (tryAgainButton != null)
                tryAgainButton.onClick.AddListener(gameOverManager.RestartGame);
        }
        if (scriptManager == null)
        {
            scriptManager = FindFirstObjectByType<ScriptManager>();
        }
        if (scoreManager == null) scoreManager = FindFirstObjectByType<ScoreManager>();
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }
    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    public void ShowGameOverUI(string gameDuration = "", int score = 0, int waveNumber = 0, bool win = false)
    {
        scriptManager.DisablePlayerScripts();
        scriptManager.DisableObjects();
        gameOverPanel.SetActive(true);
        timeText.text = $"Time: {gameDuration}";
        scoreText.text = $"Score: {score.ToString()}";
        waveText.text = $"Wave Reached: {waveNumber.ToString()}";
        if (highScoreText) highScoreText.text = $"High Score: {scoreManager.GetHighScore()}";
        if (win)
        {
            winLossText.text = $"VICTORY";
        }
        else
        {
            winLossText.text = "YOU DIED";
        }
    }

}
