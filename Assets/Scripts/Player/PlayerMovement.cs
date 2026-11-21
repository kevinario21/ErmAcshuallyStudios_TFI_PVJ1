using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpForce = 12f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private PlayerStats playerStats;

    private float horizontalInput;
    private bool isGrounded;
    private bool jump;
    private bool canMove = true;

    public float HorizontalInput => horizontalInput;
    public bool IsGrounded => isGrounded;
    public bool IsWalking => canMove && Mathf.Abs(horizontalInput) > 0.01f && isGrounded;

    public event Action OnJump;

    public void SetMovementState(bool state) => canMove = state;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerStats = GetComponent<PlayerStats>();

        if (playerStats != null)
            playerStats.Checkpoint = transform.position;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
            return;

        horizontalInput = canMove ? Input.GetAxisRaw("Horizontal") : 0f;

        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (canMove && isGrounded && (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space)))
        {
            jump = true;
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
            return;

        if (!canMove) return;

        Move();

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
    }

    private void Jump()
    {
    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    jump = false;
    isGrounded = false;
    OnJump?.Invoke();
    AudioManager.Instance?.PlayJump();
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}