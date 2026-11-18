using UnityEngine;

public enum GameState { Playing, Paused }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState CurrentState { get; private set; } = GameState.Playing;

    private void Awake()
    {
        Instance = this;
    }
}