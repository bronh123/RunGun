using System;
using UnityEngine;

[Serializable]
public class Upgrade
{
    [Tooltip("Name of the upgrade")]
    public string upgradeName;
    
    [Tooltip("Description of what the upgrade does")]
    public string description;
    
    [Tooltip("Type of stat this upgrade affects")]
    public UpgradeType type;

    [Tooltip("if a weapon upgrade, which one.")]
    public String weaponName;
    
    [Tooltip("Whether this upgrade uses the quality system")]
    public bool useQualitySystem = true;
    
    [Tooltip("Quality level of the upgrade (only used if useQualitySystem is true)")]
    public UpgradeQuality quality = UpgradeQuality.Normal;
    
    [Tooltip("Value of the stat change this upgrade provides")]
    public float value;
    
    [Tooltip("Optional icon for the upgrade")]
    public Sprite icon;

    // Types of stats that can be upgraded
    public enum UpgradeType
    {
        MaxHealth,
        Speed,
        Strength,
        Defense,
        JumpForce,
        CommonPickupRange,
        ShotgunSlug,
        ShotgunBlast

    }
    
    // Quality levels for upgrades
    public enum UpgradeQuality
    {
        Minor,
        Normal,
        Major,
        Epic,   
        Legendary 
    }
    
    // Helper method to get color based on quality
    public Color GetQualityColor()
    {
        switch (quality)
        {
            case UpgradeQuality.Minor:
                return new Color(0.7f, 0.7f, 0.7f); // Gray
            case UpgradeQuality.Normal:
                return Color.white; // White
            case UpgradeQuality.Major:
                return new Color(0.0f, 0.7f, 1.0f); // Light Blue
            case UpgradeQuality.Epic:
                return new Color(0.6f, 0.2f, 1.0f); // Purple
            case UpgradeQuality.Legendary:
                return new Color(1.0f, 0.8f, 0.0f); // Gold
            default:
                return Color.white;
        }
    }
    
    // Helper method to get display name with quality
    public string GetDisplayName()
    {
        if (useQualitySystem)
        {
            return $"{quality} {upgradeName}";
        }
        return upgradeName;
    }
    
    // Helper method to get quality modifier for value scaling
    public float GetQualityModifier()
    {
        if (!useQualitySystem)
        {
            return 1.0f;
        }
        
        switch (quality)
        {
            case UpgradeQuality.Minor:
                return 0.6f;
            case UpgradeQuality.Normal:
                return 1.0f;
            case UpgradeQuality.Major:
                return 1.5f;
            case UpgradeQuality.Epic:
                return 2.0f;
            case UpgradeQuality.Legendary:
                return 3.0f;
            default:
                return 1.0f;
        }
    }
    
    // Helper method to get quality-adjusted value
    public float GetAdjustedValue()
    {
        return value * GetQualityModifier();
    }
}