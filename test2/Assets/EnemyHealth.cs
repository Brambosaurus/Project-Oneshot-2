using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public bool isInvincible = false; // vijand tijdelijk onkwetsbaar tijdens knockback

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage, Vector3 knockbackDirection, float knockbackDistance)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        Debug.Log("Enemy took damage! Current HP: " + currentHealth);

        // Start knockback met easing
        isInvincible = true;
        StartCoroutine(SmoothKnockback(knockbackDirection.normalized, knockbackDistance, 0.2f));

        // Voeg hier visuele effecten toe
        HitEffect();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator SmoothKnockback(Vector3 direction, float distance, float duration)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + direction * distance;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // Ease-out beweging (sneller in het begin, vertraagt naar einde)
            float easedT = Mathf.Sin(t * Mathf.PI * 0.5f);

            transform.position = Vector3.Lerp(startPos, endPos, easedT);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        isInvincible = false;
    }

    private void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }

    private void HitEffect()
    {
        // Voeg hier particle effects, geluid, of animatie toe
    }

    // Debugtest: met H toets manueel knockback forceren
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(25, Vector3.back, 2f); // Test: 2 meter knockback achteruit
        }
    }
}
