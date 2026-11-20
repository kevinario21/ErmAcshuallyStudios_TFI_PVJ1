using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int vidaActual = 100;

    public void RecibirDano(int cantidad)
    {
        vidaActual -= cantidad;
        Debug.Log("Jugador recibió daño: " + cantidad + ". Vida restante: " + vidaActual);
    }

    public void TakeDamage(int cantidad)
    {
        RecibirDano(cantidad);
    }
}
