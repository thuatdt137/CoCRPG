using UnityEngine;

public class Enemy_Bomb : MonoBehaviour
{
    public int damage = 1;
    public Transform attackPoint;
    public float weaponRange;
    public LayerMask playerLayer;

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

    public Transform launchPoint;
    public GameObject bombPrefab;
    public Enemy_Movement enemyMovement;
    private Vector2 aimDirection = Vector2.right;

    public float shootCooldown = .5f;
    private float shootTimer;
    public Animator anim;

    void Update()
    {
        shootTimer -= Time.deltaTime;
        HandleAiming();
        if (Input.GetButtonDown("Shoot") && shootTimer <= 0)
        {
            enemyMovement.isThrowing = true;
            anim.SetBool("isThrowing", true);
        }
    }

    void OnEnable()
    {
        anim.SetLayerWeight(0, 0);
        anim.SetLayerWeight(1, 1);
    }

    void OnDisable()
    {
        anim.SetLayerWeight(0, 1);
        anim.SetLayerWeight(1, 0);
    }

    private void HandleAiming()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        if (horizontal != 0 || vertical != 0)
        {
            aimDirection = new Vector2(horizontal, vertical).normalized;
            anim.SetFloat("aimX", aimDirection.x);
            anim.SetFloat("aimY", aimDirection.y);

        }
    }

    public void Shoot()
    {
        if (shootTimer <= 0)
        {
            Arrow arrow = Instantiate(bombPrefab, launchPoint.position, Quaternion.identity).GetComponent<Arrow>();
            arrow.direction = aimDirection;
            shootTimer = shootCooldown;
        }

        anim.SetBool("isThrowing", false);
        enemyMovement.isThrowing = false;
    }
}
