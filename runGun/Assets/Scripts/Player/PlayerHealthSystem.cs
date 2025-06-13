using UnityEngine;         
using UnityEngine.UI;     


public class PlayerHealthSystem : MonoBehaviour
{
    private PlayerStats playerStats; // Reference to the player's health and stat logic

    [Header("Health Bar UI")]
    public Slider healthBarSlider; // The UI Slider that shows player's health visually

    [Header("Game Over UI")]
    public GameObject gameOverPanel; // UI panel that shows when the player dies

    [Header("Player Control Scripts")]
    public NewMovement movementScript;  // Reference to the player movement script to disable on death
    public NewCamera cameraScript;      // Reference to the camera control script to disable on death

    
    void Start()
    {
        // Get the PlayerStats component from the same GameObject
        playerStats = GetComponent<PlayerStats>();

        // If the component is missing, log an error and stop
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats not found on Player!");
            return;
        }


        // Set up the health bar slider if it's assigned
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = playerStats.maxHealth;
            healthBarSlider.value = playerStats.currentHealth;
        }

        // Hide the game over UI at the start
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    
    void Update()
    {
        // Update the health bar slider to reflect current health
        if (healthBarSlider != null)
        {
            healthBarSlider.value = playerStats.currentHealth;
        }

        // Check if player is dead
        if (playerStats.currentHealth <= 0)
        {
            // Show the Game Over panel if assigned
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            // Disable player movement
            if (movementScript != null)
                movementScript.enabled = false;

            // Disable camera look
            if (cameraScript != null)
                cameraScript.enabled = false;

            // Stop running Update() after this frame
            enabled = false;
        }
    }

}
