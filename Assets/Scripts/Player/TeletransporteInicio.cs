using UnityEngine;

public class TeletransporteInicio : MonoBehaviour
{
    [Header("Destino")]
    [SerializeField] private Transform puntoInicio;

    [Header("Opciones")]
    [SerializeField] private bool reiniciarVelocidad = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (puntoInicio == null)
        {
            Debug.LogWarning("Falta asignar el Punto de Inicio en " + gameObject.name);
            return;
        }

      
        other.transform.position = puntoInicio.position;


        if (reiniciarVelocidad)
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
}