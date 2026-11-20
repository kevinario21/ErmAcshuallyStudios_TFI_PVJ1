using System.Collections;
using UnityEngine;
using TMPro; // Necesario para TextMeshPro

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
    [Header("UI del Globo de Texto")]
    [SerializeField] private TextMeshProUGUI textoGlobo;
    [SerializeField] private float velocidadEscritura = 0.04f;

    [Header("Configuración de Tiempo")]
    [SerializeField] private float tiempoMinimoVisible = 3f;

    [Header("Efectos Mágicos")]
    [SerializeField] private ParticleSystem particulasTeletransporte;

    private Coroutine corrutinaDesaparicion;
    private Coroutine corrutinaEscritura;

    private TipoTutorial temaActual;
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void InvocarRana(TipoTutorial tema, Transform puntoAparicion)
    {
        if (corrutinaDesaparicion != null)
        {
            StopCoroutine(corrutinaDesaparicion);
            corrutinaDesaparicion = null;
        }

        // Si ya estaba visible explicando exactamente este mismo tema, no reiniciamos el texto ni las partículas
        if (gameObject.activeInHierarchy && temaActual == tema)
        {
            return;
        }

        temaActual = tema;
        transform.position = puntoAparicion.position;
        gameObject.SetActive(true);

        if (particulasTeletransporte != null)
        {
            particulasTeletransporte.Play();
        }

        DecirMensaje(tema);
    }

    public void ProgramarOcultarRana()
    {
        if (gameObject.activeInHierarchy)
        {
            if (corrutinaDesaparicion != null)
            {
                StopCoroutine(corrutinaDesaparicion);
            }

            corrutinaDesaparicion = StartCoroutine(EsperarYOcultar());
        }
    }

    private IEnumerator EsperarYOcultar()
    {
        yield return new WaitForSeconds(tiempoMinimoVisible);

        if (particulasTeletransporte != null)
        {
            particulasTeletransporte.Play();
        }

        gameObject.SetActive(false);
        corrutinaDesaparicion = null;
    }

    private void DecirMensaje(TipoTutorial tema)
    {
        string mensajeElegido = "";

        switch (tema)
        {
            case TipoTutorial.Saltar:
                mensajeElegido = "¡Presiona ESPACIO para saltar!";
                break;

            case TipoTutorial.Agacharse:
                mensajeElegido = "Mantén presionado ABAJO para agacharte.";
                break;

            case TipoTutorial.Golpear:
                mensajeElegido = "Pulsa Z para dar un golpe.";
                break;

            case TipoTutorial.Trepar:
                mensajeElegido = "Mantén ARRIBA junto a la pared para trepar.";
                break;

            case TipoTutorial.Mapa:
                mensajeElegido = "Presiona M para abrir el mapa.";
                break;
        }

        if (corrutinaEscritura != null)
        {
            StopCoroutine(corrutinaEscritura);
        }

        corrutinaEscritura = StartCoroutine(EscribirLetraPorLetra(mensajeElegido));
    }

    private IEnumerator EscribirLetraPorLetra(string textoCompleto)
    {
        textoGlobo.text = textoCompleto;
        textoGlobo.maxVisibleCharacters = 0;

        for (int i = 0; i <= textoCompleto.Length; i++)
        {
            textoGlobo.maxVisibleCharacters = i;
            yield return new WaitForSeconds(velocidadEscritura);
        }

        corrutinaEscritura = null;
    }
}