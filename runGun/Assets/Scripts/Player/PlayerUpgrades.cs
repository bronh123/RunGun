using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUpgrades : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private WeaponStats weaponStats;
    [SerializeField] private ScriptManager scriptManager;

    [Header("Upgrade System")]
    [Tooltip("All possible upgrades the player can receive. If left empty, upgrades will be loaded from UpgradeDatabase.")]
    public List<Upgrade> possibleUpgrades = new List<Upgrade>();

    [Tooltip("How many upgrade options to present per level")]
    public int upgradeOptionsPerLevel = 3;

    [Tooltip("Number of weapon upgrades presented when appropriate.")]
    public int weaponUpgradeOptions = 1;

    [Tooltip("Reference to the upgrade selection UI panel")]
    public UpgradeSelectionUI upgradeSelectionUI;

    [Tooltip("Time scale when upgrade selection UI is open. Warning: a value of 0 may cause issues")]
    [Range(0f, 1f)]
    public float upgradeSelectionTimeScale = 0f;

    [Tooltip("Whether to use the UpgradeDatabase for upgrades")]
    public bool useUpgradeDatabase = true;

    [Tooltip("Whether to use level-based weighted quality selection")]
    public bool useWeightedQualitySelection = true;

    [Tooltip("level multiple that will trigger weapon upgrade option.")]
    public int weaponUpgradeUnlockInterval = 5;

    private int nextLevel;

    private void Start()
    {

        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
        }

        if (weaponStats == null)
        {
            weaponStats = GetComponent<WeaponStats>();
        }
        if (scriptManager == null)
        {
            scriptManager = FindFirstObjectByType<ScriptManager>();
        }

        // Load upgrades from database if using the database and the list is empty
        if (useUpgradeDatabase && possibleUpgrades.Count == 0)
        {
            if (UpgradeDatabase.Instance != null)
            {
                possibleUpgrades = UpgradeDatabase.Instance.GetAllUpgrades();
            }
            else
            {
                Debug.LogWarning("UpgradeDatabase instance not found, but useUpgradeDatabase is true.");
            }
        }
    }

    public void HandleLevelUp(int nextLevel)
    {
        this.nextLevel = nextLevel;
        // Disable scripts during upgrade selection (most likely just movement and camera)
        scriptManager.DisablePlayerScripts();
        // Show upgrade selection UI
        PresentUpgradeOptions();

        // Pause/ slow game 
        Time.timeScale = upgradeSelectionTimeScale;
    }

    private void PresentUpgradeOptions()
    {
        if (upgradeSelectionUI == null)
        {
            Debug.LogError("Upgrade Selection UI reference is missing");
            return;
        }

        // Get random selection of upgrades
        List<Upgrade> selectedUpgrades = GetUpgrades(upgradeOptionsPerLevel);

        Debug.Log("Attempting to show upgrade panel");
        upgradeSelectionUI.gameObject.SetActive(true);
        upgradeSelectionUI.ShowUpgradeOptions(selectedUpgrades, OnUpgradeSelected);
    }


    private List<Upgrade> GetUpgrades(int count)
    {
        List<Upgrade> upgradesResult;;
        // If we're using the upgrade database and it exists, get random upgrades from there
        if (useUpgradeDatabase && UpgradeDatabase.Instance != null)
        {
            if (useWeightedQualitySelection)
            {
                // Use weighted selection based on player level
                upgradesResult = UpgradeDatabase.Instance.GetRandomUpgradesWeighted(count, playerStats.GetLevel());
            }
            else
            {
                // Use completely random selection
                upgradesResult = UpgradeDatabase.Instance.GetRandomUpgrades(count);
            }
        }
        else
        {
            // Otherwise use the local list
            upgradesResult = UpgradeDatabase.Instance.GetRandomUpgrades(count, possibleUpgrades);
        }

        // Add weapon upgrades
        if (nextLevel % weaponUpgradeUnlockInterval == 0)
        {
            foreach (Upgrade weaponUpgrade in UpgradeDatabase.Instance.GetRandomWeaponUpgrades(weaponUpgradeOptions))
            {
                upgradesResult.Add(weaponUpgrade);
            }
        }

        return upgradesResult;
    }

    public void OnUpgradeSelected(Upgrade selectedUpgrade)
    {
        string upgradeName = selectedUpgrade.useQualitySystem
            ? $"{selectedUpgrade.quality} {selectedUpgrade.upgradeName}"
            : selectedUpgrade.upgradeName;

        Debug.Log($"OnUpgradeSelected called with: {upgradeName}");

        ApplyUpgrade(selectedUpgrade);
        Time.timeScale = 1f;
        upgradeSelectionUI.gameObject.SetActive(false);
        scriptManager.ReenablePlayerScripts();

        // Check if there are more level-ups pending
        playerStats.OnUpgradeComplete();
    }

    private void ApplyUpgrade(Upgrade upgrade)
    {
        if (playerStats == null)
        {
            Debug.LogError("Cannot apply upgrade: PlayerStats reference is missing!");
            return;
        }

        // Calculate adjusted value based on quality
        float adjustedValue = upgrade.GetAdjustedValue();


        // Apply the appropriate stat boost based on upgrade type
        switch (upgrade.type)
        {
            case Upgrade.UpgradeType.MaxHealth:
                playerStats.ModifyMaxHealth(adjustedValue);
                playerStats.ModifyCurrentHealth(adjustedValue);
                break;
            case Upgrade.UpgradeType.Speed:
                playerStats.ModifySpeed(adjustedValue);
                break;
            case Upgrade.UpgradeType.Strength:
                playerStats.ModifyStrength(adjustedValue);
                break;
            case Upgrade.UpgradeType.Defense:
                playerStats.ModifyDefense(adjustedValue);
                break;
            case Upgrade.UpgradeType.JumpForce:
                playerStats.ModifyJump(adjustedValue);
                break;
            case Upgrade.UpgradeType.CommonPickupRange:
                playerStats.ModifyCommonPickupRange(adjustedValue);
                break;
            case Upgrade.UpgradeType.ShotgunSlug:
                weaponStats.ModifyShotgunSlugQualities();
                break;
            case Upgrade.UpgradeType.ShotgunBlast:
                weaponStats.ModifyShotgunBlastQualities();
                break;
        }

        Debug.Log($"Applied {upgrade.quality} {upgrade.upgradeName} with adjusted value: {adjustedValue}");
    }
}