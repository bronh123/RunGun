using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    public PlayerStats playerStats;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Death plane triggered");
            playerStats.TakeDamage(1000000f);
        }
    }
}
