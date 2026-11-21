using UnityEngine;
using UnityEngine.SceneManagement;

public class AchievementTester : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) GameEvents.RaiseTutorialCompleted();
        if (Input.GetKeyDown(KeyCode.K))
        {
            // En Nivel2 manda "boss_nivel2", en Nivel3 "boss_nivel3", etc.
            string id = "boss_" + SceneManager.GetActiveScene().name.ToLower();
            Debug.Log("Tester: jefe derrotado -> " + id);
            GameEvents.RaiseBossDefeated(id);
        }
        if (Input.GetKeyDown(KeyCode.L)) GameEvents.RaisePlayerDied();
    }
}