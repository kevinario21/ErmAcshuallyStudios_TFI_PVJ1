using UnityEngine;

public class PortalNivel : MonoBehaviour
{
    [SerializeField] private bool usarSiguienteNivel = true;
    [SerializeField] private string escenaDestinoManual = "Nivel2";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance != null)
        {
            if (usarSiguienteNivel) GameManager.Instance.LoadNextLevel();
            else GameManager.Instance.LoadScene(escenaDestinoManual);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(escenaDestinoManual);
        }
    }
}