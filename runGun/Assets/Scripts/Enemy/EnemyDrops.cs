using UnityEngine;

public class EnemyDrops : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WaveManager waveManager;

    [Header("Common Drops")]
    [Tooltip("Exp drops that can be dropped. Should be size 3 (unless changed later).")]
    public GameObject[] expDrops = new GameObject[3];
    [Tooltip("Probability that the enemy will also drop a exp pickup. When maxPotentialExpDrops is more than one, the probability is applied multiplicatively with every subsequent drop.")]
    public float expDropProbability = 1f;
    [Tooltip("Reduction factor for increasingly good drops. probabilityForBetterDrop is divided by this everytime it rolls for drop quality.")]
    public float reductionFactorForSubsequentExpDrops = 2f;
    [Tooltip("Max number of exp drops that may be dropped. Note: exp drop size depends on drop quality field.")]
    public int maxPotentialExpDrops = 3;
    [Tooltip("Min number of exp drops that may be dropped. Note: exp drop size depends on drop quality field.")]
    public int minPotentialExpDrops = 3;

    [Tooltip("Health drops that can be dropped. Should be size 3 (unless changed later).")]
    public GameObject[] healthDrops = new GameObject[3];
    [Tooltip("Probability that the enemy will also drop a health pickup. When maxPotentialHealthDrops is more than one, the probability is applied multiplicatively with every subsequent drop.")]
    public float healthDropProbability = .5f;
    [Tooltip("Reduction factor for increasingly good drops. probabilityForBetterDrop is divided by this everytime it rolls for drop quality.")]
    public float reductionFactorForSubsequentHealthDrops = 2f;
    [Tooltip("Max number of health drops that may be dropped. Note: health drop size depends on drop quality field.")]
    public int maxPotentialHealthDrops = 3;
    [Tooltip("Min number of health drops that may be dropped. Note: health drop size depends on drop quality field.")]
    public int minPotentialHealthDrops = 0;

    [Header("Rare Drops")]
    [Tooltip("Array of rare items that can be dropped")]
    public GameObject[] rareDrops;

    [Tooltip("Probability of dropping any rare item (0-1)")]
    [Range(0f, 1f)]
    public float rareDropChance = 0.05f;

    [Tooltip("Whether wave number increases rare drop chance")]
    public bool waveIncreasesRareDropChance = true;

    [Tooltip("Additional rare drop chance per wave (if waveIncreasesRareDropChance is true)")]
    [Range(0f, 0.01f)]
    public float rareDropChanceIncreasePerWave = 0.001f;

    [Tooltip("Maximum rare drop chance regardless of wave number")]
    [Range(0f, 1f)]
    public float maxRareDropChance = 0.25f;

    [Header("Drop Quality")]

    [Tooltip("Quality of drops (0=Small, 1=Medium, 2=Big). Note this is used to index the drops array.")]
    public int dropQuality = 0;

    [Tooltip("Base probability that drops better than dropQuality are dropped. 0 is no chance, 1 is guaranteed items are at least one tier better.")]
    public float probabilityForBetterDrop = 0f;

    [Tooltip("Reduction factor for increasingly good drops. probabilityForBetterDrop is divided by this everytime it rolls for drop quality.")]
    public float reductionFactorForIncreasinglyBetterDrop = 2f;

    [Header("Wave Based Drop Scaling")]
    [Tooltip("How much the wave number affects drop quality. 0 = no effect, 1 = full effect")]
    public float waveScalingFactor = 0.05f;

    [Tooltip("Maximum bonus to better drop probability from wave number")]
    public float maxWaveBonus = 0.5f;

    [Header("Drop Burst")]
    [Tooltip("Force applied to experience drops when spawned. Gets out of hand very quickly, leave around 1.")]
    public float dropExplosionForce = 1f;

    [Tooltip("Lower bounds for the random explosion vector applied when dropping items.")]
    public Vector3 dropBurstVectorLowerBounds = new(-1f, 1f, 1f);
    [Tooltip("Upper bounds for the random explosion vector applied when dropping items.")]
    public Vector3 dropBurstVectorUpperBounds = new(1f, 3f, 1f);

    private int waveThresholdForMediumDrops = 20;
    private int waveThresholdForLargeDrops = 40;
    private int maxDropsSelectionAmount = 3;

    void Start()
    {
        // Find EnemyManager if not assigned
        if (waveManager == null)
        {
            waveManager = FindFirstObjectByType<WaveManager>();
        }

        waveThresholdForMediumDrops = waveManager.GetMaxWaves() / 3;
        waveThresholdForLargeDrops = 2 * waveThresholdForMediumDrops;

        dropQuality = Mathf.Clamp(dropQuality, 0, maxDropsSelectionAmount - 1);
        probabilityForBetterDrop = Mathf.Clamp(probabilityForBetterDrop, 0f, 1f);

        // Check for empty rare drops array and log warning
        if (rareDrops == null || rareDrops.Length == 0)
        {
            Debug.LogWarning("Rare drops array is empty on " + gameObject.name);
        }
    }

    public void DropCommons()
    {
        // First check for rare drops
        TryDropRareItem();

        // Then drop common items
        DropExp();
        DropHealth();
    }

    private void TryDropRareItem()
    {
        // Skip if no rare drops available
        if (rareDrops == null || rareDrops.Length == 0)
            return;

        // Get current wave number
        int waveNumber = waveManager != null ? waveManager.GetCurrentWave() : 0;

        // Calculate adjusted drop chance based on wave number
        float adjustedDropChance = rareDropChance;
        if (waveIncreasesRareDropChance)
        {
            adjustedDropChance += waveNumber * rareDropChanceIncreasePerWave;
            adjustedDropChance = Mathf.Min(adjustedDropChance, maxRareDropChance);
        }

        // Roll for rare drop
        if (Random.value <= adjustedDropChance)
        {
            // Select a random rare item
            int randomIndex = Random.Range(0, rareDrops.Length);
            GameObject rareDrop = rareDrops[randomIndex];

            // Spawn the rare drop with a less dramatic burst effect
            GameObjectPoolManager.SpawnObject(rareDrop, transform.localPosition, Quaternion.identity);
        }
    }

    private void DropExp()
    {
        GameObject drop;
        int amountToDrop = GetNumberInRangeWithCompoundingProbability(minPotentialExpDrops, maxPotentialExpDrops, expDropProbability, reductionFactorForSubsequentExpDrops);
        for (int i = 0; i < amountToDrop; i++)
        {
            drop = GetDropByQuality(expDrops);
            BurstSpawnDrop(drop);
        }
    }

    private void DropHealth()
    {
        GameObject drop;
        int amountToDrop = GetNumberInRangeWithCompoundingProbability(minPotentialHealthDrops, maxPotentialHealthDrops, healthDropProbability, reductionFactorForSubsequentHealthDrops);
        for (int i = 0; i < amountToDrop; i++)
        {
            drop = GetDropByQuality(healthDrops);
            BurstSpawnDrop(drop);
        }
    }

    private int GetNumberInRangeWithCompoundingProbability(int min, int max, float probability, float probabilityReductionFactor)
    {
        int result = min;
        if (max - min >= 1)
        {
            float p = probability;
            while (result < max)
            {
                if (Random.Range(0f, 1f) > p)
                {
                    break;
                }
                p /= probabilityReductionFactor;
                result += 1;
            }
        }
        return result;
    }

    private GameObject GetDropByQuality(GameObject[] drops)
    {
        // Get current player level
        int waveNumber = waveManager != null ? waveManager.GetCurrentWave() : 0;

        // Calculate minimum quality based on wave thresholds
        int minimumQuality = dropQuality;
        if (waveNumber >= waveThresholdForLargeDrops)
        {
            minimumQuality = Mathf.Max(minimumQuality, 2); // Guarantee large drops at high wave
        }
        else if (waveNumber >= waveThresholdForMediumDrops)
        {
            minimumQuality = Mathf.Max(minimumQuality, 1); // Guarantee medium drops at mid wave
        }

        // Calculate wave-based bonus to better drop probability
        float waveBonus = Mathf.Min(waveNumber * waveScalingFactor, maxWaveBonus);
        float adjustedProbability = Mathf.Min(probabilityForBetterDrop + waveBonus, 1f);

        // Get the actual drop quality using the adjusted probability
        int quality = GetNumberInRangeWithCompoundingProbability(
            minimumQuality,
            maxDropsSelectionAmount - 1,
            adjustedProbability,
            reductionFactorForIncreasinglyBetterDrop
        );

        return drops[quality];
    }

    private void BurstSpawnDrop(GameObject drop, float forceMultiplier = 1f)
    {
        GameObject currSpawned = GameObjectPoolManager.SpawnObject(drop,
            transform.position,
            Quaternion.identity);
        Rigidbody rb = currSpawned.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Add random upward direction to make pickups fly out
            Vector3 randomDirection = new Vector3(
                Random.Range(dropBurstVectorLowerBounds.x, dropBurstVectorUpperBounds.x),
                Random.Range(dropBurstVectorLowerBounds.y, dropBurstVectorUpperBounds.y),
                Random.Range(dropBurstVectorLowerBounds.z, dropBurstVectorUpperBounds.z)
            ).normalized;
            rb.isKinematic = false;
            currSpawned.GetComponent<SphereCollider>().enabled = true;
            rb.AddForce(randomDirection * dropExplosionForce * forceMultiplier, ForceMode.Impulse);
        }
    }
}