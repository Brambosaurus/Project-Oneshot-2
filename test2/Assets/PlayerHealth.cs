using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public GameObject gameOverCanvas;

    private void Start()
    {
        currentHealth = maxHealth;

        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(false);
        else
            Debug.LogWarning("Game Over Canvas niet toegewezen!");
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Player Health: " + currentHealth);

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

        Time.timeScale = 0f; // ❄️ Game bevriezen
    }

    private void Update()
    {
        // Test met toets H
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10);
        }
    }
}
