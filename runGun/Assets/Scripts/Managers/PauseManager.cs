using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject playerObject;
    [SerializeField] private ScriptManager scriptManager;
    [SerializeField] private GameObject statsDisplay;
    [SerializeField] private GameObject upgradeUI;
    public Button homeButton; //  hook this up in the Inspector
    public Button resumeButton;
    public Button restartButton;
    public Button pauseButton;

    private bool isPaused = false;

    void Start()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (homeButton != null) homeButton.onClick.AddListener(ReturnToMainMenu); // sets up click event
        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        if (pauseButton != null) pauseButton.onClick.AddListener(PauseGame);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }


    void PauseGame()
    {
        if (pauseMenu != null) pauseMenu.SetActive(true);
        if (statsDisplay != null) statsDisplay.SetActive(true);
        scriptManager.DisablePlayerScripts();
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ResumeGame()
    {
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (statsDisplay != null) statsDisplay.SetActive(false);
        scriptManager.ReenablePlayerScripts();
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


        isPaused = false;


    }

    void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    void RestartGame()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

}



