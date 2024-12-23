using UnityEngine;
using UnityEngine.Animations;

public class Player : MonoBehaviour
{
    public enum FacingDir
    {
        Right,
        Left
    }

    [Header("Component Refs")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private AnimatorController animator;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 12f;
    [Tooltip("Time from rest to full speed")]
    [SerializeField] private float accelerationTime = 0.1f;
    [Tooltip("Time from full speed to rest")]
    [SerializeField] private float decelerationTime = 0.1f;

    [Header("Jumping Settings")]
    [SerializeField] private float maxJumpHeight = 8;
    [Tooltip("Time to reach the peak of a max-height jump")]
    [SerializeField] private float jumpDuration = 1.2f;
    [Tooltip("Time the player can press jump after grounded and still register a jump")]
    [SerializeField] private float coyoteTime = 0.2f;
    [Tooltip("Time the player can press jump before grounded and still register a jump")]
    [SerializeField] private float jumpBufferTime = 0.2f;
    [Tooltip("Gravity multiplier for when the player is falling")]
    [SerializeField] private float fallGravityMultiplier = 2f;

    [Header("Wall Cling Settings")]
    [Tooltip("How long to wall cling before sliding")]
    [SerializeField] private float wallClingTime = 0.2f;

    [Header("Wall Slide Settings")]
    [SerializeField] private float maxWallSlideSpeed = 10f;
    [Tooltip("Gravity multiplier for when the player is wall sliding")]
    [SerializeField] private float wallSlideGravityMultiplier = 0.2f;

    [Header("Wall Jump Settings")]
    [SerializeField] private float wallJumpForce = 15f;
    [SerializeField] private Vector2 wallJumpAngle = new Vector2(1, 2);
    [Tooltip("How long before the player can re-cling to the wall")]
    [SerializeField] private float wallJumpCooldown = 0.2f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Wall Check")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance = 0.1f;
    [SerializeField] private LayerMask wallLayer;

    [Header("Health Settings")]
    [SerializeField] public float maxHealth;

    [Header("Needle Settings")]
    [SerializeField] public GameObject needlePrefab;
    [SerializeField] public float throwRechargeTime;
    [SerializeField] public float shootForce;
    [SerializeField] public bool hasNeedle;

    public static Player Instance { get; private set; }

    //physics vars
    private float gravity;
    private float jumpForce;

    private FacingDir currentFacing = FacingDir.Right;
    private float moveDirection;
    private bool isJumpHeld;

    // Ground and wall state tracking
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallClinging;
    private bool isWallSliding;
    private bool isWallJumping;

    // Timing and jump tracking
    private float coyoteTimeTimer;
    private float jumpBufferTimer;
    private float wallClingTimer;
    private float wallJumpTimer;

    //state
    public static float CurrHealth { get; private set; }
    public static float MaxHealth { get { return Instance.maxHealth; } }

    private float nextThrowTime = 0f;

    private void Awake()
    {
        //singleton initialization
        if (Instance != null)
        {
            Debug.LogWarning($"A later instance of {nameof(CheckIsGrounded)} on {gameObject.name} was destroyed to keep an earlier instance on {Instance.gameObject.name}.");
            DestroyImmediate(this);
            return;
        }
        Instance = this;

        CurrHealth = maxHealth;

        CalculateJumpPhysics();
    }

    private void Update()
    {
        ProcessMoveInput();
        ProcessJumpInput();
        ProcessGround();
        ProcessWall();
        UpdateNeedle();
    }

    private void FixedUpdate()
    {
        DecrementTimers();
        Move();
        Jump();
        WallCling();
        WallSlide();
        ModifyGravity();
    }

    /// <summary>
    /// Processes player movement input
    /// </summary>
    private void ProcessMoveInput()
    {
        moveDirection = Input.GetAxisRaw("Horizontal");

        if (moveDirection > 0 && currentFacing == FacingDir.Left)
        {
            SetFacingDir(FacingDir.Right);
        }
        else if (moveDirection < 0 && currentFacing == FacingDir.Right)
        {
            SetFacingDir(FacingDir.Left);
        }
    }

    /// <summary>
    /// Updates state related to player grounded
    /// </summary>
    private void ProcessGround()
    {
        isGrounded = CheckIsGrounded();

        if (isGrounded)
        {
            coyoteTimeTimer = coyoteTime;
        }
    }

    /// <summary>
    /// Upates state related to walls
    /// </summary>
    private void ProcessWall()
    {
        isTouchingWall = CheckIsTouchingWall();

        bool isHoldingIntoWall = (currentFacing == FacingDir.Right && moveDirection > 0) ||
                                (currentFacing == FacingDir.Left && moveDirection < 0);

        if (isGrounded || !isTouchingWall || !isHoldingIntoWall || wallJumpTimer > 0)
        {
            isWallClinging = false;
            isWallSliding = false;
            return;
        }

        // Start a new wall cling
        if (!isWallClinging && !isWallSliding)
        {
            isWallClinging = true;
            wallClingTimer = wallClingTime;
        }

        // Transition from cling to slide
        if (isWallClinging && wallClingTimer <= 0)
        {
            isWallClinging = false;
            isWallSliding = true;
        }
    }

    /// <summary>
    /// Checks if player is grounded
    /// </summary>
    private bool CheckIsGrounded()
    {
        // Check against all ground overlaps
        Collider2D[] hits = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundLayer);
        if (hits.Length == 0)
        {
            return false;
        }

        Collider2D closestGround = null;
        float minDistance = float.MaxValue;
        foreach (var hit in hits)
        {
            Vector2 closestPoint = hit.ClosestPoint(groundCheck.position);
            float dist = (closestPoint - (Vector2)groundCheck.position).sqrMagnitude;
            if (dist < minDistance)
            {
                minDistance = dist;
                closestGround = hit;
            }
        }

        if (closestGround == null)
        {
            return false;
        }

        var currentGround = closestGround.attachedRigidbody;
        float groundVerticalVel = currentGround ? currentGround.velocity.y : 0f;
        float relativeVerticalVel = rb.velocity.y - groundVerticalVel;

        // If the player is moving upwards they are not grounded
        if (relativeVerticalVel > 0f)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if player is touching a wall
    /// </summary>
    private bool CheckIsTouchingWall()
    {
        Vector2 wallCheckDir = currentFacing == FacingDir.Right ? Vector2.right : Vector2.left;
        return Physics2D.Raycast(wallCheck.position, wallCheckDir, wallCheckDistance, wallLayer);
    }

    /// <summary>
    /// Handles player movement with smooth acceleration and deceleration
    /// </summary>
    private void Move()
    {
        // Prevent horizontal movement when on a wall
        if (isWallClinging || isWallSliding)
        {
            return;
        }

        // Temporarily prevent horizontal movement when jumping off a wall
        if (isWallJumping)
        {
            if (wallJumpTimer <= 0)
            {
                isWallJumping = false;
            }
            return;
        }

        float targetSpeed = moveDirection * moveSpeed;
        float speedDiff = targetSpeed - rb.velocity.x;

        bool isAccelerating = Mathf.Abs(targetSpeed) > Mathf.Abs(rb.velocity.x);
        float chosenTime = isAccelerating ? accelerationTime : decelerationTime;

        if (chosenTime == 0f)
        {
            rb.velocity = new Vector2(targetSpeed, rb.velocity.y);
            return; // No need to apply force.
        }

        float force = speedDiff / chosenTime * rb.mass;
        rb.AddForce(force * Vector2.right, ForceMode2D.Force);
    }

    /// <summary>
    /// Processes jump input with coyote time and jump buffering
    /// </summary>
    private void ProcessJumpInput()
    {
        isJumpHeld = Input.GetButton("Jump");
        // GetButtonDown is only active on the first frame it is pressed
        if (Input.GetButtonDown("Jump")) jumpBufferTimer = jumpBufferTime;
    }

    /// <summary>
    /// Perform jumps
    /// </summary>
    private void Jump()
    {
        if (jumpBufferTimer <= 0) return;

        // Ground jump
        if (coyoteTimeTimer > 0)
        {
            PerformJump();
        }
        // Wall jump
        else if (isWallClinging || isWallSliding)
        {
            PerformWallJump();
        }

    }

    /// <summary>
    /// Performs a standard ground jump
    /// </summary>
    private void PerformJump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        jumpBufferTimer = 0f;
        coyoteTimeTimer = 0f;
    }

    /// <summary>
    /// Performs a wall jump
    /// </summary>
    private void PerformWallJump()
    {
        // Determine wall jump direction based on facing
        FacingDir wallJumpDirection = (currentFacing == FacingDir.Right) ? FacingDir.Left : FacingDir.Right;

        // Reset velocity and apply wall jump force
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(
            (wallJumpDirection == FacingDir.Left ? -1 : 1) * wallJumpForce * wallJumpAngle.x,
            wallJumpForce * wallJumpAngle.y
        ), ForceMode2D.Impulse);

        // Set wall jump state
        jumpBufferTimer = 0f;
        wallJumpTimer = wallJumpCooldown;
        isWallJumping = true;

        SetFacingDir(wallJumpDirection);
    }

    /// <summary>
    /// Handles wall clinging behavior
    /// </summary>
    private void WallCling()
    {
        if (!isWallClinging) return;

        rb.velocity = new Vector2(rb.velocity.x, 0);
    }

    /// <summary>
    /// Handles wall sliding behavior
    /// </summary>
    private void WallSlide()
    {
        if (!isWallSliding) return;

        // Cap wall sliding speed
        if (rb.velocity.y < -maxWallSlideSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, -maxWallSlideSpeed);
        }
    }

    /// <summary>
    /// Apply additional gravity for more responsive jumping and wall sliding
    /// </summary>
    private void ModifyGravity()
    {
        float gravModifier = 1f;

        if (isWallClinging)
        {
            // Do not fall when wall clinging
            gravModifier = 0f;
        }
        if (isWallSliding)
        {
            // Reduced gravity while wall sliding
            gravModifier = wallSlideGravityMultiplier;
        }
        else if (rb.velocity.y < 0 || (rb.velocity.y > 0 && !isJumpHeld))
        {
            // Increased gravity while falling or player released the jump button
            gravModifier = fallGravityMultiplier;
        }

        rb.velocity += Vector2.up * gravity * (gravModifier - 1) * Time.deltaTime;
    }

    /// <summary>
    /// Sets facing direction of the player and flips the sprite
    /// </summary>
    private void SetFacingDir(FacingDir newFacing)
    {
        // Only flip if facing direction has changed
        if (currentFacing == newFacing) return;

        // Update current facing direction
        currentFacing = newFacing;

        // Flip using scale
        Vector3 localScale = transform.localScale;
        localScale.x = Mathf.Abs(localScale.x) * (newFacing == FacingDir.Right ? 1 : -1);
        transform.localScale = localScale;
    }

    /// <summary>
    /// Decrement all timers
    /// </summary>
    private void DecrementTimers()
    {
        if (!isGrounded) coyoteTimeTimer -= Time.fixedDeltaTime;
        jumpBufferTimer -= Time.fixedDeltaTime;
        if (isWallClinging) wallClingTimer -= Time.fixedDeltaTime;
        if (isWallJumping) wallJumpTimer -= Time.fixedDeltaTime;
    }

    private void CalculateJumpPhysics()
    {
        if (rb == null) return;
        if (jumpDuration <= 0) return;

        // Calculate required gravity for desired arc
        gravity = (-2f * maxJumpHeight) / (jumpDuration * jumpDuration);

        // Calculate initial velocity needed
        float requiredVelocity = Mathf.Sqrt(-2f * gravity * maxJumpHeight);

        // Convert to impulse force (F = mv)
        jumpForce = requiredVelocity * rb.mass;

        // Set gravity scale
        rb.gravityScale = gravity / Physics2D.gravity.y;
    }

    /// <summary>
    /// Visualize ground and wall check
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Ground check visualization
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // Wall check visualization
        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Vector2 wallCheckDir = currentFacing == FacingDir.Right ? Vector2.right : Vector2.right * -1;
            Gizmos.DrawRay(wallCheck.position, wallCheckDir * wallCheckDistance);
        }
    }

    private void UpdateNeedle()
    {
        if (Input.GetMouseButtonDown(0) && hasNeedle && Time.time >= nextThrowTime) // 0 for left mouse button
        {
            // Get the mouse position in world space
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // Ensure it's on the same 2D plane as the player

            // Calculate direction from player to mouse
            Vector2 shootDirection = (mousePosition - transform.position).normalized;

            // Offset the instantiation location to be outside the player collider
            float spawnDistance = 1.0f; // Adjust based on your player's collider size
            Vector3 instantiationLocation = transform.position + (Vector3)(shootDirection * spawnDistance);

            // Calculate the rotation for the needle
            float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg + 270;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            // Instantiate the needle
            GameObject needle = Instantiate(needlePrefab, instantiationLocation, rotation);

            // Apply velocity to the needle
            Rigidbody2D rb = needle.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = shootDirection * shootForce;
            }
            nextThrowTime = Time.time + throwRechargeTime;
            hasNeedle = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player touches a spear
        if (other.CompareTag("Needle"))
        {
            // Destroy the spear if the recharge time has passed
            if (Time.time >= nextThrowTime)
            {
                Destroy(other.gameObject);
                hasNeedle = true;
            }
        }
    }

    private void OnValidate()
    {
        CalculateJumpPhysics();
    }

    public static void TakeDamage(float damage)
    {
        CurrHealth -= damage;
        if (CurrHealth <= 0) Debug.Log("Player died!");
    }
}
