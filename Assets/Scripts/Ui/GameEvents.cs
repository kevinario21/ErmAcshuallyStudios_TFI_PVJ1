using System;


public static class GameEvents
{
    public static event Action OnTutorialCompleted;
    public static void RaiseTutorialCompleted() => OnTutorialCompleted?.Invoke();
    public static event Action<string> OnBossDefeated;
    public static event Action OnPlayerDamaged;
    public static event Action OnPlayerDamagedByBoss;
    public static event Action OnPlayerDied;

    public static void RaiseBossDefeated(string bossId) => OnBossDefeated?.Invoke(bossId);
    public static void RaisePlayerDamaged() => OnPlayerDamaged?.Invoke();
    public static void RaisePlayerDamagedByBoss() => OnPlayerDamagedByBoss?.Invoke();
    public static void RaisePlayerDied() => OnPlayerDied?.Invoke();
}
