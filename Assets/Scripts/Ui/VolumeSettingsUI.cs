using UnityEngine;
using UnityEngine.UI;

/// Panel de opciones de audio. Se reutiliza en Menú Principal y Menú de Pausa.
public class VolumeSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void OnEnable()
    {
        if (AudioManager.Instance == null) return;
        // Cargar valores guardados sin disparar el evento
        masterSlider.SetValueWithoutNotify(AudioManager.Instance.GetSavedVolume(AudioManager.MasterParam));
        musicSlider.SetValueWithoutNotify(AudioManager.Instance.GetSavedVolume(AudioManager.MusicParam));
        sfxSlider.SetValueWithoutNotify(AudioManager.Instance.GetSavedVolume(AudioManager.SFXParam));

        masterSlider.onValueChanged.AddListener(v => AudioManager.Instance.SetVolume(AudioManager.MasterParam, v));
        musicSlider.onValueChanged.AddListener(v => AudioManager.Instance.SetVolume(AudioManager.MusicParam, v));
        sfxSlider.onValueChanged.AddListener(v => AudioManager.Instance.SetVolume(AudioManager.SFXParam, v));
    }

    private void OnDisable()
    {
        masterSlider.onValueChanged.RemoveAllListeners();
        musicSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
        PlayerPrefs.Save();
    }
}