using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 2f;
    public int baseDamage = 25;

    public float lightAttackDelay = 0.1f;
    public float heavyAttackDelay = 0.6f;

    public float attackCooldown = 0.5f;
    private bool isAttacking = false;

    public float lightKnockback = 1f;
    public float heavyKnockback = 2f;

    void Update()
    {
        if (!isAttacking)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(PerformLightAttack());
            }

            if (Input.GetMouseButtonDown(1))
            {
                StartCoroutine(PerformHeavyAttack());
            }
        }
    }

    IEnumerator PerformLightAttack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(lightAttackDelay);

        DoAttack(baseDamage, lightKnockback, Color.red);

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    IEnumerator PerformHeavyAttack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(heavyAttackDelay);

        DoAttack(baseDamage * 2, heavyKnockback, Color.blue);

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    void DoAttack(int damage, float knockbackForce, Color debugColor)
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, attackRange))
        {
            Debug.DrawRay(origin, direction * attackRange, debugColor, 1f);

            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                Vector3 knockbackDir = (hit.transform.position - transform.position).normalized;
                enemy.TakeDamage(damage, knockbackDir, knockbackForce);
            }
        }
    }
}
