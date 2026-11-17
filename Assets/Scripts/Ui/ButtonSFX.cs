using UnityEngine;
using UnityEngine.UI;

/// Agregar a cualquier Button para que suene el clic.
[RequireComponent(typeof(Button))]
public class ButtonSFX : MonoBehaviour
{
    private void Awake() => GetComponent<Button>().onClick.AddListener(() =>
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
    });
}