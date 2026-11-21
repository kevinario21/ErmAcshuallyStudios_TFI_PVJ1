using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Vida genérica para enemigos (Slime, Hongo, etc.).
/// El proyectil del jugador necesita que el enemigo tenga tag "Enemy".
/// Al morir: reproduce animación (si tiene), deshabilita colisiones y se destruye.
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int damagePerHit = 1;

    [Header("Feedback")]
    [SerializeField] private float flashTime = 0.08f;
    [SerializeField] private float deathDelay = 0.4f;

    [Header("Logro — id del jefe (dejar vacío si no es jefe)")]
    [SerializeField] private string bossId = "";

    private int currentHealth;
    private bool isDead;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Animator animator;

    public event Action OnDeath;

    private void Awake()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        currentHealth -= amount;
        if (currentHealth <= 0)
            Die();
        else
            StartCoroutine(FlashRoutine());
    }

    private void Die()
    {
        isDead = true;

        // Deshabilitar colisiones y comportamiento
        foreach (Collider2D col in GetComponents<Collider2D>())
            col.enabled = false;

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // Disparar animación de muerte si existe
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            if (HasAnimParam(animator, "morir"))
                animator.SetTrigger("morir");
            else if (HasAnimParam(animator, "Die"))
                animator.SetTrigger("Die");
        }

        OnDeath?.Invoke();

        // Avisar logros si es un jefe
        if (!string.IsNullOrEmpty(bossId))
            GameEvents.RaiseBossDefeated(bossId);

        // Sonido de enemigo muerto
        AudioManager.Instance?.PlayEnemy();

        Destroy(gameObject, deathDelay);
    }

    private IEnumerator FlashRoutine()
    {
        if (spriteRenderer == null) yield break;
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(flashTime);
        if (!isDead) spriteRenderer.color = originalColor;
    }

    // Recibe daño desde el proyectil (OnTriggerEnter2D en Projectile.cs)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack") || other.GetComponent<Projectile>() != null)
        {
            TakeDamage(damagePerHit);
        }
    }

    // También puede recibir daño desde colisión directa (si se usa así)
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("PlayerAttack"))
        {
            TakeDamage(damagePerHit);
        }
    }

    private static bool HasAnimParam(Animator anim, string paramName)
    {
        foreach (var p in anim.parameters)
            if (p.name == paramName) return true;
        return false;
    }
}
