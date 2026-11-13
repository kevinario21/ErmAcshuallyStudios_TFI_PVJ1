using UnityEngine;

public class AchievementTester : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) GameEvents.RaiseBossDefeated("boss_nivel1");
    }
}