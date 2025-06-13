using UnityEngine;

public class StateTracker : MonoBehaviour
{
    private void OnDisable()
    {
        Debug.Log($"{gameObject.name} was deactivated. Stack trace: {System.Environment.StackTrace}");
    }
    private void OnEnable()
    {
        Debug.Log($"{gameObject.name} was activated. Stack trace: {System.Environment.StackTrace}");
    }
    
}