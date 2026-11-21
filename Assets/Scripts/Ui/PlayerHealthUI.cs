using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Muestra la vida y vidas del jugador en pantalla.
/// Colocar en un Canvas en la escena. Asignar en el Inspector:
///   - healthBar (Slider) — barra de vida
///   - healthText (TextMeshProUGUI o Text) — "3 / 5"
///   - livesText (TextMeshProUGUI) — "Vidas: 2"
///   - iconos de corazón (opcionales)
/// </summary>
public class PlayerHealthUI : MonoBehaviour
{
    [Header("Barra de vida (opcional)")]
    [SerializeField] private Slider healthBar;

    [Header("Texto vida (opcional — asignar TextMeshProUGUI o Text)")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI livesText;

    [Header("Corazones (opcional — asignar imágenes)")]
    [SerializeField] private Image[] heartIcons;
    [SerializeField] private Sprite heartFull;
    [SerializeField] private Sprite heartEmpty;

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

        // Corazones
        if (heartIcons != null && heartFull != null && heartEmpty != null)
        {
            for (int i = 0; i < heartIcons.Length; i++)
            {
                if (heartIcons[i] == null) continue;
                heartIcons[i].sprite = i < current ? heartFull : heartEmpty;
            }
        }
    }

    private void UpdateLives(int lives)
    {
        if (livesText != null)
            livesText.text = "Vidas: " + lives;
    }
}
