using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [Header("Datos y UI")]
    [SerializeField] private ScoreDataSO scoreData;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private string prefix = "";

    private void OnEnable()
    {
        if (scoreData != null)
        {
            scoreData.OnScoreChanged += UpdateText;
            UpdateText(scoreData.CurrentScore);
        }
    }

    private void OnDisable()
    {
        if (scoreData != null)
        {
            scoreData.OnScoreChanged -= UpdateText;
        }
    }

    private void UpdateText(int value)
    {
        if (scoreText != null)
        {
            scoreText.text = $"{prefix}{value}";
        }
    }
}