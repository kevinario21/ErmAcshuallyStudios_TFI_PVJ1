using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// Panel de logros del Menú Principal. 'itemPrefab' tiene: Image "Icon", TMP_Text "Title", TMP_Text "Status".
public class AchievementsListUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private GameObject itemPrefab;

    private void OnEnable()
    {
        foreach (Transform c in container) Destroy(c.gameObject);
        if (AchievementManager.Instance == null) return;

        foreach (var a in AchievementManager.Instance.All)
        {
            var go = Instantiate(itemPrefab, container);
            bool unlocked = AchievementManager.Instance.IsUnlocked(a);
            go.transform.Find("Title").GetComponent<TMP_Text>().text = a.title + "\n<size=70%>" + a.description + "</size>";
            go.transform.Find("Status").GetComponent<TMP_Text>().text = unlocked ? "Desbloqueado" : "Bloqueado";
            var icon = go.transform.Find("Icon").GetComponent<Image>();
            icon.sprite = a.icon;
            icon.color = unlocked ? Color.white : new Color(1, 1, 1, 0.25f);
        }
    }
}