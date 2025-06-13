using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles scene loading functionality for UI buttons.
/// Attach this script to buttons or a manager object in your scene.
/// </summary>
public class LevelSelect : MonoBehaviour
{
    [Header("Scene Loading Buttons")]
    [SerializeField] private Button scene1Button;
    [SerializeField] private Button scene2Button;
    
    [Header("Optional Settings")]
    [SerializeField] private bool useSceneIndex = false;
    [SerializeField] private int scene1Index = 1;
    [SerializeField] private int scene2Index = 2;
    [SerializeField] private string scene1Name = "Scene1";
    [SerializeField] private string scene2Name = "Scene2";
    [SerializeField] private bool showLoadingScreen = false;
    
    // Loading screen reference
    [SerializeField] private GameObject loadingScreenPrefab;
    
    private void Start()
    {
        // Add listeners to buttons
        if (scene1Button != null)
        {
            scene1Button.onClick.AddListener(LoadScene1);
        }
        
        if (scene2Button != null)
        {
            scene2Button.onClick.AddListener(LoadScene2);
        }
    }
    
    /// <summary>
    /// Loads Scene 1 based on configuration
    /// </summary>
    public void LoadScene1()
    {
        if (useSceneIndex)
        {
            LoadSceneByIndex(scene1Index);
        }
        else
        {
            LoadSceneByName(scene1Name);
        }
    }
    
    /// <summary>
    /// Loads Scene 2 based on configuration
    /// </summary>
    public void LoadScene2()
    {
        if (useSceneIndex)
        {
            LoadSceneByIndex(scene2Index);
        }
        else
        {
            LoadSceneByName(scene2Name);
        }
    }
    
    /// <summary>
    /// Loads a scene by its build index
    /// </summary>
    /// <param name="sceneIndex">The build index of the scene to load</param>
    private void LoadSceneByIndex(int sceneIndex)
    {
        if (showLoadingScreen && loadingScreenPrefab != null)
        {
            // Instantiate loading screen
            Instantiate(loadingScreenPrefab);
            
            // Use async loading for loading screen support
            StartCoroutine(LoadSceneAsync(sceneIndex, true));
        }
        else
        {
            // Direct loading
            SceneManager.LoadScene(sceneIndex);
        }
    }
    
    /// <summary>
    /// Loads a scene by its name
    /// </summary>
    /// <param name="sceneName">The name of the scene to load</param>
    private void LoadSceneByName(string sceneName)
    {
        if (showLoadingScreen && loadingScreenPrefab != null)
        {
            // Instantiate loading screen
            Instantiate(loadingScreenPrefab);
            
            // Use async loading for loading screen support
            StartCoroutine(LoadSceneAsync(sceneName, false));
        }
        else
        {
            // Direct loading
            SceneManager.LoadScene(sceneName);
        }
    }
    
    /// <summary>
    /// Asynchronously loads a scene with a loading screen
    /// </summary>
    private System.Collections.IEnumerator LoadSceneAsync(object sceneIdentifier, bool isIndex)
    {
        AsyncOperation asyncLoad;
        
        if (isIndex)
        {
            asyncLoad = SceneManager.LoadSceneAsync((int)sceneIdentifier);
        }
        else
        {
            asyncLoad = SceneManager.LoadSceneAsync((string)sceneIdentifier);
        }
        
        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            // You can access asyncLoad.progress (0 to 1) here 
            // to update a loading bar if needed
            yield return null;
        }
    }
    
    private void OnDestroy()
    {
        // Clean up listeners to prevent memory leaks
        if (scene1Button != null)
        {
            scene1Button.onClick.RemoveListener(LoadScene1);
        }
        
        if (scene2Button != null)
        {
            scene2Button.onClick.RemoveListener(LoadScene2);
        }
    }
}