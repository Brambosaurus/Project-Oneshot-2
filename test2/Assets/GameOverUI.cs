using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public void RestartGame()
    {
        // Zet timeScale terug op normaal
        Time.timeScale = 1f;

        // Herlaad huidige scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
