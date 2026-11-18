using UnityEngine;

public class PlayerStatsTest : MonoBehaviour
{
    [SerializeField] private float speedPlatform = 8f;
    [SerializeField] private int maxHealth = 5;

    private int currentHealth;

    public float SpeedPlatform => speedPlatform;
    public Vector3 Checkpoint { get; set; }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Vida: " + currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = maxHealth;
            transform.position = Checkpoint;
        }
    }
}