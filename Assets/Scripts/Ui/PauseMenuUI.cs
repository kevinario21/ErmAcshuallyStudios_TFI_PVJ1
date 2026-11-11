using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject optionsPanel;

    private void OnEnable() => GameManager.OnGameStateChanged += HandleState;
    private void OnDisable() => GameManager.OnGameStateChanged -= HandleState;

    private void Start()
    {
        pausePanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) GameManager.Instance.TogglePause();
    }

    private void HandleState(GameState state)
    {
        bool paused = state == GameState.Paused;
        pausePanel.SetActive(paused);
        if (!paused) optionsPanel.SetActive(false);
    }

    // Botones (OnClick)
    public void OnResume() => GameManager.Instance.SetState(GameState.Playing);
    public void OnOpenOptions() { pausePanel.SetActive(false); optionsPanel.SetActive(true); }
    public void OnCloseOptions() { optionsPanel.SetActive(false); pausePanel.SetActive(true); }
    public void OnMainMenu() => GameManager.Instance.GoToMainMenu();
}