using UnityEngine;

public class SlimeAI : MonoBehaviour
{
    enum Estado { Patrulla, Persecucion, Ataque }
    Estado estado = Estado.Patrulla;

    [Header("Patrulla")]
    public Transform puntoA;
    public Transform puntoB;
    public float velPatrulla = 1.5f;

    [Header("Persecución")]
    public float velPersecucion = 3f;
    public float rangoVision = 6f;
    public float rangoVisionAgachado = 2f;

    [Header("Ataque")]
    public float rangoAtaque = 1f;
    public float cooldownAtaque = 1.5f;
    public int dano = 1;

    Transform jugador;
    PlayerController controlJugador;
    PlayerHealth vidaJugador;
    Transform destino;
    Rigidbody2D rb;
    Animator anim;
    float proximoAtaque;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
        {
            jugador = p.transform;
            controlJugador = p.GetComponent<PlayerController>();
            vidaJugador = p.GetComponent<PlayerHealth>();
        }

        destino = puntoB;
    }

    void Update()
    {
        if (jugador == null) return;

        float dist = Vector2.Distance(transform.position, jugador.position);
        bool agachado = controlJugador != null && controlJugador.estaAgachado;
        float vision = agachado ? rangoVisionAgachado : rangoVision;

        switch (estado)
        {
            case Estado.Patrulla:
                Patrullar();
                if (dist <= vision)
                    estado = Estado.Persecucion;
                break;

            case Estado.Persecucion:
                Mover(jugador.position.x, velPersecucion);
                if (dist <= rangoAtaque)
                    estado = Estado.Ataque;
                else if (dist > vision * 1.5f)
                    estado = Estado.Patrulla;
                break;

            case Estado.Ataque:
                Detener();
                if (Time.time >= proximoAtaque)
                {
                    if (vidaJugador != null) vidaJugador.RecibirDano(dano);
                    if (anim != null) anim.SetTrigger("atacar");
                    proximoAtaque = Time.time + cooldownAtaque;
                }
                if (dist > rangoAtaque)
                    estado = Estado.Persecucion;
                break;
        }

        if (anim != null)
            anim.SetBool("caminando", Mathf.Abs(rb.linearVelocity.x) > 0.1f);
    }

    void Patrullar()
    {
        if (puntoA == null || puntoB == null) { Detener(); return; }

        Mover(destino.position.x, velPatrulla);
        if (Mathf.Abs(transform.position.x - destino.position.x) < 0.2f)
            destino = (destino == puntoA) ? puntoB : puntoA;
    }

    void Mover(float objetivoX, float vel)
    {
        float dir = Mathf.Sign(objetivoX - transform.position.x);
        rb.linearVelocity = new Vector2(dir * vel, rb.linearVelocity.y);

        Vector3 escala = transform.localScale;
        escala.x = -dir * Mathf.Abs(escala.x);
        transform.localScale = escala;
    }

    void Detener()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    // Dibuja los rangos en la escena para ajustarlos
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoVision);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoVisionAgachado);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}