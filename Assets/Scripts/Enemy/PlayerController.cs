using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float velocidad = 5f;
    public bool isCrouching = false;
    public bool estaAgachado = false;

    void Update()
    {
        float mover = Input.GetAxis("Horizontal");
        transform.Translate(Vector2.right * mover * velocidad * Time.deltaTime);

        bool agachado = Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.LeftControl);
        isCrouching = agachado;
        estaAgachado = agachado;

        transform.localScale = agachado ? new Vector3(1f, 0.5f, 1f) : new Vector3(1f, 1f, 1f);
    }
}
