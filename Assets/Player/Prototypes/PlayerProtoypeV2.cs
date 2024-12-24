using UnityEngine;

public class PlayerProtoypeV2 : MonoBehaviour
{
    //NOTES:
    //  A/D on ground to walk left/right
    //  W to jump
    //  Space to dash
    //  A/D into wall to cling
    //  Space during wall cling to jump
    //  mouse click for needle

    [Header("Component Refs")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float accelerationTime = 0.1f;
    [SerializeField] private float decelerationTime = 0.1f;

    [Header("Jumping Settings")]
    //[SerializeField] private float maxJumpHeight = 8;
    //[SerializeField] private float jumpDuration = 1.2f;
    [SerializeField] private float jumpForce;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;
    [SerializeField] private float fallGravityMultiplier = 2f;

    [Header("Wall Cling Settings")]
    [SerializeField] private float wallClingTime = 0.2f;

    [Header("Wall Slide Settings")]
    [SerializeField] private float maxWallSlideSpeed = 10f;
    [SerializeField] private float wallSlideGravityMultiplier = 0.2f;

    [Header("Wall Jump Settings")]
    [SerializeField] private float wallJumpForce = 15f;
    [SerializeField] private Vector2 wallJumpAngle = new Vector2(1, 2);
    [SerializeField] private float wallJumpCooldown = 0.2f;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashRefreshDuration;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Wall Check")]
    [SerializeField] private Transform leftWallCheck;
    [SerializeField] private Transform rightWallCheck;
    [SerializeField] private float wallCheckDistance = 0.1f;
    [SerializeField] private LayerMask wallLayer;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth;

    [Header("Needle Settings")]
    [SerializeField] private GameObject needlePrefab;
    [SerializeField] private float throwRechargeTime;
    [SerializeField] private float shootForce;

    //physics vars
    private float gravity;
    //private float jumpForce;
    private bool isFacingLeft;
    private float moveDirection;
    private bool isJumpHeld;

    // Ground and wall state tracking
    private bool isGrounded;
    private bool isTouchingLeftWall;
    private bool isTouchingRightWall;
    //private bool isWallClinging;
    //private bool isWallSliding;
    //private bool isWallJumping;

    // Timing and jump tracking
    private float coyoteTimeTimer;
    private float jumpBufferTimer;
    private float wallClingTimer;
    private float wallJumpTimer;
    private float dashTimer;
    private float dashResetTimer;

    public static float CurrHealth { get; private set; }
    public static float MaxHealth { get; private set; }

    private float nextThrowTime = 0f;
    private bool hasNeedle;

    private void Awake() {
        MaxHealth = maxHealth;
        CurrHealth = maxHealth;

        dashTimer = 0f;
    }

    private void Update() {

        JumpInput();
        DashInput();


        ProcessTerrain();
        ProcessHorizontalInput();
    }

    private void FixedUpdate() {
        Move();
    }

    private void ProcessTerrain() {
        //NOTE: should prob either be doing BoxCastNonAlloc or a row of raycasting along a surface to see if Player made contact. But this is fine for prototyping
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius, groundLayer);
        isTouchingLeftWall = Physics2D.Raycast(leftWallCheck.position, Vector2.left, wallCheckDistance, wallLayer);
        isTouchingRightWall = Physics2D.Raycast(rightWallCheck.position, Vector2.right, wallCheckDistance, wallLayer);
    }

    private void ProcessHorizontalInput() {
        moveDirection = Input.GetAxisRaw("Horizontal");

        //if player has dashed, lock direction
        if (dashTimer > 0f) return;

        // If player is on a wall, override direction
        if (!isGrounded && isTouchingLeftWall) {
            isFacingLeft = false;
            if (moveDirection < 0) moveDirection = 0;
        }
        else if (!isGrounded && isTouchingRightWall) {
            isFacingLeft = true;
            if (moveDirection > 0) moveDirection = 0;
        }

        // Else, let horizontal controller input modify direction
        else if (moveDirection < 0) isFacingLeft = true;
        else if (moveDirection > 0) isFacingLeft = false;
    }

    private void JumpInput(){ if (isGrounded && Input.GetButtonDown("Jump")){ rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse); } }

    private void DashInput() { 

        if (dashResetTimer <= 0 && Input.GetButtonDown("Dash")) { 
            dashTimer = dashDuration;
            dashResetTimer = dashRefreshDuration;
            Debug.Log("dashed");
        }
    }

    private void Move() {
        Vector2 velocity = rb.velocity;
        velocity.x = dashTimer <= 0 ? moveDirection * moveSpeed : (isFacingLeft ? -1 : 1) * dashSpeed;
        if (dashTimer > 0f) velocity.y = 0;
        rb.velocity = velocity;

        if (dashTimer > 0) dashTimer -= Time.fixedDeltaTime;
        if (dashResetTimer > 0) dashResetTimer -= Time.fixedDeltaTime;
    }

}
