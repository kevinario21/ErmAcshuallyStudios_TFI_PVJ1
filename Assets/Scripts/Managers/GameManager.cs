using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Start, Playing, Paused, Victory, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event Action<GameState> OnGameStateChanged;

    [Header("Nombres de escenas (deben coincidir EXACTO con los del proyecto)")]
    [SerializeField] private string mainMenuScene = "MenuPrincipal"; // verificar nombre completo
    [SerializeField] private string victoryScene = "Victory";
    [SerializeField] private string defeatScene = "Defeat";
    [SerializeField] private string startScene = "Nivel1";          // cambiar a "Tutorial" si arranca por ahí
    [SerializeField] private string[] levelScenes = { "Tutorial", "Nivel1", "Nivel2", "Nivel3", "Nivel4", "Nivel5" };

    [SerializeField] private float delayAfterBoss = 3f; // tiempo para ver la notificación de logro

    public GameState CurrentState { get; private set; } = GameState.Start;
    private string lastLevelPlayed;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
        GameEvents.OnBossDefeated += HandleBossDefeated;
        GameEvents.OnPlayerDied += HandlePlayerDied;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        GameEvents.OnBossDefeated -= HandleBossDefeated;
        GameEvents.OnPlayerDied -= HandlePlayerDied;
    }

    // El estado se deduce de la escena cargada
    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string n = scene.name;
        if (n == victoryScene) SetState(GameState.Victory);
        else if (n == defeatScene) SetState(GameState.GameOver);
        else if (n == mainMenuScene) SetState(GameState.Start);
        else
        {
            if (Array.IndexOf(levelScenes, n) >= 0) lastLevelPlayed = n;
            SetState(GameState.Playing);
        }
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        // Pausa: congela físicas y temporizadores; la UI sigue funcionando
        Time.timeScale = newState == GameState.Paused ? 0f : 1f;
        OnGameStateChanged?.Invoke(newState);
    }

    public void TogglePause()
    {
        if (CurrentState == GameState.Playing) SetState(GameState.Paused);
        else if (CurrentState == GameState.Paused) SetState(GameState.Playing);
    }

    // ---------- Flujo de escenas ----------
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void StartGame() => LoadScene(startScene);
    public void GoToMainMenu() => LoadScene(mainMenuScene);
    public void RestartLevel() => LoadScene(SceneManager.GetActiveScene().name);

    // Desde Defeat: reintenta el último nivel jugado
    public void RetryLastLevel()
    {
        if (!string.IsNullOrEmpty(lastLevelPlayed)) LoadScene(lastLevelPlayed);
        else StartGame();
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // ---------- Reacciones a eventos ----------
    private void HandleBossDefeated(string bossId) => StartCoroutine(AfterBossRoutine());

    private IEnumerator AfterBossRoutine()
    {
        yield return new WaitForSecondsRealtime(delayAfterBoss);
        int i = Array.IndexOf(levelScenes, SceneManager.GetActiveScene().name);
        // Siguiente nivel de la lista; si era el último (o no está en la lista) -> Victory
        if (i >= 0 && i + 1 < levelScenes.Length) LoadScene(levelScenes[i + 1]);
        else LoadScene(victoryScene);
    }

    private void HandlePlayerDied() => LoadScene(defeatScene);
}