using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // Método que llama SlimeAI al atacar al jugador
    public void RecibirDano(int cantidad)
    {
        Debug.Log("El jugador recibió daño: " + cantidad);
    }
}