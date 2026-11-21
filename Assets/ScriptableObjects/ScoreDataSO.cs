using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoreData", menuName = "Game/Score Data")]
public class ScoreDataSO : ScriptableObject
{
    [SerializeField] private string saveKey = "TotalScore";
    [SerializeField] private bool autoSaveToPlayerPrefs = true;

    [NonSerialized] private int currentScore;
    [NonSerialized] private bool isLoaded;

    public int CurrentScore
    {
        get
        {
            if (!isLoaded)
            {
                Load();
            }
            return currentScore;
        }
        private set => currentScore = value;
    }

    public event Action<int> OnScoreChanged;

    private void OnEnable()
    {
        // Evita que un estado viejo en memoria quede pegado al entrar en Play Mode
        isLoaded = false;
    }

    public void Load()
    {
        currentScore = autoSaveToPlayerPrefs ? PlayerPrefs.GetInt(saveKey, 0) : 0;
        isLoaded = true;
    }

    public void AddPoints(int points)
    {
   
        if (points <= 0) return;

        if (!isLoaded)
        {
            Load();
        }

        currentScore += points;

        if (autoSaveToPlayerPrefs)
        {
            PlayerPrefs.SetInt(saveKey, currentScore);
            PlayerPrefs.Save();
        }

        OnScoreChanged?.Invoke(currentScore);
    }

    public void ResetScore(bool clearSavedData = true)
    {
        currentScore = 0;
        isLoaded = true;

        if (clearSavedData && autoSaveToPlayerPrefs)
        {
            PlayerPrefs.DeleteKey(saveKey);
            PlayerPrefs.Save();
        }

        OnScoreChanged?.Invoke(currentScore);
    }
}