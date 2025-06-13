using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifeTime = 5f;
    public int damage = 10;
    public bool useIsTrigger = true; // Use trigger collider instead of physics collider
    
    private bool hasHitPlayer = false;
    private Rigidbody rb;
    private Collider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        
        // If no Rigidbody, add one
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // Configure Rigidbody for projectile
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        
        // Prevent bouncing
        if (useIsTrigger && col != null)
        {
            col.isTrigger = true; // Use trigger instead of physics collision
        }
        else
        {
            // Prevent bouncing with physics settings
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            
        }
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.gameObject);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        HandleHit(other.gameObject);
    }
    
    private void HandleHit(GameObject hitObject)
    {
        if (hasHitPlayer) return; // Prevent double-hits
        
        if (hitObject.CompareTag("Player"))
        {
            Debug.Log("Hit Player!");
            
            // Freeze the projectile immediately
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
            
            PlayerStats playerStats = hitObject.GetComponent<PlayerStats>();
            if (playerStats == null)
            {
                // Try to find PlayerStats on parent
                playerStats = hitObject.GetComponentInParent<PlayerStats>();
            }
            
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
                hasHitPlayer = true;
                
                // Destroy after a tiny delay to ensure damage is registered
                Destroy(gameObject, 0.01f);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else if (hitObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}