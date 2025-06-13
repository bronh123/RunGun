using UnityEngine;

public class WeaponRecoilAnimation : MonoBehaviour
{
    // Recoil settings
    public float recoilDistance = 0.3f;         // How far back the weapon goes
    public float recoilUpAngle = 15f;           // How much the weapon rotates upward
    public float recoilDuration = 0.1f;         // Time to reach max recoil
    public float returnDuration = 0.2f;         // Time to return to original position
    
    // Internal variables
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isRecoiling = false;
    private bool isReturning = false;
    private float recoilTimer = 0f;
    private float returnTimer = 0f;
    
    private void Start()
    {
        // Store the original position and rotation
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }
    
    private void Update()
    {
        // Handle recoil animation
        if (isRecoiling)
        {
            recoilTimer += Time.deltaTime;
            float t = recoilTimer / recoilDuration;
            
            // Calculate recoil position and rotation
            Vector3 recoilPos = originalPosition - Vector3.forward * recoilDistance * t;
            Quaternion recoilRot = originalRotation * Quaternion.Euler(-recoilUpAngle * t, 0, 0);
            
            // Apply recoil
            transform.localPosition = recoilPos;
            transform.localRotation = recoilRot;
            
            // Complete recoil phase
            if (recoilTimer >= recoilDuration)
            {
                isRecoiling = false;
                isReturning = true;
                returnTimer = 0f;
            }
        }
        
        // Handle return animation
        if (isReturning)
        {
            returnTimer += Time.deltaTime;
            float t = returnTimer / returnDuration;
            
            // Calculate return position and rotation (easing out)
            Vector3 recoilPos = originalPosition - Vector3.forward * recoilDistance * (1 - t);
            Quaternion recoilRot = originalRotation * Quaternion.Euler(-recoilUpAngle * (1 - t), 0, 0);
            
            // Apply return
            transform.localPosition = recoilPos;
            transform.localRotation = recoilRot;
            
            // Complete return phase
            if (returnTimer >= returnDuration)
            {
                isReturning = false;
                transform.localPosition = originalPosition;
                transform.localRotation = originalRotation;
            }
        }
    }
    
    // Call this method to trigger the recoil animation
    public void TriggerRecoil()
    {
        // Store original position/rotation again in case they changed
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        
        // Start recoil sequence
        isRecoiling = true;
        isReturning = false;
        recoilTimer = 0f;
    }
    
    // Method to stop animation and reset position
    public void ResetRecoil()
    {
        isRecoiling = false;
        isReturning = false;
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
    }
}