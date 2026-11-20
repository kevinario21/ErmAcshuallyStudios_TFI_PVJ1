using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpForce = 12f;

    [Header("Wall Slide & Climb")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.2f, 0.5f);
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float wallClimbSpeed = 5f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private PlayerStats playerStats;

    private float horizontalInput;
    private float verticalInput;
    private bool isGrounded;
    private bool isTouchingWall;
    private int wallDirection; // -1 izquierda, 1 derecha, 0 sin pared
    private bool isWallSliding;
    private bool jump;
    private bool canMove = true;
    private float wallJumpLockTimer;
    private float initialWallCheckX;

    public void SetMovementState(bool state) => canMove = state;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        playerStats = GetComponent<PlayerStats>();

        if (playerStats != null)
            playerStats.Checkpoint = transform.position;

        if (wallCheck != null)
        {
            initialWallCheckX = Mathf.Abs(wallCheck.localPosition.x);
        }
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState == GameState.Playing)
        {
            horizontalInput = canMove ? Input.GetAxisRaw("Horizontal") : 0f;
            verticalInput = canMove ? Input.GetAxisRaw("Vertical") : 0f;

            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            UpdateWallCheckPosition();
            DetectWalls();

            if (wallJumpLockTimer > 0f)
            {
                wallJumpLockTimer -= Time.deltaTime;
            }

            if (canMove && Input.GetButtonDown("Jump"))
            {
                if (isGrounded)
                {
                    jump = true;
                }
                else if (isWallSliding && wallJumpLockTimer <= 0f)
                {
                    JumpFromWall();
                }
            }

            HandleWallSlideAndClimb();
            UpdateAnimator();
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.CurrentState == GameState.Playing && canMove)
        {
            if (wallJumpLockTimer > 0f) return;

            if (!isWallSliding)
            {
                Move();
            }
            else
            {
                float targetVerticalVelocity = -wallSlideSpeed;

                if (verticalInput > 0f)
                {
                    targetVerticalVelocity = wallClimbSpeed;
                }
                else if (verticalInput < 0f)
                {
                    targetVerticalVelocity = -wallClimbSpeed * 1.5f;
                }

                rb.linearVelocity = new Vector2(0f, targetVerticalVelocity);
            }

            if (jump) Jump();
        }
    }

    private void UpdateWallCheckPosition()
    {
        if (wallCheck == null || spriteRenderer == null) return;

        float targetX = spriteRenderer.flipX ? -initialWallCheckX : initialWallCheckX;
        wallCheck.localPosition = new Vector3(targetX, wallCheck.localPosition.y, wallCheck.localPosition.z);
    }

    private void DetectWalls()
    {
        if (wallCheck == null) return;

        Collider2D hit = Physics2D.OverlapBox(wallCheck.position, wallCheckSize, 0f, wallLayer);

        if (hit != null && !isGrounded)
        {
            isTouchingWall = true;
            wallDirection = wallCheck.localPosition.x > 0 ? 1 : -1;
        }
        else
        {
            isTouchingWall = false;
            wallDirection = 0;
        }
    }

    private void Move()
    {
        float speed = playerStats != null ? playerStats.SpeedPlatform : 8f;
        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);

        if (horizontalInput > 0f) spriteRenderer.flipX = false;
        else if (horizontalInput < 0f) spriteRenderer.flipX = true;
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jump = false;
    }

    private void HandleWallSlideAndClimb()
    {
        if (!isGrounded && isTouchingWall && Mathf.Abs(horizontalInput) > 0.1f)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void JumpFromWall()
    {
        float jumpDir = -wallDirection;
        rb.linearVelocity = new Vector2(jumpDir * 9f, jumpForce);

        isWallSliding = false;
        jump = false;
        wallJumpLockTimer = 0.25f;
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        bool isWalking = Mathf.Abs(horizontalInput) > 0.01f && isGrounded && !isWallSliding;
        animator.SetBool("caminar", isWalking);
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsWallSliding", isWallSliding);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        if (wallCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);
        }
    }
}