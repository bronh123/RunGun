using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    [Tooltip("Maximum health points the player can have")]
    public float maxHealth = 100f;

    [Tooltip("Current health points of the player")]
    public float currentHealth;

    [Tooltip("Base movement speed of the player")]
    public float moveSpeed = 5f;

    [Tooltip("Player's strength stat, affects damage dealt")]
    public float strength = 0f;

    [Tooltip("Player's defense stat, reduces damage taken")]
    public float defense = 0f;

    [Tooltip("Force applied when player jumps")]
    public float jumpForce = 30f;

    [Tooltip("Radius in which player automatically collects experience points")]
    public float commonPickupRange = 3f;

    [Tooltip("Sphere collider used for automatic experience pickup")]
    public SphereCollider expPickupSphereCollider;

    [Header("double hit protection")]
    [SerializeField] private bool useDoubleHitProtection = true;
    [SerializeField] private float invincibilityDuration = 1f;


    [Header("Experience System")]
    [Tooltip("Current experience points")]
    public int Exp = 0;

    [Tooltip("Experience points required to level up")]
    public int ExpToLevel = 1;

    [Tooltip("Current player level")]
    public int level = 1;

    [SerializeField]
    [Tooltip("Factor by which experience requirement increases per level")]
    private float expRequirementIncreaseFactor = 2f;

    [Header("Temporary Boosts")]
    [Tooltip("Dictionary of active stat boost coroutines")]
    private Dictionary<string, Coroutine> activeBoosts = new();
    private BoostEffectManager boostEffectManager;

    [Header("Audio")]
    [Tooltip("Sound played when player takes damage")]
    public AudioClip damageSound;
    public AudioSource audioSource;

    private int pendingLevelUps = 0;
    private bool isProcessingLevelUp = false;
    private float timeSinceLastHit = 0f;

    [SerializeField] private PlayerUpgrades playerUpgrades;
    public enum StatType { CurrentHealth, MaxHealth, Speed, Strength, Defense, Jump, CommonPickupRange }

    private void Start()
    {
        currentHealth = maxHealth;
        if (expPickupSphereCollider != null)
        {
            expPickupSphereCollider.radius = commonPickupRange;
        }

        if (playerUpgrades == null)
        {
            playerUpgrades = GetComponent<PlayerUpgrades>();
        }
        if (boostEffectManager == null)
        {
            boostEffectManager = FindFirstObjectByType<BoostEffectManager>();
        }
    }

    private void Update()
    {
        timeSinceLastHit += Time.deltaTime;
    }
    public void TakeDamage(float damage)
    {
        if (useDoubleHitProtection && timeSinceLastHit < invincibilityDuration) return;
        float effectiveDamage = damage - defense;
        effectiveDamage = Mathf.Max(0, effectiveDamage);
        HandleDamageEffects(effectiveDamage);

        currentHealth -= effectiveDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        boostEffectManager.TakeDamageEffect();
        timeSinceLastHit = 0f;
    }

    private void HandleDamageEffects(float effectiveDamage)
    {
        if (effectiveDamage > 0 && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }
    }

    public float ApplyStrength(float dmg)
    {
        return dmg + strength;
    }
    #region Leveling/ EXP

    public void ModifyExp(int amount)
    {
        Exp += amount;
        CheckLevelUp();
    }

    public void CheckLevelUp()
    {
        while (Exp >= ExpToLevel)
        {
            Exp -= ExpToLevel;
            pendingLevelUps++;
            level += 1;
            ExpToLevel = (int)(expRequirementIncreaseFactor * ExpToLevel);
        }

        if (pendingLevelUps > 0 && !isProcessingLevelUp)
        {
            ProcessNextLevelUp(level - pendingLevelUps + 1);
        }
    }

    private void ProcessNextLevelUp(int nextLevel)
    {
        if (pendingLevelUps > 0 && !isProcessingLevelUp)
        {
            pendingLevelUps--;
            isProcessingLevelUp = true;

            if (playerUpgrades != null)
            {
                playerUpgrades.HandleLevelUp(nextLevel);
            }
            else
            {
                Debug.LogError("PlayerUpgrades component is missing");
                isProcessingLevelUp = false;
            }
            level = nextLevel;
        }
    }
    public void OnUpgradeComplete()
    {
        isProcessingLevelUp = false;
        ProcessNextLevelUp(level + 1);
    }

    #endregion

    #region Permanent Stat Modifications
    public void ModifyMaxHealth(float amount)
    {
        maxHealth += amount;
        maxHealth = Mathf.Max(1, maxHealth);
        Debug.Log($"Max Health changed by {amount}. Max health: {maxHealth}");
    }

    public void ModifyCurrentHealth(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        Debug.Log($"Current Health changed by {amount}. Current Health: {currentHealth}");
    }

    public void ModifySpeed(float amount)
    {
        moveSpeed += amount;
        moveSpeed = Mathf.Max(0.1f, moveSpeed); // Ensure speed doesn't go below 0.1
        Debug.Log($"Speed changed by {amount}. Current speed: {moveSpeed}");
    }

    public void ModifyStrength(float amount)
    {
        strength += amount;
        strength = Mathf.Max(0, strength);
        Debug.Log($"Strength changed by {amount}. Current strength: {strength}");
    }

    public void ModifyDefense(float amount)
    {
        defense += amount;
        defense = Mathf.Max(0, defense);
        Debug.Log($"Defense changed by {amount}. Current defense: {defense}");
    }

    public void ModifyJump(float amount)
    {
        jumpForce += amount;
        jumpForce = Mathf.Max(0, jumpForce);
        Debug.Log($"Jump Force changed by {amount}. Current jumpForce: {jumpForce}");
    }

    public void ModifyCommonPickupRange(float amount)
    {
        commonPickupRange += amount;
        commonPickupRange = Mathf.Max(commonPickupRange, 1f);
        expPickupSphereCollider.radius = commonPickupRange;
    }

    #endregion

    #region Temporary Stat Boosts
    public void ApplyTemporaryHealthBoost(float amount, float duration)
    {
        // Stop any active health boost
        StopBoostIfActive("health");
        // Start a new health boost
        activeBoosts["health"] = StartCoroutine(TemporaryStatBoost("health", amount, duration));

        if (boostEffectManager != null)
        {
            boostEffectManager.ShowBoostEffect("health", amount, duration);
        }
    }

    public void ApplyTemporarySpeedBoost(float amount, float duration)
    {
        StopBoostIfActive("speed");
        activeBoosts["speed"] = StartCoroutine(TemporaryStatBoost("speed", amount, duration));
        if (boostEffectManager != null)
        {
            boostEffectManager.ShowBoostEffect("speed", amount, duration);
        }
    }

    public void ApplyTemporaryStrengthBoost(float amount, float duration)
    {
        StopBoostIfActive("strength");
        activeBoosts["strength"] = StartCoroutine(TemporaryStatBoost("strength", amount, duration));
        if (boostEffectManager != null)
        {
            boostEffectManager.ShowBoostEffect("strength", amount, duration);
        }
    }

    public void ApplyTemporaryDefenseBoost(float amount, float duration)
    {
        StopBoostIfActive("defense");
        activeBoosts["defense"] = StartCoroutine(TemporaryStatBoost("defense", amount, duration));
        if (boostEffectManager != null)
        {
            boostEffectManager.ShowBoostEffect("defense", amount, duration);
        }
    }

    public void ApplyTemporaryJumpBoost(float amount, float duration)
    {
        StopBoostIfActive("jump");
        activeBoosts["jump"] = StartCoroutine(TemporaryStatBoost("jump", amount, duration));
        if (boostEffectManager != null)
        {
            boostEffectManager.ShowBoostEffect("jump", amount, duration);
        }
    }

    public void ApplyTemporaryCommonPickupRangeBoost(float amount, float duration)
    {
        StopBoostIfActive("common pickup range");
        activeBoosts["common pickup range"] = StartCoroutine(TemporaryStatBoost("common pickup range", amount, duration));
    }

    private void StopBoostIfActive(string statName)
    {
        if (activeBoosts.ContainsKey(statName) && activeBoosts[statName] != null)
        {
            StopCoroutine(activeBoosts[statName]);
            activeBoosts[statName] = null;
        }
    }

    private IEnumerator TemporaryStatBoost(string statName, float amount, float duration)
    {
        // Apply the boost
        switch (statName)
        {
            case "health":
                ModifyMaxHealth(amount);
                Debug.Log($"Applied temporary health boost of {amount} for {duration} seconds");
                break;
            case "speed":
                moveSpeed += amount;
                Debug.Log($"Applied temporary speed boost of {amount} for {duration} seconds");
                break;
            case "strength":
                strength += amount;
                Debug.Log($"Applied temporary strength boost of {amount} for {duration} seconds");
                break;
            case "defense":
                defense += amount;
                Debug.Log($"Applied temporary defense boost of {amount} for {duration} seconds");
                break;
            case "jump":
                jumpForce += amount;
                Debug.Log($"Applied temporary jump boost of {amount} for {duration} seconds");
                break;
            case "common pickup range":
                ModifyCommonPickupRange(amount);
                Debug.Log($"Applied temporary common pickup range boost of {amount} for {duration} seconds");
                break;
        }

        // Wait for the duration
        yield return new WaitForSeconds(duration);

        // Remove the boost
        switch (statName)
        {
            case "health":
                // For health, we don't typically remove health after a boost
                break;
            case "speed":
                moveSpeed -= amount;
                Debug.Log($"Speed boost of {amount} ended. Current speed: {moveSpeed}");
                break;
            case "strength":
                strength -= amount;
                Debug.Log($"Strength boost of {amount} ended. Current strength: {strength}");
                break;
            case "defense":
                defense -= amount;
                Debug.Log($"Defense boost of {amount} ended. Current defense: {defense}");
                break;
            case "jump":
                jumpForce -= amount;
                Debug.Log($"Jump boost of {amount} ended. Current jump: {jumpForce}");
                break;
            case "common pickup range":
                ModifyCommonPickupRange(-amount);
                Debug.Log($"Common pickup range boost of {amount} ended. Current cpr: {commonPickupRange}");
                break;
        }

        activeBoosts[statName] = null;
    }
    #endregion

    #region Getters/ Setters

    public int GetExp()
    {
        return Exp;
    }
    public int GetExpToLevel()
    {
        return ExpToLevel;
    }
    public float GetExpPercentage()
    {
        return (float)Exp / ExpToLevel;
    }

    public int GetLevel()
    {
        return level;
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    public float GetStat(StatType statType)
    {
        switch (statType)
        {
            case StatType.Speed:
                return moveSpeed;
            case StatType.Defense:
                return defense;
            case StatType.Jump:
                return jumpForce;
            case StatType.Strength:
                return strength;
            case StatType.CommonPickupRange:
                return commonPickupRange;
            case StatType.CurrentHealth:
                return currentHealth;
            case StatType.MaxHealth:
                return maxHealth;
        }
        return -1f;
    }
    #endregion
}