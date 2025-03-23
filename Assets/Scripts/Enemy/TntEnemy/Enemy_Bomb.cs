using UnityEngine;

public class Enemy_Bomb : MonoBehaviour
{
    public int damage = 1;
    public Transform attackPoint;
    public float weaponRange;
    public LayerMask playerLayer;
    public Transform launchPoint;
    public GameObject bombPrefab;
    public Enemy_Movement enemyMovement;
    private Vector2 aimDirection = Vector2.right;

    public float shootCooldown = .5f;
    private float shootTimer;
    public Animator anim;
    public Transform player;

    public void Attack()
    {
        if (attackPoint != null)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, playerLayer);

            if (hits.Length > 0)
            {
                hits[0].GetComponent<PlayerHealth>().ChangeHealth(-damage);
                hits[0].GetComponent<PlayerMovement>().Knockback(transform, StatsManager.Instance.knockbackForce);

            }
        }

    }

    void Update()
    {
        shootTimer -= Time.deltaTime;
        HandleAiming();
    }

    private void HandleAiming()
    {
        if (player != null)
        {
            aimDirection = (player.position - launchPoint.position).normalized;

        }
    }

    public void Shoot()
    {
        if (shootTimer <= 0)
        {
            Bomb bomb = Instantiate(bombPrefab, launchPoint.position, Quaternion.identity).GetComponent<Bomb>();
            bomb.direction = aimDirection;
            shootTimer = shootCooldown;
        }
    }
}
