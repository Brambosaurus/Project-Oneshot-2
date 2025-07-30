using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;

    void Start()
    {
        Debug.Log("PauseMenu script is gestart!"); // Controle of script actief is

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false); // Verberg menu bij start
        }
        else
        {
            Debug.LogWarning("⚠️ PauseMenuUI is niet toegewezen in de Inspector!");
        }

        Time.timeScale = 1f; // Zorg dat tijd actief is bij start
        isPaused = false;
    }

    void Update()
    {
        Debug.Log("Update draait..."); // Deze moet je continu zien in de Console

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape ingedrukt"); // Check of Escape werkt

            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        Debug.Log("Resume() wordt uitgevoerd");

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        Debug.Log("Pause() wordt uitgevoerd");

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Restart()
    {
        Debug.Log("Restart() wordt uitgevoerd");

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Exit()
    {
        Debug.Log("Exit() wordt uitgevoerd");

        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
