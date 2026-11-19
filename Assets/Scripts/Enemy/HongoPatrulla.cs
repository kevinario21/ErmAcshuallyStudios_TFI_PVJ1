using UnityEngine;

public class HongoPatrulla : MonoBehaviour
{
    public Transform puntoA;
    public Transform puntoB;
    public float velocidad = 1f;
    public int dano = 1;
    public float cooldownDano = 1f;

    Transform destino;
    float proximoDano;

    void Start()
    {
        destino = puntoB;
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

    void OnCollisionStay2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Player") && Time.time >= proximoDano)
        {
            PlayerHealth vida = c.gameObject.GetComponent<PlayerHealth>();
            if (vida != null) vida.RecibirDano(dano);
            proximoDano = Time.time + cooldownDano;
        }
    }
}