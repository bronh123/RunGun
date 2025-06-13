using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;

    [Header("Camera Settings")]
    [SerializeField] private NewCamera cameraController;

    [Header("Sensitivity Settings")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TextMeshProUGUI sensitivityValueText;
    [SerializeField] private float minSensitivity = 0.1f;
    [SerializeField] private float maxSensitivity = 10f;

    [Header("FOV Settings")]
    [SerializeField] private Slider fovSlider;
    [SerializeField] private TextMeshProUGUI fovValueText;
    [SerializeField] private float minFOV = 60f;
    [SerializeField] private float maxFOV = 120f;

    [Header("UI Elements")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button cancelButton;

    // Store original values in case user cancels
    private float originalSensitivity;
    private float originalFOV;

    private void Start()
    {
        // Initialize and set up UI elements
        if (cameraController == null)
        {
            cameraController = FindFirstObjectByType<NewCamera>();
            if (cameraController == null)
            {
                Debug.LogError("No NewCamera script found in the scene");
            }
        }
        if (playerCamera == null) playerCamera = FindFirstObjectByType<Camera>();

        // Set up camera setting listeners
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        fovSlider.onValueChanged.AddListener(OnFOVChanged);


        // Set up button listeners
        saveButton.onClick.AddListener(SaveSettings);
        cancelButton.onClick.AddListener(CancelSettings);

        // Initialize camera slider ranges
        sensitivitySlider.minValue = minSensitivity;
        sensitivitySlider.maxValue = maxSensitivity;

        fovSlider.minValue = minFOV;
        fovSlider.maxValue = maxFOV;

        // Load saved settings
        LoadSavedSettings();
    }

    public void OpenSettings()
    {
        // Show settings panel
        settingsPanel.SetActive(true);
        // Store original camera values
        originalSensitivity = cameraController.GetSensitivity();
        originalFOV = playerCamera.fieldOfView;

        // Set camera sliders to current values
        sensitivitySlider.value = originalSensitivity;
        fovSlider.value = originalFOV;


        // Update text displays for camera settings
        UpdateSensitivityText(originalSensitivity);
        UpdateFOVText(originalFOV);
    }

    private void OnSensitivityChanged(float value)
    {
        // Update the camera sensitivity in real-time
        // Apply the same sensitivity to both X and Y
        cameraController.SetSensitivity(value);
        UpdateSensitivityText(value);
    }

    private void OnFOVChanged(float value)
    {
        // Update the camera FOV in real-time
        playerCamera.fieldOfView = value;
        UpdateFOVText(value);
    }

    private void UpdateSensitivityText(float value)
    {
        sensitivityValueText.text = value.ToString("F1");
    }

    private void UpdateFOVText(float value)
    {
        fovValueText.text = value.ToString("F0") + "°";
    }

    private void SaveSettings()
    {
        // Save camera settings to PlayerPrefs
        PlayerPrefs.SetFloat("CameraSensitivity", cameraController.GetSensitivity());
        PlayerPrefs.SetFloat("CameraFOV", playerCamera.fieldOfView);

        PlayerPrefs.Save();

        CloseSettings();
    }

    private void CancelSettings()
    {
        // Revert camera values to original
        cameraController.SetSensitivity(originalSensitivity);
        playerCamera.fieldOfView = originalFOV;

        CloseSettings();
    }

    private void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void LoadSavedSettings()
    {
        // Load camera settings
        if (PlayerPrefs.HasKey("CameraSensitivity"))
        {
            float savedSensitivity = PlayerPrefs.GetFloat("CameraSensitivity");
            cameraController.SetSensitivity(savedSensitivity);
            UpdateSensitivityText(savedSensitivity);
        }

        if (PlayerPrefs.HasKey("CameraFOV"))
        {
            float savedFOV = PlayerPrefs.GetFloat("CameraFOV");
            playerCamera.fieldOfView = savedFOV;
            UpdateFOVText(savedFOV);
        }
    }
}