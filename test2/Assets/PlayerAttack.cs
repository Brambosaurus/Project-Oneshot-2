using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Damage Settings")]
    public int lightDamage = 10;
    public int heavyDamage = 25;

    [Header("Knockback")]
    public float lightKnockback = 2f;
    public float heavyKnockback = 4f;

    [Header("Range & Arc")]
    public float lightRange = 2f;
    public float heavyRange = 3f;
    [Range(0, 180)] public float lightArc = 90f;
    [Range(0, 180)] public float heavyArc = 90f;

    [Header("Cooldown")]
    public float lightCooldown = 0.3f;
    public float heavyCooldown = 0.6f;

    private float lastLightTime = -999f;
    private float lastHeavyTime = -999f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - lastLightTime >= lightCooldown)
        {
            LightAttack();
            lastLightTime = Time.time;
        }

        if (Input.GetMouseButtonDown(1) && Time.time - lastHeavyTime >= heavyCooldown)
        {
            HeavyAttack();
            lastHeavyTime = Time.time;
        }
    }

    void LightAttack()
    {
        PerformAttack(lightRange, lightArc, lightDamage, lightKnockback, Color.red, "Light");
    }

    void HeavyAttack()
    {
        PerformAttack(heavyRange, heavyArc, heavyDamage, heavyKnockback, Color.blue, "Heavy");
    }

    void PerformAttack(float radius, float arcAngle, int damageAmount, float knockbackForce, Color gizmoColor, string label)
    {
        Vector3 origin = transform.position + Vector3.up * 0.8f;
        Collider[] hits = Physics.OverlapSphere(origin, radius);

        foreach (Collider hit in hits)
        {
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                Vector3 dirToEnemy = (hit.transform.position - origin).normalized;
                float angle = Vector3.Angle(transform.forward, dirToEnemy);

                if (angle <= arcAngle)
                {
                    enemy.TakeDamage(damageAmount, dirToEnemy, knockbackForce);
                    Debug.Log($"💥 {label} attack hit: {hit.name}");
                }
            }
        }

        // Debug draw arc direction
        Debug.DrawRay(origin, Quaternion.Euler(0, -arcAngle, 0) * transform.forward * radius, gizmoColor, 1f);
        Debug.DrawRay(origin, Quaternion.Euler(0, arcAngle, 0) * transform.forward * radius, gizmoColor, 1f);
    }

    // Optional visualisation in editor
    void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin, lightRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(origin, heavyRange);
    }
}
