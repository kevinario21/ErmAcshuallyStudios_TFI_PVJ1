using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// Poner en el HUD de cada nivel. 'popup' es un RectTransform anclado arriba/derecha, fuera de pantalla.
public class AchievementNotificationUI : MonoBehaviour
{
    [SerializeField] private RectTransform popup;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descText;
    [SerializeField] private Image iconImage;
    [SerializeField] private float slideDistance = 400f;
    [SerializeField] private float slideTime = 0.4f;
    [SerializeField] private float visibleTime = 2.5f;

    private readonly Queue<AchievementData> queue = new Queue<AchievementData>();
    private bool showing;
    private Vector2 shownPos;

    private void Awake()
    {
        shownPos = popup.anchoredPosition;
        popup.anchoredPosition = shownPos + Vector2.right * slideDistance;
    }

    private void OnEnable() => AchievementManager.OnAchievementUnlocked += Enqueue;
    private void OnDisable() => AchievementManager.OnAchievementUnlocked -= Enqueue;

    private void Enqueue(AchievementData a)
    {
        queue.Enqueue(a);
        if (!showing) StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        showing = true;
        while (queue.Count > 0)
        {
            var a = queue.Dequeue();
            titleText.text = a.title;
            descText.text = a.description;
            if (iconImage != null) iconImage.sprite = a.icon;
            if (AudioManager.Instance != null) AudioManager.Instance.PlayAchievement();

            yield return Slide(shownPos + Vector2.right * slideDistance, shownPos);
            yield return new WaitForSecondsRealtime(visibleTime);
            yield return Slide(shownPos, shownPos + Vector2.right * slideDistance);
        }
        showing = false;
    }

    private IEnumerator Slide(Vector2 from, Vector2 to)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / slideTime; // funciona aunque el juego esté en pausa
            popup.anchoredPosition = Vector2.Lerp(from, to, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
    }
}