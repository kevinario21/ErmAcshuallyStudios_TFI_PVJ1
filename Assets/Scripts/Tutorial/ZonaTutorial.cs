using UnityEngine;

public class ZonaTutorial : MonoBehaviour
{
    [Header("Conexiones")]
    [SerializeField] private Rana rana;
    [SerializeField] private TipoTutorial temaDeEstaZona;

    [Header("Posicion rana")]
    [SerializeField] private Transform puntoAparicionRana;

    private void OnTriggerEnter2D(Collider2D otroObjeto)
    {
        if (otroObjeto.CompareTag("Player"))
        {
            if (rana != null && puntoAparicionRana != null)
            {
                rana.InvocarRana(temaDeEstaZona, puntoAparicionRana);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D otroObjeto)
    {
        if (otroObjeto.CompareTag("Player"))
        {
            if (rana != null)
            {
                rana.ProgramarOcultarRana();
            }
        }
    }
}