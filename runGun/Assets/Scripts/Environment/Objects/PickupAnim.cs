using UnityEngine;

public class PickupAnim : MonoBehaviour
{
    public float rotateSpeed = 100f;
    public float bobHeight = 0.5f;
    public float bobSpeed = 2f;
    
    private Vector3 startPosition;
    
    void Start()
    {
        startPosition = transform.position;
    }
    
    void Update()
    {
        // Rotate the pickup
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        
        // Bob up and down
        Vector3 position = startPosition;
        position.y += Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = position;
    }
}
