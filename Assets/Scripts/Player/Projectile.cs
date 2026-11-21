using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;
    private float direction = 1f;
    private float lifeTime = 3f;

    public void SetDirection(float dir, float moveSpeed)
    {
        direction = dir;
        speed = moveSpeed;
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(1);
            Destroy(gameObject);
            return;
        }

        
        if (collision.CompareTag("Ground") || collision.CompareTag("Enemy") || collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}