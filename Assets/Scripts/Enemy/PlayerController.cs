using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float velocidad = 5f;
    public float fuerzaSalto = 8f;
    public bool isCrouching = false;
    public bool estaAgachado = false;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        float mover = 0f;
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed) mover = -1f;
        if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed) mover = 1f;

        rb.linearVelocity = new Vector2(mover * velocidad, rb.linearVelocity.y);

        if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }

        bool agachado = Keyboard.current.cKey.isPressed || Keyboard.current.leftCtrlKey.isPressed || Keyboard.current.sKey.isPressed;
        isCrouching = agachado;
        estaAgachado = agachado;

        transform.localScale = agachado ? new Vector3(1f, 0.5f, 1f) : new Vector3(1f, 1f, 1f);
    }
}
