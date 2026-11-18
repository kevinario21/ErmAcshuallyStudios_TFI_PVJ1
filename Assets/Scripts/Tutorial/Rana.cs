using System.Collections; // Necesario para usar IEnumerator y Corrutinas
using UnityEngine;

public enum TipoTutorial
{
    Saltar,
    Agacharse,
    Golpear,
    Trepar,
    Mapa
}

public class Rana : MonoBehaviour
{
    [Header("Configuración de Tiempo")]
    [SerializeField] private float tiempoMinimoVisible = 3f;

    [Header("Efectos Mágicos")]
    [SerializeField] private ParticleSystem particulasTeletransporte;

    // Guardamos referencia a la corrutina activa para poder cancelarla si hace falta
    private Coroutine corrutinaDesaparicion;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void InvocarRana(TipoTutorial tema, Transform puntoAparicion)
    {
        // Si la rana estaba contando los 3 segundos para irse, cancelamos esa cuenta atrás
        if (corrutinaDesaparicion != null)
        {
            StopCoroutine(corrutinaDesaparicion);
            corrutinaDesaparicion = null;
        }

        transform.position = puntoAparicion.position;
        gameObject.SetActive(true);

        if (particulasTeletransporte != null)
        {
            particulasTeletransporte.Play();
        }

        DecirMensaje(tema);
    }

    // Ahora ZonaTutorial llama a este método al salir, que inicia el temporizador
    public void ProgramarOcultarRana()
    {
        // Solo iniciamos la cuenta atrás si la rana está activa en escena
        if (gameObject.activeInHierarchy)
        {
            // Por seguridad, si ya había una cuenta atrás, la reiniciamos
            if (corrutinaDesaparicion != null)
            {
                StopCoroutine(corrutinaDesaparicion);
            }

            corrutinaDesaparicion = StartCoroutine(EsperarYOcultar());
        }
    }

    private IEnumerator EsperarYOcultar()
    {
        // Pausa la ejecución de esta función durante 3 segundos sin congelar el juego
        yield return new WaitForSeconds(tiempoMinimoVisible);

        if (particulasTeletransporte != null)
        {
            particulasTeletransporte.Play();
        }

        gameObject.SetActive(false);
        corrutinaDesaparicion = null;
        Debug.Log("Pasaron 3 segundos y la rana mago desapareció.");
    }

    private void DecirMensaje(TipoTutorial tema)
    {
        switch (tema)
        {
            case TipoTutorial.Saltar:
                Debug.Log("Rana Mago: ¡Presiona ESPACIO para saltar!");
                break;

            case TipoTutorial.Agacharse:
                Debug.Log("Rana Mago: Mantén presionado ABAJO para agacharte.");
                break;

            case TipoTutorial.Golpear:
                Debug.Log("Rana Mago: Pulsa Z para dar un golpe.");
                break;

            case TipoTutorial.Trepar:
                Debug.Log("Rana Mago: Presiona M para abrir el mapa.");
                break;
            case TipoTutorial.Mapa:
                Debug.Log("Rana Mago: Presiona M para abrir el mapa.");
                break;
        }
    }
}