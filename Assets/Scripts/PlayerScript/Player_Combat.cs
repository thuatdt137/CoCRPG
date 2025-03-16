using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Transform attackPoint;
    public LayerMask enemyLayer;
    public Animator anim;
    public StatsUI statsUI;
    public float cooldown = 2;
    private float timer;

    void Start()
    {
        if (StatsManager.Instance == null)
        {
            Debug.LogError("⚠️ StatsManager.Instance vẫn null trong Start() của Player_Combat!");
        }
    }


    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }
    public void Attack()
    {
        if (timer <= 0)
        {
            anim.SetBool("isAttacking", true);
            timer = cooldown;
        }
    }

    public void DealDamage()
    {
        //StatsManager.Instance.damage += 1;
        statsUI.UpdateDamage();

        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, StatsManager.Instance.weaponRange, enemyLayer);
        if (enemies.Length > 0)
        {
            enemies[0].GetComponent<Enemy_Health>().ChangeHealth(-StatsManager.Instance.damage);
            enemies[0].GetComponent<Enemy_Knockback>().Knockback(transform, StatsManager.Instance.knockbackForce, StatsManager.Instance.knockbackTime, StatsManager.Instance.stunTime);
        }
    }

    public void FinishAttacking()
    {
        anim.SetBool("isAttacking", false);
    }

    // void OnDrawGizmosSelected()
    // {

    //     if (StatsManager.Instance == null)
    //     {
    //         Debug.LogWarning("StatsManager.Instance is null! Kiểm tra xem StatsManager có tồn tại trong scene không.", this);
    //         return;
    //     }

    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(attackPoint.position, StatsManager.Instance.weaponRange);
    // }

}
