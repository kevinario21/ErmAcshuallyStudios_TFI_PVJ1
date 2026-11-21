using UnityEngine;

public class HongoPatrulla : MonoBehaviour
{
    [Header("Patrulla")]
    public Transform puntoA;
    public Transform puntoB;
    public float velocidad = 1f;

    [Header("Combate")]
    public int vidaMaxima = 3;
    public int dano = 1;
    public float cooldownDano = 1f;

    private int vidaActual;
    private Transform destino;
    private float proximoDano;

    void Start()
    {
        destino = puntoB;
        vidaActual = vidaMaxima;
    }

    void Update()
    {
        if (puntoA == null || puntoB == null) return;

        Vector2 pos = transform.position;
        Vector2 objetivo = new Vector2(destino.position.x, pos.y);
        transform.position = Vector2.MoveTowards(pos, objetivo, velocidad * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - destino.position.x) < 0.1f)
            destino = (destino == puntoA) ? puntoB : puntoA;
    }

    // Este es el método que debe llamar el script de ataque del Player
    public void RecibirDano(int cantidad)
    {
        vidaActual -= cantidad;

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        // Aquí puedes agregar partículas, sonido o suma de puntos antes de destruirlo
        Destroy(gameObject);
    }

    void OnCollisionStay2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Player") && Time.time >= proximoDano)
        {
            // Conectado directamente al PlayerStats de tu proyecto
            PlayerStats stats = c.gameObject.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.TakeDamage(dano);
            }
            proximoDano = Time.time + cooldownDano;
        }
    }
}