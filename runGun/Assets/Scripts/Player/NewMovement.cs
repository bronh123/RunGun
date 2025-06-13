using UnityEngine;

[RequireComponent(typeof(PlayerStats))] // Require the PlayerStats component
public class NewMovement : MonoBehaviour
{
    // Reference to orientation transform (for camera-relative movement)
    [Tooltip("Transform that defines the player's orientation for movement direction")]
    [SerializeField] Transform orientation;
    [SerializeField] GameObject playerBody;

    [Header("Movement")]
    [Tooltip("Base movement speed before player stat modifications. MAY NOT WORK PROPERLY, MAYBE CHANGE IN PLAYER STATS.")]
    [SerializeField] float baseMoveSpeed = 6f;

    [Tooltip("Multiplier applied to movement force for tuning responsiveness")]
    [SerializeField] float moveMultiplier = 10f;

    [Tooltip("Multiplier for air movement control (lower means less control in air)")]
    [SerializeField] float airMultiplier = .8f;

    [Tooltip("Drag applied when on ground (higher values stop faster)")]
    [SerializeField] float groundDrag = 6f;

    [Tooltip("Drag applied when in air (lower than ground drag for physics feel)")]
    [SerializeField] float airDrag = 2f;

    [Tooltip("Time window in seconds where bunny hop physics are applied after leaving ground")]
    [SerializeField] float bHopWindow = 4f;

    [Tooltip("Base jump force before player stat modifications. CHANGE THIS IN PLAYER STATS SCRIPT")]
    [SerializeField] float baseJumpForce = 30f;

    [Header("Gravity")]
    [Tooltip("Starting gravity force when player begins falling")]
    [SerializeField] float initialGravityForce = -70f;

    [Tooltip("Maximum gravity force that can be applied (negative means downward)")]
    [SerializeField] float maxGravityForce = -200f;

    [Tooltip("How quickly gravity force increases during fall")]
    [SerializeField] float gravityAcceleration = 20f;

    [Tooltip("Maximum downward velocity the player can reach")]
    [SerializeField] float terminalFallVelocity = -40f;

    // Current gravity force being applied to player
    private float currentGravityForce;

    // Tracks how long the player has been falling
    private float fallTime = 0f;

    [Tooltip("Enables automatic jump when holding jump key (easier bunny hopping)")]
    [SerializeField] bool easyBHop = false;

    // Whether the player is currently in a jump
    private bool jumping = false;

    // Whether the jump key is currently being held down
    private bool jumpKeyHeld = false;

    // Tracks time spent on ground for bunny hopping mechanics
    float timeOnGround = 0f;

    // Reference to the PlayerStats component
    private PlayerStats playerStats;

    // Current effective move speed after player stat modifications
    private float currentMoveSpeed;

    // Current effective jump force after player stat modifications
    private float currentJumpForce;

    [Header("keybinds")]
    [Tooltip("Key used for jumping")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Detection")]
    [Tooltip("Layers that are considered as ground for detection")]
    [SerializeField] LayerMask groundMask;

    [Tooltip("Transform position where ground checks are performed")]
    [SerializeField] Transform groundCheck;

    [Tooltip("Radius of sphere cast used to check for ground")]
    [SerializeField] private float groundDistance = 0.4f;

    // Whether the player is currently touching the ground
    bool isGrounded;

    public bool getIsGrounded()
    {
        return isGrounded;
    }

    // Reference to player's rigidbody component
    Rigidbody rb;

    // Stores information about slope raycast hit
    RaycastHit slopeHit;

    // Whether player is currently on a sloped surface
    bool onSlope;

    // Current movement direction based on input
    Vector3 moveDirection;

    // Movement direction adjusted for slopes
    Vector3 slopeMoveDirection;

    // Horizontal input value (-1 to 1)
    float horizontalMovement;

    // Vertical input value (-1 to 1)
    float verticalMovement;

    // Height of player for ground checks and calculations
    float playerHeight = 2f;


    // Checks if player is on a sloped surface
    private bool OnSlope()
    {
        // Cast ray downward to detect ground
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2f + .5f))
        {
            // If the normal isn't pointing straight up, it's a slope
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
        }
        return false;
    }

    private void Start()
    {
        // Get and configure the rigidbody
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;  // Prevent physics from rotating the player

        // Get reference to PlayerStats component
        playerStats = GetComponent<PlayerStats>();

        // Initialize current move speed with base value if PlayerStats is not found
        if (playerStats == null)
        {
            Debug.LogWarning("PlayerStats component not found! Using baseMoveSpeed instead.");
            currentMoveSpeed = baseMoveSpeed;
        }

        // Initialize gravity force
        currentGravityForce = initialGravityForce;
    }

    private void Update()
    {
        // Update current movement speed from player stats
        UpdateStats();
        if (playerBody != null)
        {
            Debug.Log($"Moving player body to 0 from {playerBody.transform.localPosition}");
            playerBody.transform.localPosition = Vector3.zero;
            Debug.Log($"Moved to {playerBody.transform.localPosition}");
        }

        // Check if player is on ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Get player input and update drag
        MyInput();
        ControlDrag();

        jumpKeyHeld = Input.GetKey(jumpKey);
        // Handle jumping
        if (Input.GetKeyDown(jumpKey) && isGrounded && !jumping)
        {
            Jump();
        }

        // Project movement direction onto slope surface
        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);

        // Handle gravity and falling
        ManageFalling();
    }

    // Manages falling mechanics
    void ManageFalling()
    {
        // Reset fall time and gravity when grounded
        // rb velocity check designed to only apply increased gravity while the player is falling, not just aerial.
        if (isGrounded || rb.linearVelocity.y >= 0f)
        {
            fallTime = 0f;
            currentGravityForce = initialGravityForce;
        }
        else if (!isGrounded && rb.linearVelocity.y < 0f)
        {
            // Increase fall time when in air
            fallTime += Time.deltaTime;

            // Increase gravity force over time, but cap it at maximum
            currentGravityForce = Mathf.Max(maxGravityForce, initialGravityForce - (gravityAcceleration * fallTime));

            // If falling too fast, cap the downward velocity to terminal velocity
            if (rb.linearVelocity.y < terminalFallVelocity)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, terminalFallVelocity, rb.linearVelocity.z);
            }
        }
    }

    // Updates current move speed from PlayerStats
    void UpdateStats()
    {
        if (playerStats != null)
        {
            // Use player stats move speed 
            currentMoveSpeed = playerStats.moveSpeed;
            currentJumpForce = playerStats.jumpForce;
        }
        else
        {
            // Fallback to base speed if no PlayerStats exists
            currentMoveSpeed = baseMoveSpeed;
            currentJumpForce = baseJumpForce;
        }
    }

    // Applies jump force
    void Jump()
    {
        // Reset vertical velocity before jumping
        jumping = true;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(transform.up * currentJumpForce, ForceMode.Impulse);
    }

    // Gets movement input
    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        // Calculate movement direction relative to orientation
        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }

    // Adjusts drag based on ground state
    void ControlDrag()
    {
        if (isGrounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = airDrag;
        }
    }

    // Physics calculations happen in FixedUpdate
    private void FixedUpdate()
    {
        onSlope = OnSlope();
        if (isGrounded && jumpKeyHeld && easyBHop && !jumping)
        {
            Jump();
        }
        MovePlayer();
    }

    // Applies movement forces to the player
    void MovePlayer()
    {
        // If on ground and on a slope
        if (isGrounded && onSlope)
        {
            // Use slope-adjusted direction
            jumping = false;
            rb.AddForce(slopeMoveDirection * currentMoveSpeed * moveMultiplier, ForceMode.Acceleration);
        }
        // If on ground but not on slope
        else if (isGrounded)
        {
            jumping = false;
            // Track time on ground for bunny hop mechanics
            timeOnGround += Time.fixedDeltaTime;

            // If within bunny hop window, maintain some air mobility
            if (timeOnGround < bHopWindow)
            {
                rb.AddForce(moveDirection * currentMoveSpeed * moveMultiplier * airMultiplier, ForceMode.Acceleration);
            }
            else  // Normal ground movement
            {
                rb.AddForce(moveDirection * currentMoveSpeed * moveMultiplier, ForceMode.Acceleration);
            }
        }
        // If in air
        else
        {
            // Reset ground time when airborne
            timeOnGround = 0f;

            // Apply air movement with reduced control
            rb.AddForce(moveDirection * currentMoveSpeed * moveMultiplier * airMultiplier, ForceMode.Acceleration);

            // Apply progressively stronger gravity
            rb.AddForce(new Vector3(0, currentGravityForce, 0), ForceMode.Acceleration);
        }
    }
}