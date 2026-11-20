using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement movement;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (movement == null)
            movement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        if (movement != null)
            movement.OnJump += HandleJump;
    }

    private void OnDisable()
    {
        if (movement != null)
            movement.OnJump -= HandleJump;
    }

    private void Update()
    {
        if (animator == null || movement == null) return;

        animator.SetBool("caminar", movement.IsWalking);
        animator.SetFloat("Speed", Mathf.Abs(movement.HorizontalInput));
        animator.SetBool("IsGrounded", movement.IsGrounded);
    }

    private void HandleJump()
    {
        if (animator != null)
            animator.SetTrigger("saltar");
    }
}