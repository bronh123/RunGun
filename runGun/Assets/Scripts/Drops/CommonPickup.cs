using UnityEngine;

public class CommonPickup : MonoBehaviour
{
    // CHANGING THIS WILL ADJUST THE ASSIGNED STATS FOR THE PREFABS
    // ADD NEW STATS TO THE END OF THE LIST
    public enum StatType { CurrentHealth, Exp }

    [SerializeField] private AudioManager audioManager;

    [Header("Stat Settings")]
    [Tooltip("The player stat that this pickup will modify")]
    public StatType statToModify;
    
    [Tooltip("Amount to change the selected stat")]
    public float statChangeAmount = 10f;
    
    [Tooltip("Whether the stat boost is temporary")]
    public bool isTemporary = false;
    
    [Tooltip("Duration of temporary stat boost in seconds")]
    public float duration = 10f;

    [Header("Pickup Settings")]
    [Tooltip("Whether the pickup should be destroyed after collection")]
    public bool destroyOnPickup = true;
    
    [Tooltip("Optional visual effect to spawn when picked up")]
    public GameObject pickupEffect;
    
    [Tooltip("Optional sound to play when picked up")]
    public AudioClip pickupSound;
    
    [Header("Magnetic Effect")]
    [Tooltip("Speed at which the pickup moves toward the player when magnetized")]
    public float magnetSpeed = 100f;
    
    [Tooltip("Acceleration of the magnetic pull effect")]
    public float magnetAcceleration = 50f;
    
    // Whether the pickup is currently being pulled to the player
    private bool isBeingMagneted = false;
    
    // Reference to the player's transform
    private Transform playerTransform;
    
    // Current velocity of the magnetic movement
    private float currentMagnetSpeed;

    public float pickupDistance = 1f;

    [Header("Performance Optimization")]
    [Tooltip("How often (in frames) to update when not magnetized")]
    public int idleUpdateFrequency = 10;
    private int frameCounter = 0;

    private void OnEnable()
    {
        // Reset magnet state when object is reactivated from pool
        isBeingMagneted = false;
        currentMagnetSpeed = 0f;
        frameCounter = 0;
        audioManager = FindFirstObjectByType<AudioManager>();
    }

    private void Update()
    {
        // If not magnetized, only update occasionally to save performance
        if (!isBeingMagneted)
        {
            frameCounter++;
            if (frameCounter % idleUpdateFrequency != 0)
            {
                return; // Skip this frame
            }
        }
        
        if (isBeingMagneted && playerTransform != null)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            // Accelerate toward the player
            currentMagnetSpeed += magnetAcceleration * Time.deltaTime;
            currentMagnetSpeed = Mathf.Min(currentMagnetSpeed, magnetSpeed);
            
            // Move toward player with simple transform manipulation
            transform.position = Vector3.MoveTowards(
                transform.position, 
                playerTransform.position, 
                currentMagnetSpeed * Time.deltaTime
            );
            
            // Optional rotation effect
            transform.Rotate(Vector3.up, 180f * Time.deltaTime, Space.World);
            
            // Check if we've reached the player
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            //Debug.Log($"sucking in pickup, distance to player {distanceToPlayer}");
            if (distanceToPlayer < pickupDistance)
            {   
                //Debug.Log("trying to collect");
                CollectPickup();
            }
        }
    }

    // Called when the pickup enters the magnetic field
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger is in the player's magnetic field
        if (other.CompareTag("CommonPickupCollider") && !isBeingMagneted)
        {
            // Start magnetic effect
            isBeingMagneted = true;
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<SphereCollider>().enabled = false;
            playerTransform = other.transform.parent;
            currentMagnetSpeed = 0f;
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        // Check if the object that entered the trigger is in the player's magnetic field
        if (other.CompareTag("CommonPickupCollider") && !isBeingMagneted)
        {
            // Start magnetic effect
            isBeingMagneted = true;
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<SphereCollider>().enabled = false;
            playerTransform = other.transform.parent;
            currentMagnetSpeed = 0f;
        }
    }
    
    // Called when the pickup is collected by the player
    private void CollectPickup()
    {
        //Debug.Log($"trying to collect, pt: {playerTransform}");
        if (playerTransform != null)
        {
            // Get player's stats component
            PlayerStats playerStats = playerTransform.GetComponent<PlayerStats>();
            
            if (playerStats != null)
            {
                Debug.Log($"Collecting {statToModify} pickup with value {statChangeAmount}");
                ApplyStatModification(playerStats);
                HandlePickupEffects();
                
                if (destroyOnPickup)
                {                    
                    //Debug.Log("trying to deactive common pickup");
                    if (!GameObjectPoolManager.Deactivate(gameObject))
                    {
                        DestroyImmediate(gameObject);
                    }
                }
            }
        }
    }

    private void ApplyStatModification(PlayerStats playerStats)
    {
        switch (statToModify)
        {
            case StatType.CurrentHealth:
                playerStats.ModifyCurrentHealth(statChangeAmount);
                break;
            case StatType.Exp:
                playerStats.ModifyExp((int)statChangeAmount);
                break;
        }
    }

    private void HandlePickupEffects()
    {
        // Play sound effect if assigned
        if (pickupSound != null && audioManager != null)
        {
            audioManager.PlayAudioClipAtPlayer(pickupSound);
        }

        // Spawn visual effect if assigned
        if (pickupEffect != null)
        {
            GameObjectPoolManager.SpawnObject(pickupEffect, transform.position, Quaternion.identity);
        }
    }
}