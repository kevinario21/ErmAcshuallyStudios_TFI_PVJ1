using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float speedPlatform = 8f;
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int livesCount = 3;

    private int currentHealth;
    private int currentLives;
    [SerializeField] private float damageCooldown = 1f;
    private float nextDamageTime;

    private void Awake()
    {
        currentHealth = maxHealth;
        currentLives = livesCount;
        Checkpoint = transform.position;
    }

    public float SpeedPlatform => speedPlatform;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public int CurrentLives => currentLives;
    public Vector3 Checkpoint { get; set; }

    public static event Action<int, int> OnHealthChanged;  
    public static event Action<int> OnLivesChanged;         

    private void Start()
    {
        currentHealth = maxHealth;
        currentLives = livesCount;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnLivesChanged?.Invoke(currentLives);
    }

    public void TakeDamage(int amount)
    {
        ApplyDamage(amount, false);
    }

    public void TakeBossDamage(int amount)
    {
        ApplyDamage(amount, true);
    }

    private void ApplyDamage(int amount, bool fromBoss)
    {
        if (currentHealth <= 0 || amount <= 0 || Time.time < nextDamageTime) return;
        nextDamageTime = Time.time + damageCooldown;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        GameEvents.RaisePlayerDamaged();
 
        if (fromBoss) GameEvents.RaisePlayerDamagedByBoss();
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
            
            GameEvents.RaisePlayerDied();
        }
        else
        {
            
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            transform.position = Checkpoint;
            var body = GetComponent<Rigidbody2D>();
            if (body != null) body.linearVelocity = Vector2.zero;

            
            var anim = GetComponent<Animator>();
            if (anim != null && HasAnimParam(anim, "dano"))
                anim.SetTrigger("dano");
        }
    }


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
