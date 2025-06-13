using UnityEngine;

public class EnableMouseOnAwake : MonoBehaviour
{
    void Awake()
    {
        EnableMouse();
    }
    void Start()
    {
        EnableMouse();
    }
    void EnableMouse()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Time.timeScale = 1f;
    }
}
