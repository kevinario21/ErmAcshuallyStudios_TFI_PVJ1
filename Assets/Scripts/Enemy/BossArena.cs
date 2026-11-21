using UnityEngine;
using UnityEngine.UI;

public class BossArena : MonoBehaviour
{
    [SerializeField] private BossShooter boss;
    [SerializeField] private BossHealth bossHealth;
    [SerializeField] private GameObject[] barriers;
    [SerializeField] private GameObject bossUI;
    [SerializeField] private Slider bossHealthBar;

    private bool started;

    private void Awake()
    {
        SetBarriers(false);
        if (bossUI != null) bossUI.SetActive(false);
    }

    private void OnEnable()
    {
        bossHealth.OnHealthChanged += UpdateHealthBar;
        bossHealth.OnDeath += EndFight;
    }

    private void OnDisable()
    {
        bossHealth.OnHealthChanged -= UpdateHealthBar;
        bossHealth.OnDeath -= EndFight;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (started || !other.CompareTag("Player")) return;

        started = true;
        SetBarriers(true);
        boss.Activate();

        if (bossUI != null) bossUI.SetActive(true);
        if (bossHealthBar != null)
        {
            bossHealthBar.maxValue = bossHealth.MaxHealth;
            bossHealthBar.value = bossHealth.CurrentHealth;
        }
    }

    private void UpdateHealthBar(int current, int max)
    {
        if (bossHealthBar != null) bossHealthBar.value = current;
    }

    private void EndFight()
    {
        SetBarriers(false);
        if (bossUI != null) bossUI.SetActive(false);
    }

    private void SetBarriers(bool state)
    {
        foreach (GameObject barrier in barriers)
        {
            barrier.SetActive(state);
        }
    }
}