using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Basic Stats")]
    [Tooltip("Amount of damage dealt to player per attack")]
    public int damage = 10;
    
    [Tooltip("Maximum health points of the enemy")]
    public float maxHealth = 5;
    
    [Tooltip("Current health points of the enemy")]
    public float health;
    
    [Tooltip("Base score value for killing this enemy")]
    public int baseScoreValue = 100;
    
    [Header("References")]
    [Tooltip("Reference to the player's transform for tracking")]
    public Transform player;
    
    [Tooltip("Reference to the player's stats component for dealing damage")]
    public PlayerStats playerStats;
    
    [Header("Effects")]
    [Tooltip("Sound played when enemy attacks")]
    public AudioClip attackSound;
    
    [Tooltip("Sound played when enemy takes damage")]
    public AudioClip hitSound;
    
    [Tooltip("Sound played when enemy dies")]
    public AudioClip deathSound;
    
    [Tooltip("Particle effect for enemy death")]
    public GameObject deathEffect;
    
    [Header("Damage Flash Effect")]
    [SerializeField] [Tooltip("Color to flash when taking damage")]
    protected Color damageFlashColor = Color.red;
    
    [SerializeField] [Tooltip("Duration of the damage flash in seconds")]
    protected float damageFlashDuration = 0.2f;
    
    [SerializeField] [Tooltip("Renderers to apply the flash effect to")]
    protected Renderer[] renderersToFlash;

    // Wave scaling properties
    protected float healthMultiplier = 1.0f;
    protected float damageMultiplier = 1.0f;
    protected float speedMultiplier = 1.0f;
    protected float baseMaxHealth;
    protected int baseDamage;
    
    // References to components
    protected AudioSource audioSource;
    protected EnemyDrops enemyDrops;
    protected ScoreManager scoreManager;
    
    // Materials and colors for flash effect
    protected Material[] materials;
    protected Color[] originalColors;
    protected bool isFlashing = false;
    protected float flashEndTime = 0f;
    
    // State tracking
    protected bool isDead = false;
    
    protected virtual void Awake()
    {
        // Get components
        audioSource = GetComponent<AudioSource>();
        enemyDrops = GetComponent<EnemyDrops>();
        
        // Store base values for scaling
        baseDamage = damage;
        baseMaxHealth = maxHealth;
        
        // Find score manager
        scoreManager = FindFirstObjectByType<ScoreManager>();
        if (scoreManager == null)
        {
            Debug.LogWarning("ScoreManager not found in scene. Scores will not be tracked.");
        }
        
        // Auto-find renderers if not manually set
        if (renderersToFlash == null || renderersToFlash.Length == 0)
        {
            renderersToFlash = GetComponentsInChildren<Renderer>();
        }
        
        // Initialize materials and colors for flash effect
        InitializeFlashMaterials();
    }
    
    /// <summary>
    /// Initialize materials for damage flash effect
    /// </summary>
    protected void InitializeFlashMaterials()
    {
        int totalMaterials = 0;
        foreach (Renderer renderer in renderersToFlash)
        {
            if (renderer != null)
            {
                totalMaterials += renderer.materials.Length;
            }
        }
        
        materials = new Material[totalMaterials];
        originalColors = new Color[totalMaterials];
        
        int index = 0;
        foreach (Renderer renderer in renderersToFlash)
        {
            if (renderer != null)
            {
                foreach (Material material in renderer.materials)
                {
                    materials[index] = material;
                    
                    // Store original color (assuming standard shader with _Color property)
                    if (material.HasProperty("_Color"))
                    {
                        originalColors[index] = material.GetColor("_Color");
                    }
                    else if (material.HasProperty("_BaseColor")) // For URP
                    {
                        originalColors[index] = material.GetColor("_BaseColor");
                    }
                    
                    index++;
                }
            }
        }
    }
    
    protected virtual void OnEnable()
    {
        // Apply current multipliers to base stats
        ApplyStatMultipliers();
        
        // Reset health and state
        health = maxHealth;
        isDead = false;
        
        // Reset flash effect
        isFlashing = false;
        ResetMaterialColors();
    }
    
    protected virtual void Update()
    {
        // Update flash effect
        if (isFlashing && Time.time >= flashEndTime)
        {
            isFlashing = false;
            ResetMaterialColors();
        }
    }
    
    public virtual float TakeDamage(float dmg)
    {
        if (isDead) return -1;
        
        // Apply player strength modifier
        dmg = playerStats != null ? playerStats.ApplyStrength(dmg) : dmg;
        
        // Apply damage
        health -= dmg;
        
        // Play hit sound
        if (audioSource != null && hitSound != null && dmg > 0)
        {
            audioSource.PlayOneShot(hitSound);
        }
        
        // Trigger damage flash effect
        if (dmg > 0)
        {
            StartDamageFlash();
        }
        
        // Check for death
        if (health <= 0 && !isDead)
        {
            KillEnemy();
            return -1;
        }
        
        return Mathf.Max(health, 0f);
    }
    
    /// <summary>
    /// Start the damage flash effect
    /// </summary>
    protected void StartDamageFlash()
    {
        // Set materials to flash color
        foreach (Material material in materials)
        {
            if (material != null)
            {
                if (material.HasProperty("_Color"))
                {
                    material.SetColor("_Color", damageFlashColor);
                }
                else if (material.HasProperty("_BaseColor")) // For URP
                {
                    material.SetColor("_BaseColor", damageFlashColor);
                }
                
                // For emission (optional)
                if (material.HasProperty("_EmissionColor"))
                {
                    material.EnableKeyword("_EMISSION");
                    material.SetColor("_EmissionColor", damageFlashColor * 0.5f);
                }
            }
        }
        
        // Set timer
        isFlashing = true;
        flashEndTime = Time.time + damageFlashDuration;
    }
    
    /// <summary>
    /// Reset material colors to original values
    /// </summary>
    protected void ResetMaterialColors()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i] != null)
            {
                if (materials[i].HasProperty("_Color"))
                {
                    materials[i].SetColor("_Color", originalColors[i]);
                }
                else if (materials[i].HasProperty("_BaseColor")) // For URP
                {
                    materials[i].SetColor("_BaseColor", originalColors[i]);
                }
                
                // Reset emission
                if (materials[i].HasProperty("_EmissionColor"))
                {
                    materials[i].SetColor("_EmissionColor", Color.black);
                    materials[i].DisableKeyword("_EMISSION");
                }
            }
        }
    }
    
    public virtual void KillEnemy()
    {
        if (isDead) return;
        
        isDead = true;
        
        // Award score
        if (scoreManager != null)
        {
            int scoreValue = CalculateScoreValue();
            scoreManager.AddScore(scoreValue);
            Debug.Log($"Awarded {scoreValue} points for killing enemy");
        }
        
        // Play death sound
        if (audioSource != null && deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }
        
        // Spawn death effect
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        
        // Handle item drops
        if (enemyDrops != null)
        {
            enemyDrops.DropCommons();
        }
        
        // Return to object pool
        GameObjectPoolManager.Deactivate(gameObject);
    }
    
    public virtual int DealDamageToPlayer()
    {
        if (playerStats != null)
        {
            playerStats.TakeDamage(damage);
            
            // Play attack sound
            if (audioSource != null && attackSound != null)
            {
                audioSource.PlayOneShot(attackSound);
            }
            
            return damage;
        }
        return 0;
    }
    
    // Set stat multipliers for wave-based difficulty scaling
    public virtual void SetStatMultipliers(float health, float damage, float speed)
    {
        healthMultiplier = health;
        damageMultiplier = damage;
        speedMultiplier = speed;
        
        ApplyStatMultipliers();
    }
    
    // Apply multipliers to base stats
    protected virtual void ApplyStatMultipliers()
    {
        maxHealth = baseMaxHealth * healthMultiplier;
        damage = Mathf.RoundToInt(baseDamage * damageMultiplier);
    }
    
    // Calculate score value based on enemy stats
    protected int CalculateScoreValue()
    {
        // Calculate a score value based on enemy strength
        float strengthFactor = (healthMultiplier + damageMultiplier) / 2f;
        
        // Apply additional bonus for faster enemies
        float speedBonus = speedMultiplier > 1.2f ? 1.5f : 1.0f;
        
        // Calculate final score with some randomization for variety
        int finalScore = Mathf.RoundToInt(baseScoreValue * strengthFactor * speedBonus);
        
        // Add a small random variance (±10%)
        finalScore = Mathf.RoundToInt(finalScore * Random.Range(0.9f, 1.1f));
        
        return finalScore;
    }
    
    public bool IsDead()
    {
        return isDead;
    }
    
    public float GetHealthPercentage()
    {
        return Mathf.Clamp01(health / maxHealth);
    }
}