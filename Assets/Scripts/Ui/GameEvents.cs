using System;

/// <summary>
/// Bus de eventos estáticos compartido por todo el equipo (Patrón Observador).
/// El que programa al jefe llama GameEvents.RaiseBossDefeated("boss_level1").
/// El que programa al jugador llama GameEvents.RaisePlayerDamaged() al recibir daño.
/// </summary>
public static class GameEvents
{
    public static event Action<string> OnBossDefeated;
    public static event Action OnPlayerDamaged;
    public static event Action OnPlayerDied;

    public static void RaiseBossDefeated(string bossId) => OnBossDefeated?.Invoke(bossId);
    public static void RaisePlayerDamaged() => OnPlayerDamaged?.Invoke();
    public static void RaisePlayerDied() => OnPlayerDied?.Invoke();
}