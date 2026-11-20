using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [Header("Datos")]
    [SerializeField] private ScoreDataSO scoreData;
    [SerializeField] private int pointsToAdd = 10;

    [Header("Detección")]
    [SerializeField] private string playerTag = "Player";

    private bool collected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        if (other.CompareTag(playerTag))
        {
            collected = true;

            if (scoreData != null)
            {
                scoreData.AddPoints(pointsToAdd);
            }

            Destroy(gameObject);
        }
    }
}