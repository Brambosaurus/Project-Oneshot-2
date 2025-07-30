using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public bool isInvincible = false;

    private Rigidbody rb;

    // 🔴 FLASH SETTINGS
    private Renderer rend;
    private Color originalColor;
    public Color hitColor = Color.red;
    public float flashDuration = 0.1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        currentHealth = maxHealth;

        // 🔴 Cache de renderer en originele kleur
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            originalColor = rend.material.color;
        }
    }

    public void TakeDamage(int damage, Vector3 knockbackDirection, float knockbackDistance)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        Debug.Log("Enemy took damage! Current HP: " + currentHealth);

        isInvincible = true;
        knockbackDirection.y = 0;
        StartCoroutine(SmoothKnockback(knockbackDirection.normalized, knockbackDistance, 0.4f));

        // 🎯 VFX hier
        HitEffect();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator SmoothKnockback(Vector3 direction, float distance, float duration)
    {
        Vector3 startPos = rb.position;
        Vector3 endPos = startPos + direction * distance;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float easedT = Mathf.Sin(t * Mathf.PI * 0.5f);
            rb.MovePosition(Vector3.Lerp(startPos, endPos, easedT));
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(endPos);
        isInvincible = false;
    }

    private void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }

    private void HitEffect()
    {
        // 🌟 Start een flash coroutine
        if (rend != null)
        {
            StartCoroutine(FlashRed());
        }

        // Hier kun je later particles, geluiden of animaties toevoegen
    }

    private IEnumerator FlashRed()
    {
        rend.material.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        rend.material.color = originalColor;
    }

    void Update()
    {
        // Debugtest
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(25, -transform.forward, 2f); // Test 2m knockback achteruit
        }
    }
}
