using UnityEngine;

public enum AchievementCondition { BossDefeated, BossDefeatedNoDamage }

[CreateAssetMenu(fileName = "Achievement_", menuName = "Game/Achievement")]
public class AchievementData : ScriptableObject
{
    public string id;                 // ej: "boss_level1"
    public string title;
    [TextArea] public string description;
    public Sprite icon;
    public AchievementCondition condition;
    public string bossId;             // debe coincidir con el id que manda el jefe
}