using UnityEngine;

public class DetectorRana : MonoBehaviour
{
    // Unity llama a esta función automáticamente cuando algo entra en el Trigger
    private void OnTriggerEnter2D(Collider2D otroObjeto)
    {
        // Preguntamos si lo que entró tiene la etiqueta "Player"
        if (otroObjeto.CompareTag("Player"))
        {
            Debug.Log("¡Croac! El Player se acercó a la rana.");
        }
    }

    // Bonus: para saber también cuando se aleja
    private void OnTriggerExit2D(Collider2D otroObjeto)
    {
        if (otroObjeto.CompareTag("Player"))
        {
            Debug.Log("El Player se alejó de la rana.");
        }
    }
}