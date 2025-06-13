using UnityEngine;

public class CommonPickupRbBehavior : MonoBehaviour
{
    public Rigidbody rb;
    public LayerMask groundMask;
    public float groundDistance = 1f;
    public float velocityThreshold = 0.1f;
    public int checkFrequency = 15; // Only check every X frames
    
    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        // Only check periodically to reduce performance impact
        if (Time.frameCount % checkFrequency != 0) return;
        
        // Skip if already kinematic
        if (rb.isKinematic) return;
        
        // Check if velocity is low enough to consider "settled"
        if (rb.linearVelocity.sqrMagnitude < velocityThreshold * velocityThreshold)
        {
            if (Physics.CheckSphere(transform.position, groundDistance, groundMask))
            {
                Debug.Log("setting exp rb to kinematic");
                rb.isKinematic = true;
                // Instead of disabling completely, we just check less frequently
                checkFrequency = 60;
            }
        }
    }
}