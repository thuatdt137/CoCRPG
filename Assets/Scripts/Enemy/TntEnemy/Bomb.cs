using UnityEngine;

public class Bomb : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector2 direction = Vector2.right; // Hướng ném bom
    public float speed = 5f; // Tốc độ bay
    public float explosionRadius = 2f; // Bán kính vụ nổ
    public float explosionDelay = 3f; // Hẹn giờ phát nổ
    public int damage = 3; // Sát thương gây ra khi nổ
    public float knockbackForce = 6f; // Lực đẩy lùi Player
    public float knockbackTime = 0.5f;
    public float stunTime = 1f;

    public LayerMask playerLayer;
    public LayerMask obstacleLayer;

    public GameObject explosionPrefab; // Hiệu ứng phát nổ

    void Start()
    {
        rb.linearVelocity = direction * speed;
        RotateBomb();
        Invoke("Explode", explosionDelay); // Hẹn giờ phát nổ
    }

    private void RotateBomb()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if ((playerLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            Explode();
        }
        else if ((obstacleLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            AttachToSurface(collision.gameObject.transform);
        }
    }

    private void Explode()
    {
        // Tạo hiệu ứng phát nổ
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // Kiểm tra Player trong vùng nổ
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, playerLayer);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<PlayerHealth>().ChangeHealth(-damage);
                //hit.GetComponent<PlayerMovement>().Knockback(transform, knockbackForce, knockbackTime, stunTime);
            }
        }

        Destroy(gameObject); // Hủy bomb sau khi nổ
    }

    private void AttachToSurface(Transform surface)
    {
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        transform.SetParent(surface);

        Invoke("Explode", 1.5f); // Nổ sau 1.5s nếu không chạm Player
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
