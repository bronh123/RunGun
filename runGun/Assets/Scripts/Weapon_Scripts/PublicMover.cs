using UnityEngine;


// Simple script that provides public functions to apply forces to an object
public class PublicMover : MonoBehaviour
{

    Rigidbody rb;                               // Reference to rigidbody
    private NewMovement nm;                      // Reference to player's NewMovement for grounded check

    void Start()
    {
        // Get and configure the rigidbody
        nm = GameObject.Find("Player").GetComponent<NewMovement>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;  // Prevent physics from rotating the player
    }

    // Public class that allows other entities to apply a one-time force to the player
    public void ApplyImpulse(Vector3 direction, float magnitude){
        rb.AddForce(direction.normalized * magnitude, ForceMode.Impulse);
    }

    // wrapper for NewMovement isGrounded() method
    public bool isGrounded(){
        return nm.getIsGrounded();
    }
}
