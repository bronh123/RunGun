using UnityEngine;
using UnityEngine.UI; // Required for UI elements

public class QuitGameButton : MonoBehaviour
{
    [SerializeField] private Button quitButton;

    void Start()
    {
        if (quitButton == null) quitButton = GetComponent<Button>();
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
    }

    // This function will be called when the button is clicked
    void QuitGame()
    {
        #if UNITY_EDITOR
            // If we are running in the editor
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // If we are running a build
            Application.Quit();
        #endif
    }
}