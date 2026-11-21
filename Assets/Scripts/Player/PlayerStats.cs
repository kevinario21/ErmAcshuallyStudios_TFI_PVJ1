using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float speedPlatform = 8f;
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int livesCount = 3;

    private int currentHealth;
    private int currentLives;

    public float SpeedPlatform => speedPlatform;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public int CurrentLives => currentLives;
    public Vector3 Checkpoint { get; set; }

    // Eventos para que la UI de vida los escuche
    public static event Action<int, int> OnHealthChanged;  // (currentHP, maxHP)
    public static event Action<int> OnLivesChanged;         // (livesLeft)

    private void Start()
    {
        currentHealth = maxHealth;
        currentLives = livesCount;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnLivesChanged?.Invoke(currentLives);
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        GameEvents.RaisePlayerDamaged();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            LoseLife();
        }
    }

    private void LoseLife()
    {
        currentLives--;
        OnLivesChanged?.Invoke(currentLives);

        if (currentLives <= 0)
        {
            // Sin vidas: ir a pantalla de derrota
            GameEvents.RaisePlayerDied();
        }
        else
        {
            // Respawnear en checkpoint con vida completa
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            transform.position = Checkpoint;

            // Pequeña invulnerabilidad (opcional, via animación si existe)
            var anim = GetComponent<Animator>();
            if (anim != null && HasAnimParam(anim, "dano"))
                anim.SetTrigger("dano");
        }
    }

    // Recuperar vida (PowerUp, etc.)
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private static bool HasAnimParam(Animator anim, string paramName)
    {
        foreach (var p in anim.parameters)
            if (p.name == paramName) return true;
        return false;
    }
}