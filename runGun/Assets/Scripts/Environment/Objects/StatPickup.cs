using UnityEngine;

public class StatPickup : MonoBehaviour
{
    // CHANGING THIS WILL ADJUST THE ASSIGNED STATS FOR THE PREFABS
    // ADD NEW STATS TO THE END OF THE LIST
    public enum StatType { CurrentHealth, MaxHealth, Speed, Strength, Defense, Jump, CommonPickupRange }

    [Header("Stat Settings")]
    public StatType statToModify;
    public float statChangeAmount = 10f;
    public bool isTemporary = false;
    public float duration = 10f;

    [Header("Pickup Settings")]
    public bool destroyOnPickup = true;
    public GameObject pickupEffect;
    public AudioClip pickupSound;


    // Maybe change to have magnetic effect 
    private void OnTriggerEnter(Collider other)
    {

        // Check if the object that entered the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Get player's stats component
            PlayerStats playerStats = other.transform.parent.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                Debug.Log($"Attempting to adjust {statToModify} by {statChangeAmount}");
                ApplyStatModification(playerStats);
                HandlePickupEffects();

                if (destroyOnPickup)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private void ApplyStatModification(PlayerStats playerStats)
    {
        switch (statToModify)
        {
            case StatType.MaxHealth:
                if (isTemporary)
                    playerStats.ApplyTemporaryHealthBoost(statChangeAmount, duration);
                else
                    playerStats.ModifyMaxHealth(statChangeAmount);
                break;
            case StatType.CurrentHealth:
                playerStats.ModifyCurrentHealth(statChangeAmount);
                break;
            case StatType.Speed:
                if (isTemporary)
                    playerStats.ApplyTemporarySpeedBoost(statChangeAmount, duration);
                else
                    playerStats.ModifySpeed(statChangeAmount);
                break;
            case StatType.Strength:
                if (isTemporary)
                    playerStats.ApplyTemporaryStrengthBoost(statChangeAmount, duration);
                else
                    playerStats.ModifyStrength(statChangeAmount);
                break;
            case StatType.Defense:
                if (isTemporary)
                    playerStats.ApplyTemporaryDefenseBoost(statChangeAmount, duration);
                else
                    playerStats.ModifyDefense(statChangeAmount);
                break;
            case StatType.Jump:
                if (isTemporary)
                    playerStats.ApplyTemporaryJumpBoost(statChangeAmount, duration);
                else
                    playerStats.ModifyJump(statChangeAmount);
                break;
            case StatType.CommonPickupRange:
                if (isTemporary)
                    playerStats.ApplyTemporaryCommonPickupRangeBoost(statChangeAmount, duration);
                else
                    playerStats.ModifyCommonPickupRange(statChangeAmount);
                break;
        }
    }

    private void HandlePickupEffects()
    {
        // Play sound effect if assigned
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        // Spawn visual effect if assigned
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        }
    }
}