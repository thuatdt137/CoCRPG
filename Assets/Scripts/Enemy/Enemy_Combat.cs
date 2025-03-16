using UnityEngine;

public class Enemy_Combat : MonoBehaviour
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
}
