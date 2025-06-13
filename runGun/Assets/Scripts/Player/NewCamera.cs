using UnityEngine;

public class NewCamera : MonoBehaviour
{
    // Mouse sensitivity settings
    [Tooltip("Horizontal mouse sensitivity - higher values make camera turn faster horizontally")]
    [SerializeField] private float sensX;

    [Tooltip("Vertical mouse sensitivity - higher values make camera turn faster vertically")]
    [SerializeField] private float sensY;

    // References to important transforms
    [Tooltip("Reference to the camera transform that will be rotated")]
    [SerializeField] Transform cam;

    [Tooltip("Reference to the player orientation transform used for movement direction")]
    [SerializeField] Transform orientation;

    // Mouse input values
    private float mouseX;                              // Raw horizontal mouse input
    private float mouseY;                              // Raw vertical mouse input

    private float multiplier = 0.01f;                  // Converts raw mouse input to reasonable rotation values
    private float xRotation;                           // Current vertical camera angle (looking up/down)
    private float yRotation;                           // Current horizontal camera angle (looking left/right)

    private void Start()
    {
        // Lock and hide cursor for FPS camera control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        LoadSavedSettings();
    }

    private void LateUpdate()
    {
        // Get input and apply rotations in LateUpdate (after all other updates)
        MyInput();

        // Apply vertical rotation to camera (looking up/down)
        cam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);

        // Apply only horizontal rotation to orientation (used for movement direction)
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    void MyInput()
    {
        // Get raw mouse input
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        // Apply sensitivity and multiplier to mouse movement
        yRotation += mouseX * sensX * multiplier;   // Add to yRotation (accumulate rotation)
        xRotation -= mouseY * sensY * multiplier;   // Subtract from xRotation (invert Y axis)

        // Clamp vertical rotation to prevent over-rotation
        xRotation = Mathf.Clamp(xRotation, -89f, 89f);
    }

    public void LoadSavedSettings()
    {
        Camera camera = FindFirstObjectByType<Camera>();
        // Load camera settings
        if (PlayerPrefs.HasKey("CameraSensitivity"))
        {
            float savedSensitivity = PlayerPrefs.GetFloat("CameraSensitivity");
            SetSensitivity(savedSensitivity);
        }

        if (PlayerPrefs.HasKey("CameraFOV"))
        {
            float savedFOV = PlayerPrefs.GetFloat("CameraFOV");
            camera.fieldOfView = savedFOV;
        }
    }

    public float GetSensitivityX()
    {
        return sensX / 100;
    }
    public float GetSensitivityY()
    {
        return sensY / 100;
    }
    public float GetSensitivity()
    {
        return sensX / 100;
    }
    public void SetSensitivityX(float newSens)
    {
        sensX = newSens * 100;
    }
    public void SetSensitivityY(float newSens)
    {
        sensY = newSens * 100;
    }
    public void SetSensitivity(float newSens)
    {
        sensX = newSens * 100;
        sensY = newSens * 100;
    }
}