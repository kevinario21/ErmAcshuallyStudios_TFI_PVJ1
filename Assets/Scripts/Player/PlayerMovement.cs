using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float jumpCooldown = 0.2f;

    [Header("Escalar Paredes")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckRadius = 0.2f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float climbSpeed = 5f;
    [SerializeField] private float maxClimbTime = 1.5f;

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private SpriteRenderer spriteRenderer;
    private PlayerStats playerStats;

    private float horizontalInput;
    private float verticalInput;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isClimbing;
    private bool canClimb = true;
    private bool jump;
    private bool canMove = true;
    private float defaultGravity;
    private float initialWallCheckX;
    private float nextJumpTime;
    private Coroutine climbTimerCoroutine;

    public float HorizontalInput => horizontalInput;
    public bool IsGrounded => isGrounded;
    public bool IsWalking => canMove && Mathf.Abs(horizontalInput) > 0.01f && isGrounded;
    public bool IsClimbing => isClimbing;

    public event Action OnJump;

    public void SetMovementState(bool state) => canMove = state;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerStats = GetComponent<PlayerStats>();
        defaultGravity = rb.gravityScale;

        if (wallCheck != null)
            initialWallCheckX = Mathf.Abs(wallCheck.localPosition.x);

        if (playerStats != null)
            playerStats.Checkpoint = transform.position;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
            return;

        horizontalInput = canMove ? Input.GetAxisRaw("Horizontal") : 0f;
        verticalInput = canMove ? Input.GetAxisRaw("Vertical") : 0f;

        isGrounded = ComprobarSuelo();

        if (wallCheck != null)
        {
            isTouchingWall = false;
            Collider2D[] paredes = Physics2D.OverlapCircleAll(wallCheck.position, wallCheckRadius, wallLayer);
            foreach (var p in paredes)
            {
                if (p != playerCollider && !p.isTrigger)
                {
                    isTouchingWall = true;
                    break;
                }
            }
        }

        // Al tocar el suelo, recupera la energía para volver a escalar
        if (isGrounded && !canClimb)
        {
            canClimb = true;
        }

        // Condición para escalar
        bool quiereEscalar = canMove && canClimb && isTouchingWall && verticalInput > 0f;

        if (quiereEscalar && !isClimbing)
        {
            isClimbing = true;
            if (climbTimerCoroutine != null) StopCoroutine(climbTimerCoroutine);
            climbTimerCoroutine = StartCoroutine(TemporizadorEscalada());
        }
        else if (!quiereEscalar && isClimbing)
        {
            DetenerEscalada();
        }

        // Solo salta si está en el suelo, no está escalando y ya pasó el cooldown del salto anterior
        if (canMove && isGrounded && !isClimbing && Time.time >= nextJumpTime && (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space)))
        {
            jump = true;
            nextJumpTime = Time.time + jumpCooldown;
        }
    }

    private bool ComprobarSuelo()
    {
        if (groundCheck == null || rb.linearVelocity.y > 0.1f) return false;

        Collider2D[] hits = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundLayer);
        foreach (var hit in hits)
        {
            if (hit != playerCollider && !hit.isTrigger)
            {
                return true;
            }
        }
        return false;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
            return;

        if (!canMove) return;

        Move();
        HandleClimb();

        if (jump)
        {
            Jump();
        }
    }

    private void Move()
    {
        float speed = playerStats != null ? playerStats.SpeedPlatform : 8f;
        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);

        if (spriteRenderer != null)
        {
            if (horizontalInput > 0f) spriteRenderer.flipX = false;
            else if (horizontalInput < 0f) spriteRenderer.flipX = true;
        }

        if (wallCheck != null && spriteRenderer != null)
        {
            float targetX = spriteRenderer.flipX ? -initialWallCheckX : initialWallCheckX;
            wallCheck.localPosition = new Vector3(targetX, wallCheck.localPosition.y, wallCheck.localPosition.z);
        }
    }

    private void HandleClimb()
    {
        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalInput * climbSpeed);
        }
        else
        {
            rb.gravityScale = defaultGravity;
        }
    }

    private void Jump()
    {
        DetenerEscalada();
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jump = false;
        isGrounded = false;
        OnJump?.Invoke();
        AudioManager.Instance?.PlayJump();
    }

    private IEnumerator TemporizadorEscalada()
    {
        yield return new WaitForSeconds(maxClimbTime);
        canClimb = false;
        isClimbing = false;
        climbTimerCoroutine = null;
    }

    private void DetenerEscalada()
    {
        isClimbing = false;
        if (climbTimerCoroutine != null)
        {
            StopCoroutine(climbTimerCoroutine);
            climbTimerCoroutine = null;
        }
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
            Gizmos.color = isTouchingWall ? Color.cyan : Color.yellow;
            Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
        }
    }
}