using UnityEngine;

public class RanaVFXController : MonoBehaviour
{
    [Header("Prefabs de VFX (desde BoldVFXPackDemo)")]
    [SerializeField] private GameObject vfxAparicionPrefab;     // Se reproduce 1 vez al aparecer
    [SerializeField] private GameObject vfxDesaparicionPrefab;  // Se reproduce al irse (independiente)

    [Header("VFX Activo (Hijo de la Rana)")]
    [SerializeField] private ParticleSystem vfxActivo;          // Loop mientras está activa

    [Header("Ajustes de Tiempo")]
    [SerializeField] private float duracionAutoDestruirVFX = 2.5f;
    [SerializeField] private Vector3 offsetVFX = Vector3.zero;

    private bool haDesaparecido = false;

    private void OnEnable()
    {
        haDesaparecido = false;

        // 1. VFX de Aparición (una sola vez en la posición de la rana)
        if (vfxAparicionPrefab != null)
        {
            GameObject vfxIn = Instantiate(vfxAparicionPrefab, transform.position + offsetVFX, Quaternion.identity);
            Destroy(vfxIn, duracionAutoDestruirVFX);
        }

        // 2. VFX Activo (se enciende mientras la rana está presente)
        if (vfxActivo != null)
        {
            vfxActivo.gameObject.SetActive(true);
            vfxActivo.Play();
        }
    }

    /// <summary>
    /// Llama a este método cuando quieras que la rana desaparezca (ej. al terminar diálogo o salir de zona)
    /// </summary>
    public void DesaparecerRana(bool destruirObjeto = false)
    {
        if (haDesaparecido) return;
        haDesaparecido = true;

        // Apagamos el VFX continuo
        if (vfxActivo != null)
        {
            vfxActivo.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        // 3. VFX de Desaparición (se instancia suelto en la escena para que siga corriendo aunque la rana desaparezca)
        if (vfxDesaparicionPrefab != null)
        {
            GameObject vfxOut = Instantiate(vfxDesaparicionPrefab, transform.position + offsetVFX, Quaternion.identity);
            Destroy(vfxOut, duracionAutoDestruirVFX);
        }

        // La rana desaparece inmediatamente mientras el VFX de salida termina su animación
        if (destruirObjeto) Destroy(gameObject);
        else gameObject.SetActive(false);
    }
}