using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }
    public static event Action<AchievementData> OnAchievementUnlocked;

    [SerializeField] private List<AchievementData> achievements;
    public IReadOnlyList<AchievementData> All => achievements;

    private bool damageTakenThisLevel;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        GameEvents.OnBossDefeated += HandleBossDefeated;
        GameEvents.OnPlayerDamaged += HandleDamage;
        SceneManager.sceneLoaded += (s, m) => damageTakenThisLevel = false;
    }

    private void OnDisable()
    {
        GameEvents.OnBossDefeated -= HandleBossDefeated;
        GameEvents.OnPlayerDamaged -= HandleDamage;
    }

    private void HandleDamage() => damageTakenThisLevel = true;

    private void HandleBossDefeated(string bossId)
    {
        foreach (var a in achievements)
        {
            if (a.bossId != bossId || IsUnlocked(a)) continue;
            if (a.condition == AchievementCondition.BossDefeatedNoDamage && damageTakenThisLevel) continue;
            Unlock(a);
        }
    }

    public bool IsUnlocked(AchievementData a) => PlayerPrefs.GetInt("ach_" + a.id, 0) == 1;

    private void Unlock(AchievementData a)
    {
        PlayerPrefs.SetInt("ach_" + a.id, 1);
        PlayerPrefs.Save();
        OnAchievementUnlocked?.Invoke(a);
    }

    [ContextMenu("Reset logros")]
    public void ResetAll()
    {
        foreach (var a in achievements) PlayerPrefs.DeleteKey("ach_" + a.id);
    }
}