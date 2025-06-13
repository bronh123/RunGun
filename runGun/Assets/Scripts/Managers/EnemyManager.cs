using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handles the spawning of enemies and their initial setup
/// </summary>
public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject flyingEnemyPrefab;
    [SerializeField] private GameObject elitePrefab;

    [SerializeField] private Transform player;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private WaypointHolder waypointHolder;

    [Header("Spawning")]
    [SerializeField] private int baseEnemiesPerWave = 10;
    [SerializeField] private float flyingEnemyRatio = 0.5f; // Percentage of enemies that should be flying
    [SerializeField] private int eliteSpawningInterval = 5;

    [Header("Enemy Stat Scaling")]
    [SerializeField] private float baseHealthMultiplierIncreasePerWave = .1f;
    [SerializeField] private float baseDamageMultiplierIncreasePerWave = .1f;
    [SerializeField] private float baseSpeedMultiplierIncreasePerWave = .05f;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private Mesh navMesh;
    private int currentWaveSize;

    /// <summary>
    /// Initialize the NavMesh for spawning positions
    /// </summary>
    void Start()
    {
        // Create a mesh from the NavMesh triangulation
        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
        navMesh = new Mesh
        {
            vertices = triangulation.vertices,
            triangles = triangulation.indices
        };

        // Check for waypointHolder
        if (waypointHolder == null)
        {
            waypointHolder = FindFirstObjectByType<WaypointHolder>();
            if (waypointHolder == null)
            {
                Debug.LogWarning("No WaypointHolder found in scene! Flying enemies won't spawn.");
            }
        }
    }

    /// <summary>
    /// Update the list of active enemies by removing any destroyed ones
    /// </summary>
    void Update()
    {
        // Clean up our list of enemies
        spawnedEnemies.RemoveAll(enemy => enemy == null);
    }

    /// <summary>
    /// Spawns a wave of enemies
    /// </summary>
    /// <param name="waveNumber">Current wave number</param>
    /// <param name="difficultyMultiplier">Optional multiplier for enemies per wave</param>
    public void SpawnWave(int waveNumber, float difficultyMultiplier = 1.0f)
    {
        // Calculate number of enemies based on wave number and difficulty
        currentWaveSize = Mathf.RoundToInt(baseEnemiesPerWave * difficultyMultiplier);

        // Calculate how many flying vs ground enemies to spawn
        int flyingEnemyCount = Mathf.RoundToInt(currentWaveSize * flyingEnemyRatio);
        int groundEnemyCount = currentWaveSize - flyingEnemyCount;

        // Spawn ground enemies
        for (int i = 0; i < groundEnemyCount; i++)
        {
            SpawnGroundEnemy(waveNumber);
        }

        // Spawn flying enemies if waypoints are available
        if (waypointHolder != null && waypointHolder.Waypoints != null && waypointHolder.Waypoints.Length > 0)
        {
            for (int i = 0; i < flyingEnemyCount; i++)
            {
                SpawnFlyingEnemy(waveNumber);
            }
        }
        else
        {
            // If no waypoints, spawn all as ground enemies
            for (int i = 0; i < flyingEnemyCount; i++)
            {
                SpawnGroundEnemy(waveNumber);
            }
            Debug.LogWarning("No waypoints found for flying enemies! Spawning ground enemies instead.");
        }

        // Spawn a single elite enemy every 5 waves
        if (waveNumber % eliteSpawningInterval == 0 && waveNumber > 1)
        {
            Debug.Log("Spawning elite enemy");
            SpawnGroundEnemy(waveNumber, true);
        }
    }

    /// <summary>
    /// Spawns a single ground enemy at a random position
    /// </summary>
    /// <param name="waveNumber">Current wave number to scale enemy difficulty</param>
    private void SpawnGroundEnemy(int waveNumber, bool elite = false)
    {
        // Get a random position on the NavMesh
        Vector3 spawnPosition = GetRandomNavmeshPosition();

        // Spawn the enemy from pool

        GameObject enemy = GameObjectPoolManager.SpawnObject(elite ? elitePrefab : enemyPrefab, spawnPosition, Quaternion.identity);

        // Set up enemy references using the base EnemyStats
        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
        if (enemyStats != null && player != null)
        {
            enemyStats.player = player;
            enemyStats.playerStats = playerStats;

            // Scale enemy stats based on wave number
            float healthMultiplier = 1.0f + (baseHealthMultiplierIncreasePerWave * waveNumber);
            float damageMultiplier = 1.0f + (baseDamageMultiplierIncreasePerWave * waveNumber);
            float speedMultiplier = 1.0f + (baseSpeedMultiplierIncreasePerWave * waveNumber);

            enemyStats.SetStatMultipliers(healthMultiplier, damageMultiplier, speedMultiplier);
        }

        // Add to our tracking list
        spawnedEnemies.Add(enemy);
    }

    /// <summary>
    /// Spawns a single flying enemy at a random waypoint
    /// </summary>
    /// <param name="waveNumber">Current wave number to scale enemy difficulty</param>
    private void SpawnFlyingEnemy(int waveNumber)
    {
        // Get a random waypoint position
        Vector3 spawnPosition = GetRandomWaypoint();

        // Spawn the flying enemy from pool
        GameObject enemy = GameObjectPoolManager.SpawnObject(flyingEnemyPrefab, spawnPosition, Quaternion.identity);

        // Set up flying enemy references
        FlyingEnemy flyingEnemy = enemy.GetComponent<FlyingEnemy>();
        if (flyingEnemy != null && player != null)
        {
            flyingEnemy.player = player;
            flyingEnemy.playerStats = playerStats;
            flyingEnemy.waypointHolder = waypointHolder;

            // Scale enemy stats based on wave number
            float healthMultiplier = 1.0f + (baseHealthMultiplierIncreasePerWave * waveNumber);
            float damageMultiplier = 1.0f + (baseDamageMultiplierIncreasePerWave * waveNumber);
            float speedMultiplier = 1.0f + (baseSpeedMultiplierIncreasePerWave * waveNumber);

            flyingEnemy.SetStatMultipliers(healthMultiplier, damageMultiplier, speedMultiplier);
        }

        // Add to our tracking list
        spawnedEnemies.Add(enemy);
    }

    /// <summary>
    /// Gets a random waypoint position for flying enemies
    /// </summary>
    private Vector3 GetRandomWaypoint()
    {
        if (waypointHolder == null || waypointHolder.Waypoints == null || waypointHolder.Waypoints.Length == 0)
        {
            Debug.LogError("No waypoints available for flying enemies!");
            return Vector3.zero;
        }

        return waypointHolder.Waypoints[Random.Range(0, waypointHolder.Waypoints.Length)].position;
    }

    /// <summary>
    /// Gets a random position on the NavMesh
    /// </summary>
    private Vector3 GetRandomNavmeshPosition()
    {
        return GetRandomPointOnMesh(navMesh);
    }

    /// <summary>
    /// Respawns an enemy that might be stuck
    /// </summary>
    /// <param name="waveNumber">Current wave number for difficulty scaling</param>
    public void RespawnEnemy(int waveNumber)
    {
        SpawnGroundEnemy(waveNumber);
    }

    /// <summary>
    /// Gets a random point on a mesh using weighted triangle selection
    /// </summary>
    private Vector3 GetRandomPointOnMesh(Mesh mesh)
    {
        // Calculate triangle sizes for weighted selection
        float[] sizes = GetTriSizes(mesh.triangles, mesh.vertices);
        float[] cumulativeSizes = new float[sizes.Length];
        float total = 0;

        for (int i = 0; i < sizes.Length; i++)
        {
            total += sizes[i];
            cumulativeSizes[i] = total;
        }

        // Select a random triangle based on size
        float randomsample = Random.value * total;
        int triIndex = -1;

        for (int i = 0; i < sizes.Length; i++)
        {
            if (randomsample <= cumulativeSizes[i])
            {
                triIndex = i;
                break;
            }
        }

        if (triIndex == -1) Debug.LogError("triIndex should never be -1");

        // Get the three vertices of the selected triangle
        Vector3 a = mesh.vertices[mesh.triangles[triIndex * 3]];
        Vector3 b = mesh.vertices[mesh.triangles[triIndex * 3 + 1]];
        Vector3 c = mesh.vertices[mesh.triangles[triIndex * 3 + 2]];

        // Generate random barycentric coordinates
        float r = Random.value;
        float s = Random.value;

        if (r + s >= 1)
        {
            r = 1 - r;
            s = 1 - s;
        }

        // Convert barycentric coordinates to a point on the mesh
        Vector3 pointOnMesh = a + r * (b - a) + s * (c - a);
        return pointOnMesh;
    }

    /// <summary>
    /// Calculates the size (area) of each triangle in the mesh
    /// </summary>
    private float[] GetTriSizes(int[] tris, Vector3[] verts)
    {
        int triCount = tris.Length / 3;
        float[] sizes = new float[triCount];
        for (int i = 0; i < triCount; i++)
        {
            sizes[i] = .5f * Vector3.Cross(verts[tris[i * 3 + 1]] - verts[tris[i * 3]], verts[tris[i * 3 + 2]] - verts[tris[i * 3]]).magnitude;
        }
        return sizes;
    }

    /// <summary>
    /// Gets the count of currently active enemies
    /// </summary>
    public int GetActiveEnemyCount()
    {
        // Clean up the list before counting
        spawnedEnemies.RemoveAll(enemy => enemy == null);
        return spawnedEnemies.Count;
    }

    /// <summary>
    /// Gets the wave size that was most recently spawned
    /// </summary>
    public int GetCurrentWaveSize()
    {
        return currentWaveSize;
    }
}