using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject achievementsPanel;

    private void Start() => ShowMain();

    public void ShowMain() => Show(mainPanel);
    public void ShowOptions() => Show(optionsPanel);
    public void ShowAchievements() => Show(achievementsPanel);

    private void Show(GameObject panel)
    {
        mainPanel.SetActive(panel == mainPanel);
        optionsPanel.SetActive(panel == optionsPanel);
        achievementsPanel.SetActive(panel == achievementsPanel);
    }

    // Botones (OnClick)
    public void OnPlay() => GameManager.Instance.StartGame();
    public void OnQuit() => GameManager.Instance.QuitGame();
}