using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer (parámetros expuestos: MasterVol, MusicVol, SFXVol)")]
    [SerializeField] private AudioMixer mixer;

    [Header("Fuentes (asignadas a los grupos Music y SFX del mixer)")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Música por estado")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip pauseMusic;
    [SerializeField] private AudioClip bossMusic;
    [SerializeField] private AudioClip victoryMusic;
    [SerializeField] private AudioClip gameOverMusic;

    [Header("SFX comunes")]
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip achievementClip;

    [Header("SFX de juego")]
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip coinClip;
    [SerializeField] private AudioClip enemyClip;
    [SerializeField] private AudioClip powerUpClip;
    [SerializeField] private AudioClip bossShootClip;

    public const string MasterParam = "MasterVol";
    public const string MusicParam = "MusicVol";
    public const string SFXParam = "SFXVol";

    private AudioClip currentGameplayClip;
    private float savedTime;
    private GameState previousState;
    private Scene pausedScene; // escena en la que se pausó

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        currentGameplayClip = gameplayMusic;
    }

    private void OnEnable()
    {
        if (Instance != this) return; // copia duplicada: no se suscribe
        GameManager.OnGameStateChanged += HandleState;
        AchievementManager.OnAchievementUnlocked += HandleAchievement;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleState;
        AchievementManager.OnAchievementUnlocked -= HandleAchievement;
    }

    private void Start()
    {
        ApplySaved(MasterParam);
        ApplySaved(MusicParam);
        ApplySaved(SFXParam);
        if (GameManager.Instance != null) HandleState(GameManager.Instance.CurrentState);
    }

    private void HandleAchievement(AchievementData a) => PlayAchievement();

    private void HandleState(GameState state)
    {
        switch (state)
        {
            case GameState.Start:
                PlayMusic(menuMusic);
                break;

            case GameState.Playing:
            {
                // Es reanudar solo si venimos de pausa Y la escena NO se recargó
                bool resuming = previousState == GameState.Paused &&
                                SceneManager.GetActiveScene() == pausedScene;
                if (resuming)
                {
                    ResumeGameplayMusic();
                }
                else
                {
                    // Nivel nuevo o reinicio: música normal desde cero
                    currentGameplayClip = gameplayMusic;
                    musicSource.Stop(); // fuerza reinicio aunque sea el mismo clip
                    PlayMusic(currentGameplayClip);
                }
                break;
            }

            case GameState.Paused:
                pausedScene = SceneManager.GetActiveScene();
                if (musicSource.clip == currentGameplayClip) savedTime = musicSource.time;
                PlayMusic(pauseMusic);
                break;

            case GameState.Victory:
                PlayMusic(victoryMusic, false);
                break;

            case GameState.GameOver:
                PlayMusic(gameOverMusic, false);
                break;
        }
        previousState = state;
    }

    private void ResumeGameplayMusic()
    {
        if (currentGameplayClip == null) return;
        // Si no hay música de pausa, la del nivel nunca se detuvo
        if (musicSource.clip == currentGameplayClip && musicSource.isPlaying) return;
        musicSource.clip = currentGameplayClip;
        musicSource.loop = true;
        musicSource.time = savedTime;
        musicSource.Play();
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null || (musicSource.clip == clip && musicSource.isPlaying)) return;
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    // Llamar cuando aparece el jefe
    public void PlayBossMusic()
    {
        currentGameplayClip = bossMusic;
        PlayMusic(bossMusic);
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null) sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayButtonClick() => PlaySFX(buttonClick);
    public void PlayAchievement() => PlaySFX(achievementClip);
    public void PlayJump()        => PlaySFX(jumpClip);
    public void PlayShoot()       => PlaySFX(shootClip);
    public void PlayCoin()        => PlaySFX(coinClip);
    public void PlayEnemy()       => PlaySFX(enemyClip);
    public void PlayPowerUp()     => PlaySFX(powerUpClip);
    public void PlayBossShoot()   => PlaySFX(bossShootClip);

    // ---------- Volumen (slider 0.0001 - 1 -> decibeles) ----------
    public void SetVolume(string param, float linear)
    {
        float db = Mathf.Log10(Mathf.Clamp(linear, 0.0001f, 1f)) * 20f;
        mixer.SetFloat(param, db);
        PlayerPrefs.SetFloat(param, linear);
    }

    public float GetSavedVolume(string param) => PlayerPrefs.GetFloat(param, 0.75f);

    private void ApplySaved(string param) => SetVolume(param, GetSavedVolume(param));
}