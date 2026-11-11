using UnityEngine;

/// Usar en las escenas Victory y Defeat.
public class EndScreenUI : MonoBehaviour
{
    public void OnRetry() => GameManager.Instance.RetryLastLevel(); // en Defeat
    public void OnPlayAgain() => GameManager.Instance.StartGame();  // en Victory
    public void OnMainMenu() => GameManager.Instance.GoToMainMenu();
    public void OnQuit() => GameManager.Instance.QuitGame();
}