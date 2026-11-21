using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerHealthUI : MonoBehaviour
{
    [Header("Barra de vida (opcional)")]
    [SerializeField] private Slider healthBar;

    [Header("Texto vida (opcional — asignar TextMeshProUGUI o Text)")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI livesText;


    private void OnEnable()
    {
        PlayerStats.OnHealthChanged += UpdateHealth;
        PlayerStats.OnLivesChanged  += UpdateLives;
    }

    private void OnDisable()
    {
        PlayerStats.OnHealthChanged -= UpdateHealth;
        PlayerStats.OnLivesChanged  -= UpdateLives;
    }

    private void UpdateHealth(int current, int max)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = max;
            healthBar.value    = current;
        }

        if (healthText != null)
            healthText.text = current + " / " + max;

    }

    private void UpdateLives(int lives)
    {
        if (livesText != null)
            livesText.text = "Vidas: " + lives;
    }
}
