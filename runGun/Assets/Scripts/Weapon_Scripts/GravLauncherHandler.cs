using UnityEngine;

public class GravLauncherHandler : MonoBehaviour
{
    private PublicMover Player;
    public AmmoTracker Weapon;
    public GameObject Bullet;
    public GameObject GravField;
    public GameObject AltBuller;
    public GameObject AntiGravField;
    public Transform ProjectileSpawner;
    
    // Audio components
    public AudioSource audioSource;
    public AudioClip fireSound;
    
    // Cooldown settings
    public float fireCooldown = 1f;
    private float cooldownTimer = 0f;
    private bool canFire = true;
    
    // Reference to the recoil animation script
    private WeaponRecoilAnimation recoilAnimation;

    void Start(){
        Player = GameObject.Find("Player").GetComponent<PublicMover>();
        // ShotgunPellet = GameObject.Find("ShotgunPellet");
        // ProjectileSpawner = GameObject.Find("GravLauncherProjectileSpawner").transform;
        Weapon.onEnemyHitEffect = (float x) => {};
        Weapon.onHitEffect = (Vector3 pos) => {
            print("Hit!");
            print("Spawning" + GravField+ " at "+gameObject.transform.position);
            GameObjectPoolManager.SpawnObject(GravField, pos, Quaternion.identity);
        };
        
        // Get audio source component or add one if it doesn't exist
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
            
        // Get recoil animation component or add one if it doesn't exist
        recoilAnimation = GetComponent<WeaponRecoilAnimation>();
        if (recoilAnimation == null)
            recoilAnimation = gameObject.AddComponent<WeaponRecoilAnimation>();
    }

    // Update is called once per frame
    void Update()
    {
        // Handle cooldown timer
        if (!canFire)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                canFire = true;
            }
        }
        
        // shoots the gun
        if (Input.GetKeyDown(KeyCode.Mouse0) && canFire /*&& Weapon.getAmmo() > 0*/) {
            Fire();
            // Start cooldown
            canFire = false;
            cooldownTimer = fireCooldown;
        }
    }

    // consolidates all functions called upon firing
    void Fire(){
        //Weapon.fireAmmo(1);
        GameObjectPoolManager.SpawnObject(Bullet, ProjectileSpawner.position, ProjectileSpawner.rotation);
        
        // Play fire sound
        if (fireSound != null && audioSource != null)
            audioSource.PlayOneShot(fireSound);
            
        // Trigger recoil animation
        if (recoilAnimation != null)
            recoilAnimation.TriggerRecoil();
    }
    
    // Setter for fire cooldown
    public void SetFireCooldown(float cooldown)
    {
        fireCooldown = Mathf.Max(0.1f, cooldown);
    }
}