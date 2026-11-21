using UnityEngine;

public class ResetScore : MonoBehaviour
{
    [SerializeField] private ScoreDataSO scoreData;

    private void Awake()
    {
        if (scoreData != null)
        {
            scoreData.ResetScore();
        }
    }
}