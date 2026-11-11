using UnityEngine;
using UnityEngine.Audio;

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
    [SerializeField] private AudioClip victoryMusic;
    [SerializeField] private AudioClip gameOverMusic;

    [Header("SFX comunes")]
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip achievementClip;

    public const string MasterParam = "MasterVol";
    public const string MusicParam = "MusicVol";
    public const string SFXParam = "SFXVol";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable() => GameManager.OnGameStateChanged += HandleState;
    private void OnDisable() => GameManager.OnGameStateChanged -= HandleState;

    private void Start()
    {
        // Los parámetros del mixer se aplican en Start (en Awake a veces no se setean)
        ApplySaved(MasterParam);
        ApplySaved(MusicParam);
        ApplySaved(SFXParam);
        if (GameManager.Instance != null) HandleState(GameManager.Instance.CurrentState);
    }

    private void HandleState(GameState state)
    {
        switch (state)
        {
            case GameState.Start: PlayMusic(menuMusic); break;
            case GameState.Playing: PlayMusic(gameplayMusic); break; // al volver de pausa no reinicia (mismo clip)
            case GameState.Victory: PlayMusic(victoryMusic, false); break;
            case GameState.GameOver: PlayMusic(gameOverMusic, false); break;
            case GameState.Paused: break; // la música sigue sonando
        }
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null || (musicSource.clip == clip && musicSource.isPlaying)) return;
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null) sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayButtonClick() => PlaySFX(buttonClick);
    public void PlayAchievement() => PlaySFX(achievementClip);

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