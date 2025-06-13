using System.Collections;
using UnityEngine;

public class FlyingEnemy : EnemyStats
{
    [Header("References")]
    [SerializeField] public WaypointHolder waypointHolder;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float rotationSpeed = 7.5f;
    [SerializeField] private float circleDuration = 5f;
    [SerializeField] private float waypointDistanceThreshold = 2f;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 35f;
    [SerializeField] private float attackDuration = 3f;
    [SerializeField] private float shootInterval = 0.5f;

    [Header("Projectile Settings")]
    [SerializeField] private float projectileSpeed = 60f;
    [SerializeField] private float angleToShootAtPlayer = 0.1f;

    // Internal state
    private Transform currentWaypointTarget;
    private Transform[] waypoints;
    private float baseMoveSpeed;

    protected override void Awake()
    {
        base.Awake();
        
        // Store base values for scaling
        baseMoveSpeed = moveSpeed;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        // If waypointHolder wasn't assigned, try to find one
        if (waypointHolder == null)
        {
            waypointHolder = FindFirstObjectByType<WaypointHolder>();
            Debug.Log("Attempting to find WaypointHolder: " + (waypointHolder != null ? "Found" : "Not found"));
        }
        
        if (waypointHolder != null)
        {
            waypointHolder.RefreshWaypoints();
            waypoints = waypointHolder.Waypoints;
            Debug.Log($"Found {(waypoints != null ? waypoints.Length : 0)} waypoints");
        }
        else
        {
            Debug.LogError("No waypointHolder assigned to flying enemy!");
            return; // Don't proceed with coroutines if we don't have waypoints
        }
        
        StopAllCoroutines();
        
        if (waypoints != null && waypoints.Length > 0)
        {
            StartCoroutine(StateMachine());
        }
        else
        {
            Debug.LogError("Waypoints array is empty! Flying enemy won't move.");
        }
    }
    
    protected override void Update()
    {
        // Only call base Update to handle damage flash
        base.Update();
    }

    private void FaceTarget(Transform t, Vector3 targetPos)
    {
        Vector3 dir = targetPos - t.position;
        if (dir.sqrMagnitude < 0.0001f) return;

        dir.Normalize();
        t.rotation = Quaternion.Lerp(t.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotationSpeed);
    }

    private bool IsFacingPlayer(float angleThreshold)
    {
        if (!player) return true;
        Vector3 toPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, toPlayer);
        return angle <= angleThreshold;
    }

    private IEnumerator RotateUntilFacingPlayer(float angleThreshold)
    {
        while (!IsFacingPlayer(angleThreshold) && !isDead)
        {
            FaceTarget(transform, player.position);
            yield return null;
        }
    }

    private void PickRandomWaypoint()
    {
        if (waypoints != null && waypoints.Length > 0)
        {
            currentWaypointTarget = waypoints[Random.Range(0, waypoints.Length)];
        }
    }

    private bool ReachedWaypoint()
    {
        if (!currentWaypointTarget) return false;
        return Vector3.Distance(transform.position, currentWaypointTarget.position) < waypointDistanceThreshold;
    }

    private void MoveTowardsTarget(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        if (dir.sqrMagnitude < 0.0001f) return;

        dir.Normalize();
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            Time.deltaTime * rotationSpeed
        );
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    private float DistanceToPlayer()
    {
        if (!player) return float.MaxValue;
        return Vector3.Distance(transform.position, player.position);
    }

    private void FireProjectile()
    {
        if (!projectilePrefab || isDead) return;
        var spawn = projectileSpawnPoint ? projectileSpawnPoint : transform;

        var proj = Instantiate(projectilePrefab, spawn.position, spawn.rotation);
        FaceTarget(proj.transform, player.position);

        var projectile = proj.GetComponent<Projectile>();
        if (projectile)
        {
            projectile.damage = damage;
        }

        var rb = proj.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.linearVelocity = spawn.forward * projectileSpeed;
        }
        
        DealDamageToPlayer();
    }

    // Override to avoid actual damage application
    public override int DealDamageToPlayer()
    {
        // Only play attack sound without actually dealing damage
        // (damage is handled by projectiles)
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
        
        return 0;
    }

    private IEnumerator CircleState(float duration)
    {
        float timer = 0f;
        PickRandomWaypoint();

        while (timer < duration && !isDead)
        {
            timer += Time.deltaTime;
            
            if (currentWaypointTarget)
                MoveTowardsTarget(currentWaypointTarget.position);

            if (ReachedWaypoint()) 
                PickRandomWaypoint();

            yield return null;
        }
    }

    private IEnumerator AttackState(float duration)
    {
        if (isDead) yield break;
        
        yield return StartCoroutine(RotateUntilFacingPlayer(angleToShootAtPlayer));
        
        if (isDead) yield break;
        
        FireProjectile();

        float timer = 0f;
        float shootTimer = 0f;

        while (timer < duration && !isDead)
        {
            timer += Time.deltaTime;
            shootTimer += Time.deltaTime;

            FaceTarget(transform, player.position);

            if (DistanceToPlayer() > attackRange)
            {
                MoveTowardsTarget(player.position);
            }

            if (shootTimer >= shootInterval)
            {
                shootTimer = 0f;
                FireProjectile();
            }
            
            yield return null;
        }
    }

    private IEnumerator StateMachine()
    {
        while (!isDead)
        {
            var randomTimeOffset = Random.Range(-2.5f, 2.5f);
            yield return StartCoroutine(CircleState(circleDuration + randomTimeOffset));
            
            if (isDead) yield break;
            
            var randomTimeOffset2 = Random.Range(-1.5f, 1.5f);
            yield return StartCoroutine(AttackState(attackDuration + randomTimeOffset2));
            
            // Short pause between state transitions
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    public override void KillEnemy()
    {
        if (isDead) return;
        
        // Stop all movement and attack coroutines first
        StopAllCoroutines();
        
        // Call base implementation which handles all the common functionality
        base.KillEnemy();
    }
    
    // Override to include move speed
    protected override void ApplyStatMultipliers()
    {
        base.ApplyStatMultipliers();
        moveSpeed = baseMoveSpeed * speedMultiplier;
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Draw waypoint connections if available
        if (waypointHolder != null && waypointHolder.Waypoints != null)
        {
            Gizmos.color = Color.cyan;
            foreach (var waypoint in waypointHolder.Waypoints)
            {
                if (waypoint == null) continue;
                Gizmos.DrawWireSphere(waypoint.position, waypointDistanceThreshold);
            }
        }
        
        // Draw current target if available
        if (currentWaypointTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, currentWaypointTarget.position);
        }
    }
}