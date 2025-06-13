using UnityEngine;

/// <summary>
/// Manages weapon statistics and upgrades for different weapon types.
/// Currently handles shotgun modifications with planned support for gravity launcher.
/// </summary>
public class WeaponStats : MonoBehaviour
{
    // References to weapon handlers
    [Tooltip("Reference to the shotgun handler component for controlling shotgun behavior")]
    public ShotgunHandler shotgunHandler;
    [Tooltip("Reference to the gravity launcher handler component for controlling gravity launcher behavior")]
    public GravLauncherHandler graveLauncherHandler;

    #region "Shotgun Stat Variables

    // Base shotgun statistics
    [Header("Shotgun Stats")]
    [SerializeField, Tooltip("Angular spread of shotgun pellets in degrees")]
    private float sgSpread = 10;
    [SerializeField, Tooltip("Minimum allowed spread angle in degrees")]
    private float sgMinSpread = 0;
    [SerializeField, Tooltip("Maximum allowed spread angle in degrees")]
    private float sgMaxSpread = 180;
    [SerializeField, Tooltip("Speed at which projectiles travel")]
    private float sgProjSpeed = 50;
    [SerializeField, Tooltip("Minimum allowed projectile speed")]
    private float sgMinProjSpeed = 30;
    [SerializeField, Tooltip("Maximum allowed projectile speed")]
    private float sgMaxProjSpeed = 200;
    [SerializeField, Tooltip("Number of pellets fired per shot")]
    private int sgProjectileCount = 10;
    [SerializeField, Tooltip("Minimum number of projectiles per shot")]
    private int sgMinProjectileCount = 1;
    [SerializeField, Tooltip("Maximum number of projectiles per shot")]
    private int sgMaxProjectileCount = 200;
    [SerializeField, Tooltip("Amount of recoil/kickback when firing")]
    private float sgRecoilIntensity = 50;
    [SerializeField, Tooltip("Minimum allowed recoil intensity")]
    private float sgMinRecoilIntensity = 0;
    [SerializeField, Tooltip("Maximum allowed recoil intensity")]
    private float sgMaxRecoilIntensity = 300;
    [SerializeField, Tooltip("Damage dealt by each individual pellet")]
    private float sgDamage = 1;
    [SerializeField, Tooltip("Minimum damage per pellet")]
    private float sgMinDamage = .1f;
    [SerializeField, Tooltip("Maximum damage per pellet")]
    private float sgMaxDamage = 100000;
    [SerializeField, Tooltip("Visual size of each projectile")]
    private Vector3 sgProjectileScale = new(.1f, .1f, .1f);
    [SerializeField, Tooltip("Minimum allowed projectile size")]
    private Vector3 sgMinProjectileScale = new(.05f, .05f, .05f);
    [SerializeField, Tooltip("Maximum allowed projectile size")]
    private Vector3 sgMaxProjectileScale = new(5f, 5f, 5f);

    // Shotgun upgrade modifiers
    [Header("Shotgun Upgrade Values")]

    // Slug upgrade - Converts shotgun to fire fewer more powerful, larger rounds
    [Header("Shotgun Slug Upgrade")]
    [SerializeField, Tooltip("Amount to adjust spread when slug upgrade is applied (negative = tighter spread)")]
    private float sgSlugSpreadAdj = -5f;
    [SerializeField, Tooltip("Amount to increase damage when slug upgrade is applied")]
    private float sgSlugDamageAdj = 5f;
    [SerializeField, Tooltip("Divisor for projectile count when slug upgrade is applied (negative value creates special division behavior)")]
    private int sgSlugProjectileCountAdj = 2;
    [SerializeField, Tooltip("Amount to adjust recoil when slug upgrade is applied (negative = less recoil)")]
    private float sgSlugRecoilAdj = -10;
    [SerializeField, Tooltip("Amount to increase projectile speed when slug upgrade is applied")]
    private float sgSlugProjectileSpeedAdj = 20f;
    [SerializeField, Tooltip("Amount to increase projectile size when slug upgrade is applied")]
    private Vector3 sgSlugProjectileSizeAdj = new Vector3(.2f, .2f, .2f);

    // Blast upgrade - Increases spread and pellet count for crowd control
    [Header("Shotgun Blast Upgrade")]
    [SerializeField, Tooltip("Amount to increase spread when blast upgrade is applied (positive = wider spread)")]
    private float sgBlastSpreadAdj = 5f;
    [SerializeField, Tooltip("Amount to adjust damage when blast upgrade is applied (negative = less damage per pellet)")]
    private float sgBlastDamageAdj = -1f;
    [SerializeField, Tooltip("Multiplier for projectile count when blast upgrade is applied")]
    private int sgBlastProjectileCountAdj = 2;
    [SerializeField, Tooltip("Amount to increase recoil when blast upgrade is applied")]
    private float sgBlastRecoilAdj = 10f;
    [SerializeField, Tooltip("Amount to increase projectile speed when blast upgrade is applied")]
    private float sgBlastProjectileSpeedAdj = 10f;

    #endregion

    #region "Grav Launcher Stat Variables

    // Gravity launcher statistics (placeholder for future implementation)
    [Header("Grav Launcher Stats - Currently Useless")]
    [SerializeField, Tooltip("Speed at which gravity launcher projectiles travel")]
    private float glProjectileSpeed;
    [SerializeField, Tooltip("Radius of the gravity effect field produced by the launcher")]
    private float glEffectFieldRadius;
    [SerializeField, Tooltip("Damage dealt per tick to enemies within the gravity field")]
    private float glDamagePerTick;
    [SerializeField, Tooltip("Force with which objects are pulled toward the center of the gravity field")]
    private float glPullForce;
    [SerializeField, Tooltip("Force with which objects are pushed away from the center of the gravity field")]
    private float glPushForce;
    #endregion

    /// <summary>
    /// Initialize weapon handlers with base statistics
    /// </summary>
    void Start()
    {
        // Set initial shotgun statistics if handler exists
        if (shotgunHandler != null)
        {
            shotgunHandler.SetSpread(sgSpread);
            shotgunHandler.SetProjectileCount(sgProjectileCount);
            shotgunHandler.SetRecoilIntensity(sgRecoilIntensity);
            shotgunHandler.SetProjectileSpeed(sgProjSpeed);
            shotgunHandler.SetProjectileScale(sgProjectileScale);
        }
    }

    /// <summary>
    /// Updates the shotgun handler with current stat values
    /// </summary>
    private void UpdateShotgunHandler()
    {
        if (shotgunHandler != null)
        {
            shotgunHandler.SetSpread(sgSpread);
            shotgunHandler.SetProjectileCount(sgProjectileCount);
            shotgunHandler.SetProjectileDamage(sgDamage);
            shotgunHandler.SetRecoilIntensity(sgRecoilIntensity);
            shotgunHandler.SetProjectileSpeed(sgProjSpeed);
            shotgunHandler.SetProjectileScale(sgProjectileScale);
        }
    }

    /// <summary>
    /// Applies slug upgrade modifications to shotgun
    /// Fewer projectiles with increased size and damage.
    /// </summary>
    public void ModifyShotgunSlugQualities()
    {
        // Apply slug modifications
        sgSpread += sgSlugSpreadAdj;                    // Reduce spread for accuracy
        sgProjectileCount /= sgSlugProjectileCountAdj;  // Divide projectile count (note: dividing by negative number)
        sgDamage += sgSlugDamageAdj;                    // Increase damage
        sgRecoilIntensity += sgSlugRecoilAdj;           // Adjust recoil
        sgProjSpeed += sgSlugProjectileSpeedAdj;        // Increase speed
        sgProjectileScale += sgSlugProjectileSizeAdj;   // Increase projectile size

        // Clamp all values to prevent invalid states
        sgSpread = ClampSGProjectileSpread();
        sgProjectileCount = ClampSGProjectileCount();
        sgDamage = ClampSGDamage();
        sgRecoilIntensity = ClampSGRecoilIntensity();
        sgProjSpeed = ClampSGProjectileSpeed();
        sgProjectileScale = ClampSGProjectileScale();

        // Update shotgun handler with new values
        UpdateShotgunHandler();
    }

    /// <summary>
    /// Applies blast upgrade modifications to shotgun
    /// Increases spread and projectile count for crowd control
    /// </summary>
    public void ModifyShotgunBlastQualities()
    {
        // Apply blast modifications
        sgSpread += sgBlastSpreadAdj;                   // Increase spread
        sgProjectileCount *= sgBlastProjectileCountAdj; // Multiply projectile count
        sgDamage += sgBlastDamageAdj;                   // Reduce damage per pellet
        sgRecoilIntensity += sgBlastRecoilAdj;          // Increase recoil
        sgProjSpeed += sgBlastProjectileSpeedAdj;       // Increase projectile speed

        // Clamp all values to prevent invalid states
        sgSpread = ClampSGProjectileSpread();
        sgProjectileCount = ClampSGProjectileCount();
        sgDamage = ClampSGDamage();
        sgRecoilIntensity = ClampSGRecoilIntensity();
        sgProjSpeed = ClampSGProjectileSpeed();

        // Update shotgun handler with new values
        UpdateShotgunHandler();
    }

    #region Clamping Functions
    #region Shotgun Clamping
    /// <summary>
    /// Clamps shotgun spread between min and max values
    /// </summary>
    private float ClampSGProjectileSpread()
    {
        return Mathf.Clamp(sgSpread, sgMinSpread, sgMaxSpread);
    }

    /// <summary>
    /// Clamps shotgun projectile count between min and max values
    /// </summary>
    private int ClampSGProjectileCount()
    {
        return Mathf.Clamp(sgProjectileCount, sgMinProjectileCount, sgMaxProjectileCount);
    }

    /// <summary>
    /// Clamps shotgun damage between min and max values
    /// </summary>
    private float ClampSGDamage()
    {
        return Mathf.Clamp(sgDamage, sgMinDamage, sgMaxDamage);
    }

    /// <summary>
    /// Clamps shotgun recoil intensity between min and max values
    /// </summary>
    private float ClampSGRecoilIntensity()
    {
        return Mathf.Clamp(sgRecoilIntensity, sgMinRecoilIntensity, sgMaxRecoilIntensity);
    }

    /// <summary>
    /// Clamps shotgun projectile speed between min and max values
    /// </summary>
    private float ClampSGProjectileSpeed()
    {
        return Mathf.Clamp(sgProjSpeed, sgMinProjSpeed, sgMaxProjSpeed);
    }

    /// <summary>
    /// Clamps shotgun projectile scale between min and max values
    /// </summary>
    private Vector3 ClampSGProjectileScale()
    {
        return new Vector3(
            Mathf.Clamp(sgProjectileScale.x, sgMinProjectileScale.x, sgMaxProjectileScale.x),
            Mathf.Clamp(sgProjectileScale.y, sgMinProjectileScale.y, sgMaxProjectileScale.y),
            Mathf.Clamp(sgProjectileScale.z, sgMinProjectileScale.z, sgMaxProjectileScale.z)
        );
    }
    #endregion
    #endregion
}