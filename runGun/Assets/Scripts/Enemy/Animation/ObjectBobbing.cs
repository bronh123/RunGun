using UnityEngine;

public class ObjectBobbing : MonoBehaviour
{
    [Tooltip("How far the object moves up")]
    [SerializeField] private float amplitude = 0.5f;
    
    [Tooltip("How fast the object bobs")]
    [SerializeField] private float frequency = 1f;
    
    [Tooltip("Random phase offset between 0-1 to make objects bob differently")]
    [SerializeField] private bool randomizePhase = true;
    
    // Manual phase offset if not using randomization
    [SerializeField] private float manualPhaseOffset = 0f;
    [SerializeField] private float yOffset = .5f;
    
    // Starting position of the object
    private Vector3 startPosition;
    
    // The phase offset for this specific instance
    private float phaseOffset;
    
    void Start()
    {
        // Store the initial position
        startPosition = transform.position;
        
        // Set a random phase offset if enabled
        if (randomizePhase)
        {
            phaseOffset = Random.Range(0f, Mathf.PI * 2); // Random offset between 0 and 2π
        }
        else
        {
            phaseOffset = manualPhaseOffset;
        }
    }

    void Update()
    {
        // Use absolute value of sine wave to make it bob only upward
        // Subtract 1 and add 1 to make it range from 0 to 1 instead of -1 to 1
        float bobFactor = (Mathf.Sin(frequency * Time.time + phaseOffset) + 1) * 0.5f;
        
        // Calculate new Y position that's only above the starting position
        float newY = startPosition.y + yOffset + amplitude * bobFactor;
        
        // Update the object's position, keeping X and Z the same
        transform.position = new Vector3(
            transform.position.x, 
            newY, 
            transform.position.z
        );
    }
}