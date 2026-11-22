using UnityEngine;

public class SlimePerseguidor : MonoBehaviour
{
    [Header("Movimiento y Detección")]
    [SerializeField] private float velocidadPatrulla = 1.5f;
    [SerializeField] private float velocidadCorrer = 4f;
    [SerializeField] private float rangoDeteccion = 7f;
    [SerializeField] private float margenAltura = 2.5f;

    [Header("Patrulla Opcional (si se dejan vacíos, espera quieto)")]
    [SerializeField] private Transform puntoA;
    [SerializeField] private Transform puntoB;

    [Header("Ataque")]
    [SerializeField] private int dano = 1;
    [SerializeField] private float cooldownAtaque = 1.2f;

    private Transform jugador;
    private PlayerStats statsJugador;
    private Rigidbody2D rb;
    private Animator anim;
    private Transform destinoPatrulla;
    private float proximoAtaque;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        destinoPatrulla = puntoB;

        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
        {
            jugador = p.transform;
            statsJugador = p.GetComponent<PlayerStats>();
        }
    }

    private void FixedUpdate()
    {
        if (jugador == null) return;

        float distX = Mathf.Abs(transform.position.x - jugador.position.x);
        float distY = Mathf.Abs(transform.position.y - jugador.position.y);

        // Si el jugador está dentro del rango horizontal y a una altura cercana, corre hacia él
        if (distX <= rangoDeteccion && distY <= margenAltura)
        {
            MoverHacia(jugador.position.x, velocidadCorrer);
        }
        else
        {
            Patrullar();
        }

        if (anim != null)
        {
            anim.SetBool("caminando", Mathf.Abs(rb.linearVelocity.x) > 0.1f);
        }
    }

    private void Patrullar()
    {
        if (puntoA == null || puntoB == null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        MoverHacia(destinoPatrulla.position.x, velocidadPatrulla);

        if (Mathf.Abs(transform.position.x - destinoPatrulla.position.x) < 0.25f)
        {
            destinoPatrulla = (destinoPatrulla == puntoA) ? puntoB : puntoA;
        }
    }

    private void MoverHacia(float objetivoX, float velocidad)
    {
        float diferencia = objetivoX - transform.position.x;

        // Evita que tiemble si ya está justo encima del objetivo
        if (Mathf.Abs(diferencia) < 0.1f)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        float dir = Mathf.Sign(diferencia);
        rb.linearVelocity = new Vector2(dir * velocidad, rb.linearVelocity.y);

        // Voltea el sprite según la dirección
        Vector3 escala = transform.localScale;
        escala.x = -dir * Mathf.Abs(escala.x);
        transform.localScale = escala;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        if (Time.time >= proximoAtaque)
        {
            if (statsJugador == null)
                statsJugador = collision.gameObject.GetComponent<PlayerStats>();

            if (statsJugador != null)
                statsJugador.TakeDamage(dano);

            if (anim != null)
                anim.SetTrigger("atacar");

            proximoAtaque = Time.time + cooldownAtaque;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
    }
}