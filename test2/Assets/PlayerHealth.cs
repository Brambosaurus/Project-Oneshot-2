using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public GameObject gameOverCanvas;

    private Renderer playerRenderer;
    private Color originalColor;

    // 🔁 Voor health regen
    public float regenDelay = 5f;
    public float regenRate = 1f;
    public float regenInterval = 1f;

    private float timeSinceLastDamage;
    private Coroutine regenCoroutine;

    private void Start()
    {
        currentHealth = maxHealth;

        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(false);
        else
            Debug.LogWarning("Game Over Canvas niet toegewezen!");

        playerRenderer = GetComponentInChildren<Renderer>();
        if (playerRenderer != null)
            originalColor = playerRenderer.material.color;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Player Health: " + currentHealth);

        StartCoroutine(FlashRed());

        timeSinceLastDamage = 0f;

        if (regenCoroutine != null)
            StopCoroutine(regenCoroutine);
        regenCoroutine = StartCoroutine(RegenerateHealth());

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log("Player Died");

        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(true);
        else
            Debug.LogWarning("Game Over Canvas is niet gekoppeld!");

        Time.timeScale = 0f; // 🧊 Game bevriezen
    }

    private IEnumerator FlashRed()
    {
        if (playerRenderer == null)
            yield break;

        playerRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        playerRenderer.material.color = originalColor;
    }

    private void Update()
    {
        // Test met toets H
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10);
        }

        timeSinceLastDamage += Time.deltaTime;
    }

    private IEnumerator RegenerateHealth()
    {
        while (timeSinceLastDamage < regenDelay)
            yield return null;

        while (currentHealth < maxHealth)
        {
            currentHealth += Mathf.RoundToInt(regenRate);
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            Debug.Log("Player Regenerated: " + currentHealth);
            yield return new WaitForSeconds(regenInterval);

            if (timeSinceLastDamage < regenDelay)
                yield break;
        }

        regenCoroutine = null;
    }
}
