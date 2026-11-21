using UnityEngine;

public class ResetScoreOnStart : MonoBehaviour
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