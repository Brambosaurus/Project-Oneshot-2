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
    public float smiteKnockback = 6f;

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

    [Header("Smite Attack")]
    public float smiteCooldown = 5f;
    private float lastSmiteTime = -999f;
    public GameObject smiteEffectPrefab;
    public float smiteRange = 10f;
    public int smiteDamage = 50;
    public float smiteDelay = 0.3f;
    public float hitPauseDuration = 0.1f;
    public float smiteZoomAmount = 20f;
    public float smiteZoomDuration = 1.5f;
    public float smiteZoomReturnDelay = 1f;

    private float lastLightTime = -999f;
    private float lastHeavyTime = -999f;

    private Rigidbody _rb;
    private CameraZoomController zoomController;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        zoomController = Camera.main.GetComponent<CameraZoomController>();
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

        if (Input.GetKeyDown(KeyCode.E) && Time.time - lastSmiteTime >= smiteCooldown)
        {
            StartCoroutine(DelayedSmite());
            lastSmiteTime = Time.time;
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
                    Debug.Log($"\uD83D\uDCA5 {label} attack hit: {hit.name}");
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

    private IEnumerator DelayedSmite()
    {
        PlayerController.SetAttacking(true);

        if (smiteEffectPrefab)
        {
            GameObject vfx = Instantiate(smiteEffectPrefab, transform.position + transform.forward * 2f, Quaternion.identity);
            Destroy(vfx, 2f);
        }

        if (zoomController)
            zoomController.TriggerZoom(smiteZoomAmount, smiteZoomDuration);

        yield return new WaitForSeconds(smiteDelay);

        Collider[] hits = Physics.OverlapSphere(transform.position, smiteRange);
        foreach (Collider hit in hits)
        {
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                enemy.TakeDamage(smiteDamage, dir, smiteKnockback);
            }
        }

        StartCoroutine(HitPause());

        yield return new WaitForSeconds(smiteZoomReturnDelay);
        if (zoomController)
            zoomController.ResetZoom();

        Debug.Log("⚡ Smite cast!");
        PlayerController.SetAttacking(false);
    }

    private IEnumerator HitPause()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitPauseDuration);
        Time.timeScale = 1f;
    }

    void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin, lightRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(origin, heavyRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, smiteRange);
    }
}
