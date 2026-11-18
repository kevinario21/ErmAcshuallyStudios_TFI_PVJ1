using UnityEngine;

public class ZonaTutorial : MonoBehaviour
{
    [Header("Conexiones")]
    [SerializeField] private Rana rana; // Referencia a nuestra rana
    [SerializeField] private Rana.TipoTutorial temaDeEstaZona; // Qué tema activa esta zona

    private void OnTriggerEnter2D(Collider2D otroObjeto)
    {
        if (otroObjeto.CompareTag("Player"))
        {
            // Le avisamos a la rana qué tema tiene que explicar
            if (rana != null)
            {
                rana.MostrarMensaje(temaDeEstaZona);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D otroObjeto)
    {
        if (otroObjeto.CompareTag("Player"))
        {
            if (rana != null)
            {
                rana.OcultarMensaje();
            }
        }
    }
}