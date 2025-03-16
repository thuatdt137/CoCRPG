using System.Collections;
using UnityEngine;

public class Enemy_Knockback : MonoBehaviour
{

    private Rigidbody2D rb;
    private Enemy_Movement enemy_Movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy_Movement = GetComponent<Enemy_Movement>();
    }
    public void Knockback(Transform forceTransform, float knowbackForce, float knockbackTime, float stunTime)
    {
        enemy_Movement.ChangeState(EnemyState.Knockback);
        StartCoroutine(StunTimer(knockbackTime, stunTime));
        Vector2 direction = (transform.position - forceTransform.position).normalized;
        rb.linearVelocity = direction * knowbackForce;
    }

    IEnumerator StunTimer(float knockbackTime, float stunTim)
    {
        yield return new WaitForSeconds(knockbackTime);
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(stunTim);
        enemy_Movement.ChangeState(EnemyState.Idle);
    }
}
