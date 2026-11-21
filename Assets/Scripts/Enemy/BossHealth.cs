using System;
using System.Collections;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private string bossName = "Jefe";    

    [Header("Identificación")]
    [SerializeField] private string bossId = "boss_nivel3"; 
    [SerializeField] private bool isFinalBoss = true;      

    [Header("Daño recibido")]
    [SerializeField] private int damagePerHit = 1;

    [Header("Feedback")]
    [SerializeField] private float flashTime = 0.1f;
    [SerializeField] private float deathDelay = 2f;

    private int currentHealth;
    private bool isDead;
    private Color originalColor = Color.white;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb;

  
    public event Action<int, int> OnHealthChanged;
    public event Action OnDeath;

    public bool IsDead => isDead;
    public float HealthPercent => (float)currentHealth / maxHealth;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public string BossName => bossName;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
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

        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (animator != null && animator.runtimeAnimatorController != null)
            animator.SetTrigger("Die");
        if (spriteRenderer != null) spriteRenderer.color = Color.gray;

       
        foreach (Collider2D col in GetComponents<Collider2D>())
            col.enabled = false;

        OnDeath?.Invoke(); 

        if (isFinalBoss)    
        {
            GameEvents.RaiseBossDefeated(bossId);
        }

        Destroy(gameObject, deathDelay);
    }

    private IEnumerator FlashRoutine()
    {
        if (spriteRenderer == null) yield break;

        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(flashTime);
        if (!isDead) spriteRenderer.color = originalColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack") || other.GetComponent<Projectile>() != null)
        {
            TakeDamage(damagePerHit);
        }
    }
}