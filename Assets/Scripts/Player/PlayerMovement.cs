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
    private float initialWallCheckX; // Guarda la distancia original del wallCheck

    public void SetMovementState(bool state) => canMove = state;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        playerStats = GetComponent<PlayerStats>();

        if (playerStats != null)
            playerStats.Checkpoint = transform.position;

        // Guardamos la posición X inicial del wallCheck respecto al centro del jugador
        if (wallCheck != null)
        {
            initialWallCheckX = Mathf.Abs(wallCheck.localPosition.x);
        }
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState == GameState.Playing)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical"); // W / S o Flechas para trepar

            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            // Actualiza la posición del wallCheck según hacia dónde mira el sprite (¡Igual que el firePoint!)
            UpdateWallCheckPosition();

            // Detección de paredes
            DetectWalls();

            if (wallJumpLockTimer > 0f)
            {
                wallJumpLockTimer -= Time.deltaTime;
            }

            if (Input.GetButtonDown("Jump"))
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
                float targetVerticalVelocity = -wallSlideSpeed; // Descenso lento por defecto

                if (verticalInput > 0f)
                {
                    targetVerticalVelocity = wallClimbSpeed; // Trepar con la W
                }
                else if (verticalInput < 0f)
                {
                    targetVerticalVelocity = -wallClimbSpeed * 1.5f; // Caída rápida con la S
                }

                rb.linearVelocity = new Vector2(0f, targetVerticalVelocity);
            }

            if (jump) Jump();
        }
    }

    private void UpdateWallCheckPosition()
    {
        if (wallCheck == null || spriteRenderer == null) return;

        // Si el sprite está volteado (flipX = true), el wallCheck pasa a la izquierda (-initialWallCheckX)
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
            // Definimos la dirección según dónde esté colocado el wallCheck en ese momento
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
        // Se activa si está en el aire, tocando la pared y empujando el control/tecla hacia ella
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