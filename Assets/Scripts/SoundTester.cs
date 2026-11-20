using UnityEngine;

public class AudioTester : MonoBehaviour
{
    void Update()
    {
        var a = AudioManager.Instance;
        if (a == null) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) a.PlayJump();
        if (Input.GetKeyDown(KeyCode.Alpha2)) a.PlayShoot();
        if (Input.GetKeyDown(KeyCode.Alpha3)) a.PlayCoin();
        if (Input.GetKeyDown(KeyCode.Alpha4)) a.PlayEnemy();
        if (Input.GetKeyDown(KeyCode.Alpha5)) a.PlayPowerUp();
        if (Input.GetKeyDown(KeyCode.Alpha6)) a.PlayBossShoot();
        if (Input.GetKeyDown(KeyCode.Alpha7)) a.PlayBossMusic();
        if (Input.GetKeyDown(KeyCode.Alpha8)) a.PlayAchievement();
        if (Input.GetKeyDown(KeyCode.Alpha9)) a.PlayButtonClick();
    }
}