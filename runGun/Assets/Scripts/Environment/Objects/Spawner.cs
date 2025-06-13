using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [Header("Spawn Settings")]
    public GameObject prefab;
    public float spawnInterval;
    public Transform spawnPoint;

    private GameObject spawnedObject;
    private Coroutine spawnCoroutine;
    void Start()
    {
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(spawnInterval);
            if (spawnedObject != null)
            {
                continue;
            }

            SpawnObject();
        }
    }

    private void SpawnObject() 
    {
        Vector3 spawnPosition;

        spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;
        spawnedObject = GameObjectPoolManager.SpawnObject(prefab, spawnPosition, Quaternion.identity);
    }

}
