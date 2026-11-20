using System.Collections;
using UnityEngine;

public enum BossPattern { High, Low, Alternate }

public class BossShooter : MonoBehaviour
{
    [Header("Puntos de disparo")]
    [SerializeField] private Transform highFirePoint;
    [SerializeField] private Transform lowFirePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Vector2 shootDirection = Vector2.left;

    [Header("Ataque")]
    [SerializeField] private float projectileSpeed = 6f;
    [SerializeField] private int projectileDamage = 1;
    [SerializeField] private int shotsPerBurst = 3;
    [SerializeField] private float timeBetweenShots = 0.5f;
    [SerializeField] private float timeBetweenBursts = 2f;
    [SerializeField] private float warningTime = 0.6f;
    [SerializeField] private int contactDamage = 1;

    [Header("Furia (poca vida)")]
    [SerializeField] private float enrageHealthPercent = 0.5f;
    [SerializeField] private int enrageExtraShots = 2;
    [SerializeField] private float enrageSpeedMultiplier = 1.4f;
    [SerializeField] private float enrageDelayMultiplier = 0.6f;

    private SpriteRenderer spriteRenderer;
    private BossHealth bossHealth;
    private Color originalColor;
    private bool isActive;
    private bool isDead;
    private float contactTimer;
    private BossPattern lastPattern = BossPattern.Alternate;

    private bool IsEnraged => bossHealth.HealthPercent <= enrageHealthPercent;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        bossHealth = GetComponent<BossHealth>();
        originalColor = spriteRenderer.color;
    }

    private void OnEnable()
    {
        bossHealth.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        bossHealth.OnDeath -= HandleDeath;
    }

    public void Activate()
    {
        if (isActive) return;
        isActive = true;
        StartCoroutine(AttackLoop());
    }

    private void Update()
    {
        contactTimer -= Time.deltaTime;
    }

    private IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(1f);

        while (!isDead)
        {
            yield return new WaitUntil(() => GameManager.Instance.CurrentState == GameState.Playing);

            BossPattern pattern = ChoosePattern();

            yield return StartCoroutine(Warning());
            yield return StartCoroutine(FireBurst(pattern));

            float delay = IsEnraged ? timeBetweenBursts * enrageDelayMultiplier : timeBetweenBursts;
            yield return new WaitForSeconds(delay);
        }
    }

    private BossPattern ChoosePattern()
    {
        BossPattern pattern;

        do
        {
            pattern = (BossPattern)Random.Range(0, 3);
        }
        while (pattern == lastPattern);

        lastPattern = pattern;
        return pattern;
    }

    private IEnumerator Warning()
    {
        spriteRenderer.color = Color.yellow;
        yield return new WaitForSeconds(warningTime);
        if (!isDead) spriteRenderer.color = originalColor;
    }

    private IEnumerator FireBurst(BossPattern pattern)
    {
        int shots = shotsPerBurst + (IsEnraged ? enrageExtraShots : 0);
        float interval = IsEnraged ? timeBetweenShots / enrageSpeedMultiplier : timeBetweenShots;

        for (int i = 0; i < shots && !isDead; i++)
        {
            bool high = pattern == BossPattern.High || (pattern == BossPattern.Alternate && i % 2 == 0);
            Shoot(high ? highFirePoint : lowFirePoint);
            yield return new WaitForSeconds(interval);
        }
    }

    private void Shoot(Transform firePoint)
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.GetComponent<BossProjectile>().Initialize(shootDirection, projectileSpeed, projectileDamage);
    }

    private void HandleDeath()
    {
        isDead = true;
        StopAllCoroutines();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead || contactTimer > 0f) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            if (playerStats != null) playerStats.TakeDamage(contactDamage);
            contactTimer = 1f;
        }
    }
}