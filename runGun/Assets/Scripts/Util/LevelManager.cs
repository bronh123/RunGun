using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class LevelManager : MonoBehaviour
{
    public void RestartScene()
    {
        Debug.Log("Restarting Scene");
        Time.timeScale = 1; // Make sure to reset time scale first
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}