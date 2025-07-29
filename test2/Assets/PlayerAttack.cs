using UnityEngine;
using System.Collections;

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

    [Header("Lunge Movement")]
    [SerializeField] private float lightLungeDistance = 0.5f;
    [SerializeField] private float heavyLungeDistance = 1f;
    [SerializeField] private float lungeDuration = 0.1f;
    [SerializeField] private AnimationCurve lungeCurve;

    private float lastLightTime = -999f;
    private float lastHeavyTime = -999f;

    private Rigidbody _rb;
    private CameraZoomController camShake;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        camShake = FindObjectOfType<CameraZoomController>();
    }

    void Update()
    {
        if (PlayerController.IsAttacking) return;

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
        StartCoroutine(Lunge(lightLungeDistance));
    }

    void HeavyAttack()
    {
        PerformAttack(heavyRange, heavyArc, heavyDamage, heavyKnockback, Color.blue, "Heavy");
        StartCoroutine(Lunge(heavyLungeDistance));
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

                if (angle <= arcAngle * 0.5f)
                {
                    enemy.TakeDamage(damageAmount, dirToEnemy, knockbackForce);
                    camShake?.TriggerShake(); // camera shake bij impact
                    Debug.Log($"💥 {label} attack hit: {hit.name}");
                }
            }
        }

        Debug.DrawRay(origin, Quaternion.Euler(0, -arcAngle, 0) * transform.forward * radius, gizmoColor, 1f);
        Debug.DrawRay(origin, Quaternion.Euler(0, arcAngle, 0) * transform.forward * radius, gizmoColor, 1f);
    }

    private IEnumerator Lunge(float distance)
    {
        PlayerController.SetAttacking(true);

        float timer = 0f;
        Vector3 start = transform.position;
        Vector3 direction = transform.forward;
        Vector3 target = start + direction * distance;

        while (timer < lungeDuration)
        {
            float percent = timer / lungeDuration;
            float curveValue = lungeCurve != null ? lungeCurve.Evaluate(percent) : percent;
            Vector3 newPos = Vector3.Lerp(start, target, curveValue);

            _rb.MovePosition(newPos);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        PlayerController.SetAttacking(false);
    }

    void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin, lightRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(origin, heavyRange);
    }
}
