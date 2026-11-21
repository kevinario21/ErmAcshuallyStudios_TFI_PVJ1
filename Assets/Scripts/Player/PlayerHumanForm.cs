using System;
using UnityEngine;

public class PlayerHumanForm : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private Sprite humanSprite;
    [SerializeField] private RuntimeAnimatorController humanController;

    public bool IsHuman { get; private set; }
    public event Action OnTransformed;

    private void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable() => GameEvents.OnBossDefeated += HandleBossDefeated;
    private void OnDisable() => GameEvents.OnBossDefeated -= HandleBossDefeated;

    private void HandleBossDefeated(string bossId)
    {
        if (IsHuman) return;
        IsHuman = true;
        Debug.Log("Forma humana activada tras vencer a " + bossId);

        if (humanController != null && animator != null)
        {
            animator.runtimeAnimatorController = humanController;
        }
        else
        {
            if (animator != null) animator.enabled = false;  
            if (spriteRenderer != null && humanSprite != null)
                spriteRenderer.sprite = humanSprite;
        }

        OnTransformed?.Invoke();
    }
}