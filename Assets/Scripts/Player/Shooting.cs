using UnityEngine;

public class Shooting : MonoBehaviour
{
    [Header("Configuracion disparo")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float fireCooldown = 0.25f; 

    private SpriteRenderer spriteRenderer;
    private float nextFireTime = 0f;
    private float initialFirePointX;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (firePoint != null)
        {
            initialFirePointX = Mathf.Abs(firePoint.localPosition.x);
        }
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing) return;

        UpdateFirePointPosition();

        if (Input.GetKeyDown(KeyCode.J) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireCooldown;
        }
    }

    private void UpdateFirePointPosition()
    {
        if (firePoint == null || spriteRenderer == null) return;

        // Volteamos la posición X del punto de disparo según hacia dónde mire el sprite
        float targetX = spriteRenderer.flipX ? -initialFirePointX : initialFirePointX;
        firePoint.localPosition = new Vector3(targetX, firePoint.localPosition.y, firePoint.localPosition.z);
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Projectile projScript = bullet.GetComponent<Projectile>();
        if (projScript != null)
        {
            float direction = spriteRenderer.flipX ? -1f : 1f;
            projScript.SetDirection(direction, projectileSpeed);
        }
    }
}