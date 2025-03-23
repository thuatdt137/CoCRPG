using UnityEngine;

public class EnemyThrower : MonoBehaviour
{
    public GameObject bombPrefab;
    public Transform throwPoint;
    public float throwForce = 5f;
    public float detectionRadius = 5f;
    public float throwCooldown = 2f;

    private Transform player;
    private float lastThrowTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= detectionRadius && Time.time - lastThrowTime >= throwCooldown)
        {
            ThrowBomb();
            lastThrowTime = Time.time;
        }
    }

    void ThrowBomb()
    {
        GameObject bomb = Instantiate(bombPrefab, throwPoint.position, Quaternion.identity);
        Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            Vector2 direction = (player.position - throwPoint.position).normalized;
            rb.linearVelocity = direction * throwForce;
        }
    }
}
