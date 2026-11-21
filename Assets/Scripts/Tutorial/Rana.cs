using System.Collections;
using UnityEngine;
using TMPro; 

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

    [Header("Efectos Mágicos (3 VFX)")]
    [SerializeField] private GameObject vfxAparicionPrefab;     
    [SerializeField] private ParticleSystem particulasActiva;   
    [SerializeField] private GameObject vfxDesaparicionPrefab;  
    [SerializeField] private float duracionAutoDestruirVFX = 2.5f;

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

        
        if (gameObject.activeInHierarchy && temaActual == tema)
        {
            return;
        }

        temaActual = tema;
        transform.position = puntoAparicion.position;
        gameObject.SetActive(true);

       
        if (vfxAparicionPrefab != null)
        {
            GameObject vfxIn = Instantiate(vfxAparicionPrefab, transform.position, Quaternion.identity);
            Destroy(vfxIn, duracionAutoDestruirVFX);
        }

       
        if (particulasActiva != null)
        {
            particulasActiva.gameObject.SetActive(true);
            particulasActiva.Play();
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

       
        if (particulasActiva != null)
        {
            particulasActiva.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        
        if (vfxDesaparicionPrefab != null)
        {
            GameObject vfxOut = Instantiate(vfxDesaparicionPrefab, transform.position, Quaternion.identity);
            Destroy(vfxOut, duracionAutoDestruirVFX);
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
                mensajeElegido = " Hola pequeña viajera, dejame guiarte ¡Presiona ESPACIO para saltar!";
                break;

            case TipoTutorial.Agacharse:
                mensajeElegido = " Por aqui suelen aparecer slimes, que no te toquen, ¡son inmunes a la magia!.";
                break;

            case TipoTutorial.Golpear:
                mensajeElegido = "Pulsa J para dar un golpe.";
                break;

            case TipoTutorial.Trepar:
                mensajeElegido = "Mantén ARRIBA junto a la pared para trepar.";
                break;

            case TipoTutorial.Mapa:
                mensajeElegido = "El mapa se dividira en secciones subterraneas y externas, ¡ten cuidado!";
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