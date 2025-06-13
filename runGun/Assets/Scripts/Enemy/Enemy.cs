using UnityEngine;
using UnityEngine.AI;

public class Enemy : EnemyStats
{
    [Header("Movement & Combat")]
    [SerializeField] [Tooltip("The NavMeshAgent component used for pathfinding")]
    protected NavMeshAgent agent;
    
    [SerializeField] [Tooltip("Maximum distance at which the enemy can attack the player")]
    protected float attackRange = 2f;
    
    [SerializeField] [Tooltip("Minimum distance the enemy tries to maintain from the player")]
    protected float bufferRange = 0.5f;
    
    [SerializeField] [Tooltip("Time in seconds between attacks")]
    protected float attackCooldown = 2f;
    
    protected float nextAttackTime = 0f;
    protected float baseSpeed;
    
    /// <summary>
    /// Initialize components and references
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        
        // Get and configure the agent
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        
        // Store base speed for scaling
        baseSpeed = agent != null ? agent.speed : 0f;
    }
    
    /// <summary>
    /// Reset enemy state when activated from pool
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();
        
        // Reset attack timer with some randomization
        nextAttackTime = Time.time + Random.Range(0f, attackCooldown * 0.5f);
    }
    
    /// <summary>
    /// Update enemy AI behavior and visual effects
    /// </summary>
protected override void Update()
{
    // Update flash effect from base class
    base.Update();
    
    if (isDead || player == null) return;
    
    // Check if NavMeshAgent is valid before using it
    if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh)
    {
        // Try to reposition the agent onto a valid NavMesh if possible
        if (agent != null && agent.isActiveAndEnabled)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                // May need to disable and re-enable the agent
                agent.enabled = false;
                agent.enabled = true;
            }
        }
        return; // Skip processing this frame
    }
    
    float distanceToPlayer = Vector3.Distance(transform.position, player.position);
    
    // Enemy is too close - stop and attack
    if (distanceToPlayer <= bufferRange)
    {
        agent.isStopped = true;
        AttemptAttack();
    }
    // Enemy is within attack range - move to optimal distance and attack
    else if (distanceToPlayer <= attackRange)
    {
        agent.isStopped = false;
        
        // Calculate a position at buffer distance from player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 targetPosition = player.position - directionToPlayer * bufferRange;
        
        // Check if SetDestination can be called
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(targetPosition);
        }
        AttemptAttack();
    }
    // Enemy is outside attack range - chase player
    else
    {
        agent.isStopped = false;
        
        // Check if SetDestination can be called
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
        }
    }
}
    
    /// <summary>
    /// Tries to attack the player if cooldown has expired
    /// </summary>
    protected virtual void AttemptAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            DealDamageToPlayer();
            nextAttackTime = Time.time + attackCooldown;
        }
    }
    
    /// <summary>
    /// Apply stat multipliers - override to include agent speed
    /// </summary>
    protected override void ApplyStatMultipliers()
    {
        base.ApplyStatMultipliers();
        
        // Apply speed multiplier if agent exists
        if (agent != null)
        {
            agent.speed = baseSpeed * speedMultiplier;
        }
    }
    
    /// <summary>
    /// Push the enemy with an impulse force
    /// </summary>
    public void PushImpulse(Vector3 forceVector) 
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(forceVector, ForceMode.Impulse);
        }
    }
    
    /// <summary>
    /// Push the enemy with an acceleration force
    /// </summary>
    public void PushAccel(Vector3 forceVector) 
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(forceVector, ForceMode.Acceleration);
        }
    }
    
    /// <summary>
    /// Draws visualization of attack and buffer ranges in the editor
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Draw buffer range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, bufferRange);
    }
}