using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager_bak : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform player;
    public PlayerStats playerStats;
    public int enemiesPerWave = 10;
    public float timeBetweenWaves = 10f;


    int currentWave = 0;
    float nextWaveTime = 0f;
    List<GameObject> spawnedEnemies = new();

    Mesh navMesh;


    void Start()
    {
        nextWaveTime = Time.time;
        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
        navMesh = new Mesh
        {
            vertices = triangulation.vertices,
            triangles = triangulation.indices
        };
    }

    void Update()
    {
        if (Time.time >= nextWaveTime)
        {
            SpawnWave();
            nextWaveTime = Time.time + timeBetweenWaves;
            currentWave++;
            Debug.Log($"Wave {currentWave} spawned!");
        }

        spawnedEnemies.RemoveAll(enemy => enemy == null);
    }

    void SpawnWave()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        Vector3 spawnPosition = GetRandomNavmeshPosition();


        GameObject enemy = GameObjectPoolManager.SpawnObject(enemyPrefab, spawnPosition, Quaternion.identity);
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent != null && player != null)
        {
            enemyComponent.player = player;
            enemyComponent.playerStats = playerStats;
        }
        spawnedEnemies.Add(enemy);

    }

    Vector3 GetRandomNavmeshPosition()
    {
        return GetRandomPointOnMesh(navMesh);
    }


    Vector3 GetRandomPointOnMesh(Mesh mesh)
    {
        //if you're repeatedly doing this on a single mesh, you'll likely want to cache cumulativeSizes and total
        float[] sizes = GetTriSizes(mesh.triangles, mesh.vertices);
        float[] cumulativeSizes = new float[sizes.Length];
        float total = 0;

        for (int i = 0; i < sizes.Length; i++)
        {
            total += sizes[i];
            cumulativeSizes[i] = total;
        }

        //so everything above this point wants to be factored out

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

        Vector3 a = mesh.vertices[mesh.triangles[triIndex * 3]];
        Vector3 b = mesh.vertices[mesh.triangles[triIndex * 3 + 1]];
        Vector3 c = mesh.vertices[mesh.triangles[triIndex * 3 + 2]];

        //generate random barycentric coordinates

        float r = Random.value;
        float s = Random.value;

        if (r + s >= 1)
        {
            r = 1 - r;
            s = 1 - s;
        }
        //and then turn them back to a Vector3
        Vector3 pointOnMesh = a + r * (b - a) + s * (c - a);
        return pointOnMesh;

    }

    float[] GetTriSizes(int[] tris, Vector3[] verts)
    {
        int triCount = tris.Length / 3;
        float[] sizes = new float[triCount];
        for (int i = 0; i < triCount; i++)
        {
            sizes[i] = .5f * Vector3.Cross(verts[tris[i * 3 + 1]] - verts[tris[i * 3]], verts[tris[i * 3 + 2]] - verts[tris[i * 3]]).magnitude;
        }
        return sizes;
    }

    public int GetWaveNumber()
    {
        return currentWave;
    }
}