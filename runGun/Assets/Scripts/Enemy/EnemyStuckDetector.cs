using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStuckDetector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private WaveManager waveManager;

    [Header("Detection Settings")]
    [Tooltip("How often to check if enemies are stuck (in seconds)")]
    [SerializeField] private float checkInterval = 2f;

    [Tooltip("How long an enemy must be stationary to be considered stuck (in seconds)")]
    [SerializeField] private float stuckThreshold = 5f;

    [Tooltip("Minimum distance an enemy must move between checks to not be considered stuck")]
    [SerializeField] private float minMovementDistance = 0.2f;

    [Tooltip("Maximum number of respawn attempts per enemy")]
    [SerializeField] private int maxRespawnAttempts = 3;

    // Dictionary to track positions and stuck times
    private Dictionary<GameObject, StuckData> stuckTracking = new Dictionary<GameObject, StuckData>();

    // Class to store stuck data per enemy
    private class StuckData
    {
        public Vector3 lastPosition;
        public float stuckTime;
        public int respawnAttempts;

        public StuckData(Vector3 position)
        {
            lastPosition = position;
            stuckTime = 0f;
            respawnAttempts = 0;
        }
    }

    private void Start()
    {
        // Find references if not assigned
        if (enemyManager == null)
        {
            enemyManager = FindFirstObjectByType<EnemyManager>();
            if (enemyManager == null)
            {
                Debug.LogError("EnemyStuckDetector: EnemyManager not found");
                enabled = false;
                return;
            }
        }

        if (waveManager == null)
        {
            waveManager = FindFirstObjectByType<WaveManager>();
        }

        // Start checking for stuck enemies
        StartCoroutine(CheckEnemies());
    }

    private IEnumerator CheckEnemies()
    {
        while (true)
        {
            // Wait for interval
            yield return new WaitForSeconds(checkInterval);

            // Find all active enemies
            Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

            // First register any new enemies
            foreach (Enemy enemy in enemies)
            {
                if (!stuckTracking.ContainsKey(enemy.gameObject))
                {
                    stuckTracking.Add(enemy.gameObject, new StuckData(enemy.transform.position));
                }
            }

            // Check each tracked enemy
            List<GameObject> enemiesToRespawn = new List<GameObject>();

            foreach (var pair in new Dictionary<GameObject, StuckData>(stuckTracking))
            {
                GameObject enemy = pair.Key;
                StuckData data = pair.Value;

                // Skip destroyed or inactive enemies
                if (enemy == null || !enemy.activeInHierarchy)
                {
                    stuckTracking.Remove(enemy);
                    continue;
                }

                // Get current position and check movement
                Vector3 currentPosition = enemy.transform.position;
                float distanceMoved = Vector3.Distance(currentPosition, data.lastPosition);

                // Check NavMeshAgent status if available
                NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                bool isAgentStuck = agent != null &&
                                   !agent.isStopped &&
                                   agent.pathStatus != NavMeshPathStatus.PathInvalid &&
                                   agent.remainingDistance > agent.stoppingDistance;

                // Update position
                data.lastPosition = currentPosition;

                // If enemy hasn't moved enough and its NavMeshAgent is trying to move
                if (distanceMoved < minMovementDistance && isAgentStuck)
                {
                    data.stuckTime += checkInterval;

                    // Check if stuck long enough to respawn
                    if (data.stuckTime >= stuckThreshold)
                    {
                        if (data.respawnAttempts < maxRespawnAttempts)
                        {
                            enemiesToRespawn.Add(enemy);
                            data.respawnAttempts++;
                            data.stuckTime = 0f;
                        }
                        else
                        {
                            // Force deactivation and remove from tracking
                            GameObjectPoolManager.Deactivate(enemy);
                            stuckTracking.Remove(enemy);
                        }
                    }
                }
                else
                {
                    // Reset stuck time if enemy is moving
                    data.stuckTime = 0f;
                }
            }

            // Respawn stuck enemies
            foreach (GameObject enemyToRespawn in enemiesToRespawn)
            {
                RespawnEnemy(enemyToRespawn);
            }
        }
    }

    private void RespawnEnemy(GameObject enemy)
    {
        // Get current wave number
        int currentWave = waveManager != null ? waveManager.GetCurrentWave() : 1;

        // First deactivate the stuck enemy
        GameObjectPoolManager.Deactivate(enemy);

        // Spawn a replacement using the public method
        enemyManager.RespawnEnemy(currentWave);
    }
}