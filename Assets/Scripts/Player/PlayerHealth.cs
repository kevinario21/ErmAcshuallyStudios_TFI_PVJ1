using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private PlayerStats playerStats;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    // Llamado por SlimeAI y HongoPatrulla al atacar al jugador
    public void RecibirDano(int cantidad)
    {
        if (playerStats != null)
            playerStats.TakeDamage(cantidad);
    }
}
