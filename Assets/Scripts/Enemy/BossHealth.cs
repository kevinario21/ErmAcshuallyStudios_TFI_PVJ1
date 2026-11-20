using System;
using System.Collections;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private string bossName = "Jefe";

    [Header("Feedback")]
    [SerializeField] private float flashTime = 0.1f;
    [SerializeField] private float deathDelay = 2f;

    private int currentHealth;
    private bool isDead;
    private Color originalColor;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb;

    public static event Action<string> OnBossDefeated;
    public event Action<int, int> OnHealthChanged;
    public event Action OnDeath;

    public bool IsDead => isDead;
    public float HealthPercent => (float)currentHealth / maxHealth;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        originalColor = spriteRenderer.color;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0) Die();
        else StartCoroutine(FlashRoutine());
    }

    private void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        if (animator != null) animator.SetTrigger("Die");
        spriteRenderer.color = Color.gray;

        OnDeath?.Invoke();
        OnBossDefeated?.Invoke(bossName);

        Destroy(gameObject, deathDelay);
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(flashTime);
        if (!isDead) spriteRenderer.color = originalColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            TakeDamage(1);
        }
    }
}